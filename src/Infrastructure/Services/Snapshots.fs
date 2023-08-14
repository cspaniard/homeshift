namespace Services.Snapshots

open System
open System.Diagnostics
open System.IO
open System.Threading
open Motsoft.Util

open Model

type private IPhrases = DI.Services.LocalizationDI.IPhrases
type private IErrors = DI.Services.LocalizationDI.IErrors
type private IDevicesBroker = DI.Brokers.IDevicesBroker
type private ISnapshotsBroker = DI.Brokers.ISnapshotsBroker
type private IConsoleBroker = DI.Brokers.IConsoleBroker
type private IUsersService = DI.Services.IUsersService


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static let getSnapshotPath (userSnapshotsPath : Directory) (dateTime : DateTimeOffset) =

        Path.Combine(userSnapshotsPath.value,
                     $"{dateTime.Year}-%02i{dateTime.Month}-%02i{dateTime.Day}_" +
                     $"%02i{dateTime.Hour}-%02i{dateTime.Minute}-%02i{dateTime.Second}")
        |> Directory.create
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let unmountDeviceOrEx () =

        let rec tryUnmountOrEx n =
            if n < 10 then
                try
                    IDevicesBroker.unmountCurrentOrEx ()
                with _ ->
                    Thread.Sleep 1000
                    tryUnmountOrEx (n + 1)
            else
                failwith IErrors.CouldNotUnmount

        tryUnmountOrEx 0
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member createOrEx (configData : ConfigData) (createData : CreateData) =

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
                        IConsoleBroker.write($"{IPhrases.Elapsed}: %02i{stopWatch.Elapsed.Hours}:" +
                                             $"%02i{stopWatch.Elapsed.Minutes}:%02i{stopWatch.Elapsed.Seconds} - " +
                                             $"{IPhrases.Completed}: {progressParts[1]} - " +
                                             $"{IPhrases.TimeRemaining}: {progressParts[3]}     \r")

            let baseSnapshotPath = getSnapshotPath userSnapshotsPath createData.CreationDateTime

            [
                $"{IPhrases.SnapshotCreating} ({createData.UserName.value}): " +
                    $"{Path.GetFileName baseSnapshotPath.value}"
                ""
            ]
            |> IConsoleBroker.writeLines

            // ---------------------------------------------------------------------------------------------------------
            Console.CancelKeyPress.AddHandler CancelKeyHandler

            ISnapshotsBroker.getLastSnapshotOptionInPathOrEx userSnapshotsPath
            |> ISnapshotsBroker.createSnapshotOrEx userHomePath baseSnapshotPath createData progressCallBack
            // ---------------------------------------------------------------------------------------------------------

            stopWatch.Stop()

            IConsoleBroker.writeLine($"{IPhrases.Elapsed}: %02i{stopWatch.Elapsed.Hours}:" +
                                     $"%02i{stopWatch.Elapsed.Minutes}:%02i{stopWatch.Elapsed.Seconds} - " +
                                     $"{IPhrases.Completed} 100%% - {IPhrases.TimeRemaining}: 0:00:00          \n")


        finally
            Console.CancelKeyPress.RemoveHandler CancelKeyHandler

            if PressedCtrlC then
                [
                    IPhrases.SnapshotInterrupted
                    IPhrases.DeletingIncompleteSnapshot
                    ""
                ]
                |> IConsoleBroker.writeLines

                ISnapshotsBroker.deleteLastSnapshotOrEx userSnapshotsPath

            unmountDeviceOrEx ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member getListForUserOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) =

        let mountPoint = IDevicesBroker.mountDeviceOrEx snapshotDevice

        try
            let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{userName}"
                                    |> Directory.create

            ISnapshotsBroker.getAllInfoInPathOrEx userSnapshotsPath

        finally
            unmountDeviceOrEx ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member outputOrEx (userName : UserName) (snapshots : Snapshot seq) =

        snapshots |> Seq.isEmpty |> failWithIfTrue $"{IErrors.SnapshotNonFound} ({userName.value})"

        [
            $"{IPhrases.UserSnapshots}: {userName.value}"
            ""
        ]
        |> IConsoleBroker.writeLines

        [|
            [| IPhrases.SnapshotName ; IPhrases.SnapshotComments |]

            for d in snapshots do
                [| d.Name ; d.Comments.value |]
        |]
        |> IConsoleBroker.WriteMatrixWithFooter [| false ; false |] true [ "" ]
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member deleteOrEx (snapshotDevice : SnapshotDevice) (deleteData : DeleteData) =

        [
            $"{IPhrases.SnapshotDeleting} ({deleteData.UserName.value}): {deleteData.SnapshotName}"
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
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member deleteAll (snapshotDevice : SnapshotDevice) (userName : UserName) =

        try
            let snapshotList = Service.getListForUserOrEx snapshotDevice userName
            let mountPoint = IDevicesBroker.mountDeviceOrEx snapshotDevice

            snapshotList |> Seq.isEmpty |> failWithIfTrue $"{IErrors.SnapshotNonFound} ({userName.value})"

            [
                $"{IPhrases.SnapshotDeletingAll} ({userName.value})"
                ""
            ]
            |> IConsoleBroker.writeLines

            snapshotList
            |> Seq.iter (fun s ->
                            [ $"{IPhrases.SnapshotDeleting} %s{s.Name}" ]
                            |> IConsoleBroker.writeLines

                            $"{mountPoint}/homeshift/snapshots/{userName}/{s.Name}"
                            |> Directory.create
                            |> ISnapshotsBroker.deleteSnapshotPathOrEx)

            $"{mountPoint}/homeshift/snapshots/{userName}"
            |> Directory.create
            |> ISnapshotsBroker.deleteUserPathIfEmptyOrEx

            IConsoleBroker.writeLine ""

        finally
            unmountDeviceOrEx ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member isValidOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

        Service.getListForUserOrEx snapshotDevice userName
        |> Seq.exists (fun s -> s.Name = snapshotName)
    // -----------------------------------------------------------------------------------------------------------------
