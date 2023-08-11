module AppCore.Helpers

open System
open Model
open Motsoft.Util

type private IErrors = DI.Services.LocalizationDI.IErrors
type private IDevicesService = DI.Services.IDevicesService
type private IUsersService = DI.Services.IUsersService
type private ISnapshotService = DI.Services.ISnapshotsService

//----------------------------------------------------------------------------------------------------------------------
let checkRootUserOrEx () =

    Environment.UserName = "root" |> failWithIfFalse IErrors.NeedRootAccess
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkDeviceOrEx (snapshotDevice : SnapshotDevice) =

    snapshotDevice
    |> IDevicesService.isValidDeviceOrEx
    |> failWithIfFalse $"{IErrors.InvalidDevice} ({snapshotDevice})"
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkUserOrEx (userName : UserName) =

   IUsersService.isValidUser userName |> failWithIfFalse IErrors.UserInvalid
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkSnapshotOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

   ISnapshotService.isValidOrEx snapshotDevice userName snapshotName
   |> failWithIfFalse $"{IErrors.SnapshotInvalid} ({snapshotName})"
//----------------------------------------------------------------------------------------------------------------------
