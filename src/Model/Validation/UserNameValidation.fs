module Model.UserNameValidation

open Model.ValidationHelper

type private IErrors = DI.Services.LocalizationDI.IErrors

let private UserNameErrors = ErrorDict ()

UserNameErrors.Add (ValueIsEmpty, IErrors.UserNameIsEmpty)
UserNameErrors.Add (ValueContainsSpaces, IErrors.UserNameHasSpaces)

let getValidators () =
    [
        checkEmptyTry UserNameErrors
        checkForSpaces UserNameErrors
    ]
