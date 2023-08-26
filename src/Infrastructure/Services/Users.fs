namespace Services

open DI.Interfaces
open Model


type UsersService (usersBroker : IUsersBroker) as this =

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersService with

        // -------------------------------------------------------------------------------------------------------------
        member _.getHomeForUserOrEx (userName : UserName) =

            let line = usersBroker.getUserInfoFromPasswordFileOrEx userName

            (line.Split ":")[5]
            |> Directory.create
            |> usersBroker.checkUserHomeExistsOrEx
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.isValidUser (userName : UserName) =

            try
                (this :> IUsersService).getHomeForUserOrEx userName |> ignore
                true
            with _ -> false
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
