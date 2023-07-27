module AppCore.Config

open System
open Model

let Run (options : ConfigData) =

    Helpers.checkRootUserOrException ()
    Console.WriteLine "Pues seguimos como root."
