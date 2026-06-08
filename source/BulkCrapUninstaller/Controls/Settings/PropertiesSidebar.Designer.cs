using System.ComponentModel;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.Tracking;
using Klocman.Controls;

namespace BulkCrapUninstaller.Controls
{
    partial class PropertiesSidebar
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PropertiesSidebar));
            checkBoxOrphans = new CheckBox();
            checkBoxInvalidTest = new CheckBox();
            checkBoxCertTest = new CheckBox();
            groupBox2 = new GroupBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            checkBoxListHideMicrosoft = new CheckBox();
            checkBoxListSysComp = new CheckBox();
            checkBoxListProtected = new CheckBox();
            checkBoxTweaks = new CheckBox();
            checkBoxShowUpdates = new CheckBox();
            checkBoxWinFeature = new CheckBox();
            checkBoxShowStoreApps = new CheckBox();
            groupBox1 = new GroupBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            checkBoxViewCheckboxes = new CheckBox();
            checkBoxViewGroups = new CheckBox();
            checkBoxHighlightSpecial = new CheckBox();
            toolTip1 = new ToolTip(components);
            usageTracker1 = new UsageTracker();
            groupBox2.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            groupBox1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // checkBoxOrphans
            // 
            resources.ApplyResources(checkBoxOrphans, "checkBoxOrphans");
            checkBoxOrphans.Name = "checkBoxOrphans";
            toolTip1.SetToolTip(checkBoxOrphans, resources.GetString("checkBoxOrphans.ToolTip"));
            checkBoxOrphans.UseVisualStyleBackColor = true;
            // 
            // checkBoxInvalidTest
            // 
            resources.ApplyResources(checkBoxInvalidTest, "checkBoxInvalidTest");
            checkBoxInvalidTest.Name = "checkBoxInvalidTest";
            toolTip1.SetToolTip(checkBoxInvalidTest, resources.GetString("checkBoxInvalidTest.ToolTip"));
            checkBoxInvalidTest.UseVisualStyleBackColor = true;
            // 
            // checkBoxCertTest
            // 
            resources.ApplyResources(checkBoxCertTest, "checkBoxCertTest");
            checkBoxCertTest.Name = "checkBoxCertTest";
            toolTip1.SetToolTip(checkBoxCertTest, resources.GetString("checkBoxCertTest.ToolTip"));
            checkBoxCertTest.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(flowLayoutPanel2);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(flowLayoutPanel2, "flowLayoutPanel2");
            flowLayoutPanel2.Controls.Add(checkBoxListHideMicrosoft);
            flowLayoutPanel2.Controls.Add(checkBoxOrphans);
            flowLayoutPanel2.Controls.Add(checkBoxListSysComp);
            flowLayoutPanel2.Controls.Add(checkBoxListProtected);
            flowLayoutPanel2.Controls.Add(checkBoxTweaks);
            flowLayoutPanel2.Controls.Add(checkBoxShowUpdates);
            flowLayoutPanel2.Controls.Add(checkBoxWinFeature);
            flowLayoutPanel2.Controls.Add(checkBoxShowStoreApps);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // checkBoxListHideMicrosoft
            // 
            resources.ApplyResources(checkBoxListHideMicrosoft, "checkBoxListHideMicrosoft");
            checkBoxListHideMicrosoft.Name = "checkBoxListHideMicrosoft";
            toolTip1.SetToolTip(checkBoxListHideMicrosoft, resources.GetString("checkBoxListHideMicrosoft.ToolTip"));
            checkBoxListHideMicrosoft.UseVisualStyleBackColor = true;
            // 
            // checkBoxListSysComp
            // 
            resources.ApplyResources(checkBoxListSysComp, "checkBoxListSysComp");
            checkBoxListSysComp.Name = "checkBoxListSysComp";
            toolTip1.SetToolTip(checkBoxListSysComp, resources.GetString("checkBoxListSysComp.ToolTip"));
            checkBoxListSysComp.UseVisualStyleBackColor = true;
            // 
            // checkBoxListProtected
            // 
            resources.ApplyResources(checkBoxListProtected, "checkBoxListProtected");
            checkBoxListProtected.Name = "checkBoxListProtected";
            toolTip1.SetToolTip(checkBoxListProtected, resources.GetString("checkBoxListProtected.ToolTip"));
            checkBoxListProtected.UseVisualStyleBackColor = true;
            // 
            // checkBoxTweaks
            // 
            resources.ApplyResources(checkBoxTweaks, "checkBoxTweaks");
            checkBoxTweaks.Name = "checkBoxTweaks";
            checkBoxTweaks.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowUpdates
            // 
            resources.ApplyResources(checkBoxShowUpdates, "checkBoxShowUpdates");
            checkBoxShowUpdates.Name = "checkBoxShowUpdates";
            toolTip1.SetToolTip(checkBoxShowUpdates, resources.GetString("checkBoxShowUpdates.ToolTip"));
            checkBoxShowUpdates.UseVisualStyleBackColor = true;
            // 
            // checkBoxWinFeature
            // 
            resources.ApplyResources(checkBoxWinFeature, "checkBoxWinFeature");
            checkBoxWinFeature.Name = "checkBoxWinFeature";
            checkBoxWinFeature.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowStoreApps
            // 
            resources.ApplyResources(checkBoxShowStoreApps, "checkBoxShowStoreApps");
            checkBoxShowStoreApps.Name = "checkBoxShowStoreApps";
            checkBoxShowStoreApps.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(flowLayoutPanel1);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(checkBoxViewCheckboxes);
            flowLayoutPanel1.Controls.Add(checkBoxViewGroups);
            flowLayoutPanel1.Controls.Add(checkBoxCertTest);
            flowLayoutPanel1.Controls.Add(checkBoxInvalidTest);
            flowLayoutPanel1.Controls.Add(checkBoxHighlightSpecial);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // checkBoxViewCheckboxes
            // 
            resources.ApplyResources(checkBoxViewCheckboxes, "checkBoxViewCheckboxes");
            checkBoxViewCheckboxes.Name = "checkBoxViewCheckboxes";
            toolTip1.SetToolTip(checkBoxViewCheckboxes, resources.GetString("checkBoxViewCheckboxes.ToolTip"));
            checkBoxViewCheckboxes.UseVisualStyleBackColor = true;
            // 
            // checkBoxViewGroups
            // 
            resources.ApplyResources(checkBoxViewGroups, "checkBoxViewGroups");
            checkBoxViewGroups.Name = "checkBoxViewGroups";
            toolTip1.SetToolTip(checkBoxViewGroups, resources.GetString("checkBoxViewGroups.ToolTip"));
            checkBoxViewGroups.UseVisualStyleBackColor = true;
            // 
            // checkBoxHighlightSpecial
            // 
            resources.ApplyResources(checkBoxHighlightSpecial, "checkBoxHighlightSpecial");
            checkBoxHighlightSpecial.Name = "checkBoxHighlightSpecial";
            toolTip1.SetToolTip(checkBoxHighlightSpecial, resources.GetString("checkBoxHighlightSpecial.ToolTip"));
            checkBoxHighlightSpecial.UseVisualStyleBackColor = true;
            // 
            // usageTracker1
            // 
            usageTracker1.ContainerControl = this;
            // 
            // PropertiesSidebar
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Name = "PropertiesSidebar";
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private GroupBox groupBox2;
        private FlowLayoutPanel flowLayoutPanel2;
        private CheckBox checkBoxListHideMicrosoft;
        private CheckBox checkBoxShowUpdates;
        private CheckBox checkBoxListSysComp;
        private CheckBox checkBoxListProtected;
        private GroupBox groupBox1;
        private FlowLayoutPanel flowLayoutPanel1;
        private CheckBox checkBoxViewCheckboxes;
        private CheckBox checkBoxViewGroups;
        private ToolTip toolTip1;
        private UsageTracker usageTracker1;
        private CheckBox checkBoxCertTest;
        private CheckBox checkBoxInvalidTest;
        private CheckBox checkBoxOrphans;
        private CheckBox checkBoxShowStoreApps;
        private CheckBox checkBoxWinFeature;
        private CheckBox checkBoxHighlightSpecial;
        private CheckBox checkBoxTweaks;
    }
}
