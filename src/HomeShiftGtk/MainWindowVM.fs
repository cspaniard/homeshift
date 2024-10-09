namespace HomeShiftGtk

open System
open DI.Providers
open Gtk
open Motsoft.Binder.NotifyObject

open Model
open DI.Interfaces


type MainWindowVM(SnapshotsListStore : ListStore) =
    inherit NotifyObject()

    let list = ServiceProvider.GetService<IList> ()
    let listDevices = ServiceProvider.GetService<IListDevices> ()
    let usersService = ServiceProvider.GetService<IUsersService> ()

    let mutable devices = Seq.empty<DeviceDataChild>
    let mutable deviceSelected = Unchecked.defaultof<DeviceDataChild>

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
                usersService.isValidUserOrEx (UserName.create this.UserName)
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
            let snapshots = list.getSnapshotListOrEx listData

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

    //------------------------------------------------------------------------------------------------------------------
    member this.GetDeviceList() =
        devices <- listDevices.getDeviceListOrEx ()
    //------------------------------------------------------------------------------------------------------------------
