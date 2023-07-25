module AppCore.Config

open System
open Model

let Run (options : ConfigData) =
    Console.WriteLine $"Hello %A{options} 5"

    Helpers.checkRootUserOrException ()
    Console.WriteLine "Pues seguimos como root."
