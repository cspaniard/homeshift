namespace Services.HelpText

open System
open System.Reflection
open System.Text.RegularExpressions
open CommandLine
open CommandLine.Text

type ISentenceBuilder = DI.Services.LocalizationDI.ISentenceBuilder

type Service () =

    static member Heading with get () =
        let version = Assembly.GetEntryAssembly().GetName().Version
        $"\nHomeshift v{version.Major}.{version.Minor}.{version.Build} by David Sanroma"

    static member public helpTextFromResult (result : ParserResult<_>) =

        SentenceBuilder.Factory <- fun () -> ISentenceBuilder ()
        let helpText = HelpText.AutoBuild(result, 100)
        helpText.MaximumDisplayWidth <- Console.WindowWidth
        helpText.Heading <- Service.Heading
        helpText.Copyright <- ""
        helpText.AddNewLineBetweenHelpSections <- true

        let groupWord = ISentenceBuilder().OptionGroupWord.Invoke()
        Regex.Replace(helpText, $"\({groupWord}:.*\) ", "")
