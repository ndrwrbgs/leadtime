using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTime.Library
{
    public static class LeadTimeCalculator
    {
        // TODO: Lead time can be reported the date the changes are deployed, or when they were created
        // both have different 'null'/unreported semantics, but could be useful in different cases
        public static LeadTimeResult<TItemType> Calculate<TItemType>(
            IStartTimeSupplier<TItemType> startTimeSupplier,
            IShipTimeSupplier<TItemType> shipTimeSupplier,
            LeadTimeMode mode = LeadTimeMode.MeasureFromShipTime)
        {
            var shipTimes = shipTimeSupplier.GetAllItemShipTimes();
            var startTimes = startTimeSupplier.GetAllItemStartTimes();

            IDictionary<TItemType, (DateTimeOffset measureFrom, TimeSpan leadTime)> leadTimes = new Dictionary<TItemType, (DateTimeOffset measureFrom, TimeSpan leadTime)>();
            foreach (var shipTimeItem in shipTimes)
            {
                var key = shipTimeItem.Key;
                var shipTime = shipTimeItem.Value;

                if (!startTimes.TryGetValue(key, out DateTimeOffset startTime))
                {
                    throw new InvalidOperationException("No start time data found for " + key);
                }

                var leadTime = shipTime - startTime;
                var measureFrom = shipTime;

                leadTimes[key] = (measureFrom, leadTime);
            }

            return new LeadTimeResult<TItemType>(leadTimes);
        }

        public enum LeadTimeMode
        {
            MeasureFromShipTime
        }
    }
}
