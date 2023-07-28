
open System

open CommandLine
open AppCore
open Model

type IHelpService = DI.Services.IHelpTextService

// ---------------------------------------------------------------------------------------------------------------------

try
    let args = Environment.GetCommandLineArgs() |> Array.tail

    let parser = new Parser (fun o -> o.HelpWriter <- null)

    parser.ParseArguments<ListOptions, ConfigOptions,
                          CreateOptions, RestoreOptions, DeleteOptions> args
    |> function
    | :? Parsed<obj> as command ->
        match command.Value with
        | :? ListOptions -> List.Run ()
        | :? ConfigOptions as opts -> Config.RunOfOptionsOrEx opts
        | :? CreateOptions as opts -> CreateData.ofOptions opts |> Create.Run
        | :? RestoreOptions as opts -> RestoreData.ofOptions opts |> Restore.Run

        | :? DeleteOptions as o ->
            match parser.ParseArguments<DeleteOptionsAtLeastOne> args with
            | :? Parsed<DeleteOptionsAtLeastOne> -> DeleteData.ofOptions o |> Delete.Run
            | :? NotParsed<DeleteOptionsAtLeastOne> as notParsed ->
                    IHelpService.helpTextFromResult notParsed |> Console.WriteLine
            | _ -> Console.WriteLine "Should not get here 3."

        | _ -> Console.WriteLine "Should not get here 1."

    | :? NotParsed<obj> as notParsed when notParsed.Errors.IsVersion() ->
            IHelpService.Heading |> printfn "%s\n"
    | :? NotParsed<obj> as notParsed ->
            IHelpService.helpTextFromResult notParsed |> Console.WriteLine
    | _ -> Console.WriteLine "Should not get here 2."

with e ->
    IHelpService.Heading |> printfn "%s\n"
    Console.WriteLine $"{e.Message}\n"
    Console.WriteLine $"{e.StackTrace}\n"
