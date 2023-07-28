module AppCore.Config

open System
open Model

type IConfigService = DI.Services.IConfigService

//----------------------------------------------------------------------------------------------------------------------
let RunOfData (data : ConfigData) =

    Helpers.checkRootUserOrEx ()
    IConfigService.storeConfigData data
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptions (options : ConfigOptions) =

    Helpers.checkRootUserOrEx ()
    Console.WriteLine "Pues seguimos como root."

    IConfigService.getConfigData ()
    |> ConfigData.mergeWithOptions options
    |> RunOfData
//----------------------------------------------------------------------------------------------------------------------
