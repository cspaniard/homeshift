module AppCore.Create

open Model
open AppCore.Helpers

type private IConfigService = DI.Services.IConfigService
type private ICreateService = DI.Services.ICreateService

//----------------------------------------------------------------------------------------------------------------------
let Run (createData : CreateData) =

    checkRootUserOrEx ()

    let configData = IConfigService.getConfigDataOrEx ()

    configData.SnapshotDevice.value
    |> checkDeviceExists

    createData
    |> ICreateService.createSnapshot configData
//----------------------------------------------------------------------------------------------------------------------
