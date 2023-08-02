namespace Brokers.Console

open System
open Motsoft.Util


type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static member writeLines (lines : string seq) =

        lines
        |> Seq.iter Console.WriteLine
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member WriteMatrix (rightAlignments : bool array) (hasHeader : bool) (data : string array array) =

        // ToDo: Check rightAlignments size vs data line size. Must be the same.

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
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member WriteMatrixWithFooter (rightAlignments : bool array) (hasHeader : bool)
                                        (footer : string seq) (data : string array array) =

        Broker.WriteMatrix rightAlignments hasHeader data

        footer
        |> Broker.writeLines
    // -----------------------------------------------------------------------------------------------------------------
