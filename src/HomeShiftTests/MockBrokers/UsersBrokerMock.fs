module MockBrokers.UsersBrokerMock

open DI.Interfaces
open Localization
open Model

type UsersBrokerMock () =

    let [<Literal>] VALID_USER_NAME = "valid_user_name"
    let [<Literal>] HOME_DIR = "/var/empty"

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getUserInfoFromPasswordFileOrEx (userName : UserName) =

            if userName.value = VALID_USER_NAME
            then $"{userName.value}:*:441:441:OAH Daemon:{HOME_DIR}:/usr/bin/false"
            else failwith Errors.UserNoInfoFound
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.checkUserHomeExistsOrEx (homeDirectory : Directory) =

            homeDirectory
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
