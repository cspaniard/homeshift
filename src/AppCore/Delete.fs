module AppCore.Delete

open Model
open Helpers

type private IConfigService = DI.Services.IConfigService
type private ISnapshotsService = DI.Services.ISnapshotsService

//----------------------------------------------------------------------------------------------------------------------
let RunOfDataOrEx (deleteData : DeleteData) =

    checkRootUserOrEx ()

    let configData = IConfigService.getConfigDataOrEx ()

    checkUserOrEx deleteData.UserName
    checkDeviceOrEx configData.SnapshotDevice

    if deleteData.DeleteAll then
        ISnapshotsService.deleteAll configData.SnapshotDevice deleteData.UserName
    else
        checkSnapshotOrEx configData.SnapshotDevice deleteData.UserName deleteData.SnapshotName
        ISnapshotsService.deleteOrEx configData.SnapshotDevice deleteData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let RunOfOptionsOrEx (deleteOptions : DeleteOptions) =

    deleteOptions
    |> DeleteData.ofOptions
    |> RunOfDataOrEx
//----------------------------------------------------------------------------------------------------------------------
