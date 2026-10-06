using System;
using System.Drawing;
using System.Windows.Forms;
namespace BulkCrapUninstaller.Controls.Settings
{
    partial class CacheSettings
    {
        internal void ConstrainExplanationWidth()
        {
            // This auto-sized label's unconstrained width exceeds its Settings host.
            // Wrap the existing localized text to the available width at each layout size.
            groupBox1.SizeChanged += (_, _) => UpdateWidth();
            UpdateWidth();

            void UpdateWidth()
            {
                var width = Math.Max(1, groupBox1.ClientSize.Width - groupBox1.Padding.Right
                    - flowLayoutPanel1.Left - flowLayoutPanel1.Padding.Horizontal - label1.Margin.Horizontal);
                var maximum = new Size(width, 0);
                if (label1.MaximumSize != maximum) label1.MaximumSize = maximum;
            }
        }
    }
}
