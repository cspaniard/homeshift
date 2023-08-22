namespace Brokers

open System
open System.IO
open Motsoft.Util

open Model

open Localization
open DI


type UsersBroker private (processBroker : IProcessBroker) =

    // -----------------------------------------------------------------------------------------------------------------
    let IProcessBroker = processBroker
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let mutable instance = Unchecked.defaultof<IUsersBroker>
    
    static member getInstance (processBroker : IProcessBroker) =
        
        if obj.ReferenceEquals(instance, null) then
            instance <- UsersBroker processBroker
        
        instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IUsersBroker with
        
        // -------------------------------------------------------------------------------------------------------------
        member _.getUserInfoFromPasswordFileOrEx (userName : UserName) =

            let line =
                IProcessBroker.startProcessAndReadToEndOrEx
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
