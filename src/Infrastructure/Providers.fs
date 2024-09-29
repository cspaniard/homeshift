module DI.Providers

open Microsoft.Extensions.DependencyInjection

open DI.Interfaces
open Brokers
open Services
open Localization.LocalizedText
open Localization.DI


type ServiceProvider () =

    static let serviceCollection =
        ServiceCollection()
            .AddSingleton<IConsoleBroker, ConsoleBroker>()
            .AddSingleton<IConfigBroker, ConfigBroker>()
            .AddSingleton<IProcessBroker, ProcessBroker>()
            .AddSingleton<IUsersBroker, UsersBroker>()
            .AddSingleton<IDevicesBroker, DevicesBroker>()
            .AddSingleton<ISnapshotsBroker, SnapshotsBroker>()

            .AddSingleton<ISentenceBuilder, LocalizedSentenceBuilder>()
            .AddSingleton<IConfigService, ConfigService>()
            .AddSingleton<IHelpService, HelpService>()
            .AddSingleton<ISnapshotsService, SnapshotsService>()
            .AddSingleton<IDevicesService, DevicesService>()
            .AddSingleton<IUsersService, UsersService>()

    static let mutable _serviceProvider = serviceCollection.BuildServiceProvider()
    static member ServiceProvider = _serviceProvider
    static member GetService<'T> () = _serviceProvider.GetService<'T>()

    static member AddSingleton<'TService, 'TImplementation> () =
        serviceCollection.AddSingleton(typeof<'TService>, typeof<'TImplementation>)

    static member Rebuild<'TService, 'TImplementation> () =
        _serviceProvider <- serviceCollection.BuildServiceProvider()

    static member AddAndRebuild<'TService, 'TImplementation> () =
        ServiceProvider.AddSingleton<'TService, 'TImplementation> () |> ignore
        ServiceProvider.Rebuild()
