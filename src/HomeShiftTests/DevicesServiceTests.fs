module DevicesServiceTests

open System
open Localization
open NUnit.Framework
open FsUnit
open Microsoft.Extensions.DependencyInjection
open Motsoft.Util

open DI.Interfaces
open DI.Providers
open Model
open Services

type DevicesBrokerMock (throwError: bool) =

    new () = DevicesBrokerMock(false)

    // -----------------------------------------------------------------------------------------------------------------
    interface IDevicesBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getDeviceInfoOrEx() =

            throwError |> failWithIfTrue "Mock Exception"

            """{
              "blockdevices": [
                {
                  "name": "sda",
                  "kname": "sda",
                  "ro": false,
                  "type": "disk",
                  "mountpoint": "/",
                  "label": "root",
                  "path": "/dev/sda",
                  "fstype": "ext4",
                  "parttypename": "Linux",
                  "size": "50G",
                  "children": [
                    {
                      "name": "sda1",
                      "kname": "sda1",
                      "ro": false,
                      "type": "part",
                      "mountpoint": "/boot",
                      "label": "boot",
                      "path": "/dev/sda1",
                      "fstype": "ext4",
                      "parttypename": "Linux filesystem",
                      "size": "500M"
                    },
                    {
                      "name": "sda2",
                      "kname": "sda2",
                      "ro": false,
                      "type": "part",
                      "mountpoint": "/",
                      "label": "root",
                      "path": "/dev/sda2",
                      "fstype": "ext4",
                      "parttypename": "Linux filesystem",
                      "size": "45G"
                    }
                  ]
                },
                {
                  "name": "sdb",
                  "kname": "sdb",
                  "ro": false,
                  "type": "disk",
                  "mountpoint": "",
                  "label": "",
                  "path": "/dev/sdb",
                  "fstype": "",
                  "parttypename": "",
                  "size": "10G",
                  "children": []
                }
              ]
            }"""
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.mountDeviceOrEx(_: SnapshotDevice) =
            failwith "Not implemented"
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.unmountCurrentOrEx() =
            failwith "Not implemented"
        // -------------------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------------------


[<TestFixture>]
[<Category("IDevicesService")>]
type ``getValidDevicesDataOrEx tests`` () =

    let devicesBrokerMock = DevicesBrokerMock() :> IDevicesBroker
    let consoleBroker = ServiceProvider.GetService<IConsoleBroker>()
    let devicesService = DevicesService(devicesBrokerMock, consoleBroker) :> IDevicesService

    [<Test>]
    member _.``getValidDevicesDataOrEx: under normal conditions should return valid data`` () =

        let data = devicesService.getValidDevicesDataOrEx ()

        data |> should not' (be Null)
        data |> Seq.length |> should equal 2

        let deviceNames = data |> Seq.map _.Name
        deviceNames |> should contain "sda1"
        deviceNames |> should contain "sda2"

    [<Test>]
    member _.``getValidDevicesDataOrEx: if error, an Exception should be propagated.`` () =

        let devicesBrokerMock = DevicesBrokerMock(true) :> IDevicesBroker
        let devicesService = DevicesService(devicesBrokerMock, consoleBroker) :> IDevicesService

        (fun () -> devicesService.getValidDevicesDataOrEx () |> ignore)
        |> should throw typeof<Exception>

[<TestFixture>]
[<Category("IDevicesService")>]
type ``isValidDeviceOrEx tests`` () =

    let devicesBrokerMock = DevicesBrokerMock() :> IDevicesBroker
    let consoleBroker = ServiceProvider.GetService<IConsoleBroker>()
    let devicesService = DevicesService(devicesBrokerMock, consoleBroker) :> IDevicesService

    [<Test>]
    member _.``isValidDeviceOrEx: with a valid device, it should return true`` () =

        SnapshotDevice.create "/dev/sda1"
        |> devicesService.isValidDeviceOrEx
        |> should equal true

    [<Test>]
    member _.``isValidDeviceOrEx: with an invalid device, it should return false`` () =

        SnapshotDevice.create "bad_snapshot_device"
        |> devicesService.isValidDeviceOrEx
        |> should equal false

    [<Test>]
    member _.``isValidDeviceOrEx: when an error occurs, an Exception should be propagated.`` () =

        let devicesBrokerMock = DevicesBrokerMock(true) :> IDevicesBroker
        let devicesService = DevicesService(devicesBrokerMock, consoleBroker) :> IDevicesService

        (fun () -> (devicesService.isValidDeviceOrEx (SnapshotDevice.create "/dev/sda1")) |> ignore)
        |> should throw typeof<Exception>
