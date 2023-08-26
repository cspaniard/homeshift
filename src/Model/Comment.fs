namespace Model

open Motsoft.Util


type Comment =

    private Comment of string with

        interface IValueType<Comment, string> with
            static member create (value : string) = Comment.create value

        static member private canonicalize (value : string) =
            value
            |> trim

        member this.value = let (Comment value) = this in value

        override this.ToString () = this.value

        static member create (value : string) =

            let value = if value = null then "" else value

            value
            |> Comment.canonicalize
            |> Comment
