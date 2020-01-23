
namespace LeadTime.Library.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Distributions;

    using LeadTime.Core.Core;
    using LeadTime.Library.Core.DataTypes;
    using LeadTime.Library.Core.Util;

    public static class LeadTimeCalculator
    {
        public static IDictionary<DateRange, IUnivariateDistribution> Calculate(
            IEnumerable<(DateTimeOffset inDate, DateTimeOffset outDate)> inAndOutDates,
            TimeSpan rangeDuration,
            DateTimeOffset snapDateRangesTo,
            LeadTimeMode leadTimeMode = LeadTimeMode.ReportCommitAtShipDate)
        {
            var leadTimes = GetLeadTimes(inAndOutDates);

            var dateRangeLeadTimes = GroupOverTimeRanges(leadTimes, rangeDuration, snapDateRangesTo, leadTimeMode);

            var dateRangeDistributions = dateRangeLeadTimes.ToDictionary(
                o => o.dateRange,
                o =>
                {
                    double[] observations = o.leadTimesInRange.Select(t => (double)t.Ticks).ToArray();

                    var analysis = new DistributionAnalysis();
                    analysis.Learn(observations);

                    var matchingOutputType = analysis.GoodnessOfFit
                        // Since type doesn't expose IEnumerable, forces it to (using old C# tricks)
                        .OfType<GoodnessOfFit>()
                        // Ha. haha. hahaha.... GoodnessOfFit enumerates the items NOT in sorted order
                        .OrderBy(gof => gof.Index)
                        .Select(gof => gof.Distribution)
                        // IFittable is the type of Distribution, but we must return IUnivariateDistribution
                        .OfType<IUnivariateDistribution>()
                        // GoodnessOfFit is a descending-in-likelihood collection
                        .First();

                    // TODO: Enforce a minimum likelihood or error out

                    return matchingOutputType;
                });

            return dateRangeDistributions;
        }

        private static IEnumerable<(DateTimeOffset outDate, DateTimeOffset inDate, TimeSpan leadTime)> GetLeadTimes(
            IEnumerable<(DateTimeOffset inDate, DateTimeOffset outDate)> inAndOutDates)
        {
            return inAndOutDates
                .Select(o => (o.outDate, o.inDate, o.outDate - o.inDate));
        }

        private static IEnumerable<(DateRange dateRange, List<TimeSpan> leadTimesInRange)> GroupOverTimeRanges(
            IEnumerable<(DateTimeOffset outDate, DateTimeOffset inDate, TimeSpan leadTime)> leadTimes,
            TimeSpan rangeDuration,
            DateTimeOffset snapDateRangesTo,
            LeadTimeMode leadTimeMode)
        {
            var snappedToDates = ToDateRange.SnapToDateRanges(
                leadTimes,
                o =>
                {
                    switch (leadTimeMode)
                    {
                        case LeadTimeMode.ReportCommitAtShipDate:
                            return o.outDate;
                        case LeadTimeMode.ReportCommitAtCommitDate:
                            return o.inDate;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(leadTimeMode), leadTimeMode, null);
                    }
                },
                snapDateRangesTo,
                rangeDuration);

            return snappedToDates
                .Select(
                    kvp =>
                    {
                        var dateRange = kvp.Key;
                        var leadTimesWithDatesInRange = kvp.Value;

                        var leadTimesInRange = leadTimesWithDatesInRange.Select(o => o.leadTime).ToList();
                        return (dateRange, leadTimesInRange);
                    });
        }
    }
}
