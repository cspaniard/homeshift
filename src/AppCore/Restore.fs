module AppCore.Restore

open System
open Model

let Run (options : RestoreData) =
    Console.WriteLine $"%s{options.GetType().Name} 5"
    Console.WriteLine $"Hello %A{options} 5"
