namespace AppCore

open System
open DI.Interfaces
open Model
open Helpers

type Restore () =

    interface IRestore with
        //--------------------------------------------------------------------------------------------------------------
        member _.runOrEx (options : RestoreData) =
            Console.WriteLine $"Hello %A{options} 5"

            checkRootUserOrEx ()
            Console.WriteLine "Pues seguimos como root."
        //--------------------------------------------------------------------------------------------------------------
