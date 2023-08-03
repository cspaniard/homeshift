namespace Brokers.Users

open System.Diagnostics
open Model


type Broker () =

    static member getUserLineFromPasswordFileOrEx (userName : UserName) =

        let startInfo = ProcessStartInfo()
        startInfo.FileName <- "grep"
        startInfo.Arguments <- $"^{userName.value}: /etc/passwd"
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardOutput <- true

        let proc = Process.Start(startInfo)

        proc.WaitForExit()
        proc.StandardOutput.ReadToEnd()
