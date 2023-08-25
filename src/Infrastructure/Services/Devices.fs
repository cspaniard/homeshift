namespace Services

open Newtonsoft.Json
open Motsoft.Util

open DI
open Model
open Localization


type DevicesService private (devicesBroker : IDevicesBroker, consoleBroker : IConsoleBroker) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IDevicesBroker = devicesBroker
    let IConsoleBroker = consoleBroker
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let mutable instance = Unchecked.defaultof<IDevicesService>
    
    static member getInstance (devicesBroker : IDevicesBroker, consoleBroker : IConsoleBroker) =
        
        if obj.ReferenceEquals(instance, null) then
            instance <- DevicesService (devicesBroker, consoleBroker)
        
        instance
    // -----------------------------------------------------------------------------------------------------------------

    interface IDevicesService with
    
        // -----------------------------------------------------------------------------------------------------------------
        member _.getValidDevicesDataOrEx () =

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
        member _.isValidDeviceOrEx (device : SnapshotDevice) =

            (this :>IDevicesService).getValidDevicesDataOrEx ()
            |> Seq.exists (fun d -> d.Path = device.value)
        // -----------------------------------------------------------------------------------------------------------------

        // -----------------------------------------------------------------------------------------------------------------
        member _.outputDevices (devices : DeviceDataChild seq) =

            [
                Phrases.MountedDevices
                ""
            ]
            |> IConsoleBroker.writeLines

            [|
                [| Phrases.Device ; Phrases.Size ; Phrases.MountPoint ; Phrases.Type ; Phrases.Label |]

                for d in devices do
                    [| d.Path ; d.Size ; d.MountPoint ; d.FileSystemType ; d.Label |]
            |]
            |> IConsoleBroker.writeMatrixWithFooter [| false ; true ; false ; false ; false |] true [ "" ]
        // -----------------------------------------------------------------------------------------------------------------
    
    // -----------------------------------------------------------------------------------------------------------------
