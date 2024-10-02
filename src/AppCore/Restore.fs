namespace AppCore

open DI.Interfaces
open Model
open Helpers


type Restore (configService : IConfigService, snapshotsService : ISnapshotsService) =

    interface IRestore with
        //--------------------------------------------------------------------------------------------------------------
        member _.runOrEx (options : RestoreOptions) =

            checkRootUserOrEx ()

            let restoreData = RestoreData.ofOptions options
            let configData = configService.getConfigDataOrEx ()

            checkUserOrEx restoreData.UserName
            checkDeviceOrEx configData.SnapshotDevice
            checkSnapshotOrEx configData.SnapshotDevice restoreData.UserName restoreData.SnapshotName

            snapshotsService.restoreOrEx configData.SnapshotDevice restoreData
        //--------------------------------------------------------------------------------------------------------------
