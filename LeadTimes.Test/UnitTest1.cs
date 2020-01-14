using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeadTimes.Test
{
    using System.Collections.Generic;
    using System.Linq;

    using LeadTime.Library;

    using NSubstitute;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IStartTimeSupplier<string> startTimeSupplier = Substitute.For<IStartTimeSupplier<string>>();
            IShipTimeSupplier<string> shipTimeSupplier = Substitute.For<IShipTimeSupplier<string>>();

            DateTimeOffset shipTime = DateTimeOffset.Parse("1/2/2019");
            TimeSpan expectedLeadTime = TimeSpan.FromDays(1);

            startTimeSupplier
                .GetAllItemStartTimes()
                .Returns(
                    (IReadOnlyDictionary<string, DateTimeOffset>)(new Dictionary<string, DateTimeOffset>
                    {
                        ["a"] = shipTime - expectedLeadTime
                    }));
            shipTimeSupplier
                .GetAllItemShipTimes()
                .Returns(
                    (IReadOnlyDictionary<string, DateTimeOffset>)(new Dictionary<string, DateTimeOffset>
                    {
                        ["a"] = shipTime
                    }));

            var results = LeadTimeCalculator.Calculate<string>(
                startTimeSupplier,
                shipTimeSupplier);

            Assert.AreEqual(1, results.LeadTimes.Count, "one input");
            var one = results.LeadTimes.Single();
            Assert.AreEqual(1, one.Value.Count, "one input");

            // Due to the 'bad math' we cannot assert the day to be 1/2, but only to include it
            // The ship time falls within the 'week' of the result
            Assert.IsTrue(one.Key <= shipTime);
            Assert.IsTrue(one.Key + TimeSpan.FromDays(7) >= shipTime);

            // Correct lead time difference
            Assert.AreEqual(expectedLeadTime, one.Value.Single());
        }
    }
}
