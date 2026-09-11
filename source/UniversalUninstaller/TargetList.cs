using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Klocman.IO;
using Klocman.Tools;
using UniversalUninstaller.Properties;

namespace UniversalUninstaller
{
    public partial class TargetList : UserControl
    {
        public TargetList()
        {
            InitializeComponent();

            treeListView1.CanExpandGetter = model => ((TreeEntry)model).IsDirectory;
            treeListView1.ChildrenGetter = ChildrenGetter;

            treeListView1.HierarchicalCheckboxes = false;
            treeListView1.UseWaitCursorWhenExpanding = true;
            treeListView1.PersistentCheckBoxes = false;

            treeListView1.CellToolTipGetter =
                (column, modelObject) => (modelObject as TreeEntry)?.FileSystemInfo.FullName;

            treeListView1.BooleanCheckStateGetter = BooleanCheckStateGetter;
            treeListView1.BooleanCheckStatePutter = BooleanCheckStatePutter;

            olvColumnName.AspectGetter = rowObject => (rowObject as TreeEntry)?.FileSystemInfo.Name;
            olvColumnName.ImageGetter = ImageGetter;

            olvColumnSize.AspectGetter = SizeGetter;

            var il = new ImageList { ColorDepth = ColorDepth.Depth24Bit, ImageSize = new Size(16, 16) };
            il.Images.Add(Resources.Folder_48x48);
            il.Images.Add(Resources.Generic_Document);
            il.Images.Add(Resources.Generic_Application);
            treeListView1.SmallImageList = il;
        }

        private object SizeGetter(object rowObject)
        {
            if (!(rowObject is TreeEntry treeEntry))
                return FileSize.Empty;

            if (treeEntry.IsDirectory == false)
                return FileSize.FromBytes(((FileInfo)treeEntry.FileSystemInfo).Length);

            if (!(treeEntry.FileSystemInfo is DirectoryInfo dirInfo) || !dirInfo.Exists)
                return FileSize.Empty;

            try
            {
                var bytes = ScriptingFileSystem.GetDirectorySizeBytes(dirInfo.FullName);
                if (bytes.HasValue)
                {
                    return FileSize.FromBytes(bytes.Value);
                }
                return FileSize.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return FileSize.Empty;
            }
        }

        private object ImageGetter(object rowObject)
        {
            if (rowObject is TreeEntry treeEntry)
            {
                if (treeEntry.IsDirectory)
                    return 0;

                if (WindowsTools.IsExectuable(treeEntry.FileSystemInfo.FullName, false))
                    return 2;
            }

            return 1;
        }

        private IEnumerable ChildrenGetter(object model)
        {
            return (((TreeEntry)model).FileSystemInfo as DirectoryInfo)?.GetFileSystemInfos()
                .Select(x => new TreeEntry(x));
        }

        private bool BooleanCheckStatePutter(object rowObject, bool value)
        {
            if (value)
            {
                treeListView1.CheckObjects(treeListView1.GetChildren(rowObject));
            }
            else
            {
                if (treeListView1.GetParent(rowObject) is TreeEntry parent && parent.Checked)
                {
                    treeListView1.UncheckObject(parent);
                }
            }

            return ((TreeEntry)rowObject).Checked = value;
        }

        private bool BooleanCheckStateGetter(object rowObject)
        {
            return ((TreeEntry)rowObject).Checked;
        }

        public void Populate(DirectoryInfo rootDirectory)
        {
            treeListView1.ClearObjects();
            var root = new TreeEntry(rootDirectory);
            treeListView1.AddObject(root);
            treeListView1.Expand(root);

            // If there is no executable in the root, expand the subdirectories to show more info
            var subs = treeListView1.GetChildren(rootDirectory).Cast<TreeEntry>().ToList();
            if (subs.All(x => x.IsDirectory))
            {
                foreach (var dir in subs)
                    treeListView1.Expand(dir);
            }

            treeListView1.CheckAll();
        }

        public IEnumerable<FileSystemInfo> GetItemsToDelete()
        {
            return treeListView1.Roots.Cast<TreeEntry>().SelectMany(GetSelectedItems);
        }


        public IEnumerable<FileSystemInfo> GetSelectedItems(object modelItem)
        {
            if (!(modelItem is TreeEntry treeEntry))
                return Enumerable.Empty<FileSystemInfo>();

            if (treeEntry.Checked)
                return Enumerable.Repeat(treeEntry.FileSystemInfo, 1);

            return treeListView1.GetChildren(modelItem).Cast<object>().SelectMany(GetSelectedItems);
        }

        private void toolStripButtonSelAll_Click(object sender, EventArgs e)
        {
            treeListView1.CheckAll();
        }

        private void expand_Click(object sender, EventArgs e)
        {
            treeListView1.ExpandAll();
        }

        private void collapse_Click(object sender, EventArgs e)
        {
            treeListView1.CollapseAll();
        }

        private void treeListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            treeListView1.Refresh();
        }

        private void treeListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var x = treeListView1.GetItemAt(e.X, e.Y) as OLVListItem;
            if (!(x?.RowObject is TreeEntry en)) return;
            try
            {
                if (en.IsDirectory)
                {
                    Process.Start(new ProcessStartInfo('"' + en.FileSystemInfo.FullName + '"') { UseShellExecute = true });
                }
                else
                {
                    WindowsTools.OpenExplorerFocusedOnObject(en.FileSystemInfo.FullName);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}