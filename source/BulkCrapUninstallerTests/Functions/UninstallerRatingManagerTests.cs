using System;
using System.IO;
using BulkCrapUninstaller;
using BulkCrapUninstaller.Functions.Ratings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests.Functions
{
    [TestClass]
    public class UninstallerRatingManagerTests
    {
        private static readonly string[] TestEntryNames = {"Test_1", "Test_2", "Test_3", "Test_4"};
        private UninstallerRatingManager _manager;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _manager = new UninstallerRatingManager(1);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _manager?.ClearRatings();
        }

        [TestMethod]
        public void RefreshStatsTest()
        {
            Assert.Inconclusive("Expensive, no need to always test");

            _manager.FetchRatings();
            if (_manager.RemoteRatingCount == 0)
                Assert.Fail();
        }

        [TestMethod]
        public void GetRatingTest()
        {
            _manager.FetchRatings();
            var rating = _manager.GetRating(TestEntryNames[0]);

            if (rating.IsEmpty)
                Assert.Fail();
        }

        [TestMethod]
        public void SetMyRatingTest()
        {
            _manager.FetchRatings();

            try
            {
                _manager.SetMyRating(null, UninstallerRating.Bad);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
            try
            {
                _manager.SetMyRating(TestEntryNames[0], UninstallerRating.Unknown);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            _manager.SetMyRating(TestEntryNames[0], UninstallerRating.Good);
            Assert.AreEqual((int) UninstallerRating.Good, _manager.GetRating(TestEntryNames[0]).MyRating);

            var rating = _manager.GetRating("Test_SetMyRatingTest");
            var newRating = rating.MyRating == (int) UninstallerRating.Bad
                ? UninstallerRating.Good
                : UninstallerRating.Bad;
            _manager.SetMyRating("Test_SetMyRatingTest", newRating);
            Assert.AreEqual((int) newRating, _manager.GetRating("Test_SetMyRatingTest").MyRating);
        }

        [TestMethod]
        public void SerializeDeserializeCasheTest()
        {
            _manager.FetchRatings();
            var count = _manager.RemoteRatingCount + _manager.UserRatingCount;
            if (count == 0)
                Assert.Fail("No items received");

            var filename = Path.Combine(Program.AssemblyLocation.FullName, "TestTempDir");

            var dir = new DirectoryInfo(filename);
            dir.Create();
            _manager.SerializeCache(dir);

            TestCleanup();
            TestInitialize();

            _manager.DeserializeCache(dir);
            Assert.AreEqual(count, _manager.RemoteRatingCount + _manager.UserRatingCount);
        }
    }
}