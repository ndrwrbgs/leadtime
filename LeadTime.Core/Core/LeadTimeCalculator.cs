
namespace LeadTime.Library.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeadTime.Library.Core.DataTypes;
    using LeadTime.Library.Core.Util;

    /// <summary>
    /// 
    /// </summary>
    public static class LeadTimeCalculator
    {
        public static IDictionary<DateRange, IHistogram<TimeSpan>> Calculate(
            IEnumerable<(DateTimeOffset inDate, DateTimeOffset outDate)> inAndOutDates,
            TimeSpan rangeDuration,
            DateTimeOffset snapDateRangesTo)
        {
            var leadTimes = GetLeadTimes(inAndOutDates);

            var dateRangeLeadTimes = GroupOverTimeRanges(leadTimes, rangeDuration, snapDateRangesTo);

            var dateRangeHistograms = dateRangeLeadTimes.ToDictionary(o => o.dateRange, o => (IHistogram<TimeSpan>)new TimeSpanHistogram(o.leadTimesInRange));

            return dateRangeHistograms;
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
