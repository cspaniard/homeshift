namespace AppCore

open Model

open DI.Interfaces

type List (configService : IConfigService, snapshotsService : ISnapshotsService, helpers : IHelpers) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> IList
    // -----------------------------------------------------------------------------------------------------------------

    interface IList with
        //--------------------------------------------------------------------------------------------------------------
        member _.getSnapshotListOrEx (listData : ListData) =

            helpers.checkRootUserOrEx ()

            let configData = configService.getConfigDataOrEx ()

            helpers.checkUserOrEx listData.UserName
            helpers.checkDeviceOrEx configData.SnapshotDevice

            snapshotsService.getListForUserOrEx configData.SnapshotDevice listData.UserName
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.showSnapshotListOrEx (listOptions : ListOptions) =

            listOptions
            |> ListData.ofOptions
            |> self.getSnapshotListOrEx
            |> snapshotsService.outputOrEx (listOptions.UserName |> UserName.create)
        //--------------------------------------------------------------------------------------------------------------
