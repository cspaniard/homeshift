namespace Services

open System
open System.Diagnostics
open System.IO
open System.Threading
open Motsoft.Util

open Model
open DI

open Localization

type SnapshotsService private (devicesBroker : IDevicesBroker, snapshotsBroker : ISnapshotsBroker,
                               consoleBroker : IConsoleBroker, usersService : IUsersService) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IDevicesBroker = devicesBroker
    let ISnapshotsBroker = snapshotsBroker
    let IConsoleBroker = consoleBroker
    let IUsersService = usersService
    // -----------------------------------------------------------------------------------------------------------------

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
                    IDevicesBroker.unmountCurrentOrEx ()
                with _ ->
                    Thread.Sleep 1000
                    tryUnmountOrEx (n + 1)
            else
                failwith Errors.CouldNotUnmount

        tryUnmountOrEx 0
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let mutable instance = Unchecked.defaultof<ISnapshotsService>
    
    static member getInstance (devicesBroker : IDevicesBroker, snapshotsBroker : ISnapshotsBroker,
                               consoleBroker : IConsoleBroker, usersService : IUsersService) =
        
        if obj.ReferenceEquals(instance, null) then
            instance <- SnapshotsService (devicesBroker, snapshotsBroker, consoleBroker, usersService)
        
        instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface ISnapshotsService with
    
        // -------------------------------------------------------------------------------------------------------------
        member _.createOrEx (configData : ConfigData) (createData : CreateData) =

            let mutable PressedCtrlC = false

            let userHomePath = IUsersService.getHomeForUserOrEx createData.UserName
            let mountPoint = IDevicesBroker.mountDeviceOrEx configData.SnapshotDevice

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
                            IConsoleBroker.write($"{Phrases.Elapsed}: %02i{stopWatch.Elapsed.Hours}:" +
                                                 $"%02i{stopWatch.Elapsed.Minutes}:%02i{stopWatch.Elapsed.Seconds} - " +
                                                 $"{Phrases.Completed}: {progressParts[1]} - " +
                                                 $"{Phrases.TimeRemaining}: {progressParts[3]}     \r")

                let baseSnapshotPath = getSnapshotPath userSnapshotsPath createData.CreationDateTime

                [
                    $"{Phrases.SnapshotCreating} ({createData.UserName.value}): " +
                        $"{Path.GetFileName baseSnapshotPath.value}"
                    ""
                ]
                |> IConsoleBroker.writeLines

                // -----------------------------------------------------------------------------------------------------
                Console.CancelKeyPress.AddHandler CancelKeyHandler

                ISnapshotsBroker.getLastSnapshotOptionInPathOrEx userSnapshotsPath
                |> ISnapshotsBroker.createSnapshotOrEx userHomePath baseSnapshotPath createData progressCallBack
                // -----------------------------------------------------------------------------------------------------

                stopWatch.Stop()

                IConsoleBroker.writeLine($"{Phrases.Elapsed}: %02i{stopWatch.Elapsed.Hours}:" +
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
                    |> IConsoleBroker.writeLines

                    ISnapshotsBroker.deleteLastSnapshotOrEx userSnapshotsPath

                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getListForUserOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) =

            let mountPoint = IDevicesBroker.mountDeviceOrEx snapshotDevice

            try
                let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{userName}"
                                        |> Directory.create

                ISnapshotsBroker.getAllInfoInPathOrEx userSnapshotsPath

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
            |> IConsoleBroker.writeLines

            [|
                [| Phrases.SnapshotName ; Phrases.SnapshotComments |]

                for d in snapshots do
                    [| d.Name ; d.Comments.value |]
            |]
            |> IConsoleBroker.WriteMatrixWithFooter [| false ; false |] true [ "" ]
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteOrEx (snapshotDevice : SnapshotDevice) (deleteData : DeleteData) =

            [
                $"{Phrases.SnapshotDeleting} ({deleteData.UserName.value}): {deleteData.SnapshotName}"
                ""
            ]
            |> IConsoleBroker.writeLines


            let mountPoint = IDevicesBroker.mountDeviceOrEx snapshotDevice

            let snapshotPath = $"{mountPoint}/homeshift/snapshots/{deleteData.UserName}/{deleteData.SnapshotName}"
                               |> Directory.create

            try
                ISnapshotsBroker.deleteSnapshotPathOrEx snapshotPath

                $"{mountPoint}/homeshift/snapshots/{deleteData.UserName.value}"
                |> Directory.create
                |> ISnapshotsBroker.deleteUserPathIfEmptyOrEx

            finally
                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteAll (snapshotDevice : SnapshotDevice) (userName : UserName) =

            try
                let snapshotList = (this :> ISnapshotsService).getListForUserOrEx snapshotDevice userName
                let mountPoint = IDevicesBroker.mountDeviceOrEx snapshotDevice

                snapshotList |> Seq.isEmpty |> failWithIfTrue $"{Errors.SnapshotNonFound} ({userName.value})"

                [
                    $"{Phrases.SnapshotDeletingAll} ({userName.value})"
                    ""
                ]
                |> IConsoleBroker.writeLines

                snapshotList
                |> Seq.iter (fun s ->
                                $"{Phrases.SnapshotDeleting} %s{s.Name}"
                                |> IConsoleBroker.writeLine

                                $"{mountPoint}/homeshift/snapshots/{userName}/{s.Name}"
                                |> Directory.create
                                |> ISnapshotsBroker.deleteSnapshotPathOrEx)

                $"{mountPoint}/homeshift/snapshots/{userName}"
                |> Directory.create
                |> ISnapshotsBroker.deleteUserPathIfEmptyOrEx

                IConsoleBroker.writeLine ""

            finally
                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.isValidOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

            (this :> ISnapshotsService).getListForUserOrEx snapshotDevice userName
            |> Seq.exists (fun s -> s.Name = snapshotName)
        // -------------------------------------------------------------------------------------------------------------
    
    // -----------------------------------------------------------------------------------------------------------------
