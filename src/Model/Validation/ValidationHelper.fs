module Model.ValidationHelper

open System
open Motsoft.Util

// TODO: Rename to xxxOrEx
let checkEmptyTry (error : string) (value : string) =
    value |> String.IsNullOrWhiteSpace |> failWithIfTrue error

// TODO: Rename to xxxOrEx
let checkForSpaces (error : string) (value : string) =
    value.Contains " " |> failWithIfTrue error
