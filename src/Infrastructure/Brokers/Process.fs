namespace Brokers

open System.Diagnostics

open DI.Interfaces


type ProcessBroker () =

    // -----------------------------------------------------------------------------------------------------------------
    interface IProcessBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.startProcessAndWaitOrEx (processName : string) (arguments : string) =

            Process
                .Start(processName, arguments)
                .WaitForExit()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.startProcessNoOuputAtAll (processName : string) (arguments : string) =

            let startInfo = ProcessStartInfo()
            startInfo.FileName <- processName
            startInfo.Arguments <- arguments
            startInfo.UseShellExecute <- false
            startInfo.RedirectStandardOutput <- true
            startInfo.RedirectStandardError <- true

            Process
                .Start(startInfo)
                .WaitForExit()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.startProcessAndReadToEndOrEx (processName : string) (arguments : string) =

            let startInfo = ProcessStartInfo()
            startInfo.FileName <- processName
            startInfo.Arguments <- arguments
            startInfo.UseShellExecute <- false
            startInfo.RedirectStandardOutput <- true

            let proc = Process.Start(startInfo)

            proc.WaitForExit()
            proc.StandardOutput.ReadToEnd()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.startProcessWithNotificationOrEx (callBack : string -> unit) (processName : string) (arguments : string) =

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
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
