namespace Brokers.Users

open System.Diagnostics


type Broker () =

    static member getUserLineFromPasswordFileOrEx (userName : string) =

        let startInfo = ProcessStartInfo()
        startInfo.FileName <- "grep"
        startInfo.Arguments <- $"^{userName}: /etc/passwd"
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardOutput <- true

        let proc = Process.Start(startInfo)

        proc.WaitForExit()
        proc.StandardOutput.ReadToEnd()
