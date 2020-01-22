using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeadTimes.Test
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;

    using Accord;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Univariate;
    using LeadTime.Library;
    using LeadTime.Library.Core.Util;
    using LeadTime.Library.Git;
    using LeadTime.Library.Git.DataTypes;

    using LibGit2Sharp;

    using NSubstitute;

    [TestClass]
    public class UnitTest1
    {
        [TestClass]
        public class DateSnapTests
        {
            [TestMethod]
            public void Nominal()
            {
                var snapped = ToDateRange.SnapToDateRanges(
                    new[] {DateTimeOffset.Parse("1/18/2020")},
                    o => o,
                    DateTimeOffset.Parse("1/16/2020"),
                    TimeSpan.FromDays(7))
                    .Keys
                    .Single();

                Assert.AreEqual(DateTimeOffset.Parse("1/16/2020"), snapped.StartInclusive);
                Assert.AreEqual(DateTimeOffset.Parse("1/23/2020"), snapped.EndExclusive);
            }

            [TestMethod]
            public void OverlapsStart()
            {
                var snapped = ToDateRange.SnapToDateRanges(
                        new[] { DateTimeOffset.Parse("1/18/2020") },
                        o => o,
                        DateTimeOffset.Parse("1/18/2020"),
                        TimeSpan.FromDays(7))
                    .Keys
                    .Single();

                Assert.AreEqual(DateTimeOffset.Parse("1/18/2020"), snapped.StartInclusive);
                Assert.AreEqual(DateTimeOffset.Parse("1/25/2020"), snapped.EndExclusive);
            }

            [TestMethod]
            public void OverlapsEnd()
            {
                var snapped = ToDateRange.SnapToDateRanges(
                        new[] { DateTimeOffset.Parse("1/18/2020") },
                        o => o,
                        DateTimeOffset.Parse("1/11/2020"),
                        TimeSpan.FromDays(7))
                    .Keys
                    .Single();

                Assert.AreEqual(DateTimeOffset.Parse("1/18/2020"), snapped.StartInclusive);
                Assert.AreEqual(DateTimeOffset.Parse("1/25/2020"), snapped.EndExclusive);
            }

            [TestMethod]
            public void ManualTestOfAMonthDuration()
            {
                var snapped = ToDateRange.SnapToDateRanges(
                        new[] { DateTimeOffset.Parse("10/18/2020") },
                        o => o,
                        DateTimeOffset.Parse("1/1/1990"),
                        // Accounts for leap years
                        TimeSpan.FromDays(30.4375))
                    .Keys
                    .Single();
            }

            [TestMethod]
            public void MuchFarBefore()
            {
                var snapped = ToDateRange.SnapToDateRanges(
                        new[] { DateTimeOffset.Parse("1/18/2020") },
                        o => o,
                        // A Monday
                        DateTimeOffset.Parse("2/9/1920"),
                        TimeSpan.FromDays(7))
                    .Keys
                    .Single();

                Assert.AreEqual(DateTimeOffset.Parse("1/13/2020"), snapped.StartInclusive);
                Assert.AreEqual(DateTimeOffset.Parse("1/20/2020"), snapped.EndExclusive);
            }
        }

        [TestClass]
        public class GitTests
        {
            [TestMethod]
            //[Ignore("Dev box only")]
            public void ManualTesting()
            {
                using (var repository = new Repository(@"c:\RAMDisk\Health"))
                {
                    var all = GitLeadTimeCalculator.Calculate(
                        new[]
                        {
                            ((GitCommitHash) "ff59565f369f137b5458200ddb04a73ee96e1623", DateTimeOffset.Now.AddDays(-1)),
                            ((GitCommitHash) "0fd7d90c9533194382b6d3c98aaf4204c82de12a", DateTimeOffset.Now),
                        },
                        TimeSpan.FromDays(1),
                        DateTimeOffset.Now.Date,
                        repository);

                    var orderedDateRanges = all.OrderBy(kvp => kvp.Key);
                    foreach (var dateRangeAndHistogram in orderedDateRanges)
                    {
                        var dateRange = dateRangeAndHistogram.Key;
                        var histogram = dateRangeAndHistogram.Value;
                        
                        // // [1/21/2020 12:00:00 AM +00:00, 1/22/2020 12:00:00 AM +00:00): [11.16:13:19.7169758 - 11.16:13:19.7169758 - 11.16:13:19.7169758]
                        // Try to find the percentile
                        var estimatedLocationOfPercentile = histogram.GetRange(0.43).Min;
                            //FindPercentile(histogram, 0.43);
                        var realPercentile = histogram.DistributionFunction(estimatedLocationOfPercentile);

                        Console.WriteLine($"{dateRange}: {estimatedLocationOfPercentile} - {realPercentile}");
                    }
                }
            }

            private static double FindPercentile(IDistribution source, double percentile)
            {
                double currentMin = double.MinValue;
                double currentMax = double.MaxValue;

                double desiredAccuracy = 0.01;
                int maxIterations = 2000;

                double currentGuess = double.NaN;

                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    currentGuess = currentMin / 2.0 + currentMax / 2.0;

                    var currentPercentile = source.DistributionFunction(currentGuess);
                    var error = Math.Abs(currentPercentile - percentile);
                    if (error < desiredAccuracy)
                    {
                        return currentGuess;
                    }

                    if (currentPercentile < percentile)
                    {
                        currentMin = currentGuess;
                    }
                    else // >
                    {
                        currentMax = currentGuess;
                    }
                }

                return currentGuess;
            }
        }
    }
}
