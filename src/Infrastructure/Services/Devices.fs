namespace Services

open Newtonsoft.Json
open Motsoft.Util

open DI.Interfaces
open Model
open Localization
open Spectre.Console


type DevicesService (devicesBroker : IDevicesBroker, consoleBroker : IConsoleBroker) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> IDevicesService
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IDevicesService with

        // -------------------------------------------------------------------------------------------------------------
        member _.getValidDevicesDataOrEx () =

            let isValidDevice (dataChild : DeviceDataChild) =

                dataChild.ReadOnly = false &&
                dataChild.MountPoint <> null &&
                dataChild.PartTypeName |> compareNoCaseNoAccents "Linux filesystem" = 0

            let devicesData =
                devicesBroker.getDeviceInfoOrEx ()
                |> JsonConvert.DeserializeObject<BlockDevices>

            devicesData.BlockDevices
            |> Seq.collect _.Children
            |> Seq.filter isValidDevice
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.isValidDeviceOrEx (device : SnapshotDevice) =

            self.getValidDevicesDataOrEx ()
            |> Seq.exists (fun d -> d.Path = device.value)
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.outputDevices (devices : DeviceDataChild seq) =

            [
                Phrases.MountedDevices
            ]
            |> consoleBroker.writeLines

            let columns = [|
                TableColumn(Phrases.Device).PadLeft(0)
                TableColumn(Phrases.Size).RightAligned()
                TableColumn(Phrases.MountPoint)
                TableColumn(Phrases.Type)
                TableColumn(Phrases.Label)
            |]

            let data = [|
                for d in devices do
                    [|
                        d.Path
                        d.Size
                        d.MountPoint
                        d.FileSystemType
                        d.Label |> Option.ofObj |> Option.defaultValue ""
                    |]
            |]

            consoleBroker.writeTable (columns, data)
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
