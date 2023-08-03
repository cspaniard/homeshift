module Model.UserNameValidation

open Localization
open Model.ValidationHelper

let UserNameErrors = ErrorDict ()

UserNameErrors.Add (ValueIsEmpty, Errors.UserNameIsEmpty)
UserNameErrors.Add (ValueContainsSpaces, Errors.UserNameHasSpaces)

let getValidatorsList () =
    [|
        checkEmptyTry UserNameErrors
        checkForSpaces UserNameErrors
    |]
