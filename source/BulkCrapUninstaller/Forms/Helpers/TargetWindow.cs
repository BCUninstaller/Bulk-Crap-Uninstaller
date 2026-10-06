/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using BulkCrapUninstaller.Controls;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Klocman.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Forms
{
    public partial class TargetWindow : Form
    {
        private Action<bool> _setMainWindowVisible;

        public event EventHandler<DirectoriesSelectedEventArgs> DirectoriesSelected;

        public TargetWindow()
        {
            InitializeComponent();

            fileTargeter1.DirectoriesSelected += DirectoryTargeterDirectoriesSelected;
            windowTargeter1.PickingStarted += WindowTargeter1OnPickingStarted;
            windowTargeter1.WindowSelected += WindowTargeterWindowSelected;
        }

        private void DirectoryTargeterDirectoriesSelected(object sender, DirectoriesSelectedEventArgs e)
        {
            if (e.SelectedFiles.Count == 0)
            {
                MessageBoxes.ShowMessageBox(this,
                    Localisable.TargetWindow_NoFilesSelected_Message,
                    Localisable.TargetWindow_NoFilesSelected_Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            OnDirectoriesSelected(e);
        }

        protected virtual void OnDirectoriesSelected(DirectoriesSelectedEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
            DirectoriesSelected?.Invoke(this, e);
        }

        private void WindowTargeter1OnPickingStarted(object sender, EventArgs e)
        {
            _setMainWindowVisible(false);
        }

        private void WindowTargeterWindowSelected(object sender, Klocman.Subsystems.WindowHoverEventArgs e)
        {
            _setMainWindowVisible(true);

            try
            {
                var fileName = e.TargetWindow.GetRunningProcess().MainModule?.FileName;
                if (fileName == null)
                    throw new InvalidOperationException("Process has no MainModule");

                var parentDirectory = new FileInfo(fileName).Directory;
                if (parentDirectory == null)
                    throw new InvalidOperationException("Failed to get MainModule Directory");

                // Ignore targeting BCU itself
                if (PathTools.SubPathIsInsideBasePath(Program.AssemblyLocation.FullName, fileName, true, true))
                    return;

                OnDirectoriesSelected(new DirectoriesSelectedEventArgs(parentDirectory.ToEnumerable().ToList()));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                PremadeDialogs.GenericError(exception);
            }
        }

        public static ICollection<DirectoryInfo> ShowDialog(IWin32Window owner, Action<bool> setMainWindowVisible)
        {
            using (var window = new TargetWindow())
            {
                window._setMainWindowVisible = setMainWindowVisible ?? throw new ArgumentNullException(nameof(setMainWindowVisible));

                window.StartPosition = FormStartPosition.Manual;
                var targeterHalf = window.windowTargeter1.Height / 2;
                var offsetx = targeterHalf + 10;
                var offsety = targeterHalf + 30;
                window.Location = new Point(Cursor.Position.X - offsetx, Cursor.Position.Y - offsety);

                ICollection<DirectoryInfo> results = null;
                window.DirectoriesSelected += (sender, args) => results = args.SelectedFiles;

                if (window.ShowDialog() != DialogResult.OK)
                    return null;

                return results;
            }
        }
    }
}
