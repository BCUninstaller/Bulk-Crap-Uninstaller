/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Klocman.Tools
{
    public static class DrawingTools
    {
        /// <summary>
        /// Returns an icon representation of an image contained in the specified file.
        /// This function is identical to System.Drawing.Icon.ExtractAssociatedIcon, xcept this version works for UNC paths.
        /// </summary>
        /// <param name="filePath">The path to the file that contains an image.</param>
        /// <returns>The System.Drawing.Icon representation of the image contained in the specified file.</returns>
        /// <exception cref="ArgumentException">filePath does not indicate a valid file.</exception>
        public static Icon ExtractAssociatedIcon(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            // Uri reads the "?" of extended-length paths (\\?\C:\..., \\?\UNC\server\...) as a host name and throws
            if (filePath.StartsWith(@"\\?\UNC\", StringComparison.OrdinalIgnoreCase))
                filePath = @"\\" + filePath.Substring(8);
            else if (filePath.StartsWith(@"\\?\", StringComparison.Ordinal))
                filePath = filePath.Substring(4);

            Uri uri;

            try
            {
                uri = new Uri(filePath);
            }
            catch (UriFormatException)
            {
                filePath = Path.GetFullPath(filePath);
                uri = new Uri(filePath);
            }

            if (uri.IsFile)
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException(filePath);

                var iconPath = new StringBuilder(260);
                iconPath.Append(filePath);
                
                var index = 0;
                var handle = SafeNativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
                if (handle != IntPtr.Zero)
                    return CreateOwnedIconFromHandle(handle);
            }
            return null;
        }

        /// <summary>
        /// Clone an icon handle into a managed icon instance and release the original native handle.
        /// </summary>
        public static Icon CreateOwnedIconFromHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentException("Icon handle must not be zero", nameof(handle));

            try
            {
                using (var temporaryIcon = Icon.FromHandle(handle))
                {
                    return (Icon)temporaryIcon.Clone();
                }
            }
            finally
            {
                SafeNativeMethods.DestroyIcon(handle);
            }
        }


        /// <summary>
        /// This class suppresses stack walks for unmanaged code permission. 
        /// (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
        /// This class is for methods that are safe for anyone to call. 
        /// Callers of these methods are not required to perform a full security review to make sure that the 
        /// usage is secure because the methods are harmless for any caller.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
            internal static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DestroyIcon(IntPtr hIcon);
        }

        public static Color ColorLerp(Color from, Color to, float ratio)
        {
            var aDiff = to.A - from.A;
            var rDiff = to.R - from.R;
            var gDiff = to.G - from.G;
            var bDiff = to.B - from.B;

            return Color.FromArgb((byte)(from.A + ratio * aDiff), (byte)(from.R + ratio * rDiff),
                (byte)(from.G + ratio * gDiff), (byte)(from.B + ratio * bDiff));
        }

        /// <exception cref="Exception">The operation failed.</exception>
        public static Bitmap ResizeBitmap(Image sourceBmp, int newWidth, int newHeight)
        {
            var result = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(result))
            {
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.DrawImage(sourceBmp, 0, 0, newWidth, newHeight);
            }
            return result;
        }

        /// <summary>
        /// Roughly converts Image to an Icon.
        /// http://stackoverflow.com/a/21389253/4309247
        /// Tested on .NET 4.5 and Windows 8.1. Beware of the possibility of "fringes" you'll see on PNG images with transparency on the edges.
        /// </summary>
        /// <param name="img">Image to convert</param>
        /// <returns></returns>
        public static Icon IconFromImage(Image img)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            // Header
            bw.Write((short)0); // 0 : reserved
            bw.Write((short)1); // 2 : 1=ico, 2=cur
            bw.Write((short)1); // 4 : number of images
            // Image directory
            var w = img.Width;
            if (w >= 256) w = 0;
            bw.Write((byte)w); // 0 : width of image
            var h = img.Height;
            if (h >= 256) h = 0;
            bw.Write((byte)h); // 1 : height of image
            bw.Write((byte)0); // 2 : number of colors in palette
            bw.Write((byte)0); // 3 : reserved
            bw.Write((short)0); // 4 : number of color planes
            bw.Write((short)0); // 6 : bits per pixel
            var sizeHere = ms.Position;
            bw.Write(0); // 8 : image size
            var start = (int)ms.Position + 4;
            bw.Write(start); // 12: offset of image data
            // Image data
            img.Save(ms, ImageFormat.Png);
            var imageSize = (int)ms.Position - start;
            ms.Seek(sizeHere, SeekOrigin.Begin);
            bw.Write(imageSize);
            ms.Seek(0, SeekOrigin.Begin);

            // And load it
            return new Icon(ms);
        }
    }
}
