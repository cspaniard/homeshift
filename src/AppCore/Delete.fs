module AppCore.Delete

open System
open Model

let Run (options : DeleteData) =
    Console.WriteLine $"Hello %A{options} 5"

    Helpers.checkRootUserOrEx ()
    Console.WriteLine "Pues seguimos como root."
