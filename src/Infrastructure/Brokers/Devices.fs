namespace Brokers.Devices

open System.Diagnostics
open System.IO
open Model
open Motsoft.Util

type IProcessBroker = Brokers.Process.Broker

type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getDeviceInfoOrEx () =

        let startInfo = ProcessStartInfo()
        startInfo.FileName <- "lsblk"
        startInfo.Arguments <- "--json --output NAME,KNAME,RO,TYPE,MOUNTPOINT,LABEL,PATH,FSTYPE,PARTTYPENAME,SIZE"
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardOutput <- true

        let proc = Process.Start(startInfo)

        proc.WaitForExit()
        proc.StandardOutput.ReadToEnd()
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
