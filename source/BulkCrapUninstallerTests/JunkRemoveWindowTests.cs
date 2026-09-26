using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class JunkRemoveWindowTests
    {
        [TestMethod]
        public void AllLowConfidenceJunkIsVisibleButNotSelected()
        {
            RequireWindows();
            var junk = CreateJunk("low-confidence.txt", -1);

            using var window = new JunkRemoveWindow(new[] { junk });
            var list = GetControl<ObjectListView>(window, "objectListViewMain");
            var hideLowConfidence = GetControl<CheckBox>(window, "checkBoxHideLowConfidence");

            Assert.IsFalse(hideLowConfidence.Checked);
            Assert.IsFalse(hideLowConfidence.Enabled);
            Assert.AreEqual(1, list.Items.Count);
            Assert.AreEqual(0, window.SelectedJunk.Count());
        }

        [TestMethod]
        public void MixedConfidenceJunkKeepsLowConfidenceHidden()
        {
            RequireWindows();
            var good = CreateJunk("good.txt", 2);
            var low = CreateJunk("low-confidence.txt", -1);

            using var window = new JunkRemoveWindow(new[] { good, low });
            var list = GetControl<ObjectListView>(window, "objectListViewMain");
            var hideLowConfidence = GetControl<CheckBox>(window, "checkBoxHideLowConfidence");

            Assert.IsTrue(hideLowConfidence.Checked);
            Assert.IsTrue(hideLowConfidence.Enabled);
            Assert.AreEqual(1, list.Items.Count);
            Assert.AreSame(good, ((OLVListItem)list.Items[0]).RowObject);
            Assert.AreSame(good, window.SelectedJunk.Single());
        }

        private static void RequireWindows()
        {
            if (!OperatingSystem.IsWindows())
                Assert.Inconclusive("Requires Windows Forms.");
        }

        private static FileSystemJunk CreateJunk(string name, int confidence)
        {
            var app = new ApplicationUninstallerEntry { RawDisplayName = "Portable Test App" };
            var junk = new FileSystemJunk(new FileInfo(Path.Combine(@"C:\Temp", name)), app, null);
            junk.Confidence.Add(new ConfidenceRecord(confidence, "test confidence"));
            return junk;
        }

        private static T GetControl<T>(Control parent, string name) where T : Control
        {
            return (T)parent.Controls.Find(name, true).Single();
        }
    }
}
