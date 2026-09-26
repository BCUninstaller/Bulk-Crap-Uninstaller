using Klocman.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ProcessToolsTests
    {
        [TestMethod]
        public void SeparateArgsFromCommand_HandlesBareExecutableWithDottedArgument()
        {
            const string command = "winget uninstall --product-code Exiv2.Exiv2_Microsoft.Winget.Source_8wekyb3d8bbwe";

            var result = ProcessTools.SeparateArgsFromCommand(command);

            Assert.AreEqual("winget", result.FileName);
            Assert.AreEqual("uninstall --product-code Exiv2.Exiv2_Microsoft.Winget.Source_8wekyb3d8bbwe", result.Arguments);
        }

        [TestMethod]
        [DataRow("cmd /c echo hello", "cmd", "/c echo hello")]
        [DataRow("msiexec.exe /x {12345678-1234-1234-1234-123456789ABC}", "msiexec.exe", "/x {12345678-1234-1234-1234-123456789ABC}")]
        [DataRow("My Uninstaller.exe /S", "My Uninstaller.exe", "/S")]
        [DataRow(@"C:\Program Files\Example\uninstall.exe /S", @"C:\Program Files\Example\uninstall.exe", "/S")]
        [DataRow("\"C:\\Program Files\\Example\\uninstall.exe\" /S", @"C:\Program Files\Example\uninstall.exe", "/S")]
        public void SeparateArgsFromCommand_PreservesExistingCommandShapes(string command, string expectedFileName, string expectedArguments)
        {
            var result = ProcessTools.SeparateArgsFromCommand(command);

            Assert.AreEqual(expectedFileName, result.FileName);
            Assert.AreEqual(expectedArguments, result.Arguments);
        }
    }
}
