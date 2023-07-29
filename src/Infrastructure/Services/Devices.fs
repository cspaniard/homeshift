namespace Services.Devices

open FSharp.Json
open Motsoft.Util
open Model

type private IDevicesBroker = DI.Brokers.IDevicesBroker


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getValidDevicesDataOrEx () =

        let cleanDeviceInfo (deviceInfo : string) =
            deviceInfo
            |> String.split "\n"
            |> Array.filter (fun s -> s.Trim() <> "null")
            |> String.join ""

        let isValidDevice (dataChild : DeviceDataChild) =

            dataChild.DeviceType = "part" &&
            dataChild.ReadOnly = false &&
            dataChild.MountPoints.Length > 0 &&
            dataChild.MountPoints
            |> Array.exists (fun s -> s.Contains "boot" = false &&
                                      s.Contains "efi" = false)

        let devicesData =
            IDevicesBroker.getDeviceInfoOrEx ()
            |> cleanDeviceInfo
            |> Json.deserialize<BlockDevices>

        devicesData.BlockDevices
        |> Array.collect (fun d -> d.Children)
        |> Array.filter isValidDevice
    // -----------------------------------------------------------------------------------------------------------------
