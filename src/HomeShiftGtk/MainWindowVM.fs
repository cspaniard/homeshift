namespace HomeShiftGtk

open System
open Motsoft.Binder.NotifyObject


type MainWindowVM() =
    inherit NotifyObject()

    let mutable userName = try Environment.GetCommandLineArgs()[1] with _ -> ""

    //------------------------------------------------------------------------------------------------------------------
    member this.UserName
        with get() = userName
        and set value =
            if userName <> value then
                userName <- value
                this.NotifyPropertyChanged()
    //------------------------------------------------------------------------------------------------------------------
