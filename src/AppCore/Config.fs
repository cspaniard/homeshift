namespace AppCore

open Model
open AppCore.Helpers
open DI.Interfaces


type Config (configService : IConfigService, consoleBroker : IConsoleBroker) as this =

    //------------------------------------------------------------------------------------------------------------------
    let self = this :> IConfig
    //------------------------------------------------------------------------------------------------------------------

    interface IConfig with
        //--------------------------------------------------------------------------------------------------------------
        member _.storeConfigOrEx (configData : ConfigData) =

            checkRootUserOrEx ()

            checkDeviceOrEx configData.SnapshotDevice

            configService.storeConfigDataOrEx configData
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.configOrEx (options : ConfigOptions) =

            checkRootUserOrEx ()

            if options |> ConfigOptions.ConfigValueWasPassed then
                configService.getConfigDataOrEx ()
                |> ConfigData.mergeWithOptions options
                |> self.storeConfigOrEx

            if options.ShowConfig then
                [
                    configService.getConfigDataSource ()
                    ""
                ]
                |> consoleBroker.writeLines

                configService.getConfigDataStringOrEx ()
                |> consoleBroker.writeJson

                consoleBroker.writeLine ""
        //--------------------------------------------------------------------------------------------------------------
