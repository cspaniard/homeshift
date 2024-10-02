namespace Services

open DI.Interfaces
open Model


type UsersService (usersBroker : IUsersBroker) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> IUsersService
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersService with

        // -------------------------------------------------------------------------------------------------------------
        member _.getHomeForUserOrEx (userName : UserName) =

            let line = usersBroker.getUserInfoFromPasswordFileOrEx userName

            (line.Split ":")[5]
            |> Directory.create
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.isValidUser (userName : UserName) =

            self.getHomeForUserOrEx userName |> ignore
            true
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
