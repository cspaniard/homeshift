namespace DI

type Dependency<'T>(def:'T) = member val D = def with get, set
