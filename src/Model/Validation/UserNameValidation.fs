module Model.UserNameValidation

open Model.ValidationHelper

open Localization

let private UserNameErrors = ErrorDict ()

UserNameErrors.Add (ValueIsEmpty, Errors.UserNameIsEmpty)
UserNameErrors.Add (ValueContainsSpaces, Errors.UserNameHasSpaces)

let getValidators () =
    [
        checkEmptyTry UserNameErrors
        checkForSpaces UserNameErrors
    ]
