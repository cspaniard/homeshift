module AppCore.Create

open Model
open AppCore.Helpers

type private IConfigService = DI.Services.IConfigService
type private ISnapshotsService = DI.Services.ISnapshotsService
type private IConsoleBroker = DI.Brokers.IConsoleBroker

//----------------------------------------------------------------------------------------------------------------------
let Run (createData : CreateData) =

    checkRootUserOrEx ()

    let configData = IConfigService.getConfigDataOrEx ()

    checkDeviceOrEx configData.SnapshotDevice

    createData
    |> ISnapshotsService.createOrEx configData
//----------------------------------------------------------------------------------------------------------------------
