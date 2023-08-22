namespace Services

open System
open System.Reflection
open System.Text.RegularExpressions
open CommandLine
open CommandLine.Text
open DI

// TODO: Evaluate to implement as another true service.
type ISentenceBuilder = Localization.LocalizedText.LocalizedSentenceBuilder


type HelpService private (consoleBroker : IConsoleBroker) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IConsoleBroker = consoleBroker
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let mutable instance = Unchecked.defaultof<IHelpService>
    
    static member getInstance (consoleBroker : IConsoleBroker) =
        
        if obj.ReferenceEquals(instance, null) then
            instance <- HelpService(consoleBroker)
        
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

            SentenceBuilder.Factory <- fun () -> ISentenceBuilder ()
            let helpText = HelpText.AutoBuild(result, Console.WindowWidth)
            helpText.Heading <- (this :> IHelpService).Heading
            helpText.Copyright <- ""
            helpText.AddNewLineBetweenHelpSections <- true

            let groupWord = ISentenceBuilder().OptionGroupWord.Invoke()
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
