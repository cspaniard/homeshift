namespace Services

open System.IO

open DI.Interfaces
open Model
open Localization


type ConfigService (configBroker : IConfigBroker, consoleBroker : IConsoleBroker) =

    // -----------------------------------------------------------------------------------------------------------------
    interface IConfigService with

        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigDataOrEx () =

            try
                configBroker.getConfigDataFromFileOrEx ()

            with
            | :? FileNotFoundException ->
                let defaultData = ConfigData.getDefault ()
                configBroker.saveConfigDataToFileOrEx defaultData
                defaultData
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.storeConfigDataOrEx (data : ConfigData) =

            configBroker.saveConfigDataToFileOrEx data

            [
                Phrases.ConfigSaved
                ""
            ]
            |> consoleBroker.writeLines
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
