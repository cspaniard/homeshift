namespace Brokers

open System.Diagnostics
open System.IO
open Motsoft.Util

open DI.Interfaces
open Model

type DevicesBroker (processBroker : IProcessBroker) =

    // -----------------------------------------------------------------------------------------------------------------
    interface IDevicesBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getDeviceInfoOrEx () =

            processBroker.startProcessAndReadToEndOrEx
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
                processBroker.startProcessAndWaitOrEx "mount" $"{snapshotDevice.value} {mountPoint}"
                mountPoint
            with e ->
                Directory.Delete mountPoint
                reraise ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.unmountCurrentOrEx () =

            let pid = Process.GetCurrentProcess().Id
            let mountPoint = $"/run/homeshift/{pid}"   // ToDo: Make it a Broker wide value.

            processBroker.startProcessNoOuputAtAll "umount" mountPoint
            Directory.Delete mountPoint
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
