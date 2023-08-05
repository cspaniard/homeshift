module Model.SnapshotDeviceValidation

open Model.ValidationHelper

type private IErrors = DI.Services.LocalizationDI.IErrors

let private SnapshotDeviceErrors = ErrorDict ()

SnapshotDeviceErrors.Add (ValueIsEmpty, IErrors.SnapshotDeviceIsEmpty)
SnapshotDeviceErrors.Add (ValueContainsSpaces, IErrors.SnapshotDeviceHasSpaces)

let getValidators () =
    [
        checkEmptyTry SnapshotDeviceErrors
        checkForSpaces SnapshotDeviceErrors
    ]
