module DI.Interfaces

open System
open CommandLine
open Model
open Spectre.Console
open Spectre.Console.Rendering


// ---------------------------------------------------------------------------------------------------------------------
type IApp =
    abstract member run : unit -> unit

type IHelpers =
    abstract member checkRootUserOrEx: unit -> unit
    abstract member checkUserOrEx: userName: UserName -> unit
    abstract member checkDeviceOrEx: snapshotDevice: SnapshotDevice -> unit
    abstract member checkSnapshotOrEx: snapshotDevice: SnapshotDevice -> userName: UserName ->
                                       snapshotName: string -> unit

type IList =
    abstract member getSnapshotListOrEx: listData: ListData -> Snapshot seq
    abstract member showSnapshotListOrEx: listOptions: ListOptions -> unit

type IListDevices =
    abstract member getDeviceListOrEx: unit -> DeviceDataChild seq
    abstract member showDeviceListOrEx: unit -> unit

type IConfig =
    abstract member storeConfigOrEx : configData : ConfigData -> unit
    abstract member configOrEx : options : ConfigOptions -> unit

type ICreate =
    abstract member createSnapshotOrEx : createData : CreateData -> unit
    abstract member createSnapshotOrEx : createOptions : CreateOptions -> unit

type IRestore =
    abstract member restoreSnapshotOrEx : restoreOptions : RestoreOptions -> unit

type IDelete =
    abstract member deleteSnapshotOrEx : deleteData : DeleteData -> unit
    abstract member deleteSnapshotOrEx : deleteOptions : DeleteOptions -> unit

type IConsoleBroker =
    abstract member enableStdOut: unit -> unit
    abstract member write: line: string -> unit
    abstract member writeLine: line: string -> unit
    abstract member writeLines: lines: string seq -> unit
    abstract member writeTable: columns : TableColumn array * data : string array array -> unit
    abstract member writeTable: columns : TableColumn array * data : IRenderable array array -> unit
    abstract member writeInnerExceptions: ex : Exception -> unit
    abstract member writeJson: json : string -> unit

type IConfigBroker =
    abstract member saveConfigDataToFileOrEx: configData: ConfigData -> unit
    abstract member getConfigDataFromFileOrEx: unit -> string
    abstract member getConfigFullFileName: unit -> string

type IProcessBroker =
    abstract member startProcessAndWaitOrEx: processName: string -> arguments: string -> unit
    abstract member startProcessNoOuputAtAll: processName: string -> arguments: string -> unit
    abstract member startProcessAndReadToEndOrEx: processName: string -> arguments: string -> string
    abstract member startProcessWithNotificationOrEx: callBack: (string -> unit) ->
                                                      processName: string -> arguments: string -> unit

type IUsersBroker =
    abstract member getUserInfoFromPasswordFileOrEx: userName: UserName -> string

type IDevicesBroker =
    abstract member getDeviceInfoOrEx: unit -> string
    abstract member mountDeviceOrEx: snapshotDevice: SnapshotDevice -> Directory
    abstract member unmountCurrentOrEx: unit -> unit

type ISnapshotsBroker =
    abstract member getAllInfoInPathOrEx: path: Directory -> seq<Snapshot>
    abstract member getLastSnapshotOptionInPathOrEx: path: Directory -> Directory option
    abstract member createSnapshotOrEx: sourcePath: Directory -> baseSnapshotPath: Directory ->
                                        createData:  CreateData -> progressCallBack: (string -> unit) ->
                                        lastSnapshotPathOption: Directory option -> unit
    abstract member restoreSnapshotOrEx: progressCallBack : (string -> unit) ->
                                         snapshotPath: Directory -> userHomePath: Directory -> unit
    abstract member deleteSnapshotPathOrEx: snapshotsPath: Directory -> unit
    abstract member deleteUserPathIfEmptyOrEx: snapshotsPath: Directory -> unit
    abstract member deleteLastSnapshotOrEx: userSnapshotsPath: Directory -> unit

type IConfigService =
    abstract member getConfigDataSource: unit -> string
    abstract member getConfigDataStringOrEx: unit -> string
    abstract member getConfigDataOrEx: unit -> ConfigData
    abstract member storeConfigDataOrEx: configData: ConfigData -> unit

type IHelpService =
    abstract member Heading: string with get
    abstract member helpTextFromResult: parserResult: ParserResult<'a> -> string
    abstract member showHeading: unit -> unit

type ISnapshotsService =
    abstract member createOrEx: configData: ConfigData -> createData: CreateData -> unit
    abstract member getListForUserOrEx: snapshotDevice: SnapshotDevice -> userName: UserName -> seq<Snapshot>
    abstract member outputOrEx: userName: UserName -> snapshots: seq<Snapshot> -> unit
    abstract member deleteOrEx: snapshotDevice: SnapshotDevice -> deleteData: DeleteData -> unit
    abstract member deleteAll: snapshotDevice: SnapshotDevice -> userName: UserName -> unit
    abstract member isValidOrEx: snapshotDevice: SnapshotDevice -> userName: UserName -> snapshotName: string -> bool
    abstract member restoreOrEx: snapshotDevice: SnapshotDevice -> restoreData: RestoreData -> unit

type IDevicesService =
    abstract member getValidDevicesDataOrEx: unit -> DeviceDataChild seq
    abstract member findDeviceOrEx: device: string -> DeviceDataChild
    abstract member isValidDeviceOrEx: snapshotDevice: SnapshotDevice -> bool
    abstract member outputDevices: devices: DeviceDataChild seq -> unit

type IUsersService =
    abstract member getHomeForUserOrEx: userName: UserName -> Directory
    abstract member isValidUser: userName: UserName -> bool
// ---------------------------------------------------------------------------------------------------------------------
