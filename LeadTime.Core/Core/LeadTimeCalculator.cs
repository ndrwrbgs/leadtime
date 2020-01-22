
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
                    //*/
                    var normal = new NormalDistribution();

                    normal.Fit(
                        observations,
                        new NormalOptions
                        {
                            Regularization = 1e-6
                        });
                    /*/
                    var normal = new EmpiricalDistribution(
                        observations);
                    //*/

                    var analysis = new DistributionAnalysis();
                    analysis.Learn(observations);
                    var mostLikely = analysis.GoodnessOfFit[0];
                    var result = mostLikely.Distribution;

                    // TODO: Unsafe cast, IFittable isn't necessarily IUnivariateDistribution
                    return (IUnivariateDistribution)result;
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
