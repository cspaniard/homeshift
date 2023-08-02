module AppCore.ListDevices

type private IDevicesService = DI.Services.IDevicesService

let Run () =

    IDevicesService.getValidDevicesDataOrEx ()
    |> IDevicesService.outputDevices
