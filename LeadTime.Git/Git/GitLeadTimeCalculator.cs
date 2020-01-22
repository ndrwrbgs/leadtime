namespace LeadTime.Library.Git {
    using System;
    using System.Collections.Generic;

    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Univariate;

    using LeadTime.Library.Core;
    using LeadTime.Library.Core.DataTypes;
    using LeadTime.Library.Git.DataTypes;

    using LibGit2Sharp;

    public static class GitLeadTimeCalculator
    {
        public static IDictionary<DateRange, IUnivariateDistribution> Calculate(
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