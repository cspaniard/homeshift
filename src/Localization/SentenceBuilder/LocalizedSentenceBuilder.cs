using CommandLine;

namespace HomeShift.Loc.SentenceBuilder
{

    public class LocalizedSentenceBuilder : CommandLine.Text.SentenceBuilder
    {
        public override Func<string> RequiredWord
        {
            get { return () => LocSentenceBuilder.SentenceRequiredWord; }
        }

        public override Func<string> OptionGroupWord
        {
            get { return () => LocSentenceBuilder.SentenceGroupWord; }
        }

        public override Func<string> ErrorsHeadingText
        {
            // Cannot be pluralized
            get { return () => LocSentenceBuilder.SentenceErrorsHeadingText; }
        }

        public override Func<string> UsageHeadingText
        {
            get { return () => LocSentenceBuilder.SentenceUsageHeadingText; }
        }

        public override Func<bool, string> HelpCommandText
        {
            get
            {
                return isOption => isOption
                    ? LocSentenceBuilder.SentenceHelpCommandTextOption
                    : LocSentenceBuilder.SentenceHelpCommandTextVerb;
            }
        }

        public override Func<bool, string> VersionCommandText
        {
            get { return _ => LocSentenceBuilder.SentenceVersionCommandText; }
        }

        public override Func<Error, string> FormatError
        {
            get
            {
                return error =>
                {
                    switch (error.Tag)
                    {
                        case ErrorType.BadFormatTokenError:
                            return string.Format(LocSentenceBuilder.SentenceBadFormatTokenError,
                                ((BadFormatTokenError)error).Token);
                        case ErrorType.MissingValueOptionError:
                            return string.Format(LocSentenceBuilder.SentenceMissingValueOptionError,
                                ((MissingValueOptionError)error).NameInfo.NameText);
                        case ErrorType.UnknownOptionError:
                            return string.Format(LocSentenceBuilder.SentenceUnknownOptionError,
                                ((UnknownOptionError)error).Token);
                        case ErrorType.MissingRequiredOptionError:
                            var errMisssing = ((MissingRequiredOptionError)error);
                            return errMisssing.NameInfo.Equals(NameInfo.EmptyName)
                                ? LocSentenceBuilder.SentenceMissingRequiredOptionError
                                : string.Format(LocSentenceBuilder.SentenceMissingRequiredOptionError,
                                    errMisssing.NameInfo.NameText);
                        case ErrorType.BadFormatConversionError:
                            var badFormat = ((BadFormatConversionError)error);
                            return badFormat.NameInfo.Equals(NameInfo.EmptyName)
                                ? LocSentenceBuilder.SentenceBadFormatConversionErrorValue
                                : string.Format(LocSentenceBuilder.SentenceBadFormatConversionErrorOption,
                                    badFormat.NameInfo.NameText);
                        case ErrorType.SequenceOutOfRangeError:
                            var seqOutRange = ((SequenceOutOfRangeError)error);
                            return seqOutRange.NameInfo.Equals(NameInfo.EmptyName)
                                ? LocSentenceBuilder.SentenceSequenceOutOfRangeErrorValue
                                : string.Format(LocSentenceBuilder.SentenceSequenceOutOfRangeErrorOption,
                                    seqOutRange.NameInfo.NameText);
                        case ErrorType.BadVerbSelectedError:
                            return string.Format(LocSentenceBuilder.SentenceBadVerbSelectedError,
                                ((BadVerbSelectedError)error).Token);
                        case ErrorType.NoVerbSelectedError:
                            return LocSentenceBuilder.SentenceNoVerbSelectedError;
                        case ErrorType.RepeatedOptionError:
                            return string.Format(LocSentenceBuilder.SentenceRepeatedOptionError,
                                ((RepeatedOptionError)error).NameInfo.NameText);
                        case ErrorType.SetValueExceptionError:
                            var setValueError = (SetValueExceptionError)error;
                            return string.Format(LocSentenceBuilder.SentenceSetValueExceptionError,
                                setValueError.NameInfo.NameText, setValueError.Exception.Message);
                        case ErrorType.MutuallyExclusiveSetError:
                            break;
                        case ErrorType.HelpRequestedError:
                            break;
                        case ErrorType.HelpVerbRequestedError:
                            break;
                        case ErrorType.VersionRequestedError:
                            break;
                        case ErrorType.InvalidAttributeConfigurationError:
                            break;
                        case ErrorType.MissingGroupOptionError:
                            return LocSentenceBuilder.SentenceMissingGroupOptionError;
                            // return $"MISSING GROUP OPTION: {(error as MissingGroupOptionError)!.Group}";
                        case ErrorType.GroupOptionAmbiguityError:
                            break;
                        case ErrorType.MultipleDefaultVerbsError:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    throw new InvalidOperationException();
                };
            }
        }

        public override Func<IEnumerable<MutuallyExclusiveSetError>, string> FormatMutuallyExclusiveSetErrors
        {
            get
            {
                return errors =>
                {
                    var bySet = from e in errors
                        group e by e.SetName
                        into g
                        select new { SetName = g.Key, Errors = g.ToList() };

                    var msgs = bySet.Select(
                        set =>
                        {
                            var names = string.Join(
                                string.Empty,
                                (from e in set.Errors select $"'{e.NameInfo.NameText}', ").ToArray());
                            var namesCount = set.Errors.Count;

                            var incompat = string.Join(
                                string.Empty,
                                (from x in
                                        (from s in bySet
                                            where !s.SetName.Equals(set.SetName)
                                            from e in s.Errors
                                            select e)
                                        .Distinct()
                                    select $"'{x.NameInfo.NameText}', ").ToArray());
                            //TODO: Pluralize by namesCount
                            return
                                string.Format("  " + LocSentenceBuilder.SentenceMutuallyExclusiveSetErrors,
                                    names[..^2], incompat.Substring(0, incompat.Length - 2));
                        }).ToArray();
                    return string.Join(Environment.NewLine, msgs);
                };
            }
        }
    }
}
