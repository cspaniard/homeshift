namespace Model.JsonConverters

open System
open Newtonsoft.Json

open Model

type CommentsConverter () =
    inherit JsonConverter ()

    override _.CanConvert (_ : Type) = true

    override _.WriteJson(writer, value, serializer) =
        serializer.Serialize (writer, (value :?> Comments).value)

    override _.ReadJson(reader, _, _, serializer) =
        Comments.create (serializer.Deserialize<string>(reader))
