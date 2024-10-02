namespace AppCore

open Model
open DI.Interfaces


type Delete (configService : IConfigService, snapshotsService : ISnapshotsService, helpers : IHelpers) as this =

    //------------------------------------------------------------------------------------------------------------------
    let self = this :> IDelete
    //------------------------------------------------------------------------------------------------------------------

    interface IDelete with

        //--------------------------------------------------------------------------------------------------------------
        member _.deleteSnapshotOrEx (deleteData : DeleteData) =

            helpers.checkRootUserOrEx ()

            let configData = configService.getConfigDataOrEx ()

            helpers.checkUserOrEx deleteData.UserName
            helpers.checkDeviceOrEx configData.SnapshotDevice

            if deleteData.DeleteAll then
                snapshotsService.deleteAll configData.SnapshotDevice deleteData.UserName
            else
                helpers.checkSnapshotOrEx configData.SnapshotDevice deleteData.UserName deleteData.SnapshotName
                snapshotsService.deleteOrEx configData.SnapshotDevice deleteData
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.deleteSnapshotOrEx (deleteOptions : DeleteOptions) =

            deleteOptions
            |> DeleteData.ofOptions
            |> self.deleteSnapshotOrEx
        //--------------------------------------------------------------------------------------------------------------
