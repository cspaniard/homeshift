namespace Model

open System
open Model.JsonConverters
open Newtonsoft.Json


type Snapshot = {
    CreationDateTime : DateTimeOffset
    Name : string
    Comments : Comment
}


type SnapshotInfoFileData = {
    CreationDateTime : DateTimeOffset

    [<JsonConverter(typeof<GenericConverter<Comment, string>>)>]
    Comments : Comment
}
