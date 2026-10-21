using System;
using System.Reflection;
using System.Windows.Forms;
using BulkCrapUninstaller.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class MainWindowAccessibilityTests
    {
        [TestMethod]
        public void MainToolbar_ExposesItsAccessibleNameAndRole()
        {
            var configureToolbar = GetConfigureToolbarMethod();

            using var toolbar = new ToolStrip();
            configureToolbar.Invoke(null, new object[] { toolbar, "Show toolbar" });

            Assert.AreEqual("Show toolbar", toolbar.AccessibleName);
            Assert.AreEqual(AccessibleRole.ToolBar, toolbar.AccessibleRole);
        }

        [TestMethod]
        public void MainToolbar_SelectsFirstEnabledItemOnEntry()
        {
            var configureToolbar = GetConfigureToolbarMethod();

            using var toolbar = new ToolStrip();
            var disabledButton = new ToolStripButton { Enabled = false };
            var firstEnabledButton = new ToolStripButton();
            toolbar.Items.AddRange(new ToolStripItem[]
            {
                disabledButton,
                new ToolStripSeparator(),
                firstEnabledButton
            });
            configureToolbar.Invoke(null, new object[] { toolbar, "Show toolbar" });

            var onEnter = typeof(ToolStrip).GetMethod("OnEnter", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(onEnter, "The ToolStrip focus-entry event could not be invoked.");
            onEnter.Invoke(toolbar, new object[] { EventArgs.Empty });

            Assert.IsTrue(firstEnabledButton.Selected);
            Assert.IsFalse(disabledButton.Selected);
        }

        private static MethodInfo GetConfigureToolbarMethod()
        {
            var method = typeof(MainWindow).GetMethod(
                "ConfigureMainToolbarAccessibility",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(method, "The main toolbar accessibility configuration is missing.");
            return method;
        }
    }
}
