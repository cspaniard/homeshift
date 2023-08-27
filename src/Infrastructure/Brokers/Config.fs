namespace Brokers

open System
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

        Directory.CreateDirectory CONFIG_PATH |> ignore
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IConfigBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.saveConfigDataToFileOrEx (data : ConfigData) =

            createConfigPathOrEx ()

            (CONFIG_FILE,
             JsonConvert.SerializeObject(data, Formatting.Indented) + Environment.NewLine)
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
