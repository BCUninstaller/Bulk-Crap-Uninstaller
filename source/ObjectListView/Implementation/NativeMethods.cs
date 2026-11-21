/*
 * NativeMethods - All the Windows SDK structures and imports
 *
 * Author: Phillip Piper
 * Date: 10/10/2006
 *
 * Change log:
 * v2.8.0
 * 2014-05-21   JPP  - Added DeselectOneItem
 *                   - Added new imagelist drawing
 * v2.3
 * 2006-10-10   JPP  - Initial version
 *
 * To do:
 * 
 * Copyright (C) 2006-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// Wrapper for all native method calls on ListView controls
    /// </summary>
    internal static class NativeMethods
    {
        #region Constants

        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETCOLUMN = LVM_FIRST + 95;
        private const int LVM_GETCOUNTPERPAGE = LVM_FIRST + 40;
        private const int LVM_GETGROUPINFO = LVM_FIRST + 149;
        private const int LVM_GETGROUPSTATE = LVM_FIRST + 92;
        private const int LVM_GETHEADER = LVM_FIRST + 31;
        private const int LVM_GETTOOLTIPS = LVM_FIRST + 78;
        private const int LVM_GETTOPINDEX = LVM_FIRST + 39;
        private const int LVM_HITTEST = LVM_FIRST + 18;
        private const int LVM_INSERTGROUP = LVM_FIRST + 145;
        private const int LVM_REMOVEALLGROUPS = LVM_FIRST + 160;
        private const int LVM_SCROLL = LVM_FIRST + 20;
        private const int LVM_SETBKIMAGE = LVM_FIRST + 0x8A;
        private const int LVM_SETCOLUMN = LVM_FIRST + 96;
        private const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        private const int LVM_SETGROUPINFO = LVM_FIRST + 147;
        private const int LVM_SETGROUPMETRICS = LVM_FIRST + 155;
        private const int LVM_SETIMAGELIST = LVM_FIRST + 3;
        private const int LVM_SETITEM = LVM_FIRST + 76;
        private const int LVM_SETITEMCOUNT = LVM_FIRST + 47;
        private const int LVM_SETITEMSTATE = LVM_FIRST + 43;
        private const int LVM_SETSELECTEDCOLUMN = LVM_FIRST + 140;
        private const int LVM_SETTOOLTIPS = LVM_FIRST + 74;
        private const int LVM_SUBITEMHITTEST = LVM_FIRST + 57;
        private const int LVS_EX_SUBITEMIMAGES = 0x0002;

        private const int LVIF_TEXT = 0x0001;
        private const int LVIF_IMAGE = 0x0002;
        private const int LVIF_PARAM = 0x0004;
        private const int LVIF_STATE = 0x0008;
        private const int LVIF_INDENT = 0x0010;
        private const int LVIF_NORECOMPUTE = 0x0800;

        private const int LVIS_SELECTED = 2;

        private const int LVCF_FMT = 0x0001;
        private const int LVCF_WIDTH = 0x0002;
        private const int LVCF_TEXT = 0x0004;
        private const int LVCF_SUBITEM = 0x0008;
        private const int LVCF_IMAGE = 0x0010;
        private const int LVCF_ORDER = 0x0020;
        private const int LVCFMT_LEFT = 0x0000;
        private const int LVCFMT_RIGHT = 0x0001;
        private const int LVCFMT_CENTER = 0x0002;
        private const int LVCFMT_JUSTIFYMASK = 0x0003;

        private const int LVCFMT_IMAGE = 0x0800;
        private const int LVCFMT_BITMAP_ON_RIGHT = 0x1000;
        private const int LVCFMT_COL_HAS_IMAGES = 0x8000;

        private const int LVBKIF_SOURCE_NONE = 0x0;
        private const int LVBKIF_SOURCE_HBITMAP = 0x1;
        private const int LVBKIF_SOURCE_URL = 0x2;
        private const int LVBKIF_SOURCE_MASK = 0x3;
        private const int LVBKIF_STYLE_NORMAL = 0x0;
        private const int LVBKIF_STYLE_TILE = 0x10;
        private const int LVBKIF_STYLE_MASK = 0x10;
        private const int LVBKIF_FLAG_TILEOFFSET = 0x100;
        private const int LVBKIF_TYPE_WATERMARK = 0x10000000;
        private const int LVBKIF_FLAG_ALPHABLEND = 0x20000000;

        private const int LVSICF_NOINVALIDATEALL = 1;
        private const int LVSICF_NOSCROLL = 2;

        private const int HDM_FIRST = 0x1200;
        private const int HDM_HITTEST = HDM_FIRST + 6;
        private const int HDM_GETITEMRECT = HDM_FIRST + 7;
        private const int HDM_GETITEM = HDM_FIRST + 11;
        private const int HDM_SETITEM = HDM_FIRST + 12;

        private const int HDI_WIDTH = 0x0001;
        private const int HDI_TEXT = 0x0002;
        private const int HDI_FORMAT = 0x0004;
        private const int HDI_BITMAP = 0x0010;
        private const int HDI_IMAGE = 0x0020;

        private const int HDF_LEFT = 0x0000;
        private const int HDF_RIGHT = 0x0001;
        private const int HDF_CENTER = 0x0002;
        private const int HDF_JUSTIFYMASK = 0x0003;
        private const int HDF_RTLREADING = 0x0004;
        private const int HDF_STRING = 0x4000;
        private const int HDF_BITMAP = 0x2000;
        private const int HDF_BITMAP_ON_RIGHT = 0x1000;
        private const int HDF_IMAGE = 0x0800;
        private const int HDF_SORTUP = 0x0400;
        private const int HDF_SORTDOWN = 0x0200;

        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int SB_CTL = 2;
        private const int SB_BOTH = 3;

        private const int SIF_RANGE = 0x0001;
        private const int SIF_PAGE = 0x0002;
        private const int SIF_POS = 0x0004;
        private const int SIF_DISABLENOSCROLL = 0x0008;
        private const int SIF_TRACKPOS = 0x0010;
        private const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

        private const int ILD_NORMAL = 0x0;
        private const int ILD_TRANSPARENT = 0x1;
        private const int ILD_MASK = 0x10;
        private const int ILD_IMAGE = 0x20;
        private const int ILD_BLEND25 = 0x2;
        private const int ILD_BLEND50 = 0x4;

        const int SWP_NOSIZE = 1;
        const int SWP_NOMOVE = 2;
        const int SWP_NOZORDER = 4;
        const int SWP_NOREDRAW = 8;
        const int SWP_NOACTIVATE = 16;
        public const int SWP_FRAMECHANGED = 32;

        const int SWP_ZORDERONLY = SWP_NOSIZE | SWP_NOMOVE | SWP_NOREDRAW | SWP_NOACTIVATE;
        const int SWP_SIZEONLY = SWP_NOMOVE | SWP_NOREDRAW | SWP_NOZORDER | SWP_NOACTIVATE;
        const int SWP_UPDATE_FRAME = SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_NOZORDER | SWP_FRAMECHANGED;

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct HDITEM
        {
            public int mask;
            public int cxy;
            public IntPtr pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public int fmt;
            public IntPtr lParam;
            public int iImage;
            public int iOrder;
            //if (_WIN32_IE >= 0x0500)
            public int type;
            public IntPtr pvFilter;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class HDHITTESTINFO
        {
            public int pt_x;
            public int pt_y;
            public int flags;
            public int iItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class HDLAYOUT
        {
            public IntPtr prc;
            public IntPtr pwpos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;
            public int yBitmap;
            public uint rgbBk;
            public uint rgbFg;
            public uint fStyle;
            public uint dwRop;
            public uint fState;
            public uint Frame;
            public uint crEffect;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVBKIMAGE
        {
            public int ulFlags;
            public IntPtr hBmp;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszImage;
            public int cchImageMax;
            public int xOffset;
            public int yOffset;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVCOLUMN
        {
            public int mask;
            public int fmt;
            public int cx;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public int cchTextMax;
            public int iSubItem;
            // These are available in Common Controls >= 0x0300
            public int iImage;
            public int iOrder;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVFINDINFO
        {
            public int flags;
            public string psz;
            public IntPtr lParam;
            public int ptX;
            public int ptY;
            public int vkDirection;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LVGROUP
        {
            public uint cbSize;
            public uint mask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszHeader;
            public int cchHeader;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszFooter;
            public int cchFooter;
            public int iGroupId;
            public uint stateMask;
            public uint state;
            public uint uAlign;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LVGROUP2
        {
            public uint cbSize;
            public uint mask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszHeader;
            public uint cchHeader;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszFooter;
            public int cchFooter;
            public int iGroupId;
            public uint stateMask;
            public uint state;
            public uint uAlign;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszSubtitle;
            public uint cchSubtitle;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszTask;
            public uint cchTask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszDescriptionTop;
            public uint cchDescriptionTop;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszDescriptionBottom;
            public uint cchDescriptionBottom;
            public int iTitleImage;
            public int iExtendedImage;
            public int iFirstItem;         // Read only
            public int cItems;             // Read only
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszSubsetTitle;     // NULL if group is not subset
            public uint cchSubsetTitle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LVGROUPMETRICS
        {
            public uint cbSize;
            public uint mask;
            public uint Left;
            public uint Top;
            public uint Right;
            public uint Bottom;
            public int crLeft;
            public int crTop;
            public int crRight;
            public int crBottom;
            public int crHeader;
            public int crFooter;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVHITTESTINFO
        {
            public int pt_x;
            public int pt_y;
            public int flags;
            public int iItem;
            public int iSubItem;
            public int iGroup;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            // These are available in Common Controls >= 0x0300
            public int iIndent;
            // These are available in Common Controls >= 0x056
            public int iGroupId;
            public int cColumns;
            public IntPtr puColumns;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public int code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMCUSTOMDRAW
        {
            public NMHDR nmcd;
            public int dwDrawStage;
            public IntPtr hdc;
            public RECT rc;
            public IntPtr dwItemSpec;
            public int uItemState;
            public IntPtr lItemlParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHEADER
        {
            public NMHDR nhdr;
            public int iItem;
            public int iButton;
            public IntPtr pHDITEM;
        }

        const int MAX_LINKID_TEXT = 48;
        const int L_MAX_URL_LENGTH = 2048 + 32 + 4;
        //#define L_MAX_URL_LENGTH    (2048 + 32 + sizeof("://"))

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LITEM
        {
            public uint mask;
            public int iLink;
            public uint state;
            public uint stateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_LINKID_TEXT)]
            public string szID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = L_MAX_URL_LENGTH)]
            public string szUrl;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLISTVIEW
        {
            public NMHDR hdr;
            public int iItem;
            public int iSubItem;
            public int uNewState;
            public int uOldState;
            public int uChanged;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLVCUSTOMDRAW
        {
            public NMCUSTOMDRAW nmcd;
            public int clrText;
            public int clrTextBk;
            public int iSubItem;
            public int dwItemType;
            public int clrFace;
            public int iIconEffect;
            public int iIconPhase;
            public int iPartId;
            public int iStateId;
            public RECT rcText;
            public uint uAlign;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLVFINDITEM
        {
            public NMHDR hdr;
            public int iStart;
            public LVFINDINFO lvfi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLVGETINFOTIP
        {
            public NMHDR hdr;
            public int dwFlags;
            public string pszText;
            public int cchTextMax;
            public int iItem;
            public int iSubItem;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLVGROUP
        {
            public NMHDR hdr;
            public int iGroupId; // which group is changing
            public uint uNewState; // LVGS_xxx flags
            public uint uOldState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLVLINK
        {
            public NMHDR hdr;
            public LITEM link;
            public int iItem;
            public int iSubItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLVSCROLL
        {
            public NMHDR hdr;
            public int dx;
            public int dy;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct NMTTDISPINFO
        {
            public NMHDR hdr;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szText;
            public IntPtr hinst;
            public int uFlags;
            public IntPtr lParam;
            //public int hbmp; This is documented but doesn't work
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SCROLLINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(SCROLLINFO));
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class TOOLINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(TOOLINFO));
            public int uFlags;
            public IntPtr hwnd;
            public IntPtr uId;
            public RECT rect;
            public IntPtr hinst = IntPtr.Zero;
            public IntPtr lpszText;
            public IntPtr lParam = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        #endregion

        #region Entry points

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool enable);

        // Various flavours of SendMessage: plain vanilla, and passing references to various structures
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageLVItem(IntPtr hWnd, int msg, int wParam, ref LVITEM lvi);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref LVHITTESTINFO ht);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageRECT(IntPtr hWnd, int msg, int wParam, ref RECT r);
        //[DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        //private static extern IntPtr SendMessageLVColumn(IntPtr hWnd, int m, int wParam, ref LVCOLUMN lvc);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageHDItem(IntPtr hWnd, int msg, int wParam, ref HDITEM hdi);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageHDHITTESTINFO(IntPtr hWnd, int Msg, IntPtr wParam, [In, Out] HDHITTESTINFO lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTOOLINFO(IntPtr hWnd, int Msg, int wParam, TOOLINFO lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageLVBKIMAGE(IntPtr hWnd, int Msg, int wParam, ref LVBKIMAGE lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageString(IntPtr hWnd, int Msg, int wParam, string lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageIUnknown(IntPtr hWnd, int msg, [MarshalAs(UnmanagedType.IUnknown)] object wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref LVGROUP lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref LVGROUP2 lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref LVGROUPMETRICS lParam);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr objectHandle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetClientRect(IntPtr hWnd, ref Rectangle r);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, SCROLLINFO scrollInfo);

        [DllImport("user32.dll", EntryPoint = "GetUpdateRect", CharSet = CharSet.Auto)]
        private static extern bool GetUpdateRectInternal(IntPtr hWnd, ref Rectangle r, bool eraseBackground);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImageList_Draw(IntPtr himl, int i, IntPtr hdcDst, int x, int y, int fStyle);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImageList_DrawIndirect(ref IMAGELISTDRAWPARAMS parms);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle r);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "ValidateRect", CharSet = CharSet.Auto)]
        private static extern IntPtr ValidatedRectInternal(IntPtr hWnd, ref Rectangle r);

        #endregion

        //[DllImport("user32.dll", EntryPoint = "LockWindowUpdate", CharSet = CharSet.Auto)]
        //private static extern int LockWindowUpdateInternal(IntPtr hWnd);

        //public static void LockWindowUpdate(IWin32Window window) {
        //    if (window == null)
        //        NativeMethods.LockWindowUpdateInternal(IntPtr.Zero);
        //    else
        //        NativeMethods.LockWindowUpdateInternal(window.Handle);
        //}

        /// <summary>
        /// Put an image under the ListView.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The ListView must have its handle created before calling this.
        /// </para>
        /// <para>
        /// This doesn't work very well. Specifically, it doesn't play well with owner drawn, 
        /// and grid lines are drawn over it.
        /// </para>
        /// </remarks>
        /// <param name="lv"></param>
        /// <param name="image">The image to be used as the background. If this is null, any existing background image will be cleared.</param>
        /// <param name="isWatermark">If this is true, the image is pinned to the bottom right and does not scroll. The other parameters are ignored</param>
        /// <param name="isTiled">If this is true, the image will be tiled to fill the whole control background. The offset parameters will be ignored.</param>
        /// <param name="xOffset">If both watermark and tiled are false, this indicates the horizontal percentage where the image will be placed. 0 is absolute left, 100 is absolute right.</param>
        /// <param name="yOffset">If both watermark and tiled are false, this indicates the vertical percentage where the image will be placed.</param>
        /// <returns></returns>
        public static bool SetBackgroundImage(ListView lv, Image image, bool isWatermark, bool isTiled, int xOffset, int yOffset) {

            LVBKIMAGE lvbkimage = new LVBKIMAGE();

            // We have to clear any pre-existing background image, otherwise the attempt to set the image will fail.
            // We don't know which type may already have been set, so we just clear both the watermark and the image.
            lvbkimage.ulFlags = LVBKIF_TYPE_WATERMARK;
            IntPtr result = SendMessageLVBKIMAGE(lv.Handle, LVM_SETBKIMAGE, 0, ref lvbkimage);
            lvbkimage.ulFlags = LVBKIF_SOURCE_HBITMAP;
            result = SendMessageLVBKIMAGE(lv.Handle, LVM_SETBKIMAGE, 0, ref lvbkimage);

            if (image is Bitmap bm) {
                lvbkimage.hBmp = bm.GetHbitmap();
                lvbkimage.ulFlags = isWatermark ? LVBKIF_TYPE_WATERMARK : (isTiled ? LVBKIF_SOURCE_HBITMAP | LVBKIF_STYLE_TILE : LVBKIF_SOURCE_HBITMAP);
                lvbkimage.xOffset = xOffset;
                lvbkimage.yOffset = yOffset;
                result = SendMessageLVBKIMAGE(lv.Handle, LVM_SETBKIMAGE, 0, ref lvbkimage);
            }

            return (result != IntPtr.Zero);
        }

        public static bool DrawImageList(Graphics g, ImageList il, int index, int x, int y, bool isSelected, bool isDisabled) {
            ImageListDrawItemConstants flags = (isSelected ? ImageListDrawItemConstants.ILD_SELECTED : ImageListDrawItemConstants.ILD_NORMAL) | ImageListDrawItemConstants.ILD_TRANSPARENT;
            ImageListDrawStateConstants state = isDisabled ? ImageListDrawStateConstants.ILS_SATURATE : ImageListDrawStateConstants.ILS_NORMAL;
            try {
                IntPtr hdc = g.GetHdc();
                return DrawImage(il, hdc, index, x, y, flags, 0, 0, state);
            }
            finally {
                g.ReleaseHdc();
            }
        }

        /// <summary>
        /// Flags controlling how the Image List item is 
        /// drawn
        /// </summary>
        [Flags]
        public enum ImageListDrawItemConstants 
        {
            /// <summary>
            /// Draw item normally.
            /// </summary>
            ILD_NORMAL = 0x0,
            /// <summary>
            /// Draw item transparently.
            /// </summary>
            ILD_TRANSPARENT = 0x1,
            /// <summary>
            /// Draw item blended with 25% of the specified foreground colour
            /// or the Highlight colour if no foreground colour specified.
            /// </summary>
            ILD_BLEND25 = 0x2,
            /// <summary>
            /// Draw item blended with 50% of the specified foreground colour
            /// or the Highlight colour if no foreground colour specified.
            /// </summary>
            ILD_SELECTED = 0x4,
            /// <summary>
            /// Draw the icon's mask
            /// </summary>
            ILD_MASK = 0x10,
            /// <summary>
            /// Draw the icon image without using the mask
            /// </summary>
            ILD_IMAGE = 0x20,
            /// <summary>
            /// Draw the icon using the ROP specified.
            /// </summary>
            ILD_ROP = 0x40,
            /// <summary>
            /// Preserves the alpha channel in dest. XP only.
            /// </summary>
            ILD_PRESERVEALPHA = 0x1000,
            /// <summary>
            /// Scale the image to cx, cy instead of clipping it. XP only.
            /// </summary>
            ILD_SCALE = 0x2000,
            /// <summary>
            /// Scale the image to the current DPI of the display. XP only.
            /// </summary>
            ILD_DPISCALE = 0x4000
        }

        /// <summary>
        /// Enumeration containing XP ImageList Draw State options
        /// </summary>
        [Flags]
        public enum ImageListDrawStateConstants 
        {
            /// <summary>
            /// The image state is not modified. 
            /// </summary>
            ILS_NORMAL = (0x00000000),
            /// <summary>
            /// Adds a glow effect to the icon, which causes the icon to appear to glow 
            /// with a given color around the edges. (Note: does not appear to be implemented)
            /// </summary>
            ILS_GLOW = (0x00000001), //The color for the glow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 
            /// <summary>
            /// Adds a drop shadow effect to the icon. (Note: does not appear to be implemented)
            /// </summary>
            ILS_SHADOW = (0x00000002), //The color for the drop shadow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 
            /// <summary>
            /// Saturates the icon by increasing each color component 
            /// of the RGB triplet for each pixel in the icon. (Note: only ever appears to result in a completely unsaturated icon)
            /// </summary>
            ILS_SATURATE = (0x00000004), // The amount to increase is indicated by the frame member in the IMAGELISTDRAWPARAMS method. 
            /// <summary>
            /// Alpha blends the icon. Alpha blending controls the transparency 
            /// level of an icon, according to the value of its alpha channel. 
            /// (Note: does not appear to be implemented).
            /// </summary>
            ILS_ALPHA = (0x00000008) //The value of the alpha channel is indicated by the frame member in the IMAGELISTDRAWPARAMS method. The alpha channel can be from 0 to 255, with 0 being completely transparent, and 255 being completely opaque. 
        }

        private const uint CLR_DEFAULT = 0xFF000000;

        /// <summary>
        /// Draws an image using the specified flags and state on XP systems.
        /// </summary>
        /// <param name="il">The image list from which an item will be drawn</param>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        /// <param name="cx">Width to draw</param>
        /// <param name="cy">Height to draw</param>
        /// <param name="stateFlags">State flags</param>
        public static bool DrawImage(ImageList il, IntPtr hdc, int index, int x, int y, ImageListDrawItemConstants flags, int cx, int cy, ImageListDrawStateConstants stateFlags) {
            IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
            pimldp.hdcDst = hdc;
            pimldp.cbSize = Marshal.SizeOf(pimldp.GetType());
            pimldp.i = index;
            pimldp.x = x;
            pimldp.y = y;
            pimldp.cx = cx;
            pimldp.cy = cy;
            pimldp.rgbFg = CLR_DEFAULT;
            pimldp.fStyle = (uint) flags;
            pimldp.fState = (uint) stateFlags;
            pimldp.himl = il.Handle;
            return ImageList_DrawIndirect(ref pimldp);
        }

        /// <summary>
        /// Make sure the ListView has the extended style that says to display subitem images.
        /// </summary>
        /// <remarks>This method must be called after any .NET call that update the extended styles
        /// since they seem to erase this setting.</remarks>
        /// <param name="list">The listview to send a m to</param>
        public static void ForceSubItemImagesExStyle(ListView list) {
            SendMessage(list.Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, LVS_EX_SUBITEMIMAGES, LVS_EX_SUBITEMIMAGES);
        }

        /// <summary>
        /// Change the virtual list size of the given ListView (which must be in virtual mode)
        /// </summary>
        /// <remarks>This will not change the scroll position</remarks>
        /// <param name="list">The listview to send a message to</param>
        /// <param name="count">How many rows should the list have?</param>
        public static void SetItemCount(ListView list, int count) {
            SendMessage(list.Handle, LVM_SETITEMCOUNT, count, LVSICF_NOSCROLL);
        }

        /// <summary>
        /// Make sure the ListView has the extended style that says to display subitem images.
        /// </summary>
        /// <remarks>This method must be called after any .NET call that update the extended styles
        /// since they seem to erase this setting.</remarks>
        /// <param name="list">The listview to send a m to</param>
        /// <param name="style"></param>
        /// <param name="styleMask"></param>
        public static void SetExtendedStyle(ListView list, int style, int styleMask) {
            SendMessage(list.Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, styleMask, style);
        }

        /// <summary>
        /// Calculates the number of items that can fit vertically in the visible area of a list-view (which
        /// must be in details or list view.
        /// </summary>
        /// <param name="list">The listView</param>
        /// <returns>Number of visible items per page</returns>
        public static int GetCountPerPage(ListView list) {
            return (int)SendMessage(list.Handle, LVM_GETCOUNTPERPAGE, 0, 0);
        }
        /// <summary>
        /// For the given item and subitem, make it display the given image
        /// </summary>
        /// <param name="list">The listview to send a m to</param>
        /// <param name="itemIndex">row number (0 based)</param>
        /// <param name="subItemIndex">subitem (0 is the item itself)</param>
        /// <param name="imageIndex">index into the image list</param>
        public static void SetSubItemImage(ListView list, int itemIndex, int subItemIndex, int imageIndex) {
            LVITEM lvItem = new LVITEM();
            lvItem.mask = LVIF_IMAGE;
            lvItem.iItem = itemIndex;
            lvItem.iSubItem = subItemIndex;
            lvItem.iImage = imageIndex;
            SendMessageLVItem(list.Handle, LVM_SETITEM, 0, ref lvItem);
        }

        /// <summary>
        /// Setup the given column of the listview to show the given image to the right of the text.
        /// If the image index is -1, any previous image is cleared
        /// </summary>
        /// <param name="list">The listview to send a m to</param>
        /// <param name="columnIndex">Index of the column to modifiy</param>
        /// <param name="order"></param>
        /// <param name="imageIndex">Index into the small image list</param>
        public static void SetColumnImage(ListView list, int columnIndex, SortOrder order, int imageIndex) {
            IntPtr hdrCntl = GetHeaderControl(list);
            if (hdrCntl.ToInt32() == 0)
                return;

            HDITEM item = new HDITEM();
            item.mask = HDI_FORMAT;
            IntPtr result = SendMessageHDItem(hdrCntl, HDM_GETITEM, columnIndex, ref item);

            item.fmt &= ~(HDF_SORTUP | HDF_SORTDOWN | HDF_IMAGE | HDF_BITMAP_ON_RIGHT);

            if (HasBuiltinSortIndicators()) {
                if (order == SortOrder.Ascending)
                    item.fmt |= HDF_SORTUP;
                if (order == SortOrder.Descending)
                    item.fmt |= HDF_SORTDOWN;
            } else {
                item.mask |= HDI_IMAGE;
                item.fmt |= (HDF_IMAGE | HDF_BITMAP_ON_RIGHT);
                item.iImage = imageIndex;
            }

            result = SendMessageHDItem(hdrCntl, HDM_SETITEM, columnIndex, ref item);
        }

        /// <summary>
        /// Does this version of the operating system have builtin sort indicators?
        /// </summary>
        /// <returns>Are there builtin sort indicators</returns>
        /// <remarks>XP and later have these</remarks>
        public static bool HasBuiltinSortIndicators() {
            return OSFeature.Feature.GetVersionPresent(OSFeature.Themes) != null;
        }

        /// <summary>
        /// Return the bounds of the update region on the given control.
        /// </summary>
        /// <remarks>The BeginPaint() system call validates the update region, effectively wiping out this information.
        /// So this call has to be made before the BeginPaint() call.</remarks>
        /// <param name="cntl">The control whose update region is be calculated</param>
        /// <returns>A rectangle</returns>
        public static Rectangle GetUpdateRect(Control cntl) {
            Rectangle r = new Rectangle();
            GetUpdateRectInternal(cntl.Handle, ref r, false);
            return r;
        }

        /// <summary>
        /// Validate an area of the given control. A validated area will not be repainted at the next redraw.
        /// </summary>
        /// <param name="cntl">The control to be validated</param>
        /// <param name="r">The area of the control to be validated</param>
        public static void ValidateRect(Control cntl, Rectangle r) {
            ValidatedRectInternal(cntl.Handle, ref r);
        }

        /// <summary>
        /// Select all rows on the given listview
        /// </summary>
        /// <param name="list">The listview whose items are to be selected</param>
        public static void SelectAllItems(ListView list) {
            SetItemState(list, -1, LVIS_SELECTED, LVIS_SELECTED);
        }

        /// <summary>
        /// Deselect all rows on the given listview
        /// </summary>
        /// <param name="list">The listview whose items are to be deselected</param>
        public static void DeselectAllItems(ListView list) {
            SetItemState(list, -1, LVIS_SELECTED, 0);
        }

        /// <summary>
        /// Deselect a single row
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        public static void DeselectOneItem(ListView list, int index) {
            SetItemState(list, index, LVIS_SELECTED, 0);
        }

        /// <summary>
        /// Set the item state on the given item
        /// </summary>
        /// <param name="list">The listview whose item's state is to be changed</param>
        /// <param name="itemIndex">The index of the item to be changed</param>
        /// <param name="mask">Which bits of the value are to be set?</param>
        /// <param name="value">The value to be set</param>
        public static void SetItemState(ListView list, int itemIndex, int mask, int value) {
            LVITEM lvItem = new LVITEM();
            lvItem.stateMask = mask;
            lvItem.state = value;
            SendMessageLVItem(list.Handle, LVM_SETITEMSTATE, itemIndex, ref lvItem);
        }

        /// <summary>
        /// Scroll the given listview by the given deltas
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns>true if the scroll succeeded</returns>
        public static bool Scroll(ListView list, int dx, int dy) {
            return SendMessage(list.Handle, LVM_SCROLL, dx, dy) != IntPtr.Zero;
        }

        /// <summary>
        /// Return the handle to the header control on the given list
        /// </summary>
        /// <param name="list">The listview whose header control is to be returned</param>
        /// <returns>The handle to the header control</returns>
        public static IntPtr GetHeaderControl(ListView list) {
            return SendMessage(list.Handle, LVM_GETHEADER, 0, 0);
        }

        /// <summary>
        /// Return the edges of the given column.
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="columnIndex"></param>
        /// <returns>A Point holding the left and right co-ords of the column.
        /// -1 means that the sides could not be retrieved.</returns>
        public static Point GetColumnSides(ObjectListView lv, int columnIndex) {
            Point sides = new Point(-1, -1);
            IntPtr hdr = GetHeaderControl(lv);
            if (hdr == IntPtr.Zero)
                return new Point(-1, -1);

            RECT r = new RECT();
            SendMessageRECT(hdr, HDM_GETITEMRECT, columnIndex, ref r);
            return new Point(r.left, r.right);
        }

        /// <summary>
        /// Return the edges of the given column.
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="columnIndex"></param>
        /// <returns>A Point holding the left and right co-ords of the column.
        /// -1 means that the sides could not be retrieved.</returns>
        public static Point GetScrolledColumnSides(ListView lv, int columnIndex) {
            IntPtr hdr = GetHeaderControl(lv);
            if (hdr == IntPtr.Zero)
                return new Point(-1, -1);

            RECT r = new RECT();
            IntPtr result = SendMessageRECT(hdr, HDM_GETITEMRECT, columnIndex, ref r);
            int scrollH = GetScrollPosition(lv, true);
            return new Point(r.left - scrollH, r.right - scrollH);
        }

        /// <summary>
        /// Return the index of the column of the header that is under the given point.
        /// Return -1 if no column is under the pt
        /// </summary>
        /// <param name="handle">The list we are interested in</param>
        /// <param name="pt">The client co-ords</param>
        /// <returns>The index of the column under the point, or -1 if no column header is under that point</returns>
        public static int GetColumnUnderPoint(IntPtr handle, Point pt) {
            const int HHT_ONHEADER = 2;
            const int HHT_ONDIVIDER = 4;
            return HeaderControlHitTest(handle, pt, HHT_ONHEADER | HHT_ONDIVIDER);
        }

        private static int HeaderControlHitTest(IntPtr handle, Point pt, int flag) {
            HDHITTESTINFO testInfo = new HDHITTESTINFO();
            testInfo.pt_x = pt.X;
            testInfo.pt_y = pt.Y;
            IntPtr result = SendMessageHDHITTESTINFO(handle, HDM_HITTEST, IntPtr.Zero, testInfo);
            if ((testInfo.flags & flag) != 0)
                return testInfo.iItem;
            else
                return -1;
        }

        /// <summary>
        /// Return the index of the divider under the given point. Return -1 if no divider is under the pt
        /// </summary>
        /// <param name="handle">The list we are interested in</param>
        /// <param name="pt">The client co-ords</param>
        /// <returns>The index of the divider under the point, or -1 if no divider is under that point</returns>
        public static int GetDividerUnderPoint(IntPtr handle, Point pt) {
            const int HHT_ONDIVIDER = 4;
            return HeaderControlHitTest(handle, pt, HHT_ONDIVIDER);
        }

        /// <summary>
        /// Get the scroll position of the given scroll bar
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="horizontalBar"></param>
        /// <returns></returns>
        public static int GetScrollPosition(ListView lv, bool horizontalBar) {
            int fnBar = (horizontalBar ? SB_HORZ : SB_VERT);

            SCROLLINFO scrollInfo = new SCROLLINFO();
            scrollInfo.fMask = SIF_POS;
            if (GetScrollInfo(lv.Handle, fnBar, scrollInfo))
                return scrollInfo.nPos;
            else
                return -1;
        }

        /// <summary>
        /// Change the z-order to the window 'toBeMoved' so it appear directly on top of 'reference'
        /// </summary>
        /// <param name="toBeMoved"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static bool ChangeZOrder(IWin32Window toBeMoved, IWin32Window reference) {
            return SetWindowPos(toBeMoved.Handle, reference.Handle, 0, 0, 0, 0, SWP_ZORDERONLY);
        }

        /// <summary>
        /// Make the given control/window a topmost window
        /// </summary>
        /// <param name="toBeMoved"></param>
        /// <returns></returns>
        public static bool MakeTopMost(IWin32Window toBeMoved) {
            IntPtr HWND_TOPMOST = (IntPtr)(-1);
            return SetWindowPos(toBeMoved.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_ZORDERONLY);
        }

        /// <summary>
        /// Change the size of the window without affecting any other attributes
        /// </summary>
        /// <param name="toBeMoved"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool ChangeSize(IWin32Window toBeMoved, int width, int height) {
            return SetWindowPos(toBeMoved.Handle, IntPtr.Zero, 0, 0, width, height, SWP_SIZEONLY);
        }

        /// <summary>
        /// Show the given window without activating it
        /// </summary>
        /// <param name="win">The window to show</param>
        static public void ShowWithoutActivate(IWin32Window win) {
            const int SW_SHOWNA = 8;
            ShowWindow(win.Handle, SW_SHOWNA);
        }

        /// <summary>
        /// Mark the given column as being selected.
        /// </summary>
        /// <param name="objectListView"></param>
        /// <param name="value">The OLVColumn or null to clear</param>
        /// <remarks>
        /// This method works, but it prevents subitems in the given column from having
        /// back colors. 
        /// </remarks>
        static public void SetSelectedColumn(ListView objectListView, ColumnHeader value) {
            SendMessage(objectListView.Handle,
                LVM_SETSELECTEDCOLUMN, (value == null) ? -1 : value.Index, 0);
        }

        static public int GetTopIndex(ListView lv) {
            return (int)SendMessage(lv.Handle, LVM_GETTOPINDEX, 0, 0);
        }

        static public IntPtr GetTooltipControl(ListView lv) {
            return SendMessage(lv.Handle, LVM_GETTOOLTIPS, 0, 0);
        }

        static public IntPtr SetTooltipControl(ListView lv, ToolTipControl tooltip) {
            return SendMessage(lv.Handle, LVM_SETTOOLTIPS, 0, tooltip.Handle);
        }

        static public bool HasHorizontalScrollBar(ListView lv) {
            const int GWL_STYLE = -16;
            const int WS_HSCROLL = 0x00100000;

            return (GetWindowLong(lv.Handle, GWL_STYLE) & WS_HSCROLL) != 0;
        }

        public static int GetWindowLong(IntPtr hWnd, int nIndex) {
            if (IntPtr.Size == 4)
                return (int)GetWindowLong32(hWnd, nIndex);
            else
                return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
        }

        public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong) {
            if (IntPtr.Size == 4)
                return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            else
                return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SetBkColor(IntPtr hDC, int clr);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SetTextColor(IntPtr hDC, int crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SetWindowTheme(IntPtr hWnd, string subApp, string subIdList);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool InvalidateRect(IntPtr hWnd, int ignored, bool erase);

        [StructLayout(LayoutKind.Sequential)]
        public struct LVITEMINDEX
        {
            public int iItem;
            public int iGroup;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        public static int GetGroupInfo(ObjectListView olv, int groupId, ref LVGROUP2 group) {
            return (int)SendMessage(olv.Handle, LVM_GETGROUPINFO, groupId, ref group);
        }

        public static GroupState GetGroupState(ObjectListView olv, int groupId, GroupState mask) {
            return (GroupState)SendMessage(olv.Handle, LVM_GETGROUPSTATE, groupId, (int)mask);
        }

        public static int InsertGroup(ObjectListView olv, LVGROUP2 group) {
            return (int)SendMessage(olv.Handle, LVM_INSERTGROUP, -1, ref group);
        }

        public static int SetGroupInfo(ObjectListView olv, int groupId, LVGROUP2 group) {
            return (int)SendMessage(olv.Handle, LVM_SETGROUPINFO, groupId, ref group);
        }

        public static int SetGroupMetrics(ObjectListView olv, LVGROUPMETRICS metrics) {
            return (int)SendMessage(olv.Handle, LVM_SETGROUPMETRICS, 0, ref metrics);
        }

        public static void ClearGroups(VirtualObjectListView virtualObjectListView) {
            SendMessage(virtualObjectListView.Handle, LVM_REMOVEALLGROUPS, 0, 0);
        }

        public static void SetGroupImageList(ObjectListView olv, ImageList il) {
            const int LVSIL_GROUPHEADER = 3;
            IntPtr handle = IntPtr.Zero;
            if (il != null)
                handle = il.Handle;
            SendMessage(olv.Handle, LVM_SETIMAGELIST, LVSIL_GROUPHEADER, handle);
        }

        public static int HitTest(ObjectListView olv, ref LVHITTESTINFO hittest)
        {
            return (int)SendMessage(olv.Handle, olv.View == View.Details ? LVM_SUBITEMHITTEST : LVM_HITTEST, -1, ref hittest);
        }
    }
}
