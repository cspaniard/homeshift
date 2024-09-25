module ConfigServiceTests

open System.IO
open Localization
open NUnit.Framework
open FsUnit
open Microsoft.Extensions.DependencyInjection

open DI.Interfaces
open DI.Providers
open Model
open Newtonsoft.Json
open Services

let [<Literal>] DUMMY_FILE_NAME = "dummy_file_name"
let [<Literal>] UNIT_TEST_DEV = "unit_test_dev"

type ConfigBrokerMock (configFileExists : bool) =

    new () = ConfigBrokerMock(true)

    // -----------------------------------------------------------------------------------------------------------------
    interface IConfigBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.saveConfigDataToFileOrEx (_ : ConfigData) =

            ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigFullFileName () = DUMMY_FILE_NAME
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getConfigDataFromFileOrEx () =

            if configFileExists then
                let data = { (ConfigData.getDefault())
                             with SnapshotDevice = (UNIT_TEST_DEV |> SnapshotDevice.create) }

                JsonConvert.SerializeObject(data, Formatting.Indented)
            else
                raise (FileNotFoundException("Config File Not found."))
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("IConfigService")>]
type ``getConfigDataOrEx tests`` () =

    let configBrokerMock = ConfigBrokerMock () :> IConfigBroker
    let consoleBroker = ServiceProvider.GetService<IConsoleBroker>()
    let configService = ConfigService (configBrokerMock, consoleBroker) :> IConfigService

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getConfigDataOrEx: when config file exists, it should return ConfigData with stored data`` () =

        let configData = configService.getConfigDataOrEx ()
        configData.SnapshotDevice.value |> should equal UNIT_TEST_DEV
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getConfigDataOrEx: when config file does not exist, it should return default data`` () =

        let configBrokerMock = ConfigBrokerMock (configFileExists = false) :> IConfigBroker
        let configService = ConfigService (configBrokerMock, consoleBroker) :> IConfigService

        configService.getConfigDataOrEx ()
        |> should equal (ConfigData.getDefault ())
    // -----------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("IConfigService")>]
type ``getConfigDataSource tests`` () =

    let configBrokerMock = ConfigBrokerMock () :> IConfigBroker
    let consoleBroker = ServiceProvider.GetService<IConsoleBroker> ()
    let configService = ConfigService (configBrokerMock, consoleBroker) :> IConfigService

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getConfigDataSource: gets the config data source`` () =

        configService.getConfigDataSource () |> should equal DUMMY_FILE_NAME
    // -----------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("IConfigService")>]
type ``getConfigDataStringOrEx tests`` () =

    let configBrokerMock = ConfigBrokerMock () :> IConfigBroker
    let consoleBroker = ServiceProvider.GetService<IConsoleBroker> ()
    let configService = ConfigService (configBrokerMock, consoleBroker) :> IConfigService

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getConfigDataStringOrEx: when config file exists, it should return config file content`` () =

        let data = configBrokerMock.getConfigDataFromFileOrEx ()
        configService.getConfigDataStringOrEx () |> should equal data
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getConfigDataStringOrEx: when config file does not exist, it should return an error string`` () =

        let configBrokerMock = ConfigBrokerMock (configFileExists = false) :> IConfigBroker
        let configService = ConfigService (configBrokerMock, consoleBroker) :> IConfigService

        configService.getConfigDataStringOrEx ()
        |> should equal Errors.ConfigFileDoesNotExist
    // -----------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------
