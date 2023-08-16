namespace Services

open System
open System.Reflection
open System.Text.RegularExpressions
open CommandLine
open CommandLine.Text

type ISentenceBuilder = Localization.LocalizedText.LocalizedSentenceBuilder

open Brokers


type HelpService private () as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IConsoleBroker = ConsoleBrokerDI.Dep.D ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let instance = HelpService()
    static member getInstance () = instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.Heading with get () =
        let version = Assembly.GetEntryAssembly().GetName().Version
        $"\nHomeshift v{version.Major}.{version.Minor}.{version.Build} by David Sanroma"
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.helpTextFromResult (result : ParserResult<_>) =

        SentenceBuilder.Factory <- fun () -> ISentenceBuilder ()
        let helpText = HelpText.AutoBuild(result, Console.WindowWidth)
        helpText.Heading <- this.Heading
        helpText.Copyright <- ""
        helpText.AddNewLineBetweenHelpSections <- true

        let groupWord = ISentenceBuilder().OptionGroupWord.Invoke()
        Regex.Replace(helpText, $"\({groupWord}:.*\) ", "")
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.showHeading () =

        [
            this.Heading
            ""
        ]
        |> IConsoleBroker.writeLines
    // -----------------------------------------------------------------------------------------------------------------


module IHelpServiceDI =
    open Localization

    let Dep = DI.Dependency (fun () ->
            failwith $"{Errors.NotInitialized} ({nameof HelpService})" : HelpService)
