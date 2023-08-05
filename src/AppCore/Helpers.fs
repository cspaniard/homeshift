module AppCore.Helpers

open System
open Motsoft.Util

type private IErrors = DI.Services.LocalizationDI.IErrors
type private IDevicesService = DI.Services.IDevicesService

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
