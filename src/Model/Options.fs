namespace Model

open System
open CommandLine
open Localization

open type Localization.CliOptions


//----------------------------------------------------------------------------------------------------------------------
[<Verb("list", HelpText = nameof VerbList, ResourceType = typeof<CliOptions>)>]
type ListOptions () = class end
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
[<Verb("config", HelpText = nameof VerbConfig, ResourceType = typeof<CliOptions>)>]
type ConfigOptions = {
    [<Option ("snapshot-device", Group = "config",
              HelpText = nameof ConfigSnapshotDevice, ResourceType = typeof<CliOptions>)>]
    SnapshotDevice : string

    [<Option ("schedule-monthly", Group = "config",
              HelpText = nameof ConfigScheduleMonthly, ResourceType = typeof<CliOptions>)>]
    ScheduleMonthly : Nullable<bool>

    [<Option ("schedule-weekly", Group = "config",
              HelpText = nameof ConfigScheduleWeekly, ResourceType = typeof<CliOptions>)>]
    ScheduleWeekly : Nullable<bool>

    [<Option ("schedule-daily", Group = "config",
              HelpText = nameof ConfigScheduleDaily, ResourceType = typeof<CliOptions>)>]
    ScheduleDaily : Nullable<bool>

    [<Option ("schedule-hourly", Group = "config",
              HelpText = nameof ConfigScheduleHourly, ResourceType = typeof<CliOptions>)>]
    ScheduleHourly : Nullable<bool>
}
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
[<Verb("create", HelpText = nameof VerbCreate, ResourceType = typeof<CliOptions>)>]
type CreateOptions = {
    [<Option ("user", Required = true, HelpText = nameof CreateUser, ResourceType = typeof<CliOptions>)>]
    User : string

    [<Option ("comment", HelpText = nameof CreateComments , ResourceType = typeof<CliOptions>)>]
    Comment : string
}
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
[<Verb("restore", HelpText = nameof VerbRestore, ResourceType = typeof<CliOptions>)>]
type RestoreOptions = {
    [<Option ("snapshot", Required = true, HelpText = nameof RestoreSnapshot, ResourceType = typeof<CliOptions>)>]
    Snapshot : string
}
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
[<Verb("delete", HelpText = nameof VerbDelete, ResourceType = typeof<CliOptions>)>]
type DeleteOptions = {
    [<Option ("snapshot", SetName = "snapshot", HelpText = nameof DeleteSnapshot, ResourceType = typeof<CliOptions>)>]
    Snapshot : string

    [<Option ("all", SetName = "deleteAll", HelpText = nameof DeleteAll, ResourceType = typeof<CliOptions>)>]
    DeleteAll : bool
}
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
type DeleteOptionsAtLeastOne = {
    [<Option ("snapshot", Group = "delete", HelpText = nameof DeleteSnapshot, ResourceType = typeof<CliOptions>)>]
    Snapshot : string

    [<Option ("all", Group = "delete", HelpText = nameof DeleteAll, ResourceType = typeof<CliOptions>)>]
    DeleteAll : bool
}
//----------------------------------------------------------------------------------------------------------------------
