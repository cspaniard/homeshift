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
    static member getHomeForUserOrEx (userName : UserName) =

        let line = IUsersBroker.getUserInfoFromPasswordFileOrEx userName

        line |> String.IsNullOrWhiteSpace |> failWithIfTrue Errors.UserNoInfoFound

        (line |> String.split ":")[5]
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member createSnapshot (configData : ConfigData) (userName : UserName) =

        let userHome = Service.getHomeForUserOrEx userName
        let mountPoint = IDevicesBroker.mountDeviceOrEx configData.SnapshotDevice

        // ToDo: Testing
        printfn "Haciendo copia..."
        Directory.CreateDirectory $"{mountPoint}/z3" |> ignore

        IDevicesBroker.unmountCurrentOrEx ()
    // -----------------------------------------------------------------------------------------------------------------
