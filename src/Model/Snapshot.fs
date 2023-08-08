namespace Model

open System

type Snapshot = {
    CreationDateTime : DateTimeOffset
    Name : string
    Comments : string
}


type SnapshotInfoFileData = {
    CreationDateTime : DateTimeOffset
    Comments : string
}
