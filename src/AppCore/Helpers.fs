module AppCore.Helpers

open System
open Motsoft.Util
open Model

open Localization
open DI.Dependencies

//----------------------------------------------------------------------------------------------------------------------
let IDevicesService = IDevicesServiceDI.D ()
let ISnapshotsService = ISnapshotsServiceDI.D ()
let IUsersService = IUsersServiceDI.D ()
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
