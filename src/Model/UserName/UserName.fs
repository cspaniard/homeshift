namespace Model

open Motsoft.Util
open Model.UserNameValidation

type UserName =

    private UserName of string with

        static member private canonicalize (value : string) =
            value
            |> trim

        static member private validateTry (value : string) =

            try
                getValidatorsList ()
                |> Array.iter (fun f -> f value)

                value
            with e -> failwith e.Message

        member this.value = let (UserName value) = this in value

        override this.ToString () = this.value

        static member create (value : string) =
            value
            |> UserName.canonicalize
            |> UserName.validateTry
            |> UserName
