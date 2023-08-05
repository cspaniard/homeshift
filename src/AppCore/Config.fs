module AppCore.Config

open Model
open AppCore.Helpers

type private IConfigService = DI.Services.IConfigService

//----------------------------------------------------------------------------------------------------------------------
let RunOfDataOrEx (configData : ConfigData) =

    checkRootUserOrEx ()

    configData.SnapshotDevice.value
    |> checkDeviceExists

    IConfigService.storeConfigDataOrEx configData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptionsOrEx (options : ConfigOptions) =

    checkRootUserOrEx ()

    IConfigService.getConfigDataOrEx ()
    |> ConfigData.mergeWithOptions options
    |> RunOfDataOrEx
//----------------------------------------------------------------------------------------------------------------------
