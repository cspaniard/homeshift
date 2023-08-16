module Model.DirectoryValidation

open Model.ValidationHelper

open Localization

let private DirectoryErrors = ErrorDict ()

DirectoryErrors.Add (ValueIsEmpty, Errors.DirectoryIsEmpy)

let getValidators () =
    [
        checkEmptyTry DirectoryErrors
    ]
