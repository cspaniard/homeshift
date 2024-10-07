namespace HomeShiftGtk

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
