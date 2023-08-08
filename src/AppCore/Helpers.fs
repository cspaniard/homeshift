module AppCore.Helpers

open System
open Model
open Motsoft.Util

type private IErrors = DI.Services.LocalizationDI.IErrors
type private IDevicesService = DI.Services.IDevicesService
type private IUsersService = DI.Services.IUsersService

//----------------------------------------------------------------------------------------------------------------------
let checkRootUserOrEx () =

    Environment.UserName = "root" |> failWithIfFalse IErrors.NeedRootAccess
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkDeviceExists (deviceName : string) =

    deviceName
    |> IDevicesService.isValidDeviceOrEx
    |> failWithIfFalse $"{IErrors.InvalidDevice} ({deviceName})"
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
let checkValidUser (userName : UserName) =

   IUsersService.isValidUser userName |> failWithIfFalse IErrors.UserInvalid
//----------------------------------------------------------------------------------------------------------------------
