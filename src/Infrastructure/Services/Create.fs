namespace Services.Create

open System
open System.Threading
open Model

type private IUsersBroker = DI.Brokers.IUsersBroker
type private IDevicesBroker = DI.Brokers.IDevicesBroker
type private IErrors = DI.Services.LocalizationDI.IErrors
type private ISnapshotBroker = DI.Brokers.ISnapshotBroker
type private IConsoleBroker = DI.Brokers.IConsoleBroker
type private IPhrases = DI.Services.LocalizationDI.IPhrases


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getHomeForUserOrEx (userName : UserName) =

        let line = IUsersBroker.getUserInfoFromPasswordFileOrEx userName

        (line.Split ":")[5]
        |> Directory.create
        |> IUsersBroker.checkUserHomeExistsOrEx
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member createSnapshotOrEx (configData : ConfigData) (createData : CreateData) =

        let mutable PressedCtrlC = false

        let CancelKeyHandler =
            ConsoleCancelEventHandler(fun _ args -> args.Cancel <- true ; PressedCtrlC <- true)

        let userHomePath = Service.getHomeForUserOrEx createData.UserName
        let mountPoint = IDevicesBroker.mountDeviceOrEx configData.SnapshotDevice

        let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{createData.UserName}"
                                |> Directory.create

        Console.CancelKeyPress.AddHandler CancelKeyHandler

        try
            ISnapshotBroker.getLastSnapshotOptionInPathOrEx userSnapshotsPath
            |> ISnapshotBroker.createSnapshotOrEx userHomePath userSnapshotsPath

        finally
            Console.CancelKeyPress.RemoveHandler CancelKeyHandler

            if PressedCtrlC then
                [
                    IPhrases.SnapshotInterrupted
                    IPhrases.DeletingIncompleteSnapshot
                    ""
                ]
                |> IConsoleBroker.writeLines

                ISnapshotBroker.deleteLastSnapshotOrEx userSnapshotsPath

            let rec tryUnmount n =
                if n < 10 then
                    try
                        IDevicesBroker.unmountCurrentOrEx ()
                    with _ ->
                        Thread.Sleep 1000
                        tryUnmount (n + 1)

            tryUnmount 0
    // -----------------------------------------------------------------------------------------------------------------
