namespace Services

open System
open System.Reflection
open System.Text.RegularExpressions
open CommandLine
open CommandLine.Text

open DI.Interfaces
open Localization.DI


type HelpService (consoleBroker : IConsoleBroker, sentenceBuilder : ISentenceBuilder) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> IHelpService
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

            SentenceBuilder.Factory <- fun () -> (sentenceBuilder :?> SentenceBuilder)
            let helpText = HelpText.AutoBuild(result, Console.WindowWidth)
            helpText.Heading <- self.Heading
            helpText.Copyright <- ""
            helpText.AddNewLineBetweenHelpSections <- true

            let groupWord = sentenceBuilder.OptionGroupWord.Invoke()
            Regex.Replace(helpText, $"\({groupWord}:.*\) ", "")
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.showHeading () =

            [
                self.Heading
                ""
            ]
            |> consoleBroker.writeLines
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
