using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Factory;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class RuntimeProtectionTests
    {
        private const string FrameworkDependentDeps =
            @"C:\Program Files\BCUninstaller\BCUninstaller.deps.json;" +
            @"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\8.0.30\Microsoft.WindowsDesktop.App.deps.json;" +
            @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\8.0.30\Microsoft.NETCore.App.deps.json";

        [TestMethod]
        [DataRow(Architecture.X86, "x86")]
        [DataRow(Architecture.X64, "x64")]
        [DataRow(Architecture.Arm64, "arm64")]
        public void LoadedWindowsDesktopRuntimeIsProtected(Architecture architecture, string architectureName)
        {
            Assert.IsTrue(RuntimeProtection.IsLoadedRuntime(
                $"Microsoft Windows Desktop Runtime - 8.0.30 ({architectureName})", FrameworkDependentDeps, architecture));
        }

        [TestMethod]
        public void LoadedNetCoreRuntimeIsProtected()
        {
            Assert.IsTrue(RuntimeProtection.IsLoadedRuntime(
                "Microsoft .NET Runtime - 8.0.30 (x64)", FrameworkDependentDeps, Architecture.X64));
        }

        [TestMethod]
        public void SharedFrameworkUnderRootContainingSharedSegmentIsProtected()
        {
            const string depsFiles =
                @"C:\shared\dotnet\shared\Microsoft.WindowsDesktop.App\8.0.30\Microsoft.WindowsDesktop.App.deps.json";

            Assert.IsTrue(RuntimeProtection.IsLoadedRuntime(
                "Microsoft Windows Desktop Runtime - 8.0.30 (x64)", depsFiles, Architecture.X64));
        }

        [TestMethod]
        public void AppDepsPathThatResemblesSharedFrameworkIsNotProtected()
        {
            const string depsFiles =
                @"C:\apps\shared\Microsoft.WindowsDesktop.App\8.0.30\BCUninstaller.deps.json";

            Assert.IsFalse(RuntimeProtection.IsLoadedRuntime(
                "Microsoft Windows Desktop Runtime - 8.0.30 (x64)", depsFiles, Architecture.X64));
        }

        [TestMethod]
        [DataRow("Microsoft Windows Desktop Runtime - 8.0.29 (x64)", FrameworkDependentDeps, Architecture.X64)]
        [DataRow("Microsoft Windows Desktop Runtime - 8.0.30 (x86)", FrameworkDependentDeps, Architecture.X64)]
        [DataRow("Microsoft ASP.NET Core 8.0.30 - Shared Framework (x64)", FrameworkDependentDeps, Architecture.X64)]
        [DataRow("Microsoft Windows Desktop Runtime - 8.0.30 (x64)", @"C:\Program Files\BCUninstaller\BCUninstaller.deps.json", Architecture.X64)]
        public void UnloadedOrUnrelatedRuntimeIsNotProtected(string displayName, string depsFiles, Architecture architecture)
        {
            Assert.IsFalse(RuntimeProtection.IsLoadedRuntime(displayName, depsFiles, architecture));
        }
    }
}
