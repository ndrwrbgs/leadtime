namespace LeadTime.Library.Git {
    using System;
    using System.Collections.Generic;

    using LibGit2Sharp;

    public static class GitLeadTimeCalculator
    {
        public static IDictionary<DateRange, IHistogram<TimeSpan>> Calculate(
            IEnumerable<(GitCommitHash changeHash, DateTimeOffset shipDate)> individuallyShippedChanges,
            TimeSpan rangeDuration,
            DateTimeOffset snapDateRangesTo,
            IRepository repository)
        {
            var inAndOutDates = GitInAndOutDateFinder.GetInAndOutDates(
                individuallyShippedChanges,
                repository);

            var leadTimes = LeadTimeCalculator.Calculate(
                inAndOutDates,
                rangeDuration,
                snapDateRangesTo);

            return leadTimes;
        }
    }
}