open DI.Interfaces
open HomeShift
open Localization

open DI.Providers

// ---------------------------------------------------------------------------------------------------------------------
ServiceProvider
    .AddSingleton<IList, AppCore.List>()
|> ignore

ServiceProvider.AddAndRebuild<IApp, App> ()

ServiceProvider.GetService<IApp>().Run ()
// ---------------------------------------------------------------------------------------------------------------------
