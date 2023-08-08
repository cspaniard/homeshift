namespace Model

open Newtonsoft.Json
open Model.JsonConverters

//----------------------------------------------------------------------------------------------------------------------
type ListData = {
    UserName : UserName
    ListAll : bool
}
with
    static member ofOptions (options : ListOptions) =
        {
            UserName = options.UserName |> UserName.create
            ListAll = false    // ToDo: Just for testing. FIX IT.
        } : ListData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
type ConfigData = {
    [<JsonConverter(typeof<SnapshotDeviceConverter>)>]
    SnapshotDevice : SnapshotDevice
    ScheduleMonthly : bool
    ScheduleWeekly : bool
    ScheduleDaily : bool
    ScheduleHourly : bool
}
with
    static member mergeWithOptions (options : ConfigOptions) (data : ConfigData) =

        let inline ifNullUseConfig confValue optValue =

            match Option.ofNullable optValue with
            | Some v -> v
            | None -> confValue

        {
            SnapshotDevice = if options.SnapshotDevice = null
                             then data.SnapshotDevice
                             else SnapshotDevice.create <| options.SnapshotDevice.Trim()
            ScheduleMonthly = options.ScheduleMonthly |> ifNullUseConfig data.ScheduleMonthly
            ScheduleWeekly = options.ScheduleWeekly |> ifNullUseConfig data.ScheduleWeekly
            ScheduleDaily  = options.ScheduleDaily |> ifNullUseConfig data.ScheduleDaily
            ScheduleHourly = options.ScheduleHourly |> ifNullUseConfig data.ScheduleHourly
        } : ConfigData

    static member getDefault () =
        {
            SnapshotDevice = "not_configured" |> SnapshotDevice.create
            ScheduleMonthly = false
            ScheduleWeekly = false
            ScheduleDaily  = false
            ScheduleHourly = false
        } : ConfigData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
type CreateData = {
    UserName : UserName
    Comment : string
}
with
    static member ofOptions (o : CreateOptions) =
        {
            UserName = o.UserName |> UserName.create
            Comment = o.Comment
        } : CreateData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
type RestoreData = {
    Snapshot : string
}
with
    static member ofOptions (o : RestoreOptions) =
        {
            Snapshot = o.Snapshot
        } : RestoreData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
type DeleteData = {
    UserName : UserName
    Snapshot : string
    DeleteAll : bool
}
with
    static member ofOptions (o : DeleteOptions) =
        {
            UserName = o.UserName |> UserName.create
            Snapshot = o.Snapshot
            DeleteAll = o.DeleteAll
        } : DeleteData
//----------------------------------------------------------------------------------------------------------------------
