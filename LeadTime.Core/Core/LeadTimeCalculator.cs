
namespace LeadTime.Library.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using LeadTime.Library.Core.DataTypes;
    using LeadTime.Library.Core.Util;

    /// <summary>
    /// 
    /// </summary>
    public static class LeadTimeCalculator
    {
        public static IDictionary<DateRange, IUnivariateDistribution> Calculate(
            IEnumerable<(DateTimeOffset inDate, DateTimeOffset outDate)> inAndOutDates,
            TimeSpan rangeDuration,
            DateTimeOffset snapDateRangesTo)
        {
            var leadTimes = GetLeadTimes(inAndOutDates);

            var dateRangeLeadTimes = GroupOverTimeRanges(leadTimes, rangeDuration, snapDateRangesTo);

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

        private static IEnumerable<(DateTimeOffset outDate, TimeSpan leadTime)> GetLeadTimes(
            IEnumerable<(DateTimeOffset inDate, DateTimeOffset outDate)> inAndOutDates)
        {
            return inAndOutDates
                .Select(o => (o.outDate, o.outDate - o.inDate));
        }

        private static IEnumerable<(DateRange dateRange, List<TimeSpan> leadTimesInRange)> GroupOverTimeRanges(
            IEnumerable<(DateTimeOffset outDate, TimeSpan leadTime)> leadTimes,
            TimeSpan rangeDuration,
            DateTimeOffset snapDateRangesTo)
        {
            var snappedToOutDates = ToDateRange.SnapToDateRanges(
                leadTimes,
                o => o.outDate,
                snapDateRangesTo,
                rangeDuration);

            return snappedToOutDates
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
