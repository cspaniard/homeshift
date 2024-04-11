module Model.SnapshotDeviceValidation

open Model.ValidationHelper
open Localization

let getValidators () =
    [
        checkEmptyOrEx Errors.SnapshotDeviceIsEmpty
        checkForSpacesOrEx Errors.SnapshotDeviceHasSpaces
    ]
