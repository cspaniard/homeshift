namespace Model.JsonConverters

open System
open Newtonsoft.Json

open Model


type GenericConverter<'T, 'U when 'T :> IValueType<'T, 'U>> () =
    inherit JsonConverter ()

    override _.CanConvert (_ : Type) = true

    override _.WriteJson(writer, value, serializer) =
        serializer.Serialize(writer, value.ToString())

    override _.ReadJson(reader, _, _, serializer) =
        'T.create(serializer.Deserialize<'U>(reader))
