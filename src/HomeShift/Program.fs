
open System

open System.Diagnostics.CodeAnalysis
open CommandLine
open AppCore
open Localization
open Model

type private IHelpService = DI.Services.IHelpTextService
type private IConsoleBroker = DI.Brokers.IConsoleBroker

// ---------------------------------------------------------------------------------------------------------------------
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<ListOptions>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<ListDevicesOptions>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<ConfigOptions>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<CreateOptions>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<RestoreOptions>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<DeleteOptions>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<DeleteOptionsAtLeastOne>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<CliOptions>)>]
// ---------------------------------------------------------------------------------------------------------------------

try
    IConsoleBroker.enableStdOut()

    let args = Environment.GetCommandLineArgs() |> Array.tail

    let parser = new Parser (fun o -> o.HelpWriter <- null)

    parser.ParseArguments<ListOptions, ListDevicesOptions, ConfigOptions,
                          CreateOptions, RestoreOptions, DeleteOptions> args
    |> function
    | :? Parsed<obj> as command ->

        IHelpService.showHeading ()

        match command.Value with
        | :? ListOptions as opts -> List.CLI.showSnapshotList opts
        | :? ListDevicesOptions -> ListDevices.CLI.showDeviceList ()
        | :? ConfigOptions as opts -> Config.CLI.storeConfigOrEx opts
        | :? CreateOptions as opts -> Create.CLI.createSnapshotOrEx opts
        | :? RestoreOptions as opts -> RestoreData.ofOptions opts |> Restore.runOrEx

        | :? DeleteOptions as deleteOptions ->
            match parser.ParseArguments<DeleteOptionsAtLeastOne> args with
            | :? Parsed<DeleteOptionsAtLeastOne> -> Delete.CLI.deleteSnapshotOrEx deleteOptions
            | :? NotParsed<DeleteOptionsAtLeastOne> as notParsed ->
                    IHelpService.helpTextFromResult notParsed |> Console.WriteLine
            | _ -> Console.WriteLine "Should not get here 3."

        | _ -> Console.WriteLine "Should not get here 1."

    | :? NotParsed<obj> as notParsed when notParsed.Errors.IsVersion() ->
            IHelpService.showHeading ()
    | :? NotParsed<obj> as notParsed ->
            IHelpService.helpTextFromResult notParsed |> Console.WriteLine
    | _ -> Console.WriteLine "Should not get here 2."

with e ->
    Console.WriteLine $"{e.Message}\n"
    // Console.WriteLine $"{e.StackTrace}\n"
