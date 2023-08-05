module Model.DirectoryValidation

open Model.ValidationHelper

type private IErrors = DI.Services.LocalizationDI.IErrors

let private DirectoryErrors = ErrorDict ()

DirectoryErrors.Add (ValueIsEmpty, IErrors.DirectoryIsEmpy)

let getValidators () =
    [
        checkEmptyTry DirectoryErrors
    ]
