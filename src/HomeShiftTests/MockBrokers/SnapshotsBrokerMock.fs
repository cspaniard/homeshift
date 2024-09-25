module MockBrokers.SnapshotsBrokerMock

open System
open Microsoft.Extensions.DependencyInjection
open Motsoft.Util

open DI.Providers
open DI.Interfaces
open Model

let [<Literal>] CTRL_C_PRESSED = "Ctrl+C pressed"

type SnapshotsBrokerMock (throwError: bool, noPreviousSnapshots: bool, sendCtrlC: bool) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> ISnapshotsBroker

    new () = SnapshotsBrokerMock (throwError = false, noPreviousSnapshots = false, sendCtrlC = false)
    new (throwError: bool) = SnapshotsBrokerMock (throwError, noPreviousSnapshots = false, sendCtrlC = false)

    new (throwError: bool, noPreviousSnapshots: bool) =
            SnapshotsBrokerMock(throwError, noPreviousSnapshots, sendCtrlC = false)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface ISnapshotsBroker with

        // -------------------------------------------------------------------------------------------------------------
        member this.createSnapshotOrEx _ _ _ _ _ =

            throwError |> failWithIfTrue $"{self.createSnapshotOrEx}: Mock Exception"

            if sendCtrlC then
                let processBroker = ServiceProvider.GetService<IProcessBroker> ()

                let pid = System.Diagnostics.Process.GetCurrentProcess().Id
                processBroker.startProcessNoOuputAtAll "kill" $"-SIGINT {pid}"

            ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member this.deleteLastSnapshotOrEx _ =

            throwError |> failWithIfTrue $"{self.deleteLastSnapshotOrEx}: Mock Exception"
            sendCtrlC |> failWithIfTrue CTRL_C_PRESSED

            ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member this.deleteSnapshotPathOrEx _ =

            throwError |> failWithIfTrue $"{self.deleteSnapshotPathOrEx}: Mock Exception"

            failwith "deleteSnapshotPathOrEx not implemented"
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member this.deleteUserPathIfEmptyOrEx _ =

            throwError |> failWithIfTrue $"{self.deleteUserPathIfEmptyOrEx}: Mock Exception"

            failwith "deleteUserPathIfEmptyOrEx not implemented"
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member this.getAllInfoInPathOrEx _ =

            throwError |> failWithIfTrue $"{self.getAllInfoInPathOrEx}: Mock Exception"

            if noPreviousSnapshots then
                Seq.empty
            else
                seq {
                    { CreationDateTime = DateTimeOffset.Now; Name = "Snapshot 1"
                      Comments = Comment.create "First snapshot" }

                    { CreationDateTime = DateTimeOffset.Now; Name = "Snapshot 2"
                      Comments = Comment.create "Second snapshot" }

                    { CreationDateTime = DateTimeOffset.Now; Name = "Snapshot 3"
                      Comments = Comment.create "Third snapshot" }
                }
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member this.getLastSnapshotOptionInPathOrEx _ =

            throwError |> failWithIfTrue $"{self.getLastSnapshotOptionInPathOrEx}: Mock Exception"

            Some <| Directory.create "/dummy/path"
        // -------------------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------------------
