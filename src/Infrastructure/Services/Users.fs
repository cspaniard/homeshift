namespace Services

open Model

open Brokers


type UsersService private () as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IUsersBroker = UsersBrokerDI.Dep.D ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let instance = UsersService()
    static member getInstance () = instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.getHomeForUserOrEx (userName : UserName) =

        let line = IUsersBroker.getUserInfoFromPasswordFileOrEx userName

        (line.Split ":")[5]
        |> Directory.create
        |> IUsersBroker.checkUserHomeExistsOrEx
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.isValidUser (userName : UserName) =

        // ToDo: Revisit this idea.

        try
            this.getHomeForUserOrEx userName |> ignore
            true
        with _ -> false
    // -----------------------------------------------------------------------------------------------------------------

module UsersServiceDI =

    open Localization

    let Dep = DI.Dependency (fun () ->
            failwith $"{Errors.NotInitialized} ({nameof UsersService})" : UsersService)
