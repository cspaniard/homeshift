open DI.Interfaces
open HomeShift
open Localization

open DI.Providers
open Microsoft.Extensions.DependencyInjection

// ---------------------------------------------------------------------------------------------------------------------
ServiceProvider
    .AddSingleton<IList, AppCore.List>()
    .AddSingleton<IListDevices, AppCore.ListDevices>()
    .AddSingleton<IConfig, AppCore.Config>()
|> ignore

ServiceProvider.AddAndRebuild<IApp, App> ()

DI.Providers.ServiceProvider.GetService<IApp>().Run ()
// ---------------------------------------------------------------------------------------------------------------------
