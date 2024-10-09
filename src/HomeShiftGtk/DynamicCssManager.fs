namespace HomeShiftGtk

open System
open Gtk
open System.Text.RegularExpressions


type DynamicCssManager() =

    //------------------------------------------------------------------------------------------------------------------
    static let mutable cssContent = ""
    static let cssProvider = new CssProvider()
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    static member loadFromPath (cssFilePath: string) =

        cssProvider.LoadFromPath(cssFilePath) |> ignore
        cssContent <- System.IO.File.ReadAllText(cssFilePath)
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    static member formatCSS (css: string) : string =
        let mutable indentLevel = 0
        let mutable formattedCSS = ""
        let lines = Regex.Split(css.Trim(), @"\s*([{}])\s*")

        let isProperty (line: string) =
            let propertyPattern = @"[a-zA-Z-]+\s*:\s*[^;]+;"
            Regex.IsMatch(line, propertyPattern)

        for line in lines do
            match line with
            | "{" ->
                formattedCSS <- formattedCSS.TrimEnd() + " {\n"
                indentLevel <- indentLevel + 1
            | "}" ->
                indentLevel <- indentLevel - 1
                formattedCSS <- formattedCSS.TrimEnd() + "\n" + String.replicate (4 * indentLevel) " " + "}\n\n"
            | _ when line.Trim() <> "" ->
                if isProperty line then
                    // Es una propiedad CSS o conjunto de propiedades
                    let properties = Regex.Matches(line, @"([a-zA-Z-]+)\s*:\s*([^;]+);")
                    for prop in properties do
                        let name = prop.Groups[1].Value.Trim()
                        let value = prop.Groups[2].Value.Trim()
                        formattedCSS <- formattedCSS + String.replicate (4 * indentLevel) " " + name + ": " + value + ";\n"
                else
                    // Es un selector CSS (incluyendo pseudo-clases)
                    formattedCSS <- formattedCSS + (if indentLevel > 0 then "\n" else "") + String.replicate (4 * indentLevel) " " + line.Trim() + "\n"
            | _ -> ()

        formattedCSS.TrimEnd()
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    static member addOrUpdate (className: string) (propertyName: string) (value: string) =

        let classPattern = $@"\.%s{className}\s*{{([^}}]*)}}"
        let propertyPattern = $@"(%s{propertyName}\s*:\s*)[^;]+(;)"

        let updateOrAddProperty (classContent: string) =
            if Regex.IsMatch(classContent, propertyPattern) then
                // Update existing property
                Regex.Replace(classContent, propertyPattern, $"${{1}}%s{value}${{2}}")
            else
                // Add new property
                classContent.TrimEnd() + $"%s{propertyName}: %s{value};\n"

        if Regex.IsMatch(cssContent, classPattern) then
            // Update existing class
            cssContent <- Regex.Replace(cssContent, classPattern,
                fun m -> $".%s{className} {{%s{updateOrAddProperty m.Groups[1].Value}}}")
        else
            // Add new class
            cssContent <- cssContent + $"\n.%s{className} {{\n    %s{propertyName}: %s{value};\n}}\n"

        cssProvider.LoadFromData cssContent
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    static member getClassDefinition (className: string) : string =

        let classPattern = $@"\.%s{className}\s*{{([^}}]*)}}"
        let matchSearch = Regex.Match(cssContent, classPattern)

        if matchSearch.Success then
            let classContent = matchSearch.Groups[1].Value.Trim()
            $".%s{className} {{\n%s{classContent}\n}}"
        else
            ""
    //------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------------
    static member applyToScreen () =
        StyleContext.AddProviderForScreen(
            Gdk.Screen.Default,
            cssProvider,
            StyleProviderPriority.Application
        )
    //------------------------------------------------------------------------------------------------------------------
