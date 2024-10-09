namespace HomeShiftGtk

open System
open Gtk
open Motsoft.Binder.NotifyObject

open Model
open DI.Interfaces


type MainWindowVM(SnapshotsListStore : ListStore, iListService : IList) =
    inherit NotifyObject()

    let mutable userName = try Environment.GetCommandLineArgs()[1] with _ -> ""

    //------------------------------------------------------------------------------------------------------------------
    member this.UserName
        with get() = userName
        and set value =
            if userName <> value then
                userName <- value
                this.NotifyPropertyChanged()
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member this.getSnapshotList() =

        SnapshotsListStore.Clear()

        let listData = { UserName = UserName.create this.UserName } : ListData
        let snapshots = iListService.getSnapshotListOrEx listData

        snapshots
        |> Seq.iter (fun s ->
            SnapshotsListStore.AppendValues [|
                s.Name
                s.Comments.value
                s.CreationDateTime.LocalDateTime.ToString()
            |] |> ignore)
    //------------------------------------------------------------------------------------------------------------------
