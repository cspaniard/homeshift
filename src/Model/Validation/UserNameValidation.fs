module Model.UserNameValidation

open Model.ValidationHelper
open Localization

let getValidators () =
    [
        checkEmptyOrEx Errors.UserNameIsEmpty
        checkForSpacesOrEx Errors.UserNameHasSpaces
    ]
