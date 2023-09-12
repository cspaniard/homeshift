module UsersServiceTests

open System
open Localization
open NUnit.Framework
open FsUnit

open DI.Interfaces
open Model
open Services


type UsersBroker () =

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getUserInfoFromPasswordFileOrEx (userName : UserName) =

            if userName.value = "dsanroma"
            then $"{userName.value}:*:441:441:OAH Daemon:/var/empty:/usr/bin/false"
            else raise (Exception(Errors.UserNoInfoFound))
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.checkUserHomeExistsOrEx (homeDirectory : Directory) =

            homeDirectory
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------

let mutable usersBroker = Unchecked.defaultof<IUsersBroker>
let mutable usersService = Unchecked.defaultof<IUsersService>

[<SetUp>]
let Setup () =
    usersBroker <- UsersBroker() :> IUsersBroker
    usersService <- UsersService(usersBroker) :> IUsersService

[<Test>]
let ``getHomeForUserOrEx valid user returns home dir`` () =

    (UserName.create "dsanroma" |> usersService.getHomeForUserOrEx)
        .value
    |> should equal "/var/empty"

[<Test>]
let ``getHomeForUserOrEx invalid user throws exception`` () =

    (fun () -> (UserName.create "pepito" |> usersService.getHomeForUserOrEx) |> ignore)
    |> should throw typeof<Exception>

[<Test>]
let ``isValidUser valid user returns true`` () =

    "dsanroma"
    |> UserName.create
    |> usersService.isValidUser
    |> should equal true

[<Test>]
let ``isValidUser invalid user returns false`` () =

    "_invalid_user_name_"
    |> UserName.create
    |> usersService.isValidUser
    |> should equal false
