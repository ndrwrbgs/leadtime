namespace LeadTime.Library.Git {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Should handle 'rollbacks' where we ship 0,1,2 then go BACK to 1, and ship 2,3 later
    /// (2 should be attributed to the later ship date)
    /// </summary>
    internal static class GitOutDateFinder
    {
        public static IEnumerable<(GitCommitHash changeHash, DateTimeOffset shipDate)> GetOutDates(
            IEnumerable<GitCommitHash> allChanges,
            IEnumerable<(GitCommitHash changeHash, DateTimeOffset shipDate)> individuallyShippedChanges)
        {
            // This can be done more efficiently by expected both inputs to be sorted, and enumerating both
            // concurrently for a 'zip' operation, but this implementation reads more easily for a human.
            var individuallyShippedChangesDictionary = individuallyShippedChanges
                .ToDictionary(o => o.changeHash, o => o.shipDate);

            List<GitCommitHash> changesShippedInNextShipment = new List<GitCommitHash>();
            foreach (var change in allChanges)
            {
                changesShippedInNextShipment.Add(change);

                if (individuallyShippedChangesDictionary.TryGetValue(change, out DateTimeOffset shipDate))
                {
                    // Just shipped all the changes
                    foreach (var item in changesShippedInNextShipment)
                    {
                        yield return (changeHash: item, shipDate: shipDate);
                    }

                    changesShippedInNextShipment = new List<GitCommitHash>();
                }
            }

            if (changesShippedInNextShipment.Any())
            {
                throw new ArgumentException(
                    "We expected all changes in 'allChanges' to be represented by a shipped change (it likely should have been calculated from shippedChanges) but we found some that were not.");
            }
        }
    }
}