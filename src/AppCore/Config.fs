namespace AppCore

open Model
open DI.Interfaces


type Config (configService : IConfigService, consoleBroker : IConsoleBroker, devicesService : IDevicesService,
             helpers : IHelpers) as this =

    //------------------------------------------------------------------------------------------------------------------
    let self = this :> IConfig
    //------------------------------------------------------------------------------------------------------------------

    interface IConfig with
        //--------------------------------------------------------------------------------------------------------------
        member _.storeConfigOrEx (configData : ConfigData) =

            helpers.checkRootUserOrEx ()

            let deviceDataChild = devicesService.findDeviceOrEx configData.SnapshotDevice.value

            let configData = { configData with SnapshotDevice = SnapshotDevice.create deviceDataChild.Uuid }

            configService.storeConfigDataOrEx configData
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.configOrEx (options : ConfigOptions) =

            helpers.checkRootUserOrEx ()

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
