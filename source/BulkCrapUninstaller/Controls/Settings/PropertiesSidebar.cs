/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Klocman.Binding.Settings;

namespace BulkCrapUninstaller.Controls
{
    public partial class PropertiesSidebar : UserControl
    {
        private readonly SettingBinder<Properties.Settings> _settings = Properties.Settings.Default.SettingBinder;

        public PropertiesSidebar()
        {
            InitializeComponent();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode) return;

            _settings.BindControl(checkBoxViewCheckboxes, x => x.UninstallerListUseCheckboxes, this);
            _settings.BindControl(checkBoxViewGroups, x => x.UninstallerListUseGroups, this);

            _settings.BindControl(checkBoxListHideMicrosoft, x => x.FilterHideMicrosoft, this);
            _settings.BindControl(checkBoxShowUpdates, x => x.FilterShowUpdates, this);
            _settings.BindControl(checkBoxListSysComp, x => x.FilterShowSystemComponents, this);
            _settings.BindControl(checkBoxListProtected, x => x.FilterShowProtected, this);
            _settings.BindControl(checkBoxShowStoreApps, x => x.FilterShowStoreApps, this);
            _settings.BindControl(checkBoxWinFeature, x => x.FilterShowWinFeatures, this);
            _settings.BindControl(checkBoxTweaks, x => x.FilterShowTweaks, this);

            _settings.BindControl(checkBoxInvalidTest, x => x.AdvancedTestInvalid, this);
            _settings.BindControl(checkBoxCertTest, x => x.AdvancedTestCertificates, this);
            _settings.BindControl(checkBoxOrphans, x => x.AdvancedDisplayOrphans, this);
            _settings.BindControl(checkBoxHighlightSpecial, x => x.AdvancedHighlightSpecial, this);

            _settings.SendUpdates(this);
            Disposed += (x, y) => _settings.RemoveHandlers(this);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetSuggestedWidth()
        {
            var maxWidth = typeof(PropertiesSidebar)
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.FieldType == typeof(CheckBox))
                .Select(x => x.GetValue(this))
                .Cast<CheckBox>()
                .Max(c => c.Width);

            return maxWidth + (groupBox1.Width - groupBox1.DisplayRectangle.Width) + Padding.Left + Padding.Right;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SysCompEnabled
        {
            get { return checkBoxListSysComp.Enabled; }
            set { checkBoxListSysComp.Enabled = value; checkBoxListSysComp.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ProtectedEnabled
        {
            get { return checkBoxListProtected.Enabled; }
            set { checkBoxListProtected.Enabled = value; checkBoxListProtected.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UpdatesEnabled
        {
            get { return checkBoxShowUpdates.Enabled; }
            set { checkBoxShowUpdates.Enabled = value; checkBoxShowUpdates.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool OrphansEnabled
        {
            get { return checkBoxOrphans.Enabled; }
            set { checkBoxOrphans.Enabled = value; checkBoxOrphans.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool StoreAppsEnabled
        {
            get { return checkBoxShowStoreApps.Enabled; }
            set { checkBoxShowStoreApps.Enabled = value; checkBoxShowStoreApps.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InvalidEnabled
        {
            get { return checkBoxInvalidTest.Enabled; }
            set { checkBoxInvalidTest.Enabled = value; checkBoxInvalidTest.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WinFeaturesEnabled
        {
            get { return checkBoxWinFeature.Enabled; }
            set { checkBoxWinFeature.Enabled = value; checkBoxWinFeature.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowTweaksEnabled
        {
            get { return checkBoxTweaks.Enabled; }
            set { checkBoxTweaks.Enabled = value; checkBoxTweaks.Visible = value; }
        }
    }
}