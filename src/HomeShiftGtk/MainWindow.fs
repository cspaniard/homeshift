namespace HomeShiftGtk

open System
open GLib        // Necesario. Ambiguedad entre System.Object y GLib.Object
open Gtk


type MainWindow(WindowIdName : string) as this =
    inherit BaseWindow(WindowIdName)

    //------------------------------------------------------------------------------------------------------------------
    // Referencias a controles.
    //------------------------------------------------------------------------------------------------------------------

    let MainLabel = this.Gui.GetObject("MainLabel") :?> Label
    let CreateToolButton = this.Gui.GetObject("CreateToolButton") :?> ToolButton
    let RestoreToolButton = this.Gui.GetObject("RestoreToolButton") :?> ToolButton
    let DeleteToolButton = this.Gui.GetObject("DeleteToolButton") :?> ToolButton
    let ExamineToolButton = this.Gui.GetObject("ExamineToolButton") :?> ToolButton
    let ConfigureToolButton = this.Gui.GetObject("ConfigureToolButton") :?> ToolButton
    let MenuToolButton = this.Gui.GetObject("MenuToolButton") :?> ToolButton

    let VM = MainWindowVM()

    // -----------------------------------------------------------------------------------------------------------------
    // Inicializa el formulario.
    // -----------------------------------------------------------------------------------------------------------------
    do
        CreateToolButton.Label <- "Crear"
        RestoreToolButton.Label <- "Restaurar"
        DeleteToolButton.Label <- "Eliminar"
        ExamineToolButton.Label <- "Examinar"
        ConfigureToolButton.Label <- "Configurar"
        MenuToolButton.Label <- "Menú"
        // -------------------------------------------------------------------------------------------------------------
        // Prepara y muestra la ventana.
        // -------------------------------------------------------------------------------------------------------------
        // this.ThisWindow.Maximize()
        this.EnableCtrlQ()

        this.ThisWindow.Show()
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

    member _.TestButtonClicked (_ : System.Object) (_ : EventArgs) =
        MainLabel.Text <- "Hello, World!"

    member _.CreateToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        MainLabel.Text <- "Creamos un nuevo snapshot."

    member _.RestoreToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        MainLabel.Text <- "Restauramos un snapshot."

    member _.DeleteToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        MainLabel.Text <- "Eliminamos un snapshot."

    member _.ExamineToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        MainLabel.Text <- "Examinamos un snapshot."

    member _.ConfigureToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        MainLabel.Text <- "Configuramos la aplicación."

    member _.MenuToolButtonClicked (_ : System.Object) (_ : EventArgs) =
        MainLabel.Text <- "Mostramos el menú."
