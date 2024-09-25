namespace Brokers

open System
open System.IO

open DI.Interfaces
open Spectre.Console
open Spectre.Console.Rendering

type private TableBorderAscii () =
    inherit TableBorder()

    override _.GetPart (part : TableBorderPart) : string =

        match part with
        | TableBorderPart.HeaderTopLeft -> ""
        | TableBorderPart.HeaderTop -> ""
        | TableBorderPart.HeaderTopSeparator -> ""
        | TableBorderPart.HeaderTopRight -> ""
        | TableBorderPart.HeaderLeft -> ""
        | TableBorderPart.HeaderSeparator -> ""
        | TableBorderPart.HeaderRight -> ""
        | TableBorderPart.HeaderBottomLeft -> ""
        | TableBorderPart.HeaderBottom -> "-"
        | TableBorderPart.HeaderBottomSeparator -> ""
        | TableBorderPart.HeaderBottomRight -> ""
        | TableBorderPart.CellLeft -> ""
        | TableBorderPart.CellSeparator -> ""
        | TableBorderPart.CellRight -> ""
        | TableBorderPart.FooterTopLeft -> ""
        | TableBorderPart.FooterTop -> ""
        | TableBorderPart.FooterTopSeparator -> ""
        | TableBorderPart.FooterTopRight -> ""
        | TableBorderPart.FooterBottomLeft -> ""
        | TableBorderPart.FooterBottom -> ""
        | TableBorderPart.FooterBottomSeparator -> ""
        | TableBorderPart.FooterBottomRight -> ""
        | TableBorderPart.RowLeft -> ""
        | TableBorderPart.RowCenter -> ""
        | TableBorderPart.RowSeparator -> ""
        | TableBorderPart.RowRight -> ""
        | _ -> failwith "Unknown border part."

type ConsoleBroker () as this =

    // -----------------------------------------------------------------------------------------------------------------
    static let stdOut = Console.Out

    static do
        Console.SetOut(new StringWriter())
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> IConsoleBroker
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IConsoleBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.enableStdOut () =

            Console.SetOut stdOut
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.write (line : string) =

            Console.Write line
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.writeLine (line : string) =

            Console.WriteLine line
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.writeLines (lines : string seq) =

            lines
            |> Seq.iter Console.WriteLine
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.writeTable (columns : TableColumn array) (data : string array array) =

            let outputTable = Table()
            outputTable.Border <- TableBorderAscii ()

            outputTable.AddColumns columns |> ignore

            data
            |> Array.iter (fun row -> outputTable.AddRow row |> ignore)

            AnsiConsole.Write outputTable
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.writeInnerExceptions (e : Exception) =

            self.writeLine e.Message
            if e.InnerException <> null then self.writeInnerExceptions e.InnerException
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
