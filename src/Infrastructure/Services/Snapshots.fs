namespace Services.Snapshots

open System
open System.Threading

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
            |> ISnapshotsBroker.createSnapshotOrEx userHomePath userSnapshotsPath

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
    static member listOrEx (configData : ConfigData) (listData : ListData) =

        let mountPoint = IDevicesBroker.mountDeviceOrEx configData.SnapshotDevice

        let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{listData.UserName}"
                                |> Directory.create

        let list = ISnapshotsBroker.getAllInfoInPathOrEx userSnapshotsPath

        unmountDeviceOrEx ()
        list
    // -----------------------------------------------------------------------------------------------------------------
