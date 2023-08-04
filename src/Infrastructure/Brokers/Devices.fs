namespace Brokers.Devices

open System.Diagnostics
open System.IO
open Model
open Motsoft.Util

type private IProcessBroker = DI.Brokers.IProcessBrokerDI

type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getDeviceInfoOrEx () =

        IProcessBroker.startProcessAndReadToEndOrEx
            "lsblk"
            "--json --output NAME,KNAME,RO,TYPE,MOUNTPOINT,LABEL,PATH,FSTYPE,PARTTYPENAME,SIZE"
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member mountDeviceOrEx (snapshotDevice : SnapshotDevice) =

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
    static member unmountCurrentOrEx () =

        let pid = Process.GetCurrentProcess().Id
        let mountPoint = $"/run/homeshift/{pid}"   // ToDo: Make it a Broker wide value.

        IProcessBroker.startProcessAndWaitOrEx "umount" mountPoint
        Directory.Delete mountPoint
    // -----------------------------------------------------------------------------------------------------------------
