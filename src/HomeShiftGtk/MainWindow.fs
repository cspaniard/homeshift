namespace HomeShiftGtk

open System
open GLib        // Necesario. Ambiguedad entre System.Object y GLib.Object
open Gtk
open Motsoft.Binder

open Localization


type MainWindow(VM : MainWindowVM) as this =
    inherit BaseWindow(nameof MainWindow)

    //------------------------------------------------------------------------------------------------------------------
    // Referencias a controles.
    //------------------------------------------------------------------------------------------------------------------

    let CreateToolButton = this.Gui.GetObject("CreateToolButton") :?> ToolButton
    let RestoreToolButton = this.Gui.GetObject("RestoreToolButton") :?> ToolButton
    let DeleteToolButton = this.Gui.GetObject("DeleteToolButton") :?> ToolButton
    let ExamineToolButton = this.Gui.GetObject("ExamineToolButton") :?> ToolButton
    let ConfigureToolButton = this.Gui.GetObject("ConfigureToolButton") :?> ToolButton
    let MenuToolButton = this.Gui.GetObject("MenuToolButton") :?> ToolButton

    let SnapshotsForUserLabel = this.Gui.GetObject("SnapshotsForUserLabel") :?> Label
    let UserNameSearchEntry = this.Gui.GetObject("UserNameSearchEntry") :?> SearchEntry
    let InvalidUserNameImage = this.Gui.GetObject("InvalidUserNameImage") :?> Image

    let SnapshotColumn = this.Gui.GetObject("SnapshotColumn") :?> TreeViewColumn
    let CommentColumn = this.Gui.GetObject("CommentColumn") :?> TreeViewColumn
    let DateTimeColumn = this.Gui.GetObject("DateTimeColumn") :?> TreeViewColumn

    let StatusMainLineLabel = this.Gui.GetObject("StatusMainLineLabel") :?> Label
    let StatusSubLineLabel = this.Gui.GetObject("StatusSubLineLabel") :?> Label

    let SnapshotCountLabel = this.Gui.GetObject("SnapshotCountLabel") :?> Label
    let SnapshotsLabel = this.Gui.GetObject("SnapshotsLabel") :?> Label

    let AvailableAmountLabel = this.Gui.GetObject("AvailableAmountLabel") :?> Label
    let AvailableLabel = this.Gui.GetObject("AvailableLabel") :?> Label
    let DeviceNameLabel = this.Gui.GetObject("DeviceNameLabel") :?> Label

    let SnapshotsListStore = this.Gui.GetObject("SnapshotsListStore") :?> ListStore

    let binder = Binder(VM)

    // -----------------------------------------------------------------------------------------------------------------
    // Inicializa el formulario.
    // -----------------------------------------------------------------------------------------------------------------
    do
        VM.Init SnapshotsListStore

        binder
            .AddBinding(UserNameSearchEntry, "text", nameof VM.UserName)
            .AddBinding(InvalidUserNameImage, "visible", nameof VM.IsInvalidUser)
            .AddBinding(SnapshotCountLabel, "label", nameof VM.SnapshotCount, BindingProperties.OneWay)
            .AddBinding(AvailableAmountLabel, "label", nameof VM.AvailableAmount, BindingProperties.OneWay)
            .AddBinding(DeviceNameLabel, "label", nameof VM.DeviceName, BindingProperties.OneWay)
        |> ignore

        CreateToolButton.Label <- GuiPhrases.Create
        RestoreToolButton.Label <- GuiPhrases.Restore
        DeleteToolButton.Label <- GuiPhrases.Delete
        ExamineToolButton.Label <- GuiPhrases.Examine
        ConfigureToolButton.Label <- GuiPhrases.Configure
        MenuToolButton.Label <- GuiPhrases.Menu

        SnapshotsForUserLabel.Text <- GuiPhrases.SnapshotsForUser

        SnapshotColumn.Title <- GuiPhrases.Snapshot
        CommentColumn.Title <- GuiPhrases.Comment
        DateTimeColumn.Title <- GuiPhrases.DateTime

        StatusMainLineLabel.Text <- "Este es un mensaje de estado."
        StatusSubLineLabel.Text <- "Este es un mensaje de estado, pero pequeño."

        SnapshotsLabel.Text <- GuiPhrases.Snapshots
        AvailableLabel.Text <- GuiPhrases.Available

        // -------------------------------------------------------------------------------------------------------------
        // Prepara y muestra la ventana.
        // -------------------------------------------------------------------------------------------------------------
        // this.ThisWindow.Maximize()

        this.EnableCtrlQ()
        this.ThisWindow.Show()

        Timeout.Add(100u, fun _ ->
            try VM.GetSnapshotList() with e -> this.ErrorDialogBox e.Message
            false
        ) |> ignore
    //------------------------------------------------------------------------------------------------------------------


    //------------------------------------------------------------------------------------------------------------------
    // Funcionalidad General
    //------------------------------------------------------------------------------------------------------------------


    //------------------------------------------------------------------------------------------------------------------
    // Eventos.
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    // Responde al cierre de la ventana.
    // Como es la ventana principal, también cierra la aplicación.
    //------------------------------------------------------------------------------------------------------------------
    member _.OnMainWindowDelete (_ : System.Object) (args : DeleteEventArgs) =

        args.RetVal <- true
        Application.Quit()
    //------------------------------------------------------------------------------------------------------------------

    member _.CreateToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        ()

    member _.RestoreToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        ()

    member _.DeleteToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        ()

    member _.ExamineToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        ()

    member _.ConfigureToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        ()

    member _.MenuToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        ()

    member _.UserNameSearchEntryActivate (_ : System.Object) (_ : EventArgs) =

        try
            VM.GetSnapshotList()
        with e -> this.ErrorDialogBox e.Message
