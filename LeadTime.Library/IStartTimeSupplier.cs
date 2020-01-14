namespace LeadTime.Library {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Tells when an item was 'started'. E.g. a feature was started, a git commit was made in master, the dishes were dirtied
    /// </summary>
    public interface IStartTimeSupplier<TItemType>
    {
        IReadOnlyDictionary<TItemType, DateTimeOffset> GetAllItemStartTimes();
    }
}