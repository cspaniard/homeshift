namespace Model

open System

//----------------------------------------------------------------------------------------------------------------------
type ConfigData = {
    SnapshotPath : string
    ScheduleMonthly : Nullable<bool>
    ScheduleWeekly : Nullable<bool>
    ScheduleDaily : Nullable<bool>
    ScheduleHourly : Nullable<bool>
}
with
    static member ofOptions (o : ConfigOptions) =
        {
            SnapshotPath = o.SnapshotPath
            ScheduleMonthly = o.ScheduleMonthly
            ScheduleWeekly = o.ScheduleWeekly
            ScheduleDaily  = o.ScheduleDaily
            ScheduleHourly = o.ScheduleMonthly
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
