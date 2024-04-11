module AppCore.Helpers

open System
open Microsoft.Extensions.DependencyInjection
open Motsoft.Util
open Model

open DI.Interfaces
open DI.Providers
open Localization

//----------------------------------------------------------------------------------------------------------------------
let devicesService = ServiceProvider.GetService<IDevicesService>()
let snapshotsService = ServiceProvider.GetService<ISnapshotsService>()
let usersService = ServiceProvider.GetService<IUsersService>()
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkRootUserOrEx () =

    Environment.UserName = "root" |> failWithIfFalse Errors.NeedRootAccess
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkDeviceOrEx (snapshotDevice : SnapshotDevice) =

    snapshotDevice
    |> devicesService.isValidDeviceOrEx
    |> failWithIfFalse $"{Errors.InvalidDevice} ({snapshotDevice})"
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkUserOrEx (userName : UserName) =

   usersService.isValidUser userName |> failWithIfFalse Errors.UserInvalid
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkSnapshotOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

   snapshotsService.isValidOrEx snapshotDevice userName snapshotName
   |> failWithIfFalse $"{Errors.SnapshotInvalid} ({userName.value}): {snapshotName}"
//----------------------------------------------------------------------------------------------------------------------
