module AppCore.Create

open Model

type private IConfigService = DI.Services.IConfigService
type private ICreateService = DI.Services.ICreateService

let Run (createData : CreateData) =

    Helpers.checkRootUserOrEx ()

    let configData = IConfigService.getConfigDataOrEx ()

    createData.UserName
    |> ICreateService.createSnapshot configData
