namespace Services.Snapshots

open System
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

        let CancelKeyHandler =
            ConsoleCancelEventHandler(fun _ args -> args.Cancel <- true ; PressedCtrlC <- true)

        let userHomePath = IUsersService.getHomeForUserOrEx createData.UserName
        let mountPoint = IDevicesBroker.mountDeviceOrEx configData.SnapshotDevice

        let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{createData.UserName}"
                                |> Directory.create

        Console.CancelKeyPress.AddHandler CancelKeyHandler

        try
            ISnapshotsBroker.getLastSnapshotOptionInPathOrEx userSnapshotsPath
            |> ISnapshotsBroker.createSnapshotOrEx userHomePath userSnapshotsPath createData

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
    static member listOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) =

        let mountPoint = IDevicesBroker.mountDeviceOrEx snapshotDevice

        let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{userName}"
                                |> Directory.create

        let list = ISnapshotsBroker.getAllInfoInPathOrEx userSnapshotsPath

        unmountDeviceOrEx ()
        list
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member outputOrEx (userName : UserName) (snapshots : Snapshot seq) =

        snapshots |> Seq.isEmpty |> failWithIfTrue $"{IErrors.SnapshotNonFound} ({userName.value})"

        [
            ""
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
            ""
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
            let snapshotList = Service.listOrEx snapshotDevice userName
            let mountPoint = IDevicesBroker.mountDeviceOrEx snapshotDevice

            snapshotList |> Seq.isEmpty |> failWithIfTrue $"{IErrors.SnapshotNonFound} ({userName.value})"

            [
                ""
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

        finally
            unmountDeviceOrEx ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member isValidOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

        Service.listOrEx snapshotDevice userName
        |> Seq.exists (fun s -> s.Name = snapshotName)
    // -----------------------------------------------------------------------------------------------------------------
