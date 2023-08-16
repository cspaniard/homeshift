module AppCore.Create

open Model
open AppCore.Helpers

open Services

//----------------------------------------------------------------------------------------------------------------------
let IConfigService = ConfigServiceDI.Dep.D ()
let ISnapshotsService = SnapshotsServiceDI.Dep.D ()
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
