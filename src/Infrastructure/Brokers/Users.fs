namespace Brokers

open System
open System.IO
open Motsoft.Util

open Model

open Localization
open Brokers


type UsersBroker private () =

    // -----------------------------------------------------------------------------------------------------------------
    let IProcessBroker = ProcessBrokerDI.Dep.D ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let instance = UsersBroker()
    static member getInstance () = instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.getUserInfoFromPasswordFileOrEx (userName : UserName) =

        let line =
            IProcessBroker.startProcessAndReadToEndOrEx
                "grep"
                $"^{userName.value}: /etc/passwd"

        line |> String.IsNullOrWhiteSpace |> failWithIfTrue Errors.UserNoInfoFound

        line
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.checkUserHomeExistsOrEx (homeDirectory : Directory) =

        Directory.Exists homeDirectory.value
        |> failWithIfFalse Errors.HomeDirectoryDoesNotExist

        homeDirectory
    // -----------------------------------------------------------------------------------------------------------------


module UsersBrokerDI =

    let Dep = DI.Dependency (fun () ->
            failwith $"{Errors.NotInitialized} ({nameof UsersBroker})" : UsersBroker)
