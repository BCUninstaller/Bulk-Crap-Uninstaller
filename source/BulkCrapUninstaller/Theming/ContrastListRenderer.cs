using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace BulkCrapUninstaller.Theming;

// Keep ObjectListView's filter highlighting and primary-checkbox geometry.
// Its normal CheckBoxRenderer path can retain dark themed glyphs in contrast mode.
internal sealed class ContrastListRenderer : HighlightTextRenderer
{
    protected override int DrawCheckBox(Graphics graphics, Rectangle bounds)
    {
        if (!SystemInformation.HighContrast || IsPrinting || UseCustomCheckboxImages
            || ListView.View != View.Details)
            return base.DrawCheckBox(graphics, bounds);

        var size = CalculateCheckBoxSize(graphics);
        var box = new Rectangle(bounds.X, AlignVertically(bounds, size.Height), size.Width, size.Height);
        // Read the palette on each paint: no theme handle or colored bitmap cache.
        ContrastCheckBoxPainter.Draw(graphics, box, ListItem.CheckState,
            !ListView.Enabled || IsCheckBoxDisabled, IsCheckboxHot);
        return size.Width;
    }
}
