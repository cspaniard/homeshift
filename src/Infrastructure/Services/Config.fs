namespace Services.Config

open System.IO
open Model

type IConfigBroker = DI.Brokers.IConfigBroker


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getConfigData () =

        try
            IConfigBroker.getCurrentConfigOrEx ()

        with
        | :? FileNotFoundException ->
            let defaultData = ConfigData.getDefault ()
            IConfigBroker.createConfigFileOrEx defaultData
            defaultData
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member storeConfigData (data : ConfigData) =
        IConfigBroker.createConfigFileOrEx data
    // -----------------------------------------------------------------------------------------------------------------
