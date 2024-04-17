namespace Services

open System.IO
open Newtonsoft.Json

open DI.Interfaces
open Model
open Localization


type ConfigService (configBroker : IConfigBroker, consoleBroker : IConsoleBroker) =

    // -----------------------------------------------------------------------------------------------------------------
    interface IConfigService with

        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigDataSource () =

            configBroker.getConfigFullFileName ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigDataStringOrEx () =

            try
                configBroker.getConfigDataFromFileOrEx ()

            with
            | :? FileNotFoundException -> Errors.ConfigFileDoesNotExist
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigDataOrEx () =

            try
                configBroker.getConfigDataFromFileOrEx ()
                |> JsonConvert.DeserializeObject<ConfigData>

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
