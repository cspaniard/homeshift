namespace HomeShift

open System
open CommandLine
open DI.Interfaces
open Model


type App (helpService : IHelpService, consoleBroker : IConsoleBroker, listCore : IList,
          listDevicesCore : IListDevices, configCore : IConfig, createCore : ICreate,
          restoreCore : IRestore, deleteCore : IDelete) =

    interface IApp with

        member _.run () =
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
                    | :? ListOptions as opts -> listCore.showSnapshotListOrEx opts
                    | :? ListDevicesOptions -> listDevicesCore.showDeviceListOrEx ()
                    | :? ConfigOptions as opts -> configCore.configOrEx opts
                    | :? CreateOptions as opts -> createCore.createSnapshotOrEx opts
                    | :? RestoreOptions as opts -> restoreCore.restoreSnapshotOrEx opts

                    | :? DeleteOptions as deleteOptions ->
                        match parser.ParseArguments<DeleteOptionsAtLeastOne> args with
                        | :? Parsed<DeleteOptionsAtLeastOne> -> deleteCore.deleteSnapshotOrEx deleteOptions
                        | :? NotParsed<DeleteOptionsAtLeastOne> as notParsed ->
                                helpService.helpTextFromResult notParsed |> consoleBroker.writeLine
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
