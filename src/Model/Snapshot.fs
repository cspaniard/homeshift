namespace Model

open System

type Snapshot = {
    Name : string
    // Comments : string          // ToDo: Leave out for now
}


type SnapshotFileInfo = {
    CreationDateTime : DateTimeOffset
    Comments : string
}
