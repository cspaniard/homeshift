namespace Services.Config

open System.IO
open Model

type private IPhrases = DI.Services.LocalizationDI.IPhrases
type private IConfigBroker = DI.Brokers.IConfigBroker
type private IConsoleBroker = DI.Brokers.IConsoleBroker


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getConfigDataOrEx () =

        try
            IConfigBroker.getConfigDataFromFileOrEx ()

        with
        | :? FileNotFoundException ->
            let defaultData = ConfigData.getDefault ()
            IConfigBroker.saveConfigDataToFileOrEx defaultData
            defaultData
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member storeConfigDataOrEx (data : ConfigData) =

        IConfigBroker.saveConfigDataToFileOrEx data

        [
            IPhrases.ConfigSaved
            ""
        ]
        |> IConsoleBroker.writeLines
    // -----------------------------------------------------------------------------------------------------------------
