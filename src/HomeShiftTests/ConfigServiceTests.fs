module ConfigServiceTests

open System.IO
open Localization
open NUnit.Framework
open FsUnit
open Microsoft.Extensions.DependencyInjection

open DI.Interfaces
open DI.Providers
open Model
open Services

let [<Literal>] UNIT_TEST_DEV = "unit_test_dev"

type ConfigBroker (configFileExists : bool) =

    // -----------------------------------------------------------------------------------------------------------------
    interface IConfigBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.saveConfigDataToFileOrEx (_ : ConfigData) =

            ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigDataFromFileOrEx () =

            if configFileExists then
                { (ConfigData.getDefault())
                  with SnapshotDevice = (UNIT_TEST_DEV |> SnapshotDevice.create) }
            else
                raise (FileNotFoundException("Config File Not found."))
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------

let mutable configBroker = Unchecked.defaultof<IConfigBroker>
let mutable consoleBroker = Unchecked.defaultof<IConsoleBroker>
let mutable configService = Unchecked.defaultof<IConfigService>

[<SetUp>]
let Setup () =
    configBroker <- ConfigBroker(true) :> IConfigBroker
    consoleBroker <- ServiceProvider.GetService<IConsoleBroker>()
    configService <- ConfigService(configBroker, consoleBroker) :> IConfigService

[<Test>]
let ``config file exists should return ConfigData with stored data`` () =

    let configData = configService.getConfigDataOrEx()
    configData.SnapshotDevice.value |> should equal UNIT_TEST_DEV

[<Test>]
let ``config file does not exist should give default data`` () =

    configBroker <- ConfigBroker(false) :> IConfigBroker
    configService <- ConfigService(configBroker, consoleBroker) :> IConfigService

    let configData = configService.getConfigDataOrEx()
    configData |> should equal (ConfigData.getDefault())
