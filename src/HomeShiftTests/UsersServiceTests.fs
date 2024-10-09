module UsersServiceTests

open System
open Localization
open NUnit.Framework
open FsUnit

open DI.Interfaces
open Model
open Services

open MockBrokers.UsersBrokerMock

let [<Literal>] VALID_USER_NAME = "valid_user_name"
let [<Literal>] INVALID_USER_NAME = "invalid_user_name"
let [<Literal>] HOME_DIR = "/var/empty"


// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("IUsersService")>]
type ``getHomeForUserOrEx tests`` () =

    // -----------------------------------------------------------------------------------------------------------------
    let usersBrokerMock = UsersBrokerMock () :> IUsersBroker
    let usersService = UsersService usersBrokerMock :> IUsersService
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getHomeForUserOrEx: with valid user, it should returns home dir`` () =

        (UserName.create VALID_USER_NAME |> usersService.getHomeForUserOrEx)
            .value
        |> should equal HOME_DIR
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``getHomeForUserOrEx: with invalid user, it should throw exception`` () =

        (fun () -> (UserName.create INVALID_USER_NAME |> usersService.getHomeForUserOrEx) |> ignore)
        |> should throw typeof<Exception>
    // -----------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
[<TestFixture>]
[<Category("IUsersService")>]
type ``isValidUser tests`` () =

    // -----------------------------------------------------------------------------------------------------------------
    let usersBrokerMock = UsersBrokerMock () :> IUsersBroker
    let usersService = UsersService usersBrokerMock :> IUsersService
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``isValidUser: with valid user, it should return true`` () =

        UserName.create VALID_USER_NAME
        |> usersService.isValidUserOrEx |> should equal true
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    [<Test>]
    member _.``isValidUser: with invalid user, it should return false`` () =

        UserName.create INVALID_USER_NAME
        |> usersService.isValidUserOrEx |> should equal false
    // -----------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------


let a = 0            // Avoid Warning FS0988: Main module of program is empty: nothing will happen when it is run
