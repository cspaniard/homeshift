open System
open System.IO
open Gtk
open HomeShiftGtk

Application.Init()
let app = new Application("com.motsoft.HomeShiftGtk", GLib.ApplicationFlags.None)
app.Register(GLib.Cancellable.Current) |> ignore

Directory.SetCurrentDirectory AppContext.BaseDirectory

// Carga el CSS para toda la aplicación.
DynamicCssManager.loadFromPath("App.css")
DynamicCssManager.applyToScreen()

MainWindow("MainWindow") |> ignore
Application.Run()
