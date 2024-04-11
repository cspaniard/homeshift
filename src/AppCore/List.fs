module AppCore.List

open Microsoft.Extensions.DependencyInjection
open Helpers
open Model

open DI.Interfaces
open DI.Providers

//----------------------------------------------------------------------------------------------------------------------
let configService = ServiceProvider.GetService<IConfigService>()
let snapshotsService = ServiceProvider.GetService<ISnapshotsService>()
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let getSnapshotListOrEx (listData : ListData) =

    let configData = configService.getConfigDataOrEx ()

    checkUserOrEx listData.UserName
    checkDeviceOrEx configData.SnapshotDevice

    snapshotsService.getListForUserOrEx configData.SnapshotDevice listData.UserName
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
module CLI =

    //------------------------------------------------------------------------------------------------------------------
    let showSnapshotListOrEx (listOptions : ListOptions) =

        listOptions
        |> ListData.ofOptions
        |> getSnapshotListOrEx
        |> snapshotsService.outputOrEx (listOptions.UserName |> UserName.create)
    //------------------------------------------------------------------------------------------------------------------
