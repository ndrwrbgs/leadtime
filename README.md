[![nuget](https://img.shields.io/nuget/v/LeadTime.Core.svg?label=LeadTime.Core&logo=nuget)](https://www.nuget.org/packages/LeadTime.Core)
[![nuget](https://img.shields.io/nuget/v/LeadTime.Git.svg?label=LeadTime.Git&logo=nuget)](https://www.nuget.org/packages/LeadTime.Git)

# LeadTime

![Icon](/package_icon.png)

Evaluate the [lead time](https://en.wikipedia.org/wiki/Lead_time) of changes - targeted specifically at git commit lead times from commit to deployment time.

# Usage
## You supply
`IEnumerable<(GitCommitHash changeHash, DateTimeOffset shipDate)>`
In English: a collection of changes and when each shipped. You do not need to supply in-between changes or anything like that, just a set of commit hashes + when that hash was deployed.

Example:
You deploy tag-1 on Jan 1, tag-2 on Jan 5, and tag-3 on Jan 10.
You supply:
* {tag-1 commit hash}, Jan 1
* {tag-2 commit hash}, Jan 3
* {tag-3 commit hash}, Jan 10

## We do
The library handles finding all the changes in between those, collecting them into _groups_ of changes deployed on each date, figuring out when each change was committed and how long it had been between commit and deployment, and creating histograms for each day/week/month.

## Code
```C#
using (var repository = new Repository(@"c:\MyRepositoryPath")) // From LibGit2Sharp
{
    var all = GitLeadTimeCalculator.Calculate(
        new[]
        {
            // From your deployment history
            ((GitCommitHash) "00000000000000000000000000000000deadbeef", DateTimeOffset.Parse("01/01/2010")),
            ((GitCommitHash) "0000000000000000000000000000000000badbed", DateTimeOffset.Parse("01/10/2010")),
        },
        // How long to group histograms by (depends on your view requirements. Hint - if you want a month use 30.475 days)
        TimeSpan.FromDays(1),
        // In this example, just telling it to snap to days (by using .Date to drop the time part) DateTimeOffset.Parse("01/01/2001") would be a good value as well and if I make any wrapping libraries, will likely be the default
        DateTimeOffset.Now.Date,
        repository);
}
```

# Implementation details
Especially if you're trying to modify/reuse the code for non-git purposes or otherwise extend it, it's useful to have a diagram...
![image](/LeadTime.Library/Diagram/LeadTime%20Workflow.png)

## Icon

[duration](https://thenounproject.com/term/duration/2329742) designed by [Goh H Chin](https://thenounproject.com/gohhchin) from [The Noun Project](https://thenounproject.com).
