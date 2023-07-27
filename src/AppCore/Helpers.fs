module AppCore.Helpers

open System
open Motsoft.Util

type IErrors = DI.Services.LocalizationDI.IErrors

let checkRootUserOrException () =

    Environment.UserName = "root" |> failWithIfFalse IErrors.NeedRootAccess
