open DI.Interfaces
open HomeShift
open Localization

open DI.Providers
open Microsoft.Extensions.DependencyInjection


// ---------------------------------------------------------------------------------------------------------------------
ServiceProvider
    .AddSingleton<IHelpers, AppCore.Helpers>()
    .AddSingleton<IList, AppCore.List>()
    .AddSingleton<IListDevices, AppCore.ListDevices>()
    .AddSingleton<IConfig, AppCore.Config>()
    .AddSingleton<ICreate, AppCore.Create>()
    .AddSingleton<IRestore, AppCore.Restore>()
    .AddSingleton<IDelete, AppCore.Delete>()
    .AddSingleton<IApp, App>()
|> ignore

ServiceProvider.Rebuild()

DI.Providers.ServiceProvider.GetService<IApp>().run ()
// ---------------------------------------------------------------------------------------------------------------------
