module AppCore.Restore

open System
open Model

let Run (options : RestoreData) =
    Console.WriteLine $"Hello %A{options} 5"

    Helpers.checkRootUserOrException ()
    Console.WriteLine "Pues seguimos como root."
