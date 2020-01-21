namespace LeadTime.Library.Git {
    using System;
    using System.Collections.Generic;

    using LeadTime.Library.Git.DataTypes;

    using LibGit2Sharp;

    internal static class GitInDateFinder
    {
        /// <remarks>
        /// While we could easily grab the date while grabbing the interpolated commits,
        /// it would combine two concerns -- e.g. InterpolatedOutDates needs to interpolate also
        /// but does NOT need to know when the commits went in.
        /// 
        /// As such, we instead encourage someone with that performance optimization need to
        /// make an IRepository that keeps a list of recently accessed Commits by hash in order
        /// to optimize Lookup{Commit} calls.
        /// </remarks>
        public static IEnumerable<(GitCommitHash changeHash, DateTimeOffset inDate)> GetInDates(
            IEnumerable<GitCommitHash> changes,
            IRepository repository)
        {
            foreach (var commit in changes)
            {
                var c = repository.Lookup<Commit>((string)commit);

                yield return (changeHash: (GitCommitHash) c.Sha, inDate: c.Committer.When);
            }
        }
    }
}