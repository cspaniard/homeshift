namespace HomeShiftGtk

open System
open DI.Providers
open Gtk
open Motsoft.Binder.NotifyObject

open Model
open DI.Interfaces


type MainWindowVM(SnapshotsListStore : ListStore) =
    inherit NotifyObject()

    let iListService = ServiceProvider.GetService<IList> ()
    let iUsersService = ServiceProvider.GetService<IUsersService> ()

    let mutable userName = try Environment.GetCommandLineArgs()[1] with _ -> ""

    //------------------------------------------------------------------------------------------------------------------
    member this.UserName
        with get() = userName
        and set value =
            if userName <> value then
                userName <- value
                this.NotifyPropertyChanged()
                this.NotifyPropertyChanged(nameof this.IsValidUser)
                this.NotifyPropertyChanged(nameof this.IsInvalidUser)
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member this.IsValidUser
        with get() =
            try
                iUsersService.isValidUserOrEx (UserName.create this.UserName)
            with _ -> false
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member this.IsInvalidUser
        with get() = not this.IsValidUser
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member this.GetSnapshotList() =

        let getSnapshots() =
            let listData = { UserName = UserName.create this.UserName } : ListData
            let snapshots = iListService.getSnapshotListOrEx listData

            snapshots
            |> Seq.iter (fun s ->
                SnapshotsListStore.AppendValues [|
                    s.Name
                    s.Comments.value
                    s.CreationDateTime.LocalDateTime.ToString()
                |] |> ignore)

        SnapshotsListStore.Clear()
        getSnapshots()
    //------------------------------------------------------------------------------------------------------------------
