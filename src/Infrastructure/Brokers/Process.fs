namespace Brokers.Process

open System.Diagnostics

type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static member startProcessAndWaitOrEx (processName : string) (arguments : string) =

        Process.Start(processName, arguments).WaitForExit()
    // -----------------------------------------------------------------------------------------------------------------
