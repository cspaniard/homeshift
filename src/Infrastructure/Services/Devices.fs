namespace Services

open Newtonsoft.Json
open Motsoft.Util

open DI.Interfaces
open Model
open Localization


type DevicesService (devicesBroker : IDevicesBroker, consoleBroker : IConsoleBroker) as this =

    // -----------------------------------------------------------------------------------------------------------------
    interface IDevicesService with

        // -------------------------------------------------------------------------------------------------------------
        member _.getValidDevicesDataOrEx () =

            let isValidDevice (dataChild : DeviceDataChild) =

                dataChild.ReadOnly = false &&
                dataChild.MountPoint <> null &&
                dataChild.PartTypeName |> String.compareNoCaseNoAccents "Linux filesystem" = 0

            let devicesData =
                devicesBroker.getDeviceInfoOrEx ()
                |> JsonConvert.DeserializeObject<BlockDevices>

            devicesData.BlockDevices
            |> Seq.collect (fun d -> d.Children)
            |> Seq.filter isValidDevice
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.isValidDeviceOrEx (device : SnapshotDevice) =

            (this :>IDevicesService).getValidDevicesDataOrEx ()
            |> Seq.exists (fun d -> d.Path = device.value)
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.outputDevices (devices : DeviceDataChild seq) =

            [
                Phrases.MountedDevices
                ""
            ]
            |> consoleBroker.writeLines

            [|
                [| Phrases.Device ; Phrases.Size ; Phrases.MountPoint ; Phrases.Type ; Phrases.Label |]

                for d in devices do
                    [| d.Path ; d.Size ; d.MountPoint ; d.FileSystemType ; d.Label |]
            |]
            |> consoleBroker.writeMatrixWithFooter [| false ; true ; false ; false ; false |] true [ "" ]
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
