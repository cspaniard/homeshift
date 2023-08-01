module AppCore.ListDevices

open System

type private IDevicesService = DI.Services.IDevicesService

let Run () =

    IDevicesService.getValidDevicesDataOrEx ()
    |> Array.iter (
            fun d -> Console.WriteLine $"/dev/{d.Kname}\t{d.MountPoints[0]}\t{d.Name}"
        )

    // Console.WriteLine $"Listing devices."
