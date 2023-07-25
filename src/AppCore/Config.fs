module AppCore.Config

open System
open Model

let Run (options : ConfigData) =
    Console.WriteLine $"%s{options.GetType().Name} 5"
    Console.WriteLine $"Hello %A{options} 5"
