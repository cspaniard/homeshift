namespace Brokers

open System.Diagnostics
open System.IO
open Model
open Motsoft.Util

open DI

type DevicesBroker private (processBroker : IProcessBroker) =

    // -----------------------------------------------------------------------------------------------------------------
    let IProcessBroker = processBroker
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let mutable instance = Unchecked.defaultof<IDevicesBroker>
    
    static member getInstance (processBroker : IProcessBroker) =
        
        if obj.ReferenceEquals(instance, null) then
            instance <- DevicesBroker(processBroker)
        
        instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IDevicesBroker with
    
        // -------------------------------------------------------------------------------------------------------------
        member _.getDeviceInfoOrEx () =

            IProcessBroker.startProcessAndReadToEndOrEx
                "lsblk"
                "--json --output NAME,KNAME,RO,TYPE,MOUNTPOINT,LABEL,PATH,FSTYPE,PARTTYPENAME,SIZE"
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
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
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.unmountCurrentOrEx () =

            let pid = Process.GetCurrentProcess().Id
            let mountPoint = $"/run/homeshift/{pid}"   // ToDo: Make it a Broker wide value.

            IProcessBroker.startProcessNoOuputAtAll "umount" mountPoint
            Directory.Delete mountPoint
        // -------------------------------------------------------------------------------------------------------------
    
    // -----------------------------------------------------------------------------------------------------------------
