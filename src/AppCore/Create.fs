module AppCore.Create

open Model

type private IConfigService = DI.Services.IConfigService
type private ICreateService = DI.Services.ICreateService

let Run (options : CreateData) =

    Helpers.checkRootUserOrEx ()

    let configData = IConfigService.getConfigDataOrEx ()

    options.User
    |> ICreateService.createSnapshot configData
