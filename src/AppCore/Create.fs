namespace AppCore

open Model
open DI.Interfaces


type Create (configService : IConfigService, snapshotsService : ISnapshotsService, helpers : IHelpers) as this =

    //------------------------------------------------------------------------------------------------------------------
    let self = this :> ICreate
    //------------------------------------------------------------------------------------------------------------------

    interface ICreate with
        //--------------------------------------------------------------------------------------------------------------
        member _.createSnapshotOrEx (createData : CreateData) =

            helpers.checkRootUserOrEx ()

            let configData = configService.getConfigDataOrEx ()

            helpers.checkDeviceOrEx configData.SnapshotDevice

            snapshotsService.createOrEx configData createData
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.createSnapshotOrEx (createOptions : CreateOptions) =

            createOptions
            |> CreateData.ofOptions
            |> self.createSnapshotOrEx
        //--------------------------------------------------------------------------------------------------------------
