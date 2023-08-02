namespace Services.Create

open System
open System.IO
open Model
open Motsoft.Util

open Localization

type private IUsersBroker = DI.Brokers.IUsersBroker
type private IDevicesBroker = DI.Brokers.IDevicesBroker


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getHomeForUserOrEx (userName : string) =

        String.IsNullOrWhiteSpace userName |> failWithIfTrue Errors.UserIsEmpty

        let line = IUsersBroker.getUserLineFromPasswordFileOrEx userName

        String.IsNullOrWhiteSpace line |> failWithIfTrue Errors.UserNoInfoFound

        (line |> String.split ":")[5]
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member createSnapshot (configData : ConfigData) (userName : string) =

        let userHome = Service.getHomeForUserOrEx userName

        printfn $"%A{configData.SnapshotDevice}"

        IDevicesBroker.mountDeviceOrEx configData.SnapshotDevice

        // ToDo: Testing
        printfn "Haciendo copia..."
        Directory.CreateDirectory "/home/z3" |> ignore

        IDevicesBroker.unmountCurrentOrEx ()
    // -----------------------------------------------------------------------------------------------------------------
