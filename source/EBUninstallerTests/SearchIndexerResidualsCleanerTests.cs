/*
    EBUninstaller Pro - Unit Test Suite
    Search Indexer Residuals & Catalog Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class SearchIndexerResidualsCleanerTests
    {
        [TestMethod]
        public void TestQuerySearchIndexStatsSafety()
        {
            var stats = SearchIndexerResidualsCleanerEngine.QuerySearchIndexStats();
            Assert.IsNotNull(stats);
        }

        [TestMethod]
        public void TestSearchIndexStatsModel()
        {
            var stats = new SearchIndexStats
            {
                CatalogPath = @"C:\ProgramData\Microsoft\Search\Data\Applications\Windows\Windows.edb",
                CatalogSizeBytes = 1073741824,
                IsServiceRunning = true,
                IndexedLocationCount = 5
            };

            Assert.IsTrue(stats.IsServiceRunning);
            Assert.AreEqual(1073741824, stats.CatalogSizeBytes);
            Assert.AreEqual(5, stats.IndexedLocationCount);
        }
    }
}
