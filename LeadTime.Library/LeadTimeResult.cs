namespace LeadTime.Library {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Not sure if we want to support reading from this structure
    /// the lead time BY ITEM since it's supposed to be composite,
    /// but keeping the generic for now to decide later
    /// </summary>
    public sealed class LeadTimeResult<TItemType> {
        public IDictionary<DateTimeOffset, IList<TimeSpan>> LeadTimes { get;}

        public LeadTimeResult(
            IDictionary<TItemType, (DateTimeOffset measureFrom, TimeSpan leadTime)> leadTimes)
        {
            // TODO: Modes for e.g. how long of frames to combine/day of week to use
            this.LeadTimes = leadTimes
                .Select(kvp => kvp.Value)
                // TODO: This is bad date math, it 'works' but will start 'weeks' on arbitrary days/times
                .GroupBy(o => (long) (o.measureFrom.Ticks / TimeSpan.FromDays(7).Ticks))
                .ToDictionary(
                    group => new DateTimeOffset(group.Key * TimeSpan.FromDays(7).Ticks, TimeSpan.Zero),
                    group => (IList<TimeSpan>) group.Select(g => g.leadTime).ToList());
        }
    }
}