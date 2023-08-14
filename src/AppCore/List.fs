module AppCore.List

open Helpers
open Model

type private IConfigService = DI.Services.IConfigService
type private ISnapshotsService = DI.Services.ISnapshotsService


//----------------------------------------------------------------------------------------------------------------------
let getSnapshotList (listData : ListData) =

    let configData = IConfigService.getConfigDataOrEx ()

    checkUserOrEx listData.UserName
    checkDeviceOrEx configData.SnapshotDevice

    ISnapshotsService.getListForUserOrEx configData.SnapshotDevice listData.UserName
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let showSnapshotList (listOptions : ListOptions) =

        listOptions
        |> ListData.ofOptions
        |> getSnapshotList
        |> ISnapshotsService.outputOrEx (listOptions.UserName |> UserName.create)
    //------------------------------------------------------------------------------------------------------------------
