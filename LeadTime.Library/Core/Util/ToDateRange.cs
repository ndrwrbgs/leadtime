
namespace LeadTime.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Snap date ranges to make lists
    /// </summary>
    internal static class ToDateRange
    {
        public static IDictionary<DateRange, IList<TItem>> SnapToDateRanges<TItem>(
            IEnumerable<TItem> items,
            Func<TItem, DateTimeOffset> getDateFromItem,
            DateTimeOffset snapToBase,
            TimeSpan rangeDuration)
        {
            var itemsWithDates = items
                .Select(
                    item =>
                    {
                        return new
                        {
                            item,
                            date = getDateFromItem(item)
                        };
                    });
            var itemsWithRanges = itemsWithDates
                .Select(
                    anon =>
                    {
                        var range = Snap(anon.date, snapToBase, rangeDuration);
                        return new
                        {
                            anon.item,
                            range
                        };
                    });
            var grouped = itemsWithRanges
                .GroupBy(anon => anon.range, anon => anon.item);

            return grouped
                .ToDictionary(
                    group => group.Key,
                    group => (IList<TItem>)group.ToList());
        }

        private static DateRange Snap(
            DateTimeOffset input,
            DateTimeOffset to,
            TimeSpan rangeDuration)
        {
            var diff = input - to;
            var ranges = diff.Ticks / (double)rangeDuration.Ticks;
            var fullRanges = (long)Math.Floor(ranges);
            var snapMin = to + TimeSpanUtil.Multiply(rangeDuration, fullRanges);
            var snapMax = snapMin + rangeDuration;

            return new DateRange(snapMin, snapMax);
        }
    }
}
