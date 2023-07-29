module AppCore.Config

open Model

type private IConfigService = DI.Services.IConfigService

//----------------------------------------------------------------------------------------------------------------------
let RunOfDataOrEx (data : ConfigData) =

    Helpers.checkRootUserOrEx ()
    IConfigService.storeConfigDataOrEx data
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptionsOrEx (options : ConfigOptions) =

    Helpers.checkRootUserOrEx ()

    IConfigService.getConfigDataOrEx ()
    |> ConfigData.mergeWithOptions options
    |> RunOfDataOrEx
//----------------------------------------------------------------------------------------------------------------------
