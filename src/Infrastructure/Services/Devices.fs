namespace Services

open Newtonsoft.Json
open Motsoft.Util
open Model

open Localization
open Brokers


type DevicesService private () as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IDevicesBroker = DevicesBrokerDI.Dep.D ()
    let IConsoleBroker = ConsoleBrokerDI.Dep.D ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let instance = DevicesService()
    static member getInstance () = instance
    // -----------------------------------------------------------------------------------------------------------------

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

        this.getValidDevicesDataOrEx ()
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
        |> IConsoleBroker.WriteMatrixWithFooter [| false ; true ; false ; false ; false |] true [ "" ]
    // -----------------------------------------------------------------------------------------------------------------


module DevicesServiceDI =

    let Dep = DI.Dependency (fun () ->
            failwith $"{Errors.NotInitialized} ({nameof DevicesService})" : DevicesService)
