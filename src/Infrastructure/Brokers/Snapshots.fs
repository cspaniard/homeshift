namespace Brokers.Snapshots

open System.IO

open Model
open Newtonsoft.Json

type private IProcessBroker = DI.Brokers.IProcessBrokerDI
type private IPhrases = DI.Services.LocalizationDI.IPhrases


type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static let [<Literal>] USER_FILES_DIRECTORY = "userfiles"
    static let [<Literal>] INFO_FILE_NAME = "info.json"
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let getSnapshotFileInfo (createData : CreateData) =

        {
            CreationDateTime = createData.CreationDateTime
            Comments = createData.Comments
        } : SnapshotInfoFileData
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let createInfoFileOrEx (infoFilePath : Directory) (snapshotFileInfo : SnapshotInfoFileData) =

        (Path.Combine(infoFilePath.value, INFO_FILE_NAME),
         JsonConvert.SerializeObject(snapshotFileInfo, Formatting.Indented))
        |> File.WriteAllText
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member getAllInfoInPathOrEx (path : Directory) =

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
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member getLastSnapshotOptionInPathOrEx (path : Directory) =

        try
            Directory.GetDirectories path.value
            |> function
               | [||] -> None
               | dirs -> (Array.sortDescending dirs)[0] |> Directory.create |> Some
        with
        | :? DirectoryNotFoundException -> None
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member createSnapshotOrEx (sourcePath : Directory) (baseSnapshotPath : Directory)
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

            IProcessBroker.startProcessWithNotificationOrEx
                progressCallBack
                "rsync" ("-a --info=progress2 " +
                         $"--link-dest={linkDestPath.value} {sourcePath.value}/ {finalDestinationPath.value}")

        | None ->
            IProcessBroker.startProcessWithNotificationOrEx
                progressCallBack
                "rsync" $"-a --info=progress2 {sourcePath.value}/ {finalDestinationPath.value}"

        getSnapshotFileInfo createData
        |> createInfoFileOrEx baseSnapshotPath
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member deleteSnapshotPathOrEx (snapshotsPath : Directory) =

        Directory.Delete(snapshotsPath.value, true)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member deleteUserPathIfEmptyOrEx (snapshotsPath : Directory) =

        Directory.EnumerateDirectories snapshotsPath.value
        |> Seq.isEmpty
        |> function
                | true -> Directory.Delete(snapshotsPath.value, true)
                | false -> ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member deleteLastSnapshotOrEx (userSnapshotsPath : Directory) =

        match Broker.getLastSnapshotOptionInPathOrEx userSnapshotsPath with
        | Some path -> Broker.deleteSnapshotPathOrEx path
        | None -> failwith IPhrases.NeedToDeleteLastSnapshot
    // -----------------------------------------------------------------------------------------------------------------
