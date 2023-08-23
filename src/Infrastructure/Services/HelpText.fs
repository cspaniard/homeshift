namespace Services

open System
open System.Reflection
open System.Text.RegularExpressions
open CommandLine
open CommandLine.Text

open DI
open Localization.DI


type HelpService private (consoleBroker : IConsoleBroker, sentenceBuilder : ISentenceBuilder) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IConsoleBroker = consoleBroker
    let ISentenceBuilder = sentenceBuilder
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let mutable instance = Unchecked.defaultof<IHelpService>
    
    static member getInstance (consoleBroker : IConsoleBroker, sentenceBuilder : ISentenceBuilder) =
        
        if obj.ReferenceEquals(instance, null) then
            instance <- HelpService(consoleBroker, sentenceBuilder)
        
        instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IHelpService with
    
        // -------------------------------------------------------------------------------------------------------------
        member _.Heading with get () =
            let version = Assembly.GetEntryAssembly().GetName().Version
            $"\nHomeshift v{version.Major}.{version.Minor}.{version.Build} by David Sanroma"
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.helpTextFromResult (result : ParserResult<_>) =

            SentenceBuilder.Factory <- fun () -> (ISentenceBuilder :?> SentenceBuilder)
            let helpText = HelpText.AutoBuild(result, Console.WindowWidth)
            helpText.Heading <- (this :> IHelpService).Heading
            helpText.Copyright <- ""
            helpText.AddNewLineBetweenHelpSections <- true

            let groupWord = ISentenceBuilder.OptionGroupWord.Invoke()
            Regex.Replace(helpText, $"\({groupWord}:.*\) ", "")
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.showHeading () =

            [
                (this :> IHelpService).Heading
                ""
            ]
            |> IConsoleBroker.writeLines
        // -------------------------------------------------------------------------------------------------------------
    
    // -----------------------------------------------------------------------------------------------------------------
