namespace Brokers.Process

open System.Diagnostics

type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static member startProcessAndWaitOrEx (processName : string) (arguments : string) =

        Process.Start(processName, arguments).WaitForExit()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member startProcessNoOuputAtAll (processName : string) (arguments : string) =

        let startInfo = ProcessStartInfo()
        startInfo.FileName <- processName
        startInfo.Arguments <- arguments
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true

        let proc = Process.Start(startInfo)

        proc.WaitForExit()
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

    // -----------------------------------------------------------------------------------------------------------------
    static member startProcessWithNotificationOrEx (callBack : string -> unit) (processName : string) (arguments : string) =

        let eventHandler = DataReceivedEventHandler(fun _ args -> callBack args.Data)

        let proc = new Process()
        let startInfo = proc.StartInfo
        startInfo.FileName <- processName
        startInfo.Arguments <- arguments
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true

        proc.EnableRaisingEvents <- true
        proc.OutputDataReceived.AddHandler eventHandler

        proc.Start() |> ignore
        proc.BeginOutputReadLine()
        proc.WaitForExit()

        proc.OutputDataReceived.RemoveHandler eventHandler
        ()
    // -----------------------------------------------------------------------------------------------------------------
