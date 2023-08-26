module AppCore.Delete

open Microsoft.Extensions.DependencyInjection

open Model
open Helpers

open DI.Interfaces
open DI.Providers


//----------------------------------------------------------------------------------------------------------------------
let configService = serviceProvider.GetService<IConfigService>()
let snapshotsService = serviceProvider.GetService<ISnapshotsService>()
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let deleteSnapshotOrEx (deleteData : DeleteData) =

    checkRootUserOrEx ()

    let configData = configService.getConfigDataOrEx ()

    checkUserOrEx deleteData.UserName
    checkDeviceOrEx configData.SnapshotDevice

    if deleteData.DeleteAll then
        snapshotsService.deleteAll configData.SnapshotDevice deleteData.UserName
    else
        checkSnapshotOrEx configData.SnapshotDevice deleteData.UserName deleteData.SnapshotName
        snapshotsService.deleteOrEx configData.SnapshotDevice deleteData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let deleteSnapshotOrEx (deleteOptions : DeleteOptions) =

        deleteOptions
        |> DeleteData.ofOptions
        |> deleteSnapshotOrEx
    //------------------------------------------------------------------------------------------------------------------
