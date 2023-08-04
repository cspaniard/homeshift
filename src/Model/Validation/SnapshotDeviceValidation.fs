module Model.SnapshotDeviceValidation

open Localization
open Model.ValidationHelper

let private SnapshotDeviceErrors = ErrorDict ()

SnapshotDeviceErrors.Add (ValueIsEmpty, Errors.SnapshotDeviceIsEmpty)
SnapshotDeviceErrors.Add (ValueContainsSpaces, Errors.SnapshotDeviceHasSpaces)

let getValidators () =
    [
        checkEmptyTry SnapshotDeviceErrors
        checkForSpaces SnapshotDeviceErrors
    ]
