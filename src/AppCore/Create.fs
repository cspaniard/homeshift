module AppCore.Create

open Model
open AppCore.Helpers

type private IConfigService = DI.Services.IConfigService
type private ISnapshotsService = DI.Services.ISnapshotsService

//----------------------------------------------------------------------------------------------------------------------
let Run (createData : CreateData) =

    checkRootUserOrEx ()

    let configData = IConfigService.getConfigDataOrEx ()

    configData.SnapshotDevice.value
    |> checkDeviceExists

    createData
    |> ISnapshotsService.createOrEx configData
//----------------------------------------------------------------------------------------------------------------------
