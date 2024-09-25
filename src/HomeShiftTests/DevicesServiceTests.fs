module DevicesServiceTests

open System
open Localization
open NUnit.Framework
open FsUnit
open Microsoft.Extensions.DependencyInjection

open DI.Interfaces
open DI.Providers
open Model
open Services
open MockBrokers.DevicesBrokerMock


// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("IDevicesService")>]
type ``getValidDevicesDataOrEx tests`` () =

    let devicesBrokerMock = DevicesBrokerMock () :> IDevicesBroker
    let consoleBroker = ServiceProvider.GetService<IConsoleBroker>()
    let devicesService = DevicesService (devicesBrokerMock, consoleBroker) :> IDevicesService

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getValidDevicesDataOrEx: under normal conditions should return valid data`` () =

        let data = devicesService.getValidDevicesDataOrEx ()

        data |> should not' (be Null)
        data |> Seq.length |> should equal 2

        let deviceNames = data |> Seq.map _.Name
        deviceNames |> should contain "sda1"
        deviceNames |> should contain "sda2"
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getValidDevicesDataOrEx: if error, an Exception should be propagated.`` () =

        let devicesBrokerMockWithError = DevicesBrokerMock (throwError = true) :> IDevicesBroker
        let devicesService = DevicesService (devicesBrokerMockWithError, consoleBroker) :> IDevicesService

        (fun () -> devicesService.getValidDevicesDataOrEx () |> ignore)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("IDevicesService")>]
type ``isValidDeviceOrEx tests`` () =

    let devicesBrokerMock = DevicesBrokerMock () :> IDevicesBroker
    let consoleBroker = ServiceProvider.GetService<IConsoleBroker> ()
    let devicesService = DevicesService (devicesBrokerMock, consoleBroker) :> IDevicesService

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``isValidDeviceOrEx: with a valid device, it should return true`` () =

        SnapshotDevice.create "/dev/sda1"
        |> devicesService.isValidDeviceOrEx
        |> should equal true

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``isValidDeviceOrEx: with an invalid device, it should return false`` () =

        SnapshotDevice.create "bad_snapshot_device"
        |> devicesService.isValidDeviceOrEx
        |> should equal false
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``isValidDeviceOrEx: when an error occurs, an Exception should be propagated.`` () =

        let devicesBrokerMockWithError = DevicesBrokerMock (throwError = true) :> IDevicesBroker
        let devicesService = DevicesService (devicesBrokerMockWithError, consoleBroker) :> IDevicesService

        (fun () -> (devicesService.isValidDeviceOrEx (SnapshotDevice.create "/dev/sda1")) |> ignore)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------
