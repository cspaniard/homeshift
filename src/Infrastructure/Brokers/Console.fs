namespace Brokers

open System
open System.IO
open DI
open Motsoft.Util


type ConsoleBroker private () as this =

    // -----------------------------------------------------------------------------------------------------------------
    static let stdOut = Console.Out

    static do
        Console.SetOut(new StringWriter())
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let instance = ConsoleBroker ()
    static member getInstance () = instance
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
        member _.WriteMatrix (rightAlignments : bool array) (hasHeader : bool) (data : string array array) =

            // ToDo: Check rightAlignments size vs data line size. Must be the same.
            // ToDo: Maybe there is way to pass less information in the rightAlignments array.

            let columnGap = "   "

            let widths =
                array2D
                    [|
                        for row in data do
                            [|
                                for column in row do
                                    if column <> null
                                        then column.Length
                                        else 0
                            |]
                    |]

            let columnWidths =
                [|
                    for i in [0..Array2D.length2 widths - 1] do
                        widths[*, i] |> Array.max
                |]

            data
            |> Array.iteri (
                    fun rowIdx line ->
                        line
                        |> Array.iteri (
                                fun colIdx column ->
                                    let format = "{0," +
                                                 (if rightAlignments[colIdx] then "" else "-") +
                                                 columnWidths[colIdx].ToString() + "}" + columnGap

                                    String.Format(format, column)
                                    |> Console.Write)
                        Console.WriteLine ""

                        if rowIdx = 0 && hasHeader then
                            let length = (columnWidths |> Array.sum) + (columnGap.Length * (columnWidths.Length - 1))
                            Console.WriteLine(String('-', length)))
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.WriteMatrixWithFooter (rightAlignments : bool array) (hasHeader : bool)
                                       (footer : string seq) (data : string array array) =

            (this :> IConsoleBroker).WriteMatrix rightAlignments hasHeader data

            footer
            |> (this :> IConsoleBroker).writeLines
        // -----------------------------------------------------------------------------------------------------------------
    
    // -----------------------------------------------------------------------------------------------------------------
