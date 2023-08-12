namespace Services.Devices

open Newtonsoft.Json
open Motsoft.Util
open Model


type private IDevicesBroker = DI.Brokers.IDevicesBroker
type private IConsoleBroker = DI.Brokers.IConsoleBroker
type private IPhrases = DI.Services.LocalizationDI.IPhrases

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
    static member isValidDeviceOrEx (device : SnapshotDevice) =

        Service.getValidDevicesDataOrEx ()
        |> Seq.exists (fun d -> d.Path = device.value)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member outputDevices (devices : DeviceDataChild seq) =

        [
            IPhrases.MountedDevices
            ""
        ]
        |> IConsoleBroker.writeLines

        [|
            [| IPhrases.Device ; IPhrases.Size ; IPhrases.MountPoint ; IPhrases.Type ; IPhrases.Label |]

            for d in devices do
                [| d.Path ; d.Size ; d.MountPoint ; d.FileSystemType ; d.Label |]
        |]
        |> IConsoleBroker.WriteMatrixWithFooter [| false ; true ; false ; false ; false |] true [ "" ]
    // -----------------------------------------------------------------------------------------------------------------
