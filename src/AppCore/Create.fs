namespace AppCore

open Model
open AppCore.Helpers
open DI.Interfaces


type Create (configService : IConfigService, snapshotsService : ISnapshotsService) as this =

    //------------------------------------------------------------------------------------------------------------------
    let self = this :> ICreate
    //------------------------------------------------------------------------------------------------------------------

    interface ICreate with
        //--------------------------------------------------------------------------------------------------------------
        member _.createSnapshotOrEx (createData : CreateData) =

            checkRootUserOrEx ()

            let configData = configService.getConfigDataOrEx ()

            checkDeviceOrEx configData.SnapshotDevice

            snapshotsService.createOrEx configData createData
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.createSnapshotOrEx (createOptions : CreateOptions) =

            createOptions
            |> CreateData.ofOptions
            |> self.createSnapshotOrEx
        //--------------------------------------------------------------------------------------------------------------
