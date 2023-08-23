using CommandLine;

namespace Localization.DI;

public interface ISentenceBuilder
{
    Func<string> RequiredWord { get; }
    Func<string> OptionGroupWord { get; }
    Func<string> ErrorsHeadingText { get; }
    Func<string> UsageHeadingText { get; }
    Func<bool, string> HelpCommandText { get; }
    Func<bool, string> VersionCommandText { get; }
    Func<Error, string> FormatError { get; }
    Func<IEnumerable<MutuallyExclusiveSetError>, string> FormatMutuallyExclusiveSetErrors { get; }
}