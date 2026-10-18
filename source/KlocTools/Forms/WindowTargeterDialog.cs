/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Windows.Forms;
using Klocman.Forms.Tools;
using Klocman.Subsystems;

namespace Klocman.Forms
{
    public partial class WindowTargeterDialog : Form
    {
        private WindowHoverSearcher.WindowInfo _result;

        private WindowTargeterDialog()
        {
            InitializeComponent();
            ModernStyleController.ApplyOnLoad(this);

            windowTargeter1.WindowSelected += OnWindowSelected;
        }

        public static WindowHoverSearcher.WindowInfo ShowDialog(IWin32Window owner, bool useCursorPos)
        {
            using (var window = new WindowTargeterDialog())
            {
                if (useCursorPos)
                {
                    window.StartPosition = FormStartPosition.Manual;
                    var targeterHalf = window.windowTargeter1.Height / 2;
                    var offsetx = targeterHalf + 10;
                    var offsety = targeterHalf + 30;
                    window.Location = new Point(Cursor.Position.X - offsetx, Cursor.Position.Y - offsety);
                }
                else
                {
                    window.StartPosition = FormStartPosition.CenterParent;
                }

                return window.ShowDialog(owner) != DialogResult.OK ? null : window._result;
            }
        }

        private void OnWindowSelected(object sender, WindowHoverEventArgs windowHoverEventArgs)
        {
            _result = windowHoverEventArgs.TargetWindow;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}