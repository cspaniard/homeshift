namespace Model

//----------------------------------------------------------------------------------------------------------------------
type ConfigData = {
    SnapshotPath : string
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
            SnapshotPath = if options.SnapshotPath = null then data.SnapshotPath else options.SnapshotPath
            ScheduleMonthly = options.ScheduleMonthly |> ifNullUseConfig data.ScheduleMonthly
            ScheduleWeekly = options.ScheduleWeekly |> ifNullUseConfig data.ScheduleWeekly
            ScheduleDaily  = options.ScheduleDaily |> ifNullUseConfig data.ScheduleDaily
            ScheduleHourly = options.ScheduleHourly |> ifNullUseConfig data.ScheduleHourly
        } : ConfigData

    static member getDefault () =
        {
            SnapshotPath = "/"
            ScheduleMonthly = false
            ScheduleWeekly = false
            ScheduleDaily  = false
            ScheduleHourly = false
        } : ConfigData
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
type CreateData = {
    User : string
    Comment : string
}
with
    static member ofOptions (o : CreateOptions) =
        {
            User = o.User
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
    Snapshot : string
    DeleteAll : bool
}
with
    static member ofOptions (o : DeleteOptions) =
        {
            Snapshot = o.Snapshot
            DeleteAll = o.DeleteAll
        } : DeleteData
//----------------------------------------------------------------------------------------------------------------------
