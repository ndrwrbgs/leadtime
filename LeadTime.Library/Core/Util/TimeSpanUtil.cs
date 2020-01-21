namespace LeadTime.Library {
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