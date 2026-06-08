/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Controls;
using Klocman.Extensions;

namespace BulkCrapUninstaller.Forms
{
    public partial class ListLegendWindow : Form
    {
        private const double OpacityChangeAmount = .12;

        public ListLegendWindow()
        {
            InitializeComponent();

            listLegend1.CloseRequested += (sender, args) =>
            {
                Visible = false;
                Owner.Focus();
            };

            foreach (var control in this.GetAllChildren())
            {
                control.MouseLeave += ControlOnMouseEvent;
                control.MouseEnter += ControlOnMouseEvent;
            }
        }

        public void UpdatePosition(Control owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (!owner.Visible || owner.IsDisposed || owner.Disposing) return;

            var local = new Point(owner.Width - Width - 30, owner.Height - Height - 30);
            var global = owner.PointToScreen(local);
            Location = global;
        }

        public ListLegend ListLegend => listLegend1;

        private void ListLegendWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (Opacity < .9)
                opacityResetTimer.Start();
        }

        private void ListLegendWindow_EnabledChanged(object sender, EventArgs e)
        {
            if (Opacity < .9)
                opacityResetTimer.Start();
        }

        private void opacityResetTimer_Tick(object sender, EventArgs e)
        {
            if (CheckMouseHover())
            {
                if (Math.Abs(Opacity - .3) < .03)
                    opacityResetTimer.Stop();
                else
                    try { Opacity = OpacityLerp(.3); } catch (Win32Exception) { }
            }
            else
            {
                if (Math.Abs(Opacity - 1) < .03)
                    opacityResetTimer.Stop();
                else
                    try { Opacity = OpacityLerp(1); } catch (Win32Exception) { }
            }
        }

        private double OpacityLerp(double target)
        {
            return Opacity > target
                ? Math.Max(target, Opacity - OpacityChangeAmount)
                : Math.Min(target, Opacity + OpacityChangeAmount);
        }

        private void ControlOnMouseEvent(object sender, EventArgs eventArgs)
        {
            opacityResetTimer.Start();
        }

        private bool CheckMouseHover()
        {
            var pt = PointToClient(Cursor.Position);
            return ClientRectangle.Contains(pt);
        }
    }
}
