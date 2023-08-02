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

        [<JsonProperty("mountpoint")>]
        MountPoint : string

        [<JsonProperty("label")>]
        Label : string

        [<JsonProperty("path")>]
        Path : string

        [<JsonProperty("fstype")>]
        FileSystemType : string

        [<JsonProperty("parttypename")>]
        PartTypeName : string

        [<JsonProperty("size")>]
        Size : string
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

        [<JsonProperty("mountpoint")>]
        MountPoint : string

        [<JsonProperty("label")>]
        Label : string

        [<JsonProperty("path")>]
        Path : string

        [<JsonProperty("fstype")>]
        FileSystemType : string

        [<JsonProperty("parttypename")>]
        PartTypeName : string

        [<JsonProperty("size")>]
        Size : string

        [<JsonProperty("children")>]
        Children : DeviceDataChild[]
    }

type BlockDevices =
    {
        [<JsonProperty("blockdevices")>]
        BlockDevices : DeviceData[]
    }
