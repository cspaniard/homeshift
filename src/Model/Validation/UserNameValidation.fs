module Model.UserNameValidation

open Model.ValidationHelper
open Localization

let getValidators () =
    [
        checkEmptyTry Errors.UserNameIsEmpty
        checkForSpaces Errors.UserNameHasSpaces
    ]
