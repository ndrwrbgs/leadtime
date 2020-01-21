
namespace LeadTime.Library.Git
{
    using LibGit2Sharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class GitInAndOutDateFinder
    {
        public static IEnumerable<(DateTimeOffset inDate, DateTimeOffset outDate)> GetInAndOutDates(
            IEnumerable<(GitCommitHash changeHash, DateTimeOffset shipDate)> individuallyShippedChanges,
            IRepository repository)
        {
            // Single enumeration
            individuallyShippedChanges = individuallyShippedChanges.ToList();


            var interpolatedChanges = GitChangesBetweenLister
                .EnumerateWithAnyChangesBetween(
                    individuallyShippedChanges.Select(o => o.changeHash),
                    repository)
                .ToList();

            IEnumerable<(GitCommitHash changeHash, DateTimeOffset inDate)> inDates = GitInDateFinder.GetInDates(
                interpolatedChanges,
                repository);
            IEnumerable<(GitCommitHash changeHash, DateTimeOffset shipDate)> outDates = GitOutDateFinder.GetOutDates(
                interpolatedChanges,
                individuallyShippedChanges);

            var inDateDict = inDates.ToDictionary(o => o.changeHash, o => o.inDate);

            foreach ((GitCommitHash changeHash, DateTimeOffset shipDate) in outDates)
            {
                var inDate = inDateDict[changeHash];

                yield return (inDate: inDate, outDate: shipDate);
            }
        }
    }
}
