module AppCore.Config

open Model
open AppCore.Helpers

type private IConfigService = DI.Services.IConfigService

//----------------------------------------------------------------------------------------------------------------------
let storeConfigOrEx (configData : ConfigData) =

    checkRootUserOrEx ()

    checkDeviceOrEx configData.SnapshotDevice

    IConfigService.storeConfigDataOrEx configData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let storeConfigOrEx (options : ConfigOptions) =

        IConfigService.getConfigDataOrEx ()
        |> ConfigData.mergeWithOptions options
        |> storeConfigOrEx
    //------------------------------------------------------------------------------------------------------------------
