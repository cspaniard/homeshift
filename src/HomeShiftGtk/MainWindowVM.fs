namespace HomeShiftGtk

open System
open DI.Providers
open Gtk
open Motsoft.Binder.NotifyObject

open Model
open DI.Interfaces


type MainWindowVM(SnapshotsListStore : ListStore) as this =
    inherit NotifyObject()

    let list = ServiceProvider.GetService<IList> ()
    let usersService = ServiceProvider.GetService<IUsersService> ()
    let configService = ServiceProvider.GetService<IConfigService> ()
    let devicesService = ServiceProvider.GetService<IDevicesService> ()

    let mutable deviceSelected = Unchecked.defaultof<DeviceDataChild>
    let mutable configData = Unchecked.defaultof<ConfigData>

    let mutable userName = try Environment.GetCommandLineArgs()[1] with _ -> ""

    do
        this.Init()

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
        with get() = SnapshotsListStore.IterNChildren().ToString()
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
                SnapshotsListStore.AppendValues [|
                    s.Name
                    s.Comments.value
                    s.CreationDateTime.LocalDateTime.ToString()
                |] |> ignore)

        SnapshotsListStore.Clear()
        getSnapshots()

        this.NotifyPropertyChanged(nameof this.SnapshotCount)
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    member private this.Init() =
        configData <- configService.getConfigDataOrEx ()
        this.DeviceSelected <- devicesService.findDeviceOrEx configData.SnapshotDevice.value
    //------------------------------------------------------------------------------------------------------------------
