#define BUGCHECKS

namespace LeadTime.Library.Git
{
    using LibGit2Sharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using LeadTime.Library.Git.DataTypes;

    /// <summary>
    /// WARNING this was designed for repos that enforce squash-commits (as opposed to merge commits)
    /// </summary>
    internal static class GitChangesBetweenLister
    {
        /// <summary>
        /// Enumerates <paramref name="originalHashes"/> including any commits that are between sequential
        /// commits in the source enumerable.
        /// Like:
        /// originalHashes[0]
        /// git log originalHashes[0]..originalHashes[1]
        /// originalHashes[1]
        /// git log originalHashes[1]..originalHashes[2]
        /// </summary>
        public static IEnumerable<GitCommitHash> EnumerateWithAnyChangesBetween(
            IEnumerable<GitCommitHash> originalHashes,
            IRepository repository)
        {
            GitCommitHash? prev = null;
            foreach (var commit in originalHashes)
            {
                if (prev != null)
                {
                    var fromCommit = prev;
                    var toCommit = commit;
                    var between = repository.Commits.QueryBy(
                            new CommitFilter
                            {
                                ExcludeReachableFrom = (string) fromCommit.Value,
                                IncludeReachableFrom = (string) toCommit,
                            })
                        // It'll go FROM toCommit TO fromCommit by default
                        .Reverse();

#if BUGCHECKS
                    // 'any' for sanity checking the input expectations (topographically INCREASING commit list)
                    bool any = false;
#endif
                    foreach (var item in between)
                    {
#if BUGCHECKS
                        any = true;
#endif
                        yield return (GitCommitHash) item.Sha;
                    }

#if BUGCHECKS
                    if (!any)
                    {
                        throw new InvalidOperationException(
                            "BUGCHECK - we expected a topographically increasing commit list as input, e.g. item[0] is before item[1], but this seems to have been violated");
                    }
#endif
                }
                else
                {
                    yield return commit;
                }

                prev = commit;
            }
        }
    }
}
