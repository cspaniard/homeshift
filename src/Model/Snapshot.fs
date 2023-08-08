namespace Model

open System
open Model.JsonConverters
open Newtonsoft.Json

type Snapshot = {
    CreationDateTime : DateTimeOffset
    Name : string
    Comments : Comments
}


type SnapshotInfoFileData = {
    CreationDateTime : DateTimeOffset

    [<JsonConverter(typeof<CommentsConverter>)>]
    Comments : Comments
}
