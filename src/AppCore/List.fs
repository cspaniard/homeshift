module AppCore.List

open Helpers
open Model

type private IConfigService = DI.Services.IConfigService
type private ISnapshotsService = DI.Services.ISnapshotsService


//----------------------------------------------------------------------------------------------------------------------
let RunOfDataOrEx (listData : ListData) =

    let configData = IConfigService.getConfigDataOrEx ()

    checkUserOrEx listData.UserName
    checkDeviceOrEx configData.SnapshotDevice

    ISnapshotsService.listOrEx configData.SnapshotDevice listData.UserName
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptionsOrEx (listOptions : ListOptions) =

    listOptions
    |> ListData.ofOptions
    |> RunOfDataOrEx
    |> ISnapshotsService.outputOrEx (listOptions.UserName |> UserName.create)
//----------------------------------------------------------------------------------------------------------------------
