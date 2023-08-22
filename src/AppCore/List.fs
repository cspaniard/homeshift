module AppCore.List

open Helpers
open Model

open DI.Dependencies

//----------------------------------------------------------------------------------------------------------------------
let IConfigService = IConfigServiceDI.D ()
let ISnapshotsService = ISnapshotsServiceDI.D ()
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let getSnapshotListOrEx (listData : ListData) =

    let configData = IConfigService.getConfigDataOrEx ()

    checkUserOrEx listData.UserName
    checkDeviceOrEx configData.SnapshotDevice

    ISnapshotsService.getListForUserOrEx configData.SnapshotDevice listData.UserName
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let showSnapshotListOrEx (listOptions : ListOptions) =

        listOptions
        |> ListData.ofOptions
        |> getSnapshotListOrEx
        |> ISnapshotsService.outputOrEx (listOptions.UserName |> UserName.create)
    //------------------------------------------------------------------------------------------------------------------
