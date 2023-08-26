namespace Services

open System
open System.Diagnostics
open System.IO
open System.Threading
open Motsoft.Util

open DI.Interfaces
open Model
open Localization


type SnapshotsService (devicesBroker : IDevicesBroker, snapshotsBroker : ISnapshotsBroker,
                       consoleBroker : IConsoleBroker, usersService : IUsersService) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let getSnapshotPath (userSnapshotsPath : Directory) (dateTime : DateTimeOffset) =

        Path.Combine(userSnapshotsPath.value,
                     $"{dateTime.Year}-%02i{dateTime.Month}-%02i{dateTime.Day}_" +
                     $"%02i{dateTime.Hour}-%02i{dateTime.Minute}-%02i{dateTime.Second}")
        |> Directory.create
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    let unmountDeviceOrEx () =

        let rec tryUnmountOrEx n =
            if n < 10 then
                try
                    devicesBroker.unmountCurrentOrEx ()
                with _ ->
                    Thread.Sleep 1000
                    tryUnmountOrEx (n + 1)
            else
                failwith Errors.CouldNotUnmount

        tryUnmountOrEx 0
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface ISnapshotsService with

        // -------------------------------------------------------------------------------------------------------------
        member _.createOrEx (configData : ConfigData) (createData : CreateData) =

            let mutable PressedCtrlC = false

            let userHomePath = usersService.getHomeForUserOrEx createData.UserName
            let mountPoint = devicesBroker.mountDeviceOrEx configData.SnapshotDevice

            let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{createData.UserName}"
                                    |> Directory.create

            let CancelKeyHandler =
                ConsoleCancelEventHandler(fun _ args -> args.Cancel <- true ; PressedCtrlC <- true)

            try
                let stopWatch = Stopwatch.StartNew()

                let progressCallBack (progressString : string) =
                    if progressString <> null then
                        let progressParts = progressString |> String.split " "

                        if progressParts.Length > 3 then
                            consoleBroker.write($"{Phrases.Elapsed}: %02i{stopWatch.Elapsed.Hours}:" +
                                                 $"%02i{stopWatch.Elapsed.Minutes}:%02i{stopWatch.Elapsed.Seconds} - " +
                                                 $"{Phrases.Completed}: {progressParts[1]} - " +
                                                 $"{Phrases.TimeRemaining}: {progressParts[3]}     \r")

                let baseSnapshotPath = getSnapshotPath userSnapshotsPath createData.CreationDateTime

                [
                    $"{Phrases.SnapshotCreating} ({createData.UserName.value}): " +
                        $"{Path.GetFileName baseSnapshotPath.value}"
                    ""
                ]
                |> consoleBroker.writeLines

                // -----------------------------------------------------------------------------------------------------
                Console.CancelKeyPress.AddHandler CancelKeyHandler

                snapshotsBroker.getLastSnapshotOptionInPathOrEx userSnapshotsPath
                |> snapshotsBroker.createSnapshotOrEx userHomePath baseSnapshotPath createData progressCallBack
                // -----------------------------------------------------------------------------------------------------

                stopWatch.Stop()

                consoleBroker.writeLine($"{Phrases.Elapsed}: %02i{stopWatch.Elapsed.Hours}:" +
                                         $"%02i{stopWatch.Elapsed.Minutes}:%02i{stopWatch.Elapsed.Seconds} - " +
                                         $"{Phrases.Completed} 100%% - {Phrases.TimeRemaining}: 0:00:00          \n")


            finally
                Console.CancelKeyPress.RemoveHandler CancelKeyHandler

                if PressedCtrlC then
                    [
                        Phrases.SnapshotInterrupted
                        Phrases.DeletingIncompleteSnapshot
                        ""
                    ]
                    |> consoleBroker.writeLines

                    snapshotsBroker.deleteLastSnapshotOrEx userSnapshotsPath

                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getListForUserOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) =

            let mountPoint = devicesBroker.mountDeviceOrEx snapshotDevice

            try
                let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{userName}"
                                        |> Directory.create

                snapshotsBroker.getAllInfoInPathOrEx userSnapshotsPath

            finally
                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.outputOrEx (userName : UserName) (snapshots : Snapshot seq) =

            snapshots |> Seq.isEmpty |> failWithIfTrue $"{Errors.SnapshotNonFound} ({userName.value})"

            [
                $"{Phrases.UserSnapshots}: {userName.value}"
                ""
            ]
            |> consoleBroker.writeLines

            [|
                [| Phrases.SnapshotName ; Phrases.SnapshotComments |]

                for d in snapshots do
                    [| d.Name ; d.Comments.value |]
            |]
            |> consoleBroker.writeMatrixWithFooter [| false ; false |] true [ "" ]
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteOrEx (snapshotDevice : SnapshotDevice) (deleteData : DeleteData) =

            [
                $"{Phrases.SnapshotDeleting} ({deleteData.UserName.value}): {deleteData.SnapshotName}"
                ""
            ]
            |> consoleBroker.writeLines


            let mountPoint = devicesBroker.mountDeviceOrEx snapshotDevice

            let snapshotPath = $"{mountPoint}/homeshift/snapshots/{deleteData.UserName}/{deleteData.SnapshotName}"
                               |> Directory.create

            try
                snapshotsBroker.deleteSnapshotPathOrEx snapshotPath

                $"{mountPoint}/homeshift/snapshots/{deleteData.UserName.value}"
                |> Directory.create
                |> snapshotsBroker.deleteUserPathIfEmptyOrEx

            finally
                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteAll (snapshotDevice : SnapshotDevice) (userName : UserName) =

            try
                let snapshotList = (this :> ISnapshotsService).getListForUserOrEx snapshotDevice userName
                let mountPoint = devicesBroker.mountDeviceOrEx snapshotDevice

                snapshotList |> Seq.isEmpty |> failWithIfTrue $"{Errors.SnapshotNonFound} ({userName.value})"

                [
                    $"{Phrases.SnapshotDeletingAll} ({userName.value})"
                    ""
                ]
                |> consoleBroker.writeLines

                snapshotList
                |> Seq.iter (fun s ->
                                $"{Phrases.SnapshotDeleting} %s{s.Name}"
                                |> consoleBroker.writeLine

                                $"{mountPoint}/homeshift/snapshots/{userName}/{s.Name}"
                                |> Directory.create
                                |> snapshotsBroker.deleteSnapshotPathOrEx)

                $"{mountPoint}/homeshift/snapshots/{userName}"
                |> Directory.create
                |> snapshotsBroker.deleteUserPathIfEmptyOrEx

                consoleBroker.writeLine ""

            finally
                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.isValidOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

            (this :> ISnapshotsService).getListForUserOrEx snapshotDevice userName
            |> Seq.exists (fun s -> s.Name = snapshotName)
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
