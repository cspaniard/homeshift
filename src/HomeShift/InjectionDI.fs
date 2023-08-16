module InjectionDI

open Services
open Brokers


let init() =

    ConsoleBrokerDI.Dep.D <- (fun () -> ConsoleBroker.getInstance())
    ConfigBrokerDI.Dep.D <- (fun () -> ConfigBroker.getInstance())
    UsersBrokerDI.Dep.D <- (fun () -> UsersBroker.getInstance())
    ProcessBrokerDI.Dep.D <- (fun () -> ProcessBroker.getInstance())
    DevicesBrokerDI.Dep.D <- (fun () -> DevicesBroker.getInstance())
    SnapshotsBrokerDI.Dep.D <- (fun () -> SnapshotsBroker.getInstance())

    ConfigServiceDI.Dep.D <- (fun () -> ConfigService.getInstance())
    IHelpServiceDI.Dep.D <- (fun () -> HelpService.getInstance())
    SnapshotsServiceDI.Dep.D <- (fun () -> SnapshotsService.getInstance())
    DevicesServiceDI.Dep.D <- (fun () -> DevicesService.getInstance())
    UsersServiceDI.Dep.D <- (fun () -> UsersService.getInstance())
