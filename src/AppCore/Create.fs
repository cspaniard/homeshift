module AppCore.Create

open Model
open AppCore.Helpers

open DI.Dependencies

//----------------------------------------------------------------------------------------------------------------------
let IConfigService = IConfigServiceDI.D ()
let ISnapshotsService = ISnapshotsServiceDI.D ()
//----------------------------------------------------------------------------------------------------------------------

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
