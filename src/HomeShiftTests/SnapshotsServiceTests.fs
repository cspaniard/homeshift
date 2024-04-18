module SnapshotsServiceTests

open System
open Localization
open NUnit.Framework
open FsUnit
open Microsoft.Extensions.DependencyInjection
open Motsoft.Util

open DI.Interfaces
open DI.Providers
open Services
open Model

open MockBrokers.DevicesBrokerMock
open MockBrokers.UsersBrokerMock


type SnapshotsBrokerMock (throwError: bool, noPreviousSnapshots: bool) =

    // -----------------------------------------------------------------------------------------------------------------
    new () = SnapshotsBrokerMock(false, false)
    new (throwError: bool) = SnapshotsBrokerMock(throwError, false)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface ISnapshotsBroker with
        member this.createSnapshotOrEx sourcePath baseSnapshotPath createData progressCallBack lastSnapshotPathOption = failwith "todo"
        member this.deleteLastSnapshotOrEx(userSnapshotsPath) = failwith "todo"
        member this.deleteSnapshotPathOrEx(snapshotsPath) = failwith "todo"
        member this.deleteUserPathIfEmptyOrEx(snapshotsPath) = failwith "todo"

        member this.getAllInfoInPathOrEx _ =

            throwError |> failWithIfTrue "Mock Exception"

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

        member this.getLastSnapshotOptionInPathOrEx (path: Directory) = failwith "todo"
    // -----------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("ISnapshotsService")>]
type ``createOrEx tests`` () =

    let devicesBrokerMock = DevicesBrokerMock() :> IDevicesBroker
    let devicesBrokerMockWithError = DevicesBrokerMock(true) :> IDevicesBroker

    let snapshotsBrokerMock = SnapshotsBrokerMock() :> ISnapshotsBroker
    let snapshotsBrokerMockWithError = SnapshotsBrokerMock(true) :> ISnapshotsBroker

    let consoleBroker = ServiceProvider.GetService<IConsoleBroker>()

    let usersBrokerMock = UsersBrokerMock() :> IUsersBroker
    let usersService = UsersService(usersBrokerMock) :> IUsersService


    [<Test>]
    member _.``createOrEx: 1`` () =

        let snapshotsService = SnapshotsService(devicesBrokerMock, snapshotsBrokerMock,
                                                consoleBroker, usersService) :> ISnapshotsService

        1 |> should equal 2
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("ISnapshotsService")>]
type ``getListForUserOrEx tests`` () =

    let devicesBrokerMock = DevicesBrokerMock() :> IDevicesBroker
    let devicesBrokerMockWithError = DevicesBrokerMock(true) :> IDevicesBroker

    let snapshotsBrokerMock = SnapshotsBrokerMock() :> ISnapshotsBroker
    let snapshotsBrokerMockWithError = SnapshotsBrokerMock(true) :> ISnapshotsBroker
    let snapshotsBrokerMockNoSnapshots = SnapshotsBrokerMock(false, true) :> ISnapshotsBroker

    let consoleBroker = ServiceProvider.GetService<IConsoleBroker>()

    let usersBrokerMock = UsersBrokerMock() :> IUsersBroker
    let usersService = UsersService(usersBrokerMock) :> IUsersService


    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getListForUserOrEx: should return a populated sequence of snapshots`` () =

        let snapshotsService = SnapshotsService(devicesBrokerMock, snapshotsBrokerMock,
                                                consoleBroker, usersService) :> ISnapshotsService

        let snapShotDeviceDummy = SnapshotDevice.create "/dev/sda1"
        let userNameDummy = UserName.create "dummy"

        let snapshots = snapshotsService.getListForUserOrEx snapShotDeviceDummy userNameDummy

        snapshots |> should not' (be Empty)
        snapshots |> should not' (be Null)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getListForUserOrEx: should return an empty sequence of snapshots with data`` () =

        let snapshotsService = SnapshotsService(devicesBrokerMock, snapshotsBrokerMockNoSnapshots,
                                                consoleBroker, usersService) :> ISnapshotsService

        let snapShotDeviceDummy = SnapshotDevice.create "/dev/sda1"
        let userNameDummy = UserName.create "dummy"

        let snapshots = snapshotsService.getListForUserOrEx snapShotDeviceDummy userNameDummy

        snapshots |> should not' (be Null)
        snapshots |> should be Empty
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getListForUserOrEx: devicesBroker with errors, should throw an exception`` () =

        let snapshotsService = SnapshotsService(devicesBrokerMockWithError, snapshotsBrokerMock,
                                                consoleBroker, usersService) :> ISnapshotsService

        let snapShotDeviceDummy = SnapshotDevice.create "/dev/sda1"
        let userNameDummy = UserName.create "dummy"

        (fun () -> snapshotsService.getListForUserOrEx snapShotDeviceDummy userNameDummy |> ignore)
        |> should throw typeof<Exception>

    // -----------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
