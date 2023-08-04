namespace Brokers.Config

open System.IO
open Newtonsoft.Json
open Model

type Broker () =

    static let [<Literal>] CONFIG_PATH = "/etc/homeshift"
    static let [<Literal>] CONFIG_FILE = CONFIG_PATH + "/homeshift.cfg"

    // -----------------------------------------------------------------------------------------------------------------
    static let createConfigPathOrEx () =

        if Directory.Exists CONFIG_PATH = false then
            Directory.CreateDirectory CONFIG_PATH |> ignore
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member saveConfigDataToFileOrEx (data : ConfigData) =

        createConfigPathOrEx ()

        (CONFIG_FILE,
         JsonConvert.SerializeObject(data, Formatting.Indented))
        |> File.WriteAllText
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member getConfigDataFromFileOrEx () =

        createConfigPathOrEx ()

        CONFIG_FILE
        |> File.ReadAllText
        |> JsonConvert.DeserializeObject<ConfigData>
    // -----------------------------------------------------------------------------------------------------------------
