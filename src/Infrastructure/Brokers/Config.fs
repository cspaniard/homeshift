namespace Brokers.Config

open System.IO
open FSharp.Json
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

        let jsonData = Json.serialize data
        File.WriteAllText(CONFIG_FILE, jsonData)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member getConfigDataFromFileOrEx () =

        createConfigPathOrEx ()

        File.ReadAllText(CONFIG_FILE)
        |> Json.deserialize<ConfigData>
    // -----------------------------------------------------------------------------------------------------------------
