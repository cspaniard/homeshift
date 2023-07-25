module AppCore.Create

open System
open Model

let Run (options : CreateData) =
    Console.WriteLine $"%s{options.GetType().Name} 5"
    Console.WriteLine $"Hello %A{options} 5"
