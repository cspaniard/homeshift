module AppCore.Create

open System
open Model

let Run (options : CreateData) =
    Console.WriteLine $"Hello %A{options} 5"

    Helpers.checkRootUserOrEx ()
    Console.WriteLine "Pues seguimos como root."
