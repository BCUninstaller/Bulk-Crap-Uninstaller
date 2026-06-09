using System;
using System.IO;
using BulkCrapUninstaller.Functions.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests.Functions
{
    [TestClass]
    public class StartupArgumentToolsTests
    {
        [TestMethod]
        public void GetStartupUninstallListPath_ReturnsNullWhenThereAreNoArguments()
        {
            var result = StartupArgumentTools.GetStartupUninstallListPath(Array.Empty<string>());

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetStartupUninstallListPath_ReturnsNullForNonBculArgument()
        {
            var result = StartupArgumentTools.GetStartupUninstallListPath(new[] { "BCUninstaller.exe", "/setup" });

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetStartupUninstallListPath_ReturnsNullForMissingBculFile()
        {
            var result = StartupArgumentTools.GetStartupUninstallListPath(new[] { "BCUninstaller.exe", @"C:\missing\Default.bcul" });

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetStartupUninstallListPath_ReturnsExistingBculPath()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bcul");

            try
            {
                File.WriteAllText(tempPath, "<UninstallList />");

                var result = StartupArgumentTools.GetStartupUninstallListPath(new[] { "BCUninstaller.exe", tempPath });

                Assert.AreEqual(tempPath, result);
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }
    }
}
