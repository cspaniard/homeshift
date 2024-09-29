namespace HomeShift

open System
open AppCore
open CommandLine
open DI.Interfaces
open Model

type App (helpService : IHelpService, consoleBroker : IConsoleBroker, listCore : IList,
    listDevicesCore : IListDevices) =

    interface IApp with

        member _.Run () =
            try
                consoleBroker.enableStdOut()

                let args = Environment.GetCommandLineArgs() |> Array.tail

                let parser = new Parser (fun o -> o.HelpWriter <- null)

                parser.ParseArguments<ListOptions, ListDevicesOptions, ConfigOptions,
                                      CreateOptions, RestoreOptions, DeleteOptions> args
                |> function
                | :? Parsed<obj> as command ->

                    helpService.showHeading ()

                    match command.Value with
                    | :? ListOptions as opts -> listCore.cliShowSnapshotListOrEx opts
                    | :? ListDevicesOptions -> listDevicesCore.showDeviceList ()
                    | :? ConfigOptions as opts -> Config.CLI.configOrEx opts
                    | :? CreateOptions as opts -> Create.CLI.createSnapshotOrEx opts
                    | :? RestoreOptions as opts -> RestoreData.ofOptions opts |> Restore.runOrEx

                    | :? DeleteOptions as deleteOptions ->
                        match parser.ParseArguments<DeleteOptionsAtLeastOne> args with
                        | :? Parsed<DeleteOptionsAtLeastOne> -> Delete.CLI.deleteSnapshotOrEx deleteOptions
                        | :? NotParsed<DeleteOptionsAtLeastOne> as notParsed ->
                                helpService.helpTextFromResult notParsed |> Console.WriteLine
                        | _ -> Console.WriteLine "Should not get here 3."

                    | _ -> Console.WriteLine "Should not get here 1."

                | :? NotParsed<obj> as notParsed when notParsed.Errors.IsVersion() ->
                        helpService.showHeading ()
                | :? NotParsed<obj> as notParsed ->
                        helpService.helpTextFromResult notParsed |> Console.WriteLine
                | _ -> Console.WriteLine "Should not get here 2."

            with e ->
                consoleBroker.writeInnerExceptions e
                // Console.WriteLine $"{e.StackTrace}\n"
