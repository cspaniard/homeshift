module AppCore.Restore

open System
open Model

//----------------------------------------------------------------------------------------------------------------------
let runOrEx (options : RestoreData) =
    Console.WriteLine $"Hello %A{options} 5"

    Helpers.checkRootUserOrEx ()
    Console.WriteLine "Pues seguimos como root."
//----------------------------------------------------------------------------------------------------------------------
