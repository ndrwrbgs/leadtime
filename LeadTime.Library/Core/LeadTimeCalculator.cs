
namespace LeadTime.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
            var snappedToOutDates = ToDateRange.SnapToDateRanges(
                inAndOutDates,
                o => o.outDate,
                snapDateRangesTo,
                rangeDuration);

            var dateRangeHistograms = snappedToOutDates
                .Select(
                    kvp =>
                    {
                        var dateRange = kvp.Key;
                        var inAndOutDatesInRange = kvp.Value;

                        var durations = inAndOutDatesInRange.Select(o => o.outDate - o.inDate).ToList();
                        var histogram = new TimeSpanHistogram(durations);
                        return new
                        {
                            dateRange,
                            histogram
                        };
                    })
                .ToDictionary(o => o.dateRange, o => (IHistogram<TimeSpan>)o.histogram);

            return dateRangeHistograms;
        }
    }
}
