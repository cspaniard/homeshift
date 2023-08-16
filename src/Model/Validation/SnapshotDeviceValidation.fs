module Model.SnapshotDeviceValidation

open Model.ValidationHelper

open Localization

let private SnapshotDeviceErrors = ErrorDict ()

SnapshotDeviceErrors.Add (ValueIsEmpty, Errors.SnapshotDeviceIsEmpty)
SnapshotDeviceErrors.Add (ValueContainsSpaces, Errors.SnapshotDeviceHasSpaces)

let getValidators () =
    [
        checkEmptyTry SnapshotDeviceErrors
        checkForSpaces SnapshotDeviceErrors
    ]
