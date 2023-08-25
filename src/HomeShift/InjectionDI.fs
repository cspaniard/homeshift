module InjectionDI

open Services
open Brokers
open Localization.LocalizedText

open DI.Dependencies


let init () =

    IConsoleBrokerDI.D <- (fun () -> ConsoleBroker.getInstance())
    IConfigBrokerDI.D <- (fun () -> ConfigBroker.getInstance())
    IProcessBrokerDI.D <- (fun () -> ProcessBroker.getInstance())
    IUsersBrokerDI.D <- (fun () -> UsersBroker.getInstance(IProcessBrokerDI.D()))
    IDevicesBrokerDI.D <- (fun () -> DevicesBroker.getInstance(IProcessBrokerDI.D()))
    ISnapshotsBrokerDI.D <- (fun () -> SnapshotsBroker.getInstance(IProcessBrokerDI.D()))

    ISentenceBuilderDI.D <- (fun () -> LocalizedSentenceBuilder.GetInstance())
    IConfigServiceDI.D <- (fun () -> ConfigService.getInstance (IConfigBrokerDI.D(), IConsoleBrokerDI.D()))
    IHelpServiceDI.D <- (fun () -> HelpService.getInstance(IConsoleBrokerDI.D(), ISentenceBuilderDI.D()))
    ISnapshotsServiceDI.D <- (fun () -> SnapshotsService.getInstance(IDevicesBrokerDI.D(),
                                                                     ISnapshotsBrokerDI.D(),
                                                                     IConsoleBrokerDI.D(),
                                                                     IUsersServiceDI.D()))
    IDevicesServiceDI.D <- (fun () -> DevicesService.getInstance(IDevicesBrokerDI.D(), IConsoleBrokerDI.D()))
    IUsersServiceDI.D <- (fun () -> UsersService.getInstance(IUsersBrokerDI.D()))
