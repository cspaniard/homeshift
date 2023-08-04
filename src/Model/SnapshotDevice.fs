namespace Model

open Motsoft.Util

type SnapshotDevice =

    private SnapshotDevice of string with

        static member private canonicalize (value : string) =
            value
            |> trim

        static member private validateTry (value : string) =

            try
                SnapshotDeviceValidation.getValidators ()
                |> Seq.iter (fun f -> f value)

                value
            with e -> failwith e.Message

        member this.value = let (SnapshotDevice value) = this in value

        override this.ToString () = this.value

        static member create (value : string) =
            value
            |> SnapshotDevice.canonicalize
            |> SnapshotDevice.validateTry
            |> SnapshotDevice
