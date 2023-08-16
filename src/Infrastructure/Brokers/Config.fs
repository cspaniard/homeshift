namespace Brokers

open System.IO
open Newtonsoft.Json
open Model

type ConfigBroker private () =

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
    static let instance = ConfigBroker()
    static member getInstance () = instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.saveConfigDataToFileOrEx (data : ConfigData) =

        createConfigPathOrEx ()

        (CONFIG_FILE,
         JsonConvert.SerializeObject(data, Formatting.Indented))
        |> File.WriteAllText
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.getConfigDataFromFileOrEx () =

        createConfigPathOrEx ()

        CONFIG_FILE
        |> File.ReadAllText
        |> JsonConvert.DeserializeObject<ConfigData>
    // -----------------------------------------------------------------------------------------------------------------

module ConfigBrokerDI =
    open Localization

    let Dep = DI.Dependency (fun () ->
            failwith $"{Errors.NotInitialized} ({nameof ConfigBroker})" : ConfigBroker)
