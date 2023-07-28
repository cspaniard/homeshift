namespace Services.Config

open System.IO
open Model

type IConfigBroker = DI.Brokers.IConfigBroker


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
    // -----------------------------------------------------------------------------------------------------------------
