module AppCore.Delete

open Model
open Helpers

open Services

//----------------------------------------------------------------------------------------------------------------------
let IConfigService = ConfigServiceDI.Dep.D ()
let ISnapshotsService = SnapshotsServiceDI.Dep.D ()
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let deleteSnapshotOrEx (deleteData : DeleteData) =

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
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let deleteSnapshotOrEx (deleteOptions : DeleteOptions) =

        deleteOptions
        |> DeleteData.ofOptions
        |> deleteSnapshotOrEx
    //------------------------------------------------------------------------------------------------------------------
