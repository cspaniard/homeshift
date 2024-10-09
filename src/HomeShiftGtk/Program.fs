open System
open System.IO
open Gtk
open HomeShiftGtk

open DI.Interfaces
open DI.Providers
open Microsoft.Extensions.DependencyInjection

ServiceProvider
    .AddSingleton<IHelpers, AppCore.Helpers>()
    .AddSingleton<IList, AppCore.List>()
    .AddSingleton<MainWindow>(fun sp -> MainWindow("MainWindow"))
|> ignore

ServiceProvider.Rebuild()


Application.Init()
let app = new Application("com.motsoft.HomeShiftGtk", GLib.ApplicationFlags.None)
app.Register(GLib.Cancellable.Current) |> ignore

Directory.SetCurrentDirectory AppContext.BaseDirectory

// Load CSS and apply it to the entire application.
DynamicCssManager.loadFromPath("App.css")
DynamicCssManager.applyToScreen()

// We Start the application.
DI.Providers.ServiceProvider.GetService<MainWindow>() |> ignore
Application.Run()
