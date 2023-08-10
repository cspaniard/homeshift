module AppCore.List

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

    RunOfDataOrEx (options |> ListData.ofOptions)
    |> ISnapshotsService.outputSnapshots (options.UserName |> UserName.create)
//----------------------------------------------------------------------------------------------------------------------
