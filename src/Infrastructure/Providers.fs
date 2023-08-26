module DI.Providers

open Microsoft.Extensions.DependencyInjection

open DI.Interfaces
open Brokers
open Services
open Localization.LocalizedText
open Localization.DI


let serviceProvider =
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
        .BuildServiceProvider()
