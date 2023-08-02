namespace Brokers.Devices

open System.Diagnostics


type Broker () =

    static member getDeviceInfoOrEx () =

        let startInfo = ProcessStartInfo()
        startInfo.FileName <- "lsblk"
        startInfo.Arguments <- "--json --output NAME,KNAME,RO,TYPE,MOUNTPOINT,LABEL,PATH,FSTYPE,PARTTYPENAME,SIZE"
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardOutput <- true

        let proc = Process.Start(startInfo)

        proc.WaitForExit()
        proc.StandardOutput.ReadToEnd()
