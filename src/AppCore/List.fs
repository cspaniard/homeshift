module AppCore.List

open System

open Helpers
open Model

type IConfigService = DI.Services.IConfigService
type ISnapshotsService = DI.Services.ISnapshotsService


//----------------------------------------------------------------------------------------------------------------------
let RunOfDataOrEx (listData : ListData) =

    checkValidUser listData.UserName

    let configData = IConfigService.getConfigDataOrEx ()
    ISnapshotsService.listOrEx configData listData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptionsOrEx (options : ListOptions) =

    Console.WriteLine "Listing snapshots.\n"

    RunOfDataOrEx (options |> ListData.ofOptions)
    |> Seq.iter (fun s -> printfn $"%s{s.Name} - {s.Comments}")
//----------------------------------------------------------------------------------------------------------------------
