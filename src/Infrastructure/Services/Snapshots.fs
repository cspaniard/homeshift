namespace Services

open System
open System.IO
open System.Threading
open Motsoft.Util

open DI.Interfaces
open Model
open Localization
open Spectre.Console
open Spectre.Console.Rendering


type SnapshotsService (devicesBroker : IDevicesBroker, snapshotsBroker : ISnapshotsBroker,
                       consoleBroker : IConsoleBroker, usersService : IUsersService) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let getSnapshotPath (userSnapshotsPath : Directory) (dateTime : DateTimeOffset) =

        Path.Combine(userSnapshotsPath.value,
                     $"{dateTime.Year}-%02i{dateTime.Month}-%02i{dateTime.Day}_" +
                     $"%02i{dateTime.Hour}-%02i{dateTime.Minute}-%02i{dateTime.Second}")
        |> Directory.create
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    let unmountDeviceOrEx () =

        let rec tryUnmountOrEx n =
            if n < 10 then
                try
                    devicesBroker.unmountCurrentOrEx ()
                with _ ->
                    Thread.Sleep 1000
                    tryUnmountOrEx (n + 1)
            else
                failwith Errors.CouldNotUnmount

        tryUnmountOrEx 0
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> ISnapshotsService
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface ISnapshotsService with

        // -------------------------------------------------------------------------------------------------------------
        member _.createOrEx (configData : ConfigData) (createData : CreateData) =

            let userHomePath = usersService.getHomeForUserOrEx createData.UserName
            let mountPoint = devicesBroker.mountDeviceOrEx configData.SnapshotDevice

            let userSnapshotsPath = $"{mountPoint}/homeshift/snapshots/{createData.UserName}"
                                    |> Directory.create

            let mutable PressedCtrlC = false
            let CancelKeyHandler =
                ConsoleCancelEventHandler(fun _ args -> args.Cancel <- true ; PressedCtrlC <- true)

            let baseSnapshotPath = getSnapshotPath userSnapshotsPath createData.CreationDateTime

            // ---------------------------------------------------------------------------------------------------------
            [
                $"{Phrases.SnapshotCreating} ({createData.UserName.value}): " +
                $"{Path.GetFileName baseSnapshotPath.value}"
            ]
            |> consoleBroker.writeLines

            let consoleProgress =
                AnsiConsole.Progress(AutoClear = false)
                    .Columns([|
                        ElapsedTimeColumn()
                        ProgressBarColumn()
                        PercentageColumn()
                        RemainingTimeColumn()
                        SpinnerColumn()
                    |] : ProgressColumn array)
            // ---------------------------------------------------------------------------------------------------------

            // ---------------------------------------------------------------------------------------------------------
            try
                consoleProgress
                    .Start(fun ctx ->
                        let progressTask = ctx.AddTask("Snapshot progress")

                        let progressCallBack (progressString : string) =
                            if progressString <> null then
                                let progressParts = progressString |> split " "

                                if progressParts.Length > 3 then
                                    progressTask.Value <- progressParts[1] |> trimStringChars "%" |> float

                        // ---------------------------------------------------------------------------------------------
                        Console.CancelKeyPress.AddHandler CancelKeyHandler

                        snapshotsBroker.getLastSnapshotOptionInPathOrEx userSnapshotsPath
                        |> snapshotsBroker.createSnapshotOrEx userHomePath baseSnapshotPath createData progressCallBack

                        if not PressedCtrlC then
                            progressTask.Value <- 100.
                        // ---------------------------------------------------------------------------------------------
                    )
            finally
                Console.CancelKeyPress.RemoveHandler CancelKeyHandler

                if PressedCtrlC then
                    [
                        Phrases.SnapshotInterrupted
                        Phrases.DeletingIncompleteSnapshot
                        ""
                    ]
                    |> consoleBroker.writeLines

                    snapshotsBroker.deleteLastSnapshotOrEx userSnapshotsPath

                unmountDeviceOrEx ()
            // ---------------------------------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.getListForUserOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) =

            let mountPoint = devicesBroker.mountDeviceOrEx snapshotDevice

            try
                $"{mountPoint}/homeshift/snapshots/{userName}"
                |> Directory.create
                |> snapshotsBroker.getAllInfoInPathOrEx

            finally
                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.outputOrEx (userName : UserName) (snapshots : Snapshot seq) =

            snapshots |> Seq.isEmpty |> failWithIfTrue $"{Errors.SnapshotNonFound} ({userName.value})"

            consoleBroker.writeLine $"{Phrases.UserSnapshots}: {userName.value}"

            let columns = [|
                TableColumn(Phrases.SnapshotName).PadLeft(0)
                TableColumn(Phrases.SnapshotComments)
                TableColumn(Phrases.DateTimeLocal).RightAligned().PadRight(0)
            |]

            let data = [|
                for s in snapshots do
                    [|
                        Text(s.Name, Style(Color.Green))
                        Text(s.Comments.value, Style(Color.Default))
                        Text(s.CreationDateTime.LocalDateTime.ToString(), Style(Color.Blue))
                    |] : IRenderable array
            |]

            consoleBroker.writeTable (columns, data)
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteOrEx (snapshotDevice : SnapshotDevice) (deleteData : DeleteData) =

            consoleBroker.writeLine
                $"{Phrases.SnapshotDeleting} ({deleteData.UserName.value}): {deleteData.SnapshotName}"

            let consoleProgress =
                AnsiConsole.Progress()
                    .Columns(
                        [|
                            ElapsedTimeColumn()
                            ProgressBarColumn()
                            SpinnerColumn()
                        |] : ProgressColumn array
                    )

            try
                consoleProgress
                    .Start(fun ctx ->
                        let progressTask = ctx.AddTask("Delete progress", IsIndeterminate = true)

                        let mountPoint = devicesBroker.mountDeviceOrEx snapshotDevice

                        $"{mountPoint.value}/homeshift/snapshots/{deleteData.UserName.value}/{deleteData.SnapshotName}"
                        |> Directory.create
                        |> snapshotsBroker.deleteSnapshotPathOrEx

                        $"{mountPoint.value}/homeshift/snapshots/{deleteData.UserName.value}"
                        |> Directory.create
                        |> snapshotsBroker.deleteUserPathIfEmptyOrEx

                        progressTask.Value <- 100.
                    )
            finally
                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.deleteAll (snapshotDevice : SnapshotDevice) (userName : UserName) =

            try
                let snapshotList = self.getListForUserOrEx snapshotDevice userName
                let mountPoint = devicesBroker.mountDeviceOrEx snapshotDevice

                snapshotList |> Seq.isEmpty |> failWithIfTrue $"{Errors.SnapshotNonFound} ({userName.value})"

                [
                    $"{Phrases.SnapshotDeletingAll} ({userName.value})"
                    ""
                ]
                |> consoleBroker.writeLines

                snapshotList
                |> Seq.iter (fun s ->
                                $"{Phrases.SnapshotDeleting} %s{s.Name}"
                                |> consoleBroker.writeLine

                                $"{mountPoint}/homeshift/snapshots/{userName}/{s.Name}"
                                |> Directory.create
                                |> snapshotsBroker.deleteSnapshotPathOrEx)

                $"{mountPoint}/homeshift/snapshots/{userName}"
                |> Directory.create
                |> snapshotsBroker.deleteUserPathIfEmptyOrEx

                consoleBroker.writeLine ""

            finally
                unmountDeviceOrEx ()
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.isValidOrEx (snapshotDevice : SnapshotDevice) (userName : UserName) (snapshotName : string) =

            self.getListForUserOrEx snapshotDevice userName
            |> Seq.exists (fun s -> s.Name = snapshotName)
        // -------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
