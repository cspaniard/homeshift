module AppCore.Config

open Model
open Motsoft.Util
open Localization

type private IConfigService = DI.Services.IConfigService
type private IDevicesService = DI.Services.IDevicesService

//----------------------------------------------------------------------------------------------------------------------
let RunOfDataOrEx (data : ConfigData) =

    Helpers.checkRootUserOrEx ()

    IDevicesService.getValidDevicesDataOrEx ()
    |> Array.exists (fun d -> $"/dev/{d.Kname}" = data.SnapshotDevice)
    |> failWithIfFalse $"{Errors.InvalidDevice} ({data.SnapshotDevice})"

    IConfigService.storeConfigDataOrEx data
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptionsOrEx (options : ConfigOptions) =

    Helpers.checkRootUserOrEx ()

    IConfigService.getConfigDataOrEx ()
    |> ConfigData.mergeWithOptions options
    |> RunOfDataOrEx
//----------------------------------------------------------------------------------------------------------------------
