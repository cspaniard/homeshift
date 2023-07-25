module AppCore.Helpers

open System
open Motsoft.Util

type ILocaleText = DI.Services.LocalizationDI.ILocaleText

let checkRootUserOrException () =

    Environment.UserName = "root" |> failWithIfFalse ILocaleText.ErrorNeedsRoot
