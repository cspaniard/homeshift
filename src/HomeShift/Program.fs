
open System

open System.Reflection
open System.Text.RegularExpressions
open CommandLine
open CommandLine.Text
open HomeShift.Loc.SentenceBuilder
open Model

let buildHeading () =
    let version = Assembly.GetEntryAssembly().GetName().Version
    $"\nHomeshift v{version.Major}.{version.Minor}.{version.Build} by David Sanroma"

let helpTextFromResult (result : ParserResult<_>) =

    SentenceBuilder.Factory <- fun () -> LocalizedSentenceBuilder()
    let helpText = HelpText.AutoBuild(result, 100)
    helpText.MaximumDisplayWidth <- Console.WindowWidth
    helpText.Heading <- buildHeading ()
    helpText.Copyright <- ""
    helpText.AddNewLineBetweenHelpSections <- true

    let groupWord = LocalizedSentenceBuilder().OptionGroupWord.Invoke()
    Regex.Replace(helpText, $"\({groupWord}:.*\) ", "")

// -------------------------------------------------------------------------

let args = Environment.GetCommandLineArgs() |> Array.tail

let parser = new Parser (fun o -> o.HelpWriter <- null)
parser.ParseArguments<ListOptions, ConfigOptions,
                      CreateOptions, RestoreOptions, DeleteOptions> args
|> function
| :? Parsed<obj> as command ->
    match command.Value with
    | :? ListOptions as o -> Console.WriteLine $"Using ListOptions: \n%A{o}"
    | :? ConfigOptions as o -> Console.WriteLine $"Dealing with ConfigOptions: \n%A{o}"
    | :? CreateOptions as o -> Console.WriteLine $"Using CreateOptions: \n%A{o}"

    | :? DeleteOptions as o ->
        match parser.ParseArguments<DeleteOptionsAtLeastOne> args with
        | :? Parsed<DeleteOptionsAtLeastOne> -> Console.WriteLine $"Using DeleteOptions: \n%A{o}"
        | :? NotParsed<DeleteOptionsAtLeastOne> as notParsed -> helpTextFromResult notParsed |> Console.WriteLine
        | _ -> Console.WriteLine "Should not get here 3."

    | _ -> Console.WriteLine "Should not get here 1."

| :? NotParsed<obj> as notParsed when notParsed.Errors.IsVersion() -> buildHeading () |> printfn "%s\n"
| :? NotParsed<obj> as notParsed -> helpTextFromResult notParsed |> Console.WriteLine
| _ -> Console.WriteLine "Should not get here 2."
