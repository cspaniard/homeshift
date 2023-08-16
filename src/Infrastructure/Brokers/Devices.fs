namespace Brokers

open System.Diagnostics
open System.IO
open Model
open Motsoft.Util

open Brokers

type DevicesBroker private () =

    // -----------------------------------------------------------------------------------------------------------------
    let IProcessBroker = ProcessBrokerDI.Dep.D ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let instance = DevicesBroker()
    static member getInstance () = instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.getDeviceInfoOrEx () =

        IProcessBroker.startProcessAndReadToEndOrEx
            "lsblk"
            "--json --output NAME,KNAME,RO,TYPE,MOUNTPOINT,LABEL,PATH,FSTYPE,PARTTYPENAME,SIZE"
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.mountDeviceOrEx (snapshotDevice : SnapshotDevice) =

        let pid = Process.GetCurrentProcess().Id
        let mountPoint = $"/run/homeshift/{pid}"   // ToDo: Make it a Broker wide value.

        if Directory.Exists mountPoint = false then
            Directory.CreateDirectory mountPoint |> ignore

        try
            IProcessBroker.startProcessAndWaitOrEx "mount" $"{snapshotDevice.value} {mountPoint}"
            mountPoint
        with e ->
            Directory.Delete mountPoint
            reraise ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.unmountCurrentOrEx () =

        let pid = Process.GetCurrentProcess().Id
        let mountPoint = $"/run/homeshift/{pid}"   // ToDo: Make it a Broker wide value.

        IProcessBroker.startProcessNoOuputAtAll "umount" mountPoint
        Directory.Delete mountPoint
    // -----------------------------------------------------------------------------------------------------------------


module DevicesBrokerDI =
    open Localization

    let Dep = DI.Dependency (fun () ->
            failwith $"{Errors.NotInitialized} ({nameof DevicesBroker})" : DevicesBroker)
