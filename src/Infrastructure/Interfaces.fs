module DI.Interfaces

open CommandLine
open Model


// ---------------------------------------------------------------------------------------------------------------------
type IConsoleBroker =
    abstract member enableStdOut: unit -> unit
    abstract member write: string -> unit
    abstract member writeLine: string -> unit
    abstract member writeLines: string seq -> unit
    abstract member writeMatrix: bool array -> bool -> string array array -> unit
    abstract member writeMatrixWithFooter: bool array -> bool -> string seq -> string array array -> unit

type IConfigBroker =
    abstract member saveConfigDataToFileOrEx: ConfigData -> unit
    abstract member getConfigDataFromFileOrEx: unit -> ConfigData

type IProcessBroker =
    abstract member startProcessAndWaitOrEx: string -> string -> unit
    abstract member startProcessNoOuputAtAll: string -> string -> unit
    abstract member startProcessAndReadToEndOrEx: string -> string -> string
    abstract member startProcessWithNotificationOrEx: (string -> unit) -> string -> string -> unit

type IUsersBroker =
    abstract member getUserInfoFromPasswordFileOrEx: UserName -> string
    abstract member checkUserHomeExistsOrEx: Directory -> Directory

type IDevicesBroker =
    abstract member getDeviceInfoOrEx: unit -> string
    abstract member mountDeviceOrEx: SnapshotDevice -> string
    abstract member unmountCurrentOrEx: unit -> unit

type ISnapshotsBroker =
    abstract member getAllInfoInPathOrEx: Directory -> seq<Snapshot>
    abstract member getLastSnapshotOptionInPathOrEx: Directory -> Directory option
    abstract member createSnapshotOrEx: Directory -> Directory -> CreateData -> (string -> unit) -> Directory option -> unit
    abstract member deleteSnapshotPathOrEx: Directory -> unit
    abstract member deleteUserPathIfEmptyOrEx: Directory -> unit
    abstract member deleteLastSnapshotOrEx: Directory -> unit

type IConfigService =
    abstract member getConfigDataOrEx : unit -> ConfigData
    abstract member storeConfigDataOrEx : ConfigData -> unit

type IHelpService =
    abstract member Heading : string with get
    abstract member helpTextFromResult : ParserResult<'a> -> string
    abstract member showHeading : unit -> unit

type ISnapshotsService =
    abstract member createOrEx : ConfigData -> CreateData -> unit
    abstract member getListForUserOrEx : SnapshotDevice -> UserName -> seq<Snapshot>
    abstract member outputOrEx : UserName -> seq<Snapshot> -> unit
    abstract member deleteOrEx : SnapshotDevice -> DeleteData -> unit
    abstract member deleteAll : SnapshotDevice -> UserName -> unit
    abstract member isValidOrEx : SnapshotDevice -> UserName -> string -> bool

type IDevicesService =
    abstract member getValidDevicesDataOrEx: unit -> seq<DeviceDataChild>
    abstract member isValidDeviceOrEx: SnapshotDevice -> bool
    abstract member outputDevices: seq<DeviceDataChild> -> unit

type IUsersService =
    abstract member getHomeForUserOrEx: UserName -> Directory
    abstract member isValidUser: UserName -> bool
// ---------------------------------------------------------------------------------------------------------------------
