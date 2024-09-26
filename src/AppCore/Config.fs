module AppCore.Config

open Microsoft.Extensions.DependencyInjection

open Model
open AppCore.Helpers

open DI.Interfaces
open DI.Providers


//----------------------------------------------------------------------------------------------------------------------
let consoleBroker = ServiceProvider.GetService<IConsoleBroker>()
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
    let configOrEx (options : ConfigOptions) =

        checkRootUserOrEx ()

        if options |> ConfigOptions.ConfigValueWasPassed then
            configService.getConfigDataOrEx ()
            |> ConfigData.mergeWithOptions options
            |> storeConfigOrEx

        if options.ShowConfig then
            [
                configService.getConfigDataSource ()
                ""
            ]
            |> consoleBroker.writeLines

            configService.getConfigDataStringOrEx ()
            |> consoleBroker.writeJson

            consoleBroker.writeLine ""
    //------------------------------------------------------------------------------------------------------------------
