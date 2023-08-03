module Model.SnapshotDeviceValidation

open Localization
open Model.ValidationHelper

let SnapshotDeviceErrors = ErrorDict ()

SnapshotDeviceErrors.Add (ValueIsEmpty, Errors.SnapshotDeviceIsEmpty)
SnapshotDeviceErrors.Add (ValueContainsSpaces, Errors.SnapshotDeviceHasSpaces)

let getValidatorsList () =
    [|
        checkEmptyTry SnapshotDeviceErrors
        checkForSpaces SnapshotDeviceErrors
    |]
