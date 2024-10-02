namespace AppCore

open DI.Interfaces
open Model


type Restore (configService : IConfigService, snapshotsService : ISnapshotsService, helpers : IHelpers) =

    interface IRestore with
        //--------------------------------------------------------------------------------------------------------------
        member _.runOrEx (options : RestoreOptions) =

            helpers.checkRootUserOrEx ()

            let restoreData = RestoreData.ofOptions options
            let configData = configService.getConfigDataOrEx ()

            helpers.checkUserOrEx restoreData.UserName
            helpers.checkDeviceOrEx configData.SnapshotDevice
            helpers.checkSnapshotOrEx configData.SnapshotDevice restoreData.UserName restoreData.SnapshotName

            snapshotsService.restoreOrEx configData.SnapshotDevice restoreData
        //--------------------------------------------------------------------------------------------------------------
