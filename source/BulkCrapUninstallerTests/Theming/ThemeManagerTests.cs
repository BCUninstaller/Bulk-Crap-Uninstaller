using BulkCrapUninstaller.Theming;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests.Theming
{
    [TestClass]
    public class ThemeManagerTests
    {
        [DataTestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, false)]
        [DataRow(true, false, true)]
        [DataRow(true, true, false)]
        public void ShouldEnableDarkModeUsesPreferenceAndContrastOverride(
            bool preference, bool highContrast, bool expected)
        {
            Assert.AreEqual(expected, ThemeManager.ShouldEnableDarkMode(preference, highContrast));
        }
    }
}
