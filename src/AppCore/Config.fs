module AppCore.Config

open Motsoft.Util
open Model
open Localization

type private IConfigService = DI.Services.IConfigService
type private IDevicesService = DI.Services.IDevicesService

//----------------------------------------------------------------------------------------------------------------------
let RunOfDataOrEx (configData : ConfigData) =

    Helpers.checkRootUserOrEx ()

    configData.SnapshotDevice.value
    |> IDevicesService.isValidDeviceOrEx
    |> failWithIfFalse $"{Errors.InvalidDevice} ({configData.SnapshotDevice})"

    IConfigService.storeConfigDataOrEx configData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptionsOrEx (options : ConfigOptions) =

    Helpers.checkRootUserOrEx ()

    IConfigService.getConfigDataOrEx ()
    |> ConfigData.mergeWithOptions options
    |> RunOfDataOrEx
//----------------------------------------------------------------------------------------------------------------------
