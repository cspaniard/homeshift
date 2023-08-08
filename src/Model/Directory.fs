namespace Model

open Motsoft.Util

type Directory =

    private Directory of string with

        static member private canonicalize (value : string) =
            value
            |> trim

        static member private validateOrEx (value : string) =

            DirectoryValidation.getValidators ()
            |> Seq.iter (fun f -> f value)

            value

        member this.value = let (Directory value) = this in value

        override this.ToString () = this.value

        static member create (value : string) =
            value
            |> Directory.canonicalize
            |> Directory.validateOrEx
            |> Directory

type Comments =

    private Comments of string with

        static member private canonicalize (value : string) =
            value
            |> trim

        member this.value = let (Comments value) = this in value

        override this.ToString () = this.value

        static member create (value : string) =

            let value = if value = null then "" else value

            value
            |> Comments.canonicalize
            |> Comments
