namespace HomeShiftGtk

open System
open Gtk
open Motsoft.Binder.NotifyObject

open Model
open DI.Interfaces


type MainWindowVM(list : IList, usersService : IUsersService, configService : IConfigService,
                  devicesService : IDevicesService) as this =
    inherit NotifyObject()

    let mutable deviceSelected = Unchecked.defaultof<DeviceDataChild>
    let mutable configData = Unchecked.defaultof<ConfigData>

    let mutable snapshotsListStore = Unchecked.defaultof<ListStore>
    let mutable userName = try Environment.GetCommandLineArgs()[1] with _ -> ""

    do
        this.SetInitialState()

    //------------------------------------------------------------------------------------------------------------------
    member private this.SetInitialState() =
        configData <- configService.getConfigDataOrEx ()
        this.DeviceSelected <- devicesService.findDeviceOrEx configData.SnapshotDevice.value
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member this.Init(newSnapshotsListStore : ListStore) =
        snapshotsListStore <- newSnapshotsListStore
    //------------------------------------------------------------------------------------------------------------------

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
    member this.DeviceSelected
        with get() = deviceSelected
        and set value =
            deviceSelected <- value
            this.NotifyPropertyChanged()
            this.NotifyPropertyChanged(nameof this.AvailableAmount)
            this.NotifyPropertyChanged(nameof this.DeviceName)
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member this.SnapshotCount
        with get() = snapshotsListStore.IterNChildren().ToString()
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member this.AvailableAmount
        with get() = deviceSelected.Available
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member this.DeviceName
        with get() = deviceSelected.Path
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
                snapshotsListStore.AppendValues [|
                    s.Name
                    s.Comments.value
                    s.CreationDateTime.LocalDateTime.ToString()
                |] |> ignore)

        snapshotsListStore.Clear()
        getSnapshots()

        this.NotifyPropertyChanged(nameof this.SnapshotCount)
    //------------------------------------------------------------------------------------------------------------------
