namespace Services.Devices

open Newtonsoft.Json
open Model

type private IDevicesBroker = DI.Brokers.IDevicesBroker


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getValidDevicesDataOrEx () =

        let isValidDevice (dataChild : DeviceDataChild) =

            dataChild.DeviceType = "part" &&
            dataChild.ReadOnly = false &&
            dataChild.MountPoints
            |> Array.exists
                   (fun s -> s <> null &&
                             s.Contains "boot" = false &&
                             s.Contains "efi" = false)

        let devicesData =
            IDevicesBroker.getDeviceInfoOrEx ()
            |> JsonConvert.DeserializeObject<BlockDevices>

        devicesData.BlockDevices
        |> Array.collect (fun d -> d.Children)
        |> Array.filter isValidDevice
    // -----------------------------------------------------------------------------------------------------------------
