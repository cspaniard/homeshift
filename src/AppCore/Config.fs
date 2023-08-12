module AppCore.Config

open DI.Services.LocalizationDI
open Model
open AppCore.Helpers

type private IConfigService = DI.Services.IConfigService
type private IConsoleBroker = DI.Brokers.IConsoleBroker

//----------------------------------------------------------------------------------------------------------------------
let RunOfDataOrEx (configData : ConfigData) =

    checkRootUserOrEx ()

    configData.SnapshotDevice
    |> checkDeviceOrEx

    IConfigService.storeConfigDataOrEx configData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptionsOrEx (options : ConfigOptions) =

    checkRootUserOrEx ()

    IConfigService.getConfigDataOrEx ()
    |> ConfigData.mergeWithOptions options
    |> RunOfDataOrEx

    [
        IPhrases.ConfigSaved
        ""
    ]
    |> IConsoleBroker.writeLines
//----------------------------------------------------------------------------------------------------------------------
