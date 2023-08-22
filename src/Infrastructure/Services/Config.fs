namespace Services

open System.IO
open DI
open Model

open Localization


type ConfigService private (configBroker : IConfigBroker, consoleBroker : IConsoleBroker) =

    //------------------------------------------------------------------------------------------------------------------
    let IConfigBroker = configBroker
    let IConsoleBroker = consoleBroker
    //------------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let mutable instance = Unchecked.defaultof<IConfigService>
    
    static member getInstance (usersBroker : IConfigBroker, consoleBroker : IConsoleBroker) =
        
        if obj.ReferenceEquals(instance, null) then
            instance <- ConfigService(usersBroker, consoleBroker)
        
        instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IConfigService with
    
        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigDataOrEx () =

            try
                IConfigBroker.getConfigDataFromFileOrEx ()

            with
            | :? FileNotFoundException ->
                let defaultData = ConfigData.getDefault ()
                IConfigBroker.saveConfigDataToFileOrEx defaultData
                defaultData
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.storeConfigDataOrEx (data : ConfigData) =

            IConfigBroker.saveConfigDataToFileOrEx data

            [
                Phrases.ConfigSaved
                ""
            ]
            |> IConsoleBroker.writeLines
        // -------------------------------------------------------------------------------------------------------------
    
    // -----------------------------------------------------------------------------------------------------------------
