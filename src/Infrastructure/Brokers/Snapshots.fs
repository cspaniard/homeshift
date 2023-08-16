namespace Brokers

open System.IO

open Model
open Newtonsoft.Json

open Localization
open Brokers

type SnapshotsBroker private () as this =

    // -----------------------------------------------------------------------------------------------------------------
    let IProcessBroker = ProcessBrokerDI.Dep.D ()
    // -----------------------------------------------------------------------------------------------------------------

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
    static let instance = SnapshotsBroker()
    static member getInstance () = instance
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
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
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.getLastSnapshotOptionInPathOrEx (path : Directory) =

        try
            Directory.GetDirectories path.value
            |> function
               | [||] -> None
               | dirs -> (Array.sortDescending dirs)[0] |> Directory.create |> Some
        with
        | :? DirectoryNotFoundException -> None
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
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
    member _.deleteSnapshotPathOrEx (snapshotsPath : Directory) =

        Directory.Delete(snapshotsPath.value, true)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.deleteUserPathIfEmptyOrEx (snapshotsPath : Directory) =

        Directory.EnumerateDirectories snapshotsPath.value
        |> Seq.isEmpty
        |> function
                | true -> Directory.Delete(snapshotsPath.value, true)
                | false -> ()
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    member _.deleteLastSnapshotOrEx (userSnapshotsPath : Directory) =

        match this.getLastSnapshotOptionInPathOrEx userSnapshotsPath with
        | Some path -> this.deleteSnapshotPathOrEx path
        | None -> failwith Phrases.NeedToDeleteLastSnapshot
    // -----------------------------------------------------------------------------------------------------------------

module SnapshotsBrokerDI =

    let Dep = DI.Dependency (fun () ->
            failwith $"{Errors.NotInitialized} ({nameof SnapshotsBroker})" : SnapshotsBroker)
