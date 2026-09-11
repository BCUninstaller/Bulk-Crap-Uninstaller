/*
 * GlassPanelForm - A transparent form that is placed over an ObjectListView
 * to allow flicker-free overlay images during scrolling.
 *
 * Author: Phillip Piper
 * Date: 14/04/2009 4:36 PM
 *
 * Change log:
 * 2010-08-18   JPP  - Added WS_EX_TOOLWINDOW style so that the form won't appear in Alt-Tab list.
 * v2.4
 * 2010-03-11   JPP  - Work correctly in MDI applications -- more or less. Actually, less than more.
 *                     They don't crash but they don't correctly handle overlapping MDI children.
 *                     Overlays from one control are shown on top of other other windows.
 * 2010-03-09   JPP  - Correctly Unbind() when the related ObjectListView is disposed.
 * 2009-10-28   JPP  - Use FindForm() rather than TopMostControl, since the latter doesn't work
 *                     as I expected when the OLV is part of an MDI child window. Thanks to
 *                     wvd_vegt who tracked this down.
 * v2.3
 * 2009-08-19   JPP  - Only hide the glass pane on resize, not on move
 *                   - Each glass panel now only draws one overlays
 * v2.2
 * 2009-06-05   JPP  - Handle when owning window is a topmost window
 * 2009-04-14   JPP  - Initial version
 *
 * To do:
 * 
 * Copyright (C) 2009-2014 Phillip Piper
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A GlassPanelForm sits transparently over an ObjectListView to show overlays.
    /// </summary>
    internal partial class GlassPanelForm : Form
    {
        public GlassPanelForm() {
            Name = "GlassPanelForm";
            Text = "GlassPanelForm";

            ClientSize = new Size(0, 0);
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;

            SetStyle(ControlStyles.Selectable, false);
            
            Opacity = 0.5f;
            BackColor = Color.FromArgb(255, 254, 254, 254);
            TransparencyKey = BackColor;
            HideGlass();
            NativeMethods.ShowWithoutActivate(this);
        }

        protected override void Dispose(bool disposing) {
            if (disposing)
                Unbind();

            base.Dispose(disposing);
        }

        #region Properties

        /// <summary>
        /// Get the low-level windows flag that will be given to CreateWindow.
        /// </summary>
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW 
                return cp;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Attach this form to the given ObjectListView
        /// </summary>        
        public void Bind(ObjectListView olv, IOverlay overlay) {
            if (objectListView != null)
                Unbind();

            objectListView = olv;
            Overlay = overlay;
            mdiClient = null;
            mdiOwner = null;

            if (objectListView == null)
                return;

            // NOTE: If you listen to any events here, you *must* stop listening in Unbind()

            objectListView.Disposed += new EventHandler(objectListView_Disposed);
            objectListView.LocationChanged += new EventHandler(objectListView_LocationChanged);
            objectListView.SizeChanged += new EventHandler(objectListView_SizeChanged);
            objectListView.VisibleChanged += new EventHandler(objectListView_VisibleChanged);
            objectListView.ParentChanged += new EventHandler(objectListView_ParentChanged);

            // Collect our ancestors in the widget hierachy
            if (ancestors == null)
                ancestors = new List<Control>();
            Control parent = objectListView.Parent;
            while (parent != null) {
                ancestors.Add(parent);
                parent = parent.Parent;
            } 

            // Listen for changes in the hierachy
            foreach (Control ancestor in ancestors) {
                ancestor.ParentChanged += new EventHandler(objectListView_ParentChanged);
                if (ancestor is TabControl tabControl) {
                    tabControl.Selected += new TabControlEventHandler(tabControl_Selected);
                }
            }

            // Listen for changes in our owning form
            Owner = objectListView.FindForm();
            myOwner = Owner;
            if (Owner != null) {
                Owner.LocationChanged += new EventHandler(Owner_LocationChanged);
                Owner.SizeChanged += new EventHandler(Owner_SizeChanged);
                Owner.ResizeBegin += new EventHandler(Owner_ResizeBegin);
                Owner.ResizeEnd += new EventHandler(Owner_ResizeEnd);
                if (Owner.TopMost) {
                    // We can't do this.TopMost = true; since that will activate the panel,
                    // taking focus away from the owner of the listview
                    NativeMethods.MakeTopMost(this);
                }

                // We need special code to handle MDI
                mdiOwner = Owner.MdiParent;
                if (mdiOwner != null) {
                    mdiOwner.LocationChanged += new EventHandler(Owner_LocationChanged);
                    mdiOwner.SizeChanged += new EventHandler(Owner_SizeChanged);
                    mdiOwner.ResizeBegin += new EventHandler(Owner_ResizeBegin);
                    mdiOwner.ResizeEnd += new EventHandler(Owner_ResizeEnd);

                    // Find the MDIClient control, which houses all MDI children
                    foreach (Control c in mdiOwner.Controls) {
                        mdiClient = c as MdiClient;
                        if (mdiClient != null) {
                            break;
                        }
                    }
                    if (mdiClient != null) {
                        mdiClient.ClientSizeChanged += new EventHandler(myMdiClient_ClientSizeChanged);
                    }
                }
            }

            UpdateTransparency();
        }

        void myMdiClient_ClientSizeChanged(object sender, EventArgs e) {
            RecalculateBounds();
            Invalidate();
        }

        /// <summary>
        /// Made the overlay panel invisible
        /// </summary>
        public void HideGlass() {
            if (!isGlassShown)
                return;
            isGlassShown = false;
            Bounds = new Rectangle(-10000, -10000, 1, 1);
        }

        /// <summary>
        /// Show the overlay panel in its correctly location
        /// </summary>
        /// <remarks>
        /// If the panel is always shown, this method does nothing.
        /// If the panel is being resized, this method also does nothing.
        /// </remarks>
        public void ShowGlass() {
            if (isGlassShown || isDuringResizeSequence)
                return;

            isGlassShown = true;
            RecalculateBounds();
        }

        /// <summary>
        /// Detach this glass panel from its previous ObjectListView
        /// </summary>        
        /// <remarks>
        /// You should unbind the overlay panel before making any changes to the 
        /// widget hierarchy.
        /// </remarks>
        public void Unbind() {
            if (objectListView != null) {
                objectListView.Disposed -= new EventHandler(objectListView_Disposed);
                objectListView.LocationChanged -= new EventHandler(objectListView_LocationChanged);
                objectListView.SizeChanged -= new EventHandler(objectListView_SizeChanged);
                objectListView.VisibleChanged -= new EventHandler(objectListView_VisibleChanged);
                objectListView.ParentChanged -= new EventHandler(objectListView_ParentChanged);
                objectListView = null;
            }

            if (ancestors != null) {
                foreach (Control parent in ancestors) {
                    parent.ParentChanged -= new EventHandler(objectListView_ParentChanged);
                    if (parent is TabControl tabControl) {
                        tabControl.Selected -= new TabControlEventHandler(tabControl_Selected);
                    }
                }
                ancestors = null;
            }

            if (myOwner != null) {
                myOwner.LocationChanged -= new EventHandler(Owner_LocationChanged);
                myOwner.SizeChanged -= new EventHandler(Owner_SizeChanged);
                myOwner.ResizeBegin -= new EventHandler(Owner_ResizeBegin);
                myOwner.ResizeEnd -= new EventHandler(Owner_ResizeEnd);
                myOwner = null;
            }

            if (mdiOwner != null) {
                mdiOwner.LocationChanged -= new EventHandler(Owner_LocationChanged);
                mdiOwner.SizeChanged -= new EventHandler(Owner_SizeChanged);
                mdiOwner.ResizeBegin -= new EventHandler(Owner_ResizeBegin);
                mdiOwner.ResizeEnd -= new EventHandler(Owner_ResizeEnd);
                mdiOwner = null;
            }

            if (mdiClient != null) {
                mdiClient.ClientSizeChanged -= new EventHandler(myMdiClient_ClientSizeChanged);
                mdiClient = null;
            }
        }

        #endregion

        #region Event Handlers

        void objectListView_Disposed(object sender, EventArgs e) {
            Unbind();
        }

        /// <summary>
        /// Handle when the form that owns the ObjectListView begins to be resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Owner_ResizeBegin(object sender, EventArgs e) {
            // When the top level window is being resized, we just want to hide
            // the overlay window. When the resizing finishes, we want to show
            // the overlay window, if it was shown before the resize started.
            isDuringResizeSequence = true;
            wasGlassShownBeforeResize = isGlassShown;
        }

        /// <summary>
        /// Handle when the form that owns the ObjectListView finished to be resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Owner_ResizeEnd(object sender, EventArgs e) {
            isDuringResizeSequence = false;
            if (wasGlassShownBeforeResize)
                ShowGlass();
        }

        /// <summary>
        /// The owning form has moved. Move the overlay panel too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Owner_LocationChanged(object sender, EventArgs e) {
            if (mdiOwner != null)
                HideGlass();
            else
                RecalculateBounds();
        }

        /// <summary>
        /// The owning form is resizing. Hide our overlay panel until the resizing stops
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Owner_SizeChanged(object sender, EventArgs e) {
            HideGlass();
        }


        /// <summary>
        /// Handle when the bound OLV changes its location. The overlay panel must 
        /// be moved too, IFF it is currently visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void objectListView_LocationChanged(object sender, EventArgs e) {
            if (isGlassShown) {
                RecalculateBounds();
            }
        }

        /// <summary>
        /// Handle when the bound OLV changes size. The overlay panel must 
        /// resize too, IFF it is currently visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void objectListView_SizeChanged(object sender, EventArgs e) {
            // This event is triggered in all sorts of places, and not always when the size changes.
            //if (this.isGlassShown) {
            //    this.Size = this.objectListView.ClientSize;
            //}
        }

        /// <summary>
        /// Handle when the bound OLV is part of a TabControl and that
        /// TabControl changes tabs. The overlay panel is hidden. The
        /// first time the bound OLV is redrawn, the overlay panel will
        /// be shown again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabControl_Selected(object sender, TabControlEventArgs e) {
            HideGlass();
        }

        /// <summary>
        /// Somewhere the parent of the bound OLV has changed. Update
        /// our events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void objectListView_ParentChanged(object sender, EventArgs e) {
            ObjectListView olv = objectListView;
            IOverlay overlay = Overlay;
            Unbind();
            Bind(olv, overlay);
        }

        /// <summary>
        /// Handle when the bound OLV changes its visibility.
        /// The overlay panel should match the OLV's visibility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void objectListView_VisibleChanged(object sender, EventArgs e) {
            if (objectListView.Visible)
                ShowGlass();
            else
                HideGlass();
        }

        #endregion

        #region Implementation

        protected override void OnPaint(PaintEventArgs e) {
            if (objectListView == null || Overlay == null)
                return;

            Graphics g = e.Graphics;
            g.TextRenderingHint = ObjectListView.TextRenderingHint;
            g.SmoothingMode = ObjectListView.SmoothingMode;
            //g.DrawRectangle(new Pen(Color.Green, 4.0f), this.ClientRectangle);

            // If we are part of an MDI app, make sure we don't draw outside the bounds
            if (mdiClient != null) {
                Rectangle r = mdiClient.RectangleToScreen(mdiClient.ClientRectangle);
                Rectangle r2 = objectListView.RectangleToClient(r);
                g.SetClip(r2, System.Drawing.Drawing2D.CombineMode.Intersect);
            }

            Overlay.Draw(objectListView, g, objectListView.ClientRectangle);
        }

        protected void RecalculateBounds() {
            if (!isGlassShown)
                return;

            Rectangle rect = objectListView.ClientRectangle;
            rect.X = 0;
            rect.Y = 0;
            Bounds = objectListView.RectangleToScreen(rect);
        }

        internal void UpdateTransparency() {
            if (!(Overlay is ITransparentOverlay transparentOverlay))
                Opacity = objectListView.OverlayTransparency / 255.0f;
            else
                Opacity = transparentOverlay.Transparency / 255.0f;
        }

        protected override void WndProc(ref Message m) {
            const int WM_NCHITTEST = 132;
            const int HTTRANSPARENT = -1;
            switch (m.Msg) {
                // Ignore all mouse interactions
                case WM_NCHITTEST:
                    m.Result = (IntPtr)HTTRANSPARENT;
                    break;
            }
            base.WndProc(ref m);
        }

        #endregion
        
        #region Implementation variables

        internal IOverlay Overlay;

        #endregion

        #region Private variables

        private ObjectListView objectListView;
        private bool isDuringResizeSequence;
        private bool isGlassShown;
        private bool wasGlassShownBeforeResize;

        // Cache these so we can unsubscribe from events even when the OLV has been disposed.
        private Form myOwner;
        private Form mdiOwner;
        private List<Control> ancestors;
        MdiClient mdiClient;

        #endregion

    }
}
