module AppCore.Delete

open System
open Model

let Run (options : DeleteData) =
    Console.WriteLine $"%s{options.GetType().Name} 5"
    Console.WriteLine $"Hello %A{options} 5"
