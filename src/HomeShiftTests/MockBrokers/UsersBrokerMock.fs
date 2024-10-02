module MockBrokers.UsersBrokerMock

open Motsoft.Util

open DI.Interfaces
open Localization
open Model

let [<Literal>] VALID_USER_NAME = "valid_user_name"
let [<Literal>] HOME_DIR = "/var/empty"

type UsersBrokerMock (throwError: bool) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> IUsersBroker

    new () = UsersBrokerMock (throwError = false)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getUserInfoFromPasswordFileOrEx (userName : UserName) =

            throwError |> failWithIfTrue $"{self.getUserInfoFromPasswordFileOrEx}: Mock Exception"

            if userName.value = VALID_USER_NAME
            then $"{userName.value}:*:441:441:OAH Daemon:{HOME_DIR}:/usr/bin/false"
            else failwith Errors.UserNoInfoFound
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
