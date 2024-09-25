module SnapshotsServiceTests

open System
open Localization
open NUnit.Framework
open FsUnit
open Microsoft.Extensions.DependencyInjection

open DI.Interfaces
open DI.Providers
open Services
open Model

open MockBrokers.DevicesBrokerMock
open MockBrokers.UsersBrokerMock
open MockBrokers.SnapshotsBrokerMock


// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("ISnapshotsService")>]
type ``createOrEx tests`` () =

    // -----------------------------------------------------------------------------------------------------------------
    let devicesBrokerMock = DevicesBrokerMock () :> IDevicesBroker
    let devicesBrokerMockWithError = DevicesBrokerMock (throwError = true) :> IDevicesBroker

    let snapshotsBrokerMock = SnapshotsBrokerMock () :> ISnapshotsBroker
    let snapshotsBrokerMockWithError = SnapshotsBrokerMock (throwError = true) :> ISnapshotsBroker
    let snapshotsBrokerMockWithCtrlC =
            SnapshotsBrokerMock (throwError = false, noPreviousSnapshots = false, sendCtrlC = true) :> ISnapshotsBroker

    let consoleBroker = ServiceProvider.GetService<IConsoleBroker> ()

    let usersBrokerMock = UsersBrokerMock () :> IUsersBroker
    let usersBrokerMockWithError = UsersBrokerMock (throwError = true) :> IUsersBroker
    let usersService = UsersService usersBrokerMock :> IUsersService
    let usersServiceWithError = UsersService usersBrokerMockWithError :> IUsersService
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``createOrEx: normal case, should not throw an exception`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMock,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let configData = ConfigData.getDefault ()
        let createData =
            { CreationDateTime = DateTimeOffset.Now
              UserName = UserName.create VALID_USER_NAME
              Comments = Comment.create "dummy comment" } : CreateData

        (fun () -> snapshotsService.createOrEx configData createData)
        |> should not' (throw typeof<Exception>)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``createOrEx: devicesBroker with errors, should throw an exception`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMockWithError, snapshotsBrokerMock,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let configData = ConfigData.getDefault ()
        let createData =
            { CreationDateTime = DateTimeOffset.Now
              UserName = UserName.create VALID_USER_NAME
              Comments = Comment.create "dummy comment" } : CreateData

        (fun () -> snapshotsService.createOrEx configData createData)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``createOrEx: snapshotsBroker with errors, should throw an exception`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMockWithError,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let configData = ConfigData.getDefault ()
        let createData =
            { CreationDateTime = DateTimeOffset.Now
              UserName = UserName.create VALID_USER_NAME
              Comments = Comment.create "dummy comment" } : CreateData

        (fun () -> snapshotsService.createOrEx configData createData)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``createOrEx: userService with errors, should throw an exception`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMock,
                                                 consoleBroker, usersServiceWithError) :> ISnapshotsService

        let configData = ConfigData.getDefault ()
        let createData =
            { CreationDateTime = DateTimeOffset.Now
              UserName = UserName.create VALID_USER_NAME
              Comments = Comment.create "dummy comment" } : CreateData

        (fun () -> snapshotsService.createOrEx configData createData)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``createOrEx: simulate and detect Ctrl+C pressed`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMockWithCtrlC,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let configData = ConfigData.getDefault ()
        let createData =
            { CreationDateTime = DateTimeOffset.Now
              UserName = UserName.create VALID_USER_NAME
              Comments = Comment.create "dummy comment" } : CreateData

        (fun () -> snapshotsService.createOrEx configData createData)
        |> should (throwWithMessage CTRL_C_PRESSED) typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("ISnapshotsService")>]
type ``getListForUserOrEx tests`` () =

    // -----------------------------------------------------------------------------------------------------------------
    let devicesBrokerMock = DevicesBrokerMock () :> IDevicesBroker
    let devicesBrokerMockWithError = DevicesBrokerMock (throwError = true) :> IDevicesBroker

    let snapshotsBrokerMock = SnapshotsBrokerMock () :> ISnapshotsBroker
    let snapshotsBrokerMockWithError = SnapshotsBrokerMock (throwError = true) :> ISnapshotsBroker
    let snapshotsBrokerMockNoSnapshots =
            SnapshotsBrokerMock (throwError = false, noPreviousSnapshots = true) :> ISnapshotsBroker

    let consoleBroker = ServiceProvider.GetService<IConsoleBroker> ()

    let usersBrokerMock = UsersBrokerMock () :> IUsersBroker
    let usersService = UsersService usersBrokerMock :> IUsersService
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getListForUserOrEx: should return a populated sequence of snapshots`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMock,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let snapShotDeviceDummy = SnapshotDevice.create "/dev/sda1"
        let userNameDummy = UserName.create "dummy"

        let snapshots = snapshotsService.getListForUserOrEx snapShotDeviceDummy userNameDummy

        snapshots |> should not' (be Empty)
        snapshots |> should not' (be Null)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getListForUserOrEx: should return an empty sequence of snapshots`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMockNoSnapshots,
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

        let snapshotsService = SnapshotsService (devicesBrokerMockWithError, snapshotsBrokerMock,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let snapShotDeviceDummy = SnapshotDevice.create "/dev/sda1"
        let userNameDummy = UserName.create "dummy"

        (fun () -> snapshotsService.getListForUserOrEx snapShotDeviceDummy userNameDummy |> ignore)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getListForUserOrEx: snapshotsBroker with errors, should throw an exception`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMockWithError,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let snapShotDeviceDummy = SnapshotDevice.create "/dev/sda1"
        let userNameDummy = UserName.create "dummy"

        (fun () -> snapshotsService.getListForUserOrEx snapShotDeviceDummy userNameDummy |> ignore)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("ISnapshotsService")>]
type ``outputOrEx tests`` () =

    // -----------------------------------------------------------------------------------------------------------------
    let devicesBrokerMock = DevicesBrokerMock () :> IDevicesBroker
    let snapshotsBrokerMock = SnapshotsBrokerMock () :> ISnapshotsBroker
    let consoleBroker = ServiceProvider.GetService<IConsoleBroker> ()

    let usersBrokerMock = UsersBrokerMock () :> IUsersBroker
    let usersService = UsersService usersBrokerMock :> IUsersService
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``outputOrEx: normal case, should not throw an exception`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMock,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let userName = UserName.create VALID_USER_NAME
        let snapshots = snapshotsBrokerMock.getAllInfoInPathOrEx (Directory.create "/dummy/path")

        (fun () -> snapshotsService.outputOrEx userName snapshots)
        |> should not' (throw typeof<Exception>)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``outputOrEx: if no snapshots are passed, should throw an exception`` () =

        let snapshotsService = SnapshotsService (devicesBrokerMock, snapshotsBrokerMock,
                                                 consoleBroker, usersService) :> ISnapshotsService

        let userName = UserName.create "valid "
        let snapshots = Seq.empty<Snapshot>

        (fun () -> snapshotsService.outputOrEx userName snapshots)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------
