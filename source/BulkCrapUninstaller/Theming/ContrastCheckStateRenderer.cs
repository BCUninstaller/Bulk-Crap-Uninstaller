using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace BulkCrapUninstaller.Theming;

// CheckStateRenderer is the separate path used by checkbox columns after column 0.
internal sealed class ContrastCheckStateRenderer : CheckStateRenderer
{
    public override void Render(Graphics graphics, Rectangle bounds)
    {
        if (!SystemInformation.HighContrast || IsPrinting || Column == null)
        {
            base.Render(graphics, bounds);
            return;
        }

        DrawBackground(graphics, bounds);
        var box = CalculateCheckBoxBounds(graphics, ApplyCellPadding(bounds));
        ContrastCheckBoxPainter.Draw(graphics, box, Column.GetCheckState(RowObject),
            !ListView.Enabled || IsCheckBoxDisabled, IsCheckboxHot);
    }
}
