namespace Services.Devices

open Newtonsoft.Json
open Motsoft.Util
open Model
open Localization


type private IDevicesBroker = DI.Brokers.IDevicesBroker
type private IConsoleBroker = DI.Brokers.IConsoleBroker

type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getValidDevicesDataOrEx () =

        let isValidDevice (dataChild : DeviceDataChild) =

            dataChild.ReadOnly = false &&
            dataChild.MountPoint <> null &&
            dataChild.PartTypeName |> String.compareNoCaseNoAccents "Linux filesystem" = 0

        let devicesData =
            IDevicesBroker.getDeviceInfoOrEx ()
            |> JsonConvert.DeserializeObject<BlockDevices>

        devicesData.BlockDevices
        |> Seq.collect (fun d -> d.Children)
        |> Seq.filter isValidDevice
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member outputDevices (devices : DeviceDataChild seq) =

        [
            ""
            Phrases.MountedDevices
            ""
        ]
        |> IConsoleBroker.writeLines

        [|
            [| Phrases.Device ; Phrases.Size ; Phrases.MountPoint ; Phrases.Type ; Phrases.Label |]

            for d in devices do
                [| d.Path ; d.Size ; d.MountPoint ; d.FileSystemType ; d.Label |]
        |]
        |> IConsoleBroker.WriteMatrixWithFooter [| false ; true ; false ; false ; false |] true [ "" ]
    // -----------------------------------------------------------------------------------------------------------------
