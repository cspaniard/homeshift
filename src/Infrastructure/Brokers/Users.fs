namespace Brokers

open System
open System.IO
open Motsoft.Util

open DI.Interfaces
open Model
open Localization


type UsersBroker (processBroker : IProcessBroker) =

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getUserInfoFromPasswordFileOrEx (userName : UserName) =

            let line =
                processBroker.startProcessAndReadToEndOrEx
                    "grep"
                    $"^{userName.value}: /etc/passwd"

            line |> String.IsNullOrWhiteSpace |> failWithIfTrue Errors.UserNoInfoFound

            line
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.checkUserHomeExistsOrEx (homeDirectory : Directory) =

            Directory.Exists homeDirectory.value
            |> failWithIfFalse Errors.HomeDirectoryDoesNotExist

            homeDirectory
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
