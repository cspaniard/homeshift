module Model.DirectoryValidation

open Model.ValidationHelper
open Localization

let getValidators () =
    [
        checkEmptyOrEx Errors.DirectoryIsEmpy
    ]
