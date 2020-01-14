namespace LeadTime.Library {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Tells when an item was 'shipped'. E.g. a feature was completed, a git commit was deployed to production, the dishes were washed
    /// </summary>
    public interface IShipTimeSupplier<TItemType>
    {
        IReadOnlyDictionary<TItemType, DateTimeOffset> GetAllItemShipTimes();
    }
}