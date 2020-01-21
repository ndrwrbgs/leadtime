namespace LeadTime.Library.Core.DataTypes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// TODO: Possible to make more efficient, right now is lazy
    /// </remarks>
    public sealed class TimeSpanHistogram : IHistogram<TimeSpan>
    {
        private readonly IList<TimeSpan> values;

        public TimeSpanHistogram(IList<TimeSpan> values)
        {
            this.values = values.OrderBy(ts => ts).ToList();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<TimeSpan> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        public TimeSpan GetPercentile(Percentile percentile)
        {
            // TODO: This is just working around BUUUUGS
            if (percentile == 1.00)
            {
                return this.values.Last();
            }

            // TODO: Look up what is considered 'standard' for interpolating percentiles
            var lower = (int)Math.Floor(percentile * this.values.Count);
            var upper = (int)Math.Ceiling(percentile * this.values.Count);

            return this.values[lower];
        }

        public override string ToString()
        {
            return $"[{this.GetPercentile(0)} - {this.GetPercentile(0.50)} - {this.GetPercentile(1.00)}]";
        }
    }
}
