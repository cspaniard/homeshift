namespace AppCore

open System
open Motsoft.Util
open Model

open DI.Interfaces
open Localization


type Helpers(devicesService : IDevicesService, snapshotsService : ISnapshotsService, usersService : IUsersService) =

    interface IHelpers with
        //--------------------------------------------------------------------------------------------------------------
        member _.checkRootUserOrEx () =

            Environment.UserName = "root" |> failWithIfFalse Errors.NeedRootAccess
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.checkDeviceOrEx (snapshotDevice : SnapshotDevice) =

            devicesService.findDeviceOrEx snapshotDevice.value |> ignore
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.checkUserOrEx (userName : UserName) =

           usersService.isValidUserOrEx userName |> ignore
        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------
        member _.checkSnapshotOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

           snapshotsService.isValidOrEx snapshotDevice userName snapshotName
           |> failWithIfFalse $"{Errors.SnapshotInvalid} ({userName.value}): {snapshotName}"
        //--------------------------------------------------------------------------------------------------------------
