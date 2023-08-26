namespace Model

open Motsoft.Util


type SnapshotDevice =

    private SnapshotDevice of string with

        interface IValueType<SnapshotDevice, string> with
            static member create (value : string) = SnapshotDevice.create value

        static member private canonicalize (value : string) =
            value
            |> trim

        static member private validateTry (value : string) =

            SnapshotDeviceValidation.getValidators ()
            |> Seq.iter (fun f -> f value)

            value

        member this.value = let (SnapshotDevice value) = this in value

        override this.ToString () = this.value

        static member create (value : string) =
            value
            |> SnapshotDevice.canonicalize
            |> SnapshotDevice.validateTry
            |> SnapshotDevice
