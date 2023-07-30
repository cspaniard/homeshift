namespace Model

open Newtonsoft.Json

type DeviceDataChild =
    {
        [<JsonProperty("name")>]
        Name : string

        [<JsonProperty("kname")>]
        Kname : string

        [<JsonProperty("ro")>]
        ReadOnly : bool

        [<JsonProperty("type")>]
        DeviceType : string

        [<JsonProperty("mountpoints")>]
        MountPoints : string array
    }

type DeviceData =
    {
        [<JsonProperty("name")>]
        Name : string

        [<JsonProperty("kname")>]
        Kname : string

        [<JsonProperty("ro")>]
        ReadOnly : bool

        [<JsonProperty("type")>]
        DeviceType : string

        [<JsonProperty("mountpoints")>]
        MountPoints : string[]

        [<JsonProperty("children")>]
        Children : DeviceDataChild[]
    }

type BlockDevices =
    {
        [<JsonProperty("blockdevices")>]
        BlockDevices : DeviceData[]
    }
