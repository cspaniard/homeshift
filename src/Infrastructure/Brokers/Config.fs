namespace Brokers

open System.IO
open Newtonsoft.Json

open DI.Interfaces
open Model


type ConfigBroker () =

    // -----------------------------------------------------------------------------------------------------------------
    let [<Literal>] CONFIG_PATH = "/etc/homeshift"
    let [<Literal>] CONFIG_FILE = CONFIG_PATH + "/homeshift.cfg"
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    let createConfigPathOrEx () =

        if Directory.Exists CONFIG_PATH = false then
            Directory.CreateDirectory CONFIG_PATH |> ignore
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------

    interface IConfigBroker with
        // -------------------------------------------------------------------------------------------------------------
        member _.saveConfigDataToFileOrEx (data : ConfigData) =

            createConfigPathOrEx ()

            (CONFIG_FILE,
             JsonConvert.SerializeObject(data, Formatting.Indented))
            |> File.WriteAllText
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigDataFromFileOrEx () =

            createConfigPathOrEx ()

            CONFIG_FILE
            |> File.ReadAllText
            |> JsonConvert.DeserializeObject<ConfigData>
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
