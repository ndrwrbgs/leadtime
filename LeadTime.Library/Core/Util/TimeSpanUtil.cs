namespace LeadTime.Library.Core.Util {
    using System;

    internal static class TimeSpanUtil
    {
        public static TimeSpan Multiply(TimeSpan duration, long multiplier)
        {
            return new TimeSpan(
                duration.Ticks
                * multiplier);
        }
    }
}