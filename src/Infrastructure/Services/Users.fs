namespace Services

open DI
open Model


type UsersService private (usersBroker : IUsersBroker) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IUsersBroker = usersBroker
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let mutable instance = Unchecked.defaultof<IUsersService>
    
    static member getInstance (usersBroker : IUsersBroker) =
        
        if obj.ReferenceEquals(instance, null) then
            instance <- UsersService(usersBroker)
        
        instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersService with
    
        // -------------------------------------------------------------------------------------------------------------
        member _.getHomeForUserOrEx (userName : UserName) =

            let line = IUsersBroker.getUserInfoFromPasswordFileOrEx userName

            (line.Split ":")[5]
            |> Directory.create
            |> IUsersBroker.checkUserHomeExistsOrEx
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.isValidUser (userName : UserName) =

            // ToDo: Revisit this idea.

            try
                (this :> IUsersService).getHomeForUserOrEx userName |> ignore
                true
            with _ -> false
        // -------------------------------------------------------------------------------------------------------------
    
    // -----------------------------------------------------------------------------------------------------------------
