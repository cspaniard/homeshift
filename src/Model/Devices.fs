namespace Model

open FSharp.Json

type DeviceDataChild =
    {
        [<JsonField("name")>]
        Name : string

        [<JsonField("kname")>]
        Kname : string

        [<JsonField("ro")>]
        ReadOnly : bool

        [<JsonField("type")>]
        DeviceType : string

        [<JsonField("mountpoints")>]
        MountPoints : string array
    }

type DeviceData =
    {
        [<JsonField("name")>]
        Name : string

        [<JsonField("kname")>]
        Kname : string

        [<JsonField("ro")>]
        ReadOnly : bool

        [<JsonField("type")>]
        DeviceType : string

        [<JsonField("mountpoints")>]
        MountPoints : string[]

        [<JsonField("children")>]
        Children : DeviceDataChild[]
    }

type BlockDevices =
    {
        [<JsonField("blockdevices")>]
        BlockDevices : DeviceData[]
    }
