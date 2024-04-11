module AppCore.Create

open Model
open AppCore.Helpers

open Microsoft.Extensions.DependencyInjection

open DI.Interfaces
open DI.Providers


//----------------------------------------------------------------------------------------------------------------------
let configService = ServiceProvider.GetService<IConfigService>()
let snapshotsService = ServiceProvider.GetService<ISnapshotsService>()
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let createSnapshotOrEx (createData : CreateData) =

    checkRootUserOrEx ()

    let configData = configService.getConfigDataOrEx ()

    checkDeviceOrEx configData.SnapshotDevice

    snapshotsService.createOrEx configData createData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let createSnapshotOrEx (createOptions : CreateOptions) =

        createOptions
        |> CreateData.ofOptions
        |> createSnapshotOrEx
    //------------------------------------------------------------------------------------------------------------------
