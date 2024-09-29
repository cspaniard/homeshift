namespace AppCore

open Model
open Helpers

open DI.Interfaces


type Delete (configService : IConfigService, snapshotsService : ISnapshotsService) as this =

    //------------------------------------------------------------------------------------------------------------------
    let self = this :> IDelete
    //------------------------------------------------------------------------------------------------------------------

    interface IDelete with

        //--------------------------------------------------------------------------------------------------------------
        member _.deleteSnapshotOrEx (deleteData : DeleteData) =

            checkRootUserOrEx ()

            let configData = configService.getConfigDataOrEx ()

            checkUserOrEx deleteData.UserName
            checkDeviceOrEx configData.SnapshotDevice

            if deleteData.DeleteAll then
                snapshotsService.deleteAll configData.SnapshotDevice deleteData.UserName
            else
                checkSnapshotOrEx configData.SnapshotDevice deleteData.UserName deleteData.SnapshotName
                snapshotsService.deleteOrEx configData.SnapshotDevice deleteData
        //--------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------
        member _.deleteSnapshotOrEx (deleteOptions : DeleteOptions) =

            deleteOptions
            |> DeleteData.ofOptions
            |> self.deleteSnapshotOrEx
        //--------------------------------------------------------------------------------------------------------------
