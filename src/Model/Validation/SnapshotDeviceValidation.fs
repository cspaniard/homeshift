module Model.SnapshotDeviceValidation

open Model.ValidationHelper
open Localization

let getValidators () =
    [
        checkEmptyTry Errors.SnapshotDeviceIsEmpty
        checkForSpaces Errors.SnapshotDeviceHasSpaces
    ]
