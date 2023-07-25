
open System

open CommandLine
open Model

type IHelpService = DI.Services.HelpDI.IHelpService

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
        | :? NotParsed<DeleteOptionsAtLeastOne> as notParsed ->
            IHelpService.HelpTextFromResult notParsed |> Console.WriteLine
        | _ -> Console.WriteLine "Should not get here 3."

    | _ -> Console.WriteLine "Should not get here 1."

| :? NotParsed<obj> as notParsed when notParsed.Errors.IsVersion() ->
    IHelpService.Heading |> printfn "%s\n"
| :? NotParsed<obj> as notParsed ->
    IHelpService.HelpTextFromResult notParsed |> Console.WriteLine
| _ -> Console.WriteLine "Should not get here 2."
