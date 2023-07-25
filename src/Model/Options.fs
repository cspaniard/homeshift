namespace Model

open System
open CommandLine
open HomeShift.Loc

open type HomeShift.Loc.LocaleText


//----------------------------------------------------------------------------------------------------------------------
[<Verb("list", HelpText = nameof VerbList, ResourceType = typeof<LocaleText>)>]
type ListOptions () = class end
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
[<Verb("config", HelpText = nameof VerbConfig, ResourceType = typeof<LocaleText>)>]
type ConfigOptions = {
    [<Option ("snapshot-path", Group = "config",
              HelpText = nameof ConfigSnapshotPath, ResourceType = typeof<LocaleText>)>]
    SnapshotPath : string

    [<Option ("schedule-monthly", Group = "config",
              HelpText = nameof ConfigScheduleMonthly, ResourceType = typeof<LocaleText>)>]
    ScheduleMonthly : Nullable<bool>

    [<Option ("schedule-weekly", Group = "config",
              HelpText = nameof ConfigScheduleWeekly, ResourceType = typeof<LocaleText>)>]
    ScheduleWeekly : Nullable<bool>

    [<Option ("schedule-daily", Group = "config",
              HelpText = nameof ConfigScheduleDaily, ResourceType = typeof<LocaleText>)>]
    ScheduleDaily : Nullable<bool>

    [<Option ("schedule-hourly", Group = "config",
              HelpText = nameof ConfigScheduleHourly, ResourceType = typeof<LocaleText>)>]
    ScheduleHourly : Nullable<bool>
}
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
[<Verb("create", HelpText = nameof VerbCreate, ResourceType = typeof<LocaleText>)>]
type CreateOptions = {
    [<Option ("user", Required = true, HelpText = nameof CreateUser, ResourceType = typeof<LocaleText>)>]
    User : string

    [<Option ("comment", HelpText = nameof CreateComments , ResourceType = typeof<LocaleText>)>]
    Comment : string
}
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
[<Verb("restore", HelpText = nameof VerbRestore, ResourceType = typeof<LocaleText>)>]
type RestoreOptions = {
    [<Option ("snapshot", Required = true, HelpText = nameof RestoreSnapshot, ResourceType = typeof<LocaleText>)>]
    Snapshot : string
}
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
[<Verb("delete", HelpText = nameof VerbDelete, ResourceType = typeof<LocaleText>)>]
type DeleteOptions = {
    [<Option ("snapshot", SetName = "snapshot", HelpText = nameof DeleteSnapshot, ResourceType = typeof<LocaleText>)>]
    Snapshot : string

    [<Option ("all", SetName = "deleteAll", HelpText = nameof DeleteAll, ResourceType = typeof<LocaleText>)>]
    DeleteAll : bool
}
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
type DeleteOptionsAtLeastOne = {
    [<Option ("snapshot", Group = "delete", HelpText = nameof DeleteSnapshot, ResourceType = typeof<LocaleText>)>]
    Snapshot : string

    [<Option ("all", Group = "delete", HelpText = nameof DeleteAll, ResourceType = typeof<LocaleText>)>]
    DeleteAll : bool
}
//----------------------------------------------------------------------------------------------------------------------
