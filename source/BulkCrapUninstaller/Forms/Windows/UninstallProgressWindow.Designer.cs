using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions.Tracking;

namespace BulkCrapUninstaller.Forms
{
    partial class UninstallProgressWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(UninstallProgressWindow));
            progressBar1 = new ProgressBar();
            label1 = new Label();
            buttonClose = new Button();
            olvColumnStatus = new OLVColumn();
            olvColumnIsSilent = new OLVColumn();
            olvColumnName = new OLVColumn();
            objectListView1 = new ObjectListView();
            olvColumnId = new OLVColumn();
            contextMenuStrip1 = new ContextMenuStrip(components);
            skipToolStripMenuItem = new ToolStripMenuItem();
            terminateToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            runNowToolStripMenuItem = new ToolStripMenuItem();
            manualUninstallToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            openInstallDirectoryToolStripMenuItem = new ToolStripMenuItem();
            propertiesToolStripMenuItem = new ToolStripMenuItem();
            label3 = new Label();
            groupBox1 = new GroupBox();
            toolStrip1 = new ToolStrip();
            toolStripButtonRun = new ToolStripButton();
            toolStripButtonSkip = new ToolStripButton();
            toolStripButtonTerminate = new ToolStripButton();
            toolStripButtonManualUninstall = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripButtonFolderOpen = new ToolStripButton();
            toolStripButtonProperties = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButtonSettings = new ToolStripButton();
            toolStripButtonHelp = new ToolStripButton();
            label2 = new Label();
            usageTracker1 = new UsageTracker();
            panel3 = new Panel();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            forceUpdateTimer = new Timer(components);
            flowLayoutPanel2 = new FlowLayoutPanel();
            checkBoxFinishSleep = new CheckBox();
            sleepTimer = new Timer(components);
            ((ISupportInitialize)objectListView1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            toolStrip1.SuspendLayout();
            panel3.SuspendLayout();
            panel1.SuspendLayout();
            ((ISupportInitialize)pictureBox1).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // label1
            // 
            label1.AutoEllipsis = true;
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // buttonClose
            // 
            resources.ApplyResources(buttonClose, "buttonClose");
            buttonClose.DialogResult = DialogResult.Cancel;
            buttonClose.Name = "buttonClose";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // olvColumnStatus
            // 
            resources.ApplyResources(olvColumnStatus, "olvColumnStatus");
            // 
            // olvColumnIsSilent
            // 
            resources.ApplyResources(olvColumnIsSilent, "olvColumnIsSilent");
            // 
            // olvColumnName
            // 
            olvColumnName.FillsFreeSpace = true;
            resources.ApplyResources(olvColumnName, "olvColumnName");
            // 
            // objectListView1
            // 
            objectListView1.AllColumns.Add(olvColumnId);
            objectListView1.AllColumns.Add(olvColumnStatus);
            objectListView1.AllColumns.Add(olvColumnIsSilent);
            objectListView1.AllColumns.Add(olvColumnName);
            objectListView1.CellEditUseWholeCell = false;
            objectListView1.Columns.AddRange(new ColumnHeader[] { olvColumnId, olvColumnStatus, olvColumnIsSilent, olvColumnName });
            objectListView1.ContextMenuStrip = contextMenuStrip1;
            resources.ApplyResources(objectListView1, "objectListView1");
            objectListView1.FullRowSelect = true;
            objectListView1.MultiSelect = false;
            objectListView1.Name = "objectListView1";
            objectListView1.ShowItemToolTips = true;
            objectListView1.UseCompatibleStateImageBehavior = false;
            objectListView1.View = View.Details;
            objectListView1.SelectedIndexChanged += objectListView1_SelectedIndexChanged;
            // 
            // olvColumnId
            // 
            resources.ApplyResources(olvColumnId, "olvColumnId");
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { skipToolStripMenuItem, terminateToolStripMenuItem, toolStripSeparator3, runNowToolStripMenuItem, manualUninstallToolStripMenuItem, toolStripSeparator4, openInstallDirectoryToolStripMenuItem, propertiesToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(contextMenuStrip1, "contextMenuStrip1");
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // skipToolStripMenuItem
            // 
            skipToolStripMenuItem.Name = "skipToolStripMenuItem";
            resources.ApplyResources(skipToolStripMenuItem, "skipToolStripMenuItem");
            skipToolStripMenuItem.Click += toolStripButtonSkip_Click;
            // 
            // terminateToolStripMenuItem
            // 
            terminateToolStripMenuItem.Name = "terminateToolStripMenuItem";
            resources.ApplyResources(terminateToolStripMenuItem, "terminateToolStripMenuItem");
            terminateToolStripMenuItem.Click += toolStripButtonTerminate_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            // 
            // runNowToolStripMenuItem
            // 
            runNowToolStripMenuItem.Name = "runNowToolStripMenuItem";
            resources.ApplyResources(runNowToolStripMenuItem, "runNowToolStripMenuItem");
            runNowToolStripMenuItem.Click += toolStripButtonRun_Click;
            // 
            // manualUninstallToolStripMenuItem
            // 
            manualUninstallToolStripMenuItem.Name = "manualUninstallToolStripMenuItem";
            resources.ApplyResources(manualUninstallToolStripMenuItem, "manualUninstallToolStripMenuItem");
            manualUninstallToolStripMenuItem.Click += toolStripButtonManualUninstall_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            // 
            // openInstallDirectoryToolStripMenuItem
            // 
            openInstallDirectoryToolStripMenuItem.Name = "openInstallDirectoryToolStripMenuItem";
            resources.ApplyResources(openInstallDirectoryToolStripMenuItem, "openInstallDirectoryToolStripMenuItem");
            openInstallDirectoryToolStripMenuItem.Click += toolStripButtonFolderOpen_Click;
            // 
            // propertiesToolStripMenuItem
            // 
            propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            resources.ApplyResources(propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
            propertiesToolStripMenuItem.Click += toolStripButtonProperties_Click;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(objectListView1);
            groupBox1.Controls.Add(toolStrip1);
            groupBox1.Controls.Add(label2);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonRun, toolStripButtonSkip, toolStripButtonTerminate, toolStripButtonManualUninstall, toolStripSeparator2, toolStripButtonFolderOpen, toolStripButtonProperties, toolStripSeparator1, toolStripButtonSettings, toolStripButtonHelp });
            resources.ApplyResources(toolStrip1, "toolStrip1");
            toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonRun
            // 
            resources.ApplyResources(toolStripButtonRun, "toolStripButtonRun");
            toolStripButtonRun.Image = Properties.Resources.layerdown;
            toolStripButtonRun.Name = "toolStripButtonRun";
            toolStripButtonRun.Click += toolStripButtonRun_Click;
            // 
            // toolStripButtonSkip
            // 
            resources.ApplyResources(toolStripButtonSkip, "toolStripButtonSkip");
            toolStripButtonSkip.Image = Properties.Resources.control_fastforward_variant;
            toolStripButtonSkip.Name = "toolStripButtonSkip";
            toolStripButtonSkip.Click += toolStripButtonSkip_Click;
            // 
            // toolStripButtonTerminate
            // 
            resources.ApplyResources(toolStripButtonTerminate, "toolStripButtonTerminate");
            toolStripButtonTerminate.Image = Properties.Resources.stop;
            toolStripButtonTerminate.Name = "toolStripButtonTerminate";
            toolStripButtonTerminate.Click += toolStripButtonTerminate_Click;
            // 
            // toolStripButtonManualUninstall
            // 
            toolStripButtonManualUninstall.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonManualUninstall.Image = Properties.Resources.list;
            resources.ApplyResources(toolStripButtonManualUninstall, "toolStripButtonManualUninstall");
            toolStripButtonManualUninstall.Name = "toolStripButtonManualUninstall";
            toolStripButtonManualUninstall.Click += toolStripButtonManualUninstall_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonFolderOpen
            // 
            toolStripButtonFolderOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(toolStripButtonFolderOpen, "toolStripButtonFolderOpen");
            toolStripButtonFolderOpen.Image = Properties.Resources.fullscreen;
            toolStripButtonFolderOpen.Name = "toolStripButtonFolderOpen";
            toolStripButtonFolderOpen.Click += toolStripButtonFolderOpen_Click;
            // 
            // toolStripButtonProperties
            // 
            toolStripButtonProperties.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(toolStripButtonProperties, "toolStripButtonProperties");
            toolStripButtonProperties.Image = Properties.Resources.properties;
            toolStripButtonProperties.Name = "toolStripButtonProperties";
            toolStripButtonProperties.Click += toolStripButtonProperties_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonSettings
            // 
            toolStripButtonSettings.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonSettings.Image = Properties.Resources.settings;
            resources.ApplyResources(toolStripButtonSettings, "toolStripButtonSettings");
            toolStripButtonSettings.Name = "toolStripButtonSettings";
            toolStripButtonSettings.Click += toolStripButtonSettings_Click;
            // 
            // toolStripButtonHelp
            // 
            toolStripButtonHelp.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonHelp.Image = Properties.Resources.information_circle;
            resources.ApplyResources(toolStripButtonHelp, "toolStripButtonHelp");
            toolStripButtonHelp.Name = "toolStripButtonHelp";
            toolStripButtonHelp.Click += toolStripButtonHelp_Click;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // usageTracker1
            // 
            usageTracker1.ContainerControl = this;
            // 
            // panel3
            // 
            panel3.Controls.Add(label1);
            panel3.Controls.Add(panel1);
            resources.ApplyResources(panel3, "panel3");
            panel3.Name = "panel3";
            // 
            // panel1
            // 
            panel1.Controls.Add(progressBar1);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.layerdelete;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(label3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // forceUpdateTimer
            // 
            forceUpdateTimer.Interval = 3500;
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(flowLayoutPanel2, "flowLayoutPanel2");
            flowLayoutPanel2.Controls.Add(buttonClose);
            flowLayoutPanel2.Controls.Add(checkBoxFinishSleep);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // checkBoxFinishSleep
            // 
            resources.ApplyResources(checkBoxFinishSleep, "checkBoxFinishSleep");
            checkBoxFinishSleep.Name = "checkBoxFinishSleep";
            checkBoxFinishSleep.UseVisualStyleBackColor = true;
            // 
            // UninstallProgressWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonClose;
            Controls.Add(groupBox1);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(pictureBox1);
            Controls.Add(panel3);
            Controls.Add(flowLayoutPanel1);
            Name = "UninstallProgressWindow";
            FormClosing += UninstallProgressWindow_FormClosing;
            ((ISupportInitialize)objectListView1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((ISupportInitialize)pictureBox1).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private ProgressBar progressBar1;
        private Label label1;
        private Button buttonClose;
        private OLVColumn olvColumnStatus;
        private OLVColumn olvColumnIsSilent;
        private OLVColumn olvColumnName;
        private ObjectListView objectListView1;
        private GroupBox groupBox1;
        private Label label2;
        private Label label3;
        private UsageTracker usageTracker1;
        private Panel panel3;
        private PictureBox pictureBox1;
        private FlowLayoutPanel flowLayoutPanel1;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonRun;
        private ToolStripButton toolStripButtonFolderOpen;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonSettings;
        private ToolStripButton toolStripButtonSkip;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonProperties;
        private OLVColumn olvColumnId;
        private ToolStripButton toolStripButtonTerminate;
        private ToolStripButton toolStripButtonHelp;
        private Timer forceUpdateTimer;
        private ToolStripButton toolStripButtonManualUninstall;
        private Panel panel1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem skipToolStripMenuItem;
        private ToolStripMenuItem terminateToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem runNowToolStripMenuItem;
        private ToolStripMenuItem manualUninstallToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem openInstallDirectoryToolStripMenuItem;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private FlowLayoutPanel flowLayoutPanel2;
        private CheckBox checkBoxFinishSleep;
        private Timer sleepTimer;
    }
}