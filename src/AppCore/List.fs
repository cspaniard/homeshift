namespace AppCore

open Helpers
open Model

open DI.Interfaces

type List (configService : IConfigService, snapshotsService : ISnapshotsService) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> IList
    // -----------------------------------------------------------------------------------------------------------------

    interface IList with
        //--------------------------------------------------------------------------------------------------------------
        member _.getSnapshotListOrEx (listData : ListData) =

            checkRootUserOrEx ()

            let configData = configService.getConfigDataOrEx ()

            checkUserOrEx listData.UserName
            checkDeviceOrEx configData.SnapshotDevice

            snapshotsService.getListForUserOrEx configData.SnapshotDevice listData.UserName
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.showSnapshotListOrEx (listOptions : ListOptions) =

            listOptions
            |> ListData.ofOptions
            |> self.getSnapshotListOrEx
            |> snapshotsService.outputOrEx (listOptions.UserName |> UserName.create)
        //--------------------------------------------------------------------------------------------------------------
