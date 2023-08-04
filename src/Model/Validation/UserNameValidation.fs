module Model.UserNameValidation

open Localization
open Model.ValidationHelper

let private UserNameErrors = ErrorDict ()

UserNameErrors.Add (ValueIsEmpty, Errors.UserNameIsEmpty)
UserNameErrors.Add (ValueContainsSpaces, Errors.UserNameHasSpaces)

let getValidators () =
    [
        checkEmptyTry UserNameErrors
        checkForSpaces UserNameErrors
    ]
