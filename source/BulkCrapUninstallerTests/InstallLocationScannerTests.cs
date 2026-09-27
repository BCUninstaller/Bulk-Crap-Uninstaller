using System.IO;
using System.Linq;
using Klocman.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Finders.Drive;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class InstallLocationScannerTests
    {
        [TestMethod]
        [DataRow("", false)]
        [DataRow("", true)]
        [DataRow(@"System32\WindowsPowerShell\v1.0", false)]
        [DataRow(@"System32\WindowsPowerShell\v1.0", true)]
        public void FindJunk_WindowsDirectory_IsNotOfferedForRemoval(string subdirectory, bool uppercase)
        {
            var path = Path.Combine(PathTools.GetWindowsDirectory().FullName, subdirectory);
            path = uppercase ? path.ToUpperInvariant() : path.ToLowerInvariant();
            Assert.IsTrue(Directory.Exists(path));

            var entry = new ApplicationUninstallerEntry { InstallLocation = path };
            var scanner = new InstallLocationScanner();
            scanner.Setup(new[] { entry });

            var results = scanner.FindJunk(entry).ToList();

            Assert.IsEmpty(results);
        }

        [TestMethod]
        public void FindJunk_ApplicationDirectory_IsOfferedForRemoval()
        {
            var directory = Directory.CreateTempSubdirectory();
            try
            {
                var entry = new ApplicationUninstallerEntry { InstallLocation = directory.FullName };
                var scanner = new InstallLocationScanner();
                scanner.Setup(new[] { entry });

                var result = scanner.FindJunk(entry).Single();

                Assert.AreEqual(directory.FullName, result.GetDisplayName());
                Assert.AreEqual(ConfidenceLevel.Good, result.Confidence.GetConfidence());
            }
            finally
            {
                directory.Delete();
            }
        }
    }
}
