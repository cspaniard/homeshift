namespace Brokers.Process

open System.Diagnostics

type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static member startProcessAndWaitOrEx (processName : string) (arguments : string) =

        Process.Start(processName, arguments).WaitForExit()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member startProcessAndReadToEndOrEx (processName : string) (arguments : string) =

        let startInfo = ProcessStartInfo()
        startInfo.FileName <- processName
        startInfo.Arguments <- arguments
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardOutput <- true

        let proc = Process.Start(startInfo)

        proc.WaitForExit()
        proc.StandardOutput.ReadToEnd()
    // -----------------------------------------------------------------------------------------------------------------
