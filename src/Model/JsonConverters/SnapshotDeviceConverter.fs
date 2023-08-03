namespace Model.JsonConverters

open System
open Newtonsoft.Json

open Model

type SnapshotDeviceConverter () =
    inherit JsonConverter ()

    override _.CanConvert (_ : Type) = true

    override _.WriteJson(writer, value, serializer) =
        serializer.Serialize(writer, (value :?> SnapshotDevice).value)

    override _.ReadJson(reader, _, _, serializer) =
        SnapshotDevice.create(serializer.Deserialize<string>(reader))
