namespace Brokers

open System.IO
open Newtonsoft.Json

open DI.Interfaces
open Model
open Localization


type SnapshotsBroker (processBroker : IProcessBroker) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let [<Literal>] USER_FILES_DIRECTORY = "userfiles"
    let [<Literal>] INFO_FILE_NAME = "info.json"
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    let getSnapshotFileInfo (createData : CreateData) =

        {
            CreationDateTime = createData.CreationDateTime
            Comments = createData.Comments
        } : SnapshotInfoFileData
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    let createInfoFileOrEx (infoFilePath : Directory) (snapshotFileInfo : SnapshotInfoFileData) =

        (Path.Combine(infoFilePath.value, INFO_FILE_NAME),
         JsonConvert.SerializeObject(snapshotFileInfo, Formatting.Indented))
        |> File.WriteAllText
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> ISnapshotsBroker
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface ISnapshotsBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getAllInfoInPathOrEx (path : Directory) =

            let getSnapshotInfoFromDir (dirName : string) =

                let snapshotFileInfoData =
                    Path.Combine(dirName, INFO_FILE_NAME)
                    |> File.ReadAllText
                    |> JsonConvert.DeserializeObject<SnapshotInfoFileData>

                {
                    CreationDateTime = snapshotFileInfoData.CreationDateTime
                    Name = Path.GetFileName dirName
                    Comments = snapshotFileInfoData.Comments
                } : Snapshot

            if Directory.Exists path.value then
                Directory.GetDirectories path.value
                |> Array.sort
                |> Array.map getSnapshotInfoFromDir
                |> Seq.ofArray
            else
                Seq.empty<Snapshot>
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getLastSnapshotOptionInPathOrEx (path : Directory) =

            try
                Directory.GetDirectories path.value
                |> function
                        | [||] -> None
                        | dirs -> dirs |> Array.sortDescending |> Array.head |> Directory.create |> Some
            with
            | :? DirectoryNotFoundException -> None
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.createSnapshotOrEx (sourcePath : Directory) (baseSnapshotPath : Directory)
                                    (createData : CreateData) (progressCallBack : string -> unit)
                                    (lastSnapshotPathOption : Directory option) =

            let finalDestinationPath =
                (baseSnapshotPath.value, USER_FILES_DIRECTORY)
                |> Path.Combine
                |> Directory.create

            Directory.CreateDirectory finalDestinationPath.value |> ignore

            match lastSnapshotPathOption with
            | Some lsp ->
                let linkDestPath = Path.Combine(lsp.value, USER_FILES_DIRECTORY) |> Directory.create

                processBroker.startProcessWithNotificationOrEx
                    progressCallBack
                    "rsync" ("-a --info=progress2 " +
                             $"--link-dest={linkDestPath.value} {sourcePath.value}/ {finalDestinationPath.value}")

            | None ->
                processBroker.startProcessWithNotificationOrEx
                    progressCallBack
                    "rsync" $"-a --info=progress2 {sourcePath.value}/ {finalDestinationPath.value}"

            getSnapshotFileInfo createData
            |> createInfoFileOrEx baseSnapshotPath
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteSnapshotPathOrEx (snapshotsPath : Directory) =

            Directory.Delete(snapshotsPath.value, true)
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteUserPathIfEmptyOrEx (snapshotsPath : Directory) =

            Directory.EnumerateDirectories snapshotsPath.value
            |> Seq.isEmpty
            |> function
                    | true -> Directory.Delete(snapshotsPath.value, true)
                    | false -> ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteLastSnapshotOrEx (userSnapshotsPath : Directory) =

            match self.getLastSnapshotOptionInPathOrEx userSnapshotsPath with
            | Some path -> self.deleteSnapshotPathOrEx path
            | None -> failwith Phrases.NeedToDeleteLastSnapshot
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
