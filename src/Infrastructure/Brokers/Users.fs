namespace Brokers.Users

open System
open System.IO
open Motsoft.Util

open Model

type private IProcessBroker = DI.Brokers.IProcessBrokerDI
type private IErrors = DI.Services.LocalizationDI.IErrors

type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getUserInfoFromPasswordFileOrEx (userName : UserName) =

        let line =
            IProcessBroker.startProcessAndReadToEndOrEx
                "grep"
                $"^{userName.value}: /etc/passwd"

        line |> String.IsNullOrWhiteSpace |> failWithIfTrue IErrors.UserNoInfoFound

        line
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member checkUserHomeExistsOrEx (homeDirectory : Directory) =

        Directory.Exists homeDirectory.value
        |> failWithIfFalse IErrors.HomeDirectoryDoesNotExist

        homeDirectory
    // -----------------------------------------------------------------------------------------------------------------
