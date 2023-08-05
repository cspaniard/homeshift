namespace Services.Create

open System
open Model
open Motsoft.Util

type private IUsersBroker = DI.Brokers.IUsersBroker
type private IDevicesBroker = DI.Brokers.IDevicesBroker
type private IErrors = DI.Services.LocalizationDI.IErrors


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getHomeForUserOrEx (userName : UserName) =

        let line = IUsersBroker.getUserInfoFromPasswordFileOrEx userName

        (line |> String.split ":")[5]
        |> Directory.create
        |> IUsersBroker.checkUserHomeExistsOrEx
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member createSnapshot (configData : ConfigData) (createData : CreateData) =

        let userHome = Service.getHomeForUserOrEx createData.UserName
        let mountPoint = IDevicesBroker.mountDeviceOrEx configData.SnapshotDevice

        let dateTime = DateTimeOffset.Now
        let snapshotPath = $"{mountPoint}/homeshift/snapshots/{createData.UserName}/" +
                           $"{dateTime.Year}-%02i{dateTime.Month}-%02i{dateTime.Day}_" +
                           $"%02i{dateTime.Hour}-%02i{dateTime.Minute}-%02i{dateTime.Second}"

        // ToDo: Testing
        printfn "Haciendo copia..."
        // Directory.CreateDirectory snapshotPath
        // |> ignore

        printfn $"%s{userHome.value}"
        printfn $"%s{snapshotPath}"

        IDevicesBroker.unmountCurrentOrEx ()
    // -----------------------------------------------------------------------------------------------------------------
