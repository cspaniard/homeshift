namespace Brokers.Snapshots

open System
open System.Diagnostics
open System.IO
open Motsoft.Util

open Model
open Newtonsoft.Json

type private IProcessBroker = DI.Brokers.IProcessBrokerDI
type private IPhrases = DI.Services.LocalizationDI.IPhrases

type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static let getSnapshotFileInfo (createData : CreateData) =

        {
            CreationDateTime = createData.CreationDateTime
            Comments = if createData.Comments = null then "" else createData.Comments
        }
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let getSnapshotPath (userSnapshotsPath : Directory) (dateTime : DateTimeOffset) =

        Path.Combine(userSnapshotsPath.value,
                     $"{dateTime.Year}-%02i{dateTime.Month}-%02i{dateTime.Day}_" +
                     $"%02i{dateTime.Hour}-%02i{dateTime.Minute}-%02i{dateTime.Second}")
        |> Directory.create
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static let createInfoFileOrEx (infoFilePath : Directory) (snapshotFileInfo : SnapshotFileInfo) =

        (Path.Combine(infoFilePath.value, "info.json"),
         JsonConvert.SerializeObject(snapshotFileInfo, Formatting.Indented))
        |> File.WriteAllText
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member getAllInfoInPathOrEx (path : Directory) =

        Directory.GetDirectories path.value
        // ToDo: We need to dive in each one to get extra info like Description.
        |> Array.map (fun d -> { Name = Path.GetFileName d } : Snapshot)
        |> Seq.ofArray
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
    static member createSnapshotOrEx (sourcePath : Directory) (userSnapshotsPath : Directory)
                                     (createData : CreateData)
                                     (lastSnapshotPathOption : Directory option) =

        let baseSnapshotPath = getSnapshotPath userSnapshotsPath createData.CreationDateTime

        let finalDestinationPath =
           (baseSnapshotPath.value, "userfiles")
           |> Path.Combine
           |> Directory.create

        Directory.CreateDirectory finalDestinationPath.value |> ignore

        let stopWatch = Stopwatch.StartNew()

        let progressCallBack (progressString : string) =
            if progressString <> null then
                let progressParts = progressString |> String.split " "

                if progressParts.Length > 3 then
                    Console.Write($"{IPhrases.Elapsed}: %02i{stopWatch.Elapsed.Hours}:" +
                                  $"%02i{stopWatch.Elapsed.Minutes}:%02i{stopWatch.Elapsed.Seconds} - " +
                                  $"{IPhrases.Completed}: {progressParts[1]} - " +
                                  $"{IPhrases.TimeRemaining}: {progressParts[3]}     \r")

        match lastSnapshotPathOption with
        | Some lsp ->
            let linkDestPath = Path.Combine(lsp.value, "userfiles") |> Directory.create

            IProcessBroker.startProcessWithNotificationOrEx
                progressCallBack
                "rsync" ("-a --info=progress2 " +
                         $"--link-dest={linkDestPath.value} {sourcePath.value}/ {finalDestinationPath.value}")

        | None ->
            IProcessBroker.startProcessWithNotificationOrEx
                progressCallBack
                "rsync" $"-a --info=progress2 {sourcePath.value}/ {finalDestinationPath.value}"

        stopWatch.Stop()

        getSnapshotFileInfo createData
        |> createInfoFileOrEx baseSnapshotPath

        Console.WriteLine($"{IPhrases.Elapsed}: %02i{stopWatch.Elapsed.Hours}:" +
                          $"%02i{stopWatch.Elapsed.Minutes}:%02i{stopWatch.Elapsed.Seconds} - " +
                          $"{IPhrases.Completed} 100%% - {IPhrases.TimeRemaining}: 0:00:00          \n")
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    static member deleteLastSnapshotOrEx (userSnapshotsPath : Directory) =

        match Broker.getLastSnapshotOptionInPathOrEx userSnapshotsPath with
        | Some path -> Directory.Delete(path.value, true)
        | None -> failwith IPhrases.NeedToDeleteLastSnapshot
    // -----------------------------------------------------------------------------------------------------------------
