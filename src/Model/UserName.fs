namespace Model

open Motsoft.Util


type UserName =

    private UserName of string with

        interface IValueType<UserName, string> with
            static member create (value : string) = UserName.create value

        static member private canonicalize (value : string) =
            value
            |> trim

        static member private validateOrEx (value : string) =

            UserNameValidation.getValidators ()
            |> Seq.iter (fun f -> f value)

            value

        member this.value = let (UserName value) = this in value

        override this.ToString () = this.value

        static member create (value : string) =
            value
            |> UserName.canonicalize
            |> UserName.validateOrEx
            |> UserName
