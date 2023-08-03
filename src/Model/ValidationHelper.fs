module Model.ValidationHelper

open System
open System.Collections.Generic
open Motsoft.Util

type Errors =
    | ValueIsEmpty
    | ValueContainsSpaces

type ErrorDict = Dictionary<Errors, string>

let checkEmptyTry (errors : ErrorDict) (value : string) =
    value |> String.IsNullOrWhiteSpace |> failWithIfTrue errors[ValueIsEmpty]

let checkForSpaces (errors : ErrorDict) (value : string) =
    value.Contains " " |> failWithIfTrue errors[ValueContainsSpaces]
