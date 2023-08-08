namespace Model.JsonConverters

open System
open Newtonsoft.Json

open Model

// ToDo: Evaluate if this is still used.

type UserConverter () =
    inherit JsonConverter ()

    override _.CanConvert (_ : Type) = true

    override _.WriteJson(writer, value, serializer) =
        serializer.Serialize (writer, (value :?> UserName).value)

    override _.ReadJson(reader, _, _, serializer) =
        UserName.create (serializer.Deserialize<string>(reader))
