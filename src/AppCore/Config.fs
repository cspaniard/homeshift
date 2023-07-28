module AppCore.Config

open System
open Model

let private getCurrentConfigData () =

    {
        SnapshotPath = ConfigData.getDefault().SnapshotPath
        ScheduleMonthly = true
        ScheduleWeekly = false
        ScheduleDaily  = true
        ScheduleHourly = false
    } : ConfigData

//----------------------------------------------------------------------------------------------------------------------
let RunOfData (data : ConfigData) =

    Helpers.checkRootUserOrException ()
    Console.WriteLine $"%A{data}"
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptions (options : ConfigOptions) =

    Helpers.checkRootUserOrException ()
    Console.WriteLine "Pues seguimos como root."

    getCurrentConfigData ()
    |> ConfigData.mergeWithOptions options
    |> RunOfData
//----------------------------------------------------------------------------------------------------------------------
