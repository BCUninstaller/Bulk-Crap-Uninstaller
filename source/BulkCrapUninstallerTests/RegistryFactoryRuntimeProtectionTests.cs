using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Factory;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class RegistryFactoryRuntimeProtectionTests
    {
        [TestMethod]
        [DataRow("Microsoft Corporation", true)]
        [DataRow("Contoso", false)]
        public void RuntimeProtectionRequiresMicrosoftPublisher(string publisher, bool expectedProtected)
        {
            var versionDirectory = Directory.GetParent(typeof(System.Windows.Forms.Form).Assembly.Location);
            Assert.IsNotNull(versionDirectory);

            var architecture = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X86 => "x86",
                Architecture.X64 => "x64",
                Architecture.Arm64 => "arm64",
                _ => throw new AssertFailedException("Unsupported test architecture")
            };
            var displayName = $"Microsoft Windows Desktop Runtime - {versionDirectory.Name} ({architecture})";
            var depsFiles = AppContext.GetData("APP_CONTEXT_DEPS_FILES") as string;
            Assert.IsTrue(depsFiles?.IndexOf("Microsoft.WindowsDesktop.App", StringComparison.OrdinalIgnoreCase) >= 0,
                "Test runner must use the shared Windows Desktop runtime.");

            var marker = Guid.NewGuid().ToString("N");
            var keyPath = $@"Software\Microsoft\Windows\CurrentVersion\Uninstall\BCU_RuntimeProtectionTest_{marker}";

            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    Assert.IsNotNull(key);
                    key.SetValue("DisplayName", displayName);
                    key.SetValue("Publisher", publisher);
                    key.SetValue("Comment", marker);
                    key.SetValue("UninstallString", "contoso-uninstall.exe");
                }

                var entry = new RegistryFactory(Array.Empty<Guid>())
                    .GetUninstallerEntries(_ => { })
                    .Single(x => x.Comment == marker);

                Assert.AreEqual(expectedProtected, entry.IsProtected);
            }
            finally
            {
                Registry.CurrentUser.DeleteSubKeyTree(keyPath, false);
            }
        }
    }
}
