module AppCore.Create

open Model
open AppCore.Helpers

type private IConfigService = DI.Services.IConfigService
type private ISnapshotsService = DI.Services.ISnapshotsService

//----------------------------------------------------------------------------------------------------------------------
let createSnapshotOrEx (createData : CreateData) =

    checkRootUserOrEx ()

    let configData = IConfigService.getConfigDataOrEx ()

    checkDeviceOrEx configData.SnapshotDevice

    ISnapshotsService.createOrEx configData createData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let createSnapshotOrEx (createOptions : CreateOptions) =

        createOptions
        |> CreateData.ofOptions
        |> createSnapshotOrEx
    //------------------------------------------------------------------------------------------------------------------
