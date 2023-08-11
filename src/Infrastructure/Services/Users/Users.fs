namespace Services.Users

open Model

type private IUsersBroker = DI.Brokers.IUsersBroker


type Service () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getHomeForUserOrEx (userName : UserName) =

        let line = IUsersBroker.getUserInfoFromPasswordFileOrEx userName

        (line.Split ":")[5]
        |> Directory.create
        |> IUsersBroker.checkUserHomeExistsOrEx
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member isValidUser (userName : UserName) =

        // ToDo: Revisit this idea.

        try
            Service.getHomeForUserOrEx userName |> ignore
            true
        with _ -> false
    // -----------------------------------------------------------------------------------------------------------------
