open DI.Interfaces
open HomeShift
open Localization

open DI.Providers

// ---------------------------------------------------------------------------------------------------------------------
ServiceProvider.AddAndRebuild<IApp, App> ()
ServiceProvider.GetService<IApp>().Run ()
// ---------------------------------------------------------------------------------------------------------------------
