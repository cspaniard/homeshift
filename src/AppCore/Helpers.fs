module AppCore.Helpers

open System
open Motsoft.Util
open Model

open Localization
open Services

//----------------------------------------------------------------------------------------------------------------------
let IDevicesService = DevicesServiceDI.Dep.D ()
let ISnapshotsService = SnapshotsServiceDI.Dep.D ()
let IUsersService = UsersServiceDI.Dep.D ()
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkRootUserOrEx () =

    Environment.UserName = "root" |> failWithIfFalse Errors.NeedRootAccess
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkDeviceOrEx (snapshotDevice : SnapshotDevice) =

    snapshotDevice
    |> IDevicesService.isValidDeviceOrEx
    |> failWithIfFalse $"{Errors.InvalidDevice} ({snapshotDevice})"
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkUserOrEx (userName : UserName) =

   IUsersService.isValidUser userName |> failWithIfFalse Errors.UserInvalid
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkSnapshotOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

   ISnapshotsService.isValidOrEx snapshotDevice userName snapshotName
   |> failWithIfFalse $"{Errors.SnapshotInvalid} ({userName.value}): {snapshotName}"
//----------------------------------------------------------------------------------------------------------------------
