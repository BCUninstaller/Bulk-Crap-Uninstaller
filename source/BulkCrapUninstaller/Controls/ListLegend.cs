/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.ApplicationList;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Controls
{
    [WindowStyleController.ControlStyle(false)]
    public partial class ListLegend : UserControl
    {
        private readonly Label _contrastNotice = new() { Dock = DockStyle.Top, Padding = new Padding(4), Visible = false };
        private readonly string _colorLegendTitle;

        public ListLegend()
        {
            InitializeComponent();
            _colorLegendTitle = labelLegend.Text;
            _contrastNotice.Text = new ComponentResourceManager(typeof(ListLegend)).GetString("ContrastNotice");
            _contrastNotice.MouseDown += OnMouseDown;
            Controls.Add(_contrastNotice);
            Controls.SetChildIndex(labelLegend, Controls.Count - 1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Properties.Settings.Default.SettingBinder.Subscribe((x, y) => UpdateColors(), settings => settings.MiscColorblind, this);
            UpdateColors();
        }

        private void UpdateColors()
        {
            if (flowLayoutPanellabelInvalid == null || _colorLegendTitle == null) return;
            var contrast = SystemInformation.HighContrast;
            Color Surface(Color color) => contrast ? SystemColors.Window : color;
            flowLayoutPanellabelInvalid.BackColor = Surface(ApplicationListConstants.Colors.InvalidColor);
            flowLayoutPanellabelOrphaned.BackColor = Surface(ApplicationListConstants.Colors.UnregisteredColor);
            flowLayoutPanellabelUnverified.BackColor = Surface(ApplicationListConstants.Colors.UnverifiedColor);
            flowLayoutPanellabelVerified.BackColor = Surface(ApplicationListConstants.Colors.VerifiedColor);
            flowLayoutPanellabelWinFeature.BackColor = Surface(ApplicationListConstants.Colors.WindowsFeatureColor);
            flowLayoutPanellabelStoreApp.BackColor = Surface(ApplicationListConstants.Colors.WindowsStoreAppColor);
            ForeColor = SystemColors.WindowText;
            labelLegend.ForeColor = SystemColors.ControlText;
            labelLegend.Text = contrast ? new ComponentResourceManager(typeof(ListLegend)).GetString("ContrastTitle") : _colorLegendTitle;
            _contrastNotice.BackColor = SystemColors.Window;
            _contrastNotice.Visible = contrast;
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            UpdateColors();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            if (_contrastNotice?.Parent == this && _contrastNotice.Visible)
                _contrastNotice.Height = _contrastNotice.GetPreferredSize(new Size(ClientSize.Width, 0)).Height;
            base.OnLayout(e);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InvalidEnabled
        {
            get { return flowLayoutPanellabelInvalid.Visible; }
            set { flowLayoutPanellabelInvalid.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WinFeatureEnabled
        {
            get { return flowLayoutPanellabelWinFeature.Visible; }
            set { flowLayoutPanellabelWinFeature.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CertificatesEnabled
        {
            get { return flowLayoutPanellabelVerified.Visible; }
            set { flowLayoutPanellabelVerified.Visible = value; flowLayoutPanellabelUnverified.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool OrphanedEnabled
        {
            get { return flowLayoutPanellabelOrphaned.Visible; }
            set { flowLayoutPanellabelOrphaned.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool StoreAppEnabled
        {
            get { return flowLayoutPanellabelStoreApp.Visible; }
            set { flowLayoutPanellabelStoreApp.Visible = value; }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            CloseRequested?.Invoke(sender, e);
        }

        private void ThisEnabledChanged(object sender, EventArgs e)
        {
            BackColor = Enabled ? SystemColors.ControlLightLight : SystemColors.Control;
        }

        public event EventHandler CloseRequested;
    }
}
