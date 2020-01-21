namespace LeadTime.Library.Core.DataTypes {
    using System;
    using System.Collections.Generic;

    public interface IHistogram<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        T GetPercentile(Percentile percentile);
    }
}