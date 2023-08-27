namespace Brokers

open System.Diagnostics
open System.IO
open Motsoft.Util

open DI.Interfaces
open Model

type DevicesBroker (processBroker : IProcessBroker) =

    // -----------------------------------------------------------------------------------------------------------------
    let mountPoint = $"/run/homeshift/{Process.GetCurrentProcess().Id}"
                     |> Directory.create
    // -----------------------------------------------------------------------------------------------------------------

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

            Directory.CreateDirectory mountPoint.value |> ignore

            try
                processBroker.startProcessAndWaitOrEx "mount" $"{snapshotDevice.value} {mountPoint}"

                mountPoint

            with e ->
                Directory.Delete mountPoint.value
                reraise ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.unmountCurrentOrEx () =

            processBroker.startProcessNoOuputAtAll "umount" mountPoint.value
            Directory.Delete mountPoint.value
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
