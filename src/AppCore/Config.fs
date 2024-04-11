module AppCore.Config

open Microsoft.Extensions.DependencyInjection

open Model
open AppCore.Helpers

open DI.Interfaces
open DI.Providers


//----------------------------------------------------------------------------------------------------------------------
let configService = ServiceProvider.GetService<IConfigService>()
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let storeConfigOrEx (configData : ConfigData) =

    checkRootUserOrEx ()

    checkDeviceOrEx configData.SnapshotDevice

    configService.storeConfigDataOrEx configData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let storeConfigOrEx (options : ConfigOptions) =

        checkRootUserOrEx ()

        configService.getConfigDataOrEx ()
        |> ConfigData.mergeWithOptions options
        |> storeConfigOrEx
    //------------------------------------------------------------------------------------------------------------------
