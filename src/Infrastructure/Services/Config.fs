namespace Services

open System.IO
open Model

open Localization

open Brokers


type ConfigService private () =

    //------------------------------------------------------------------------------------------------------------------
    let IConfigBroker = ConfigBrokerDI.Dep.D ()
    let IConsoleBroker = ConsoleBrokerDI.Dep.D ()
    //------------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let instance = ConfigService()
    static member getInstance () = instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.getConfigDataOrEx () =

        try
            IConfigBroker.getConfigDataFromFileOrEx ()

        with
        | :? FileNotFoundException ->
            let defaultData = ConfigData.getDefault ()
            IConfigBroker.saveConfigDataToFileOrEx defaultData
            defaultData
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.storeConfigDataOrEx (data : ConfigData) =

        IConfigBroker.saveConfigDataToFileOrEx data

        [
            Phrases.ConfigSaved
            ""
        ]
        |> IConsoleBroker.writeLines
    // -----------------------------------------------------------------------------------------------------------------


module ConfigServiceDI =

    let Dep = DI.Dependency (fun () ->
            failwith $"{Errors.NotInitialized} ({nameof ConfigService})" : ConfigService)
