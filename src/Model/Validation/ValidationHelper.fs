module Model.ValidationHelper

open System
open Motsoft.Util

let checkEmptyOrEx (error : string) (value : string) =
    value |> String.IsNullOrWhiteSpace |> failWithIfTrue error

let checkForSpacesOrEx (error : string) (value : string) =
    value.Contains " " |> failWithIfTrue error
