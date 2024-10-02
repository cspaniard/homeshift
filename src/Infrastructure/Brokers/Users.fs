namespace Brokers

open System
open System.IO
open Motsoft.Util

open DI.Interfaces
open Model
open Localization



type UsersBroker (processBroker : IProcessBroker) =

    let [<Literal>] PASSWORD_FILE = "/etc/passwd"

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getUserInfoFromPasswordFileOrEx (userName : UserName) =

            let line =
                processBroker.startProcessAndReadToEndOrEx
                    "grep"
                    $"^{userName.value}: {PASSWORD_FILE}"

            line |> String.IsNullOrWhiteSpace |> failWithIfTrue Errors.UserNoInfoFound

            line
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
