using System;
using System.Drawing;
namespace BulkCrapUninstaller.Theming;

internal static class MonochromeIcons
{
    // Call only for BCU UI assets. Reject colored images as a whole, including
    // their black outlines. Never recolor program logos or colored artwork.
    // Return an owned copy; never modify the shared resource bitmap.
    internal static Bitmap CreateForForeground(Image original, Color foreground)
    {
        var bitmap = new Bitmap(original);
        var hasInk = false;
        for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                if (pixel.A == 0) continue;
                if (pixel.R != pixel.G || pixel.G != pixel.B)
                {
                    bitmap.Dispose();
                    return null;
                }
                hasInk |= pixel.R < 80;
            }
        if (!hasInk) { bitmap.Dispose(); return null; }
        for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                if (pixel.A != 0 && pixel.R < 80)
                    bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, foreground));
            }
        return bitmap;
    }
}
