using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions.Tracking;

namespace BulkCrapUninstaller.Forms
{
    partial class JunkRemoveWindow
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(JunkRemoveWindow));
            exportDialog = new SaveFileDialog();
            buttonCancel = new Button();
            buttonAccept = new Button();
            buttonExport = new Button();
            comboBoxChecker = new ComboBox();
            checkBoxHideLowConfidence = new CheckBox();
            panel1 = new Panel();
            groupBox1 = new GroupBox();
            objectListViewMain = new ObjectListView();
            olvColumnPath = new OLVColumn();
            olvColumnSafety = new OLVColumn();
            olvColumnUninstallerName = new OLVColumn();
            flowLayoutPanel1 = new FlowLayoutPanel();
            headerIntro = new Label();
            headerConfTitle = new Label();
            headerConfInfo = new Label();
            usageTracker1 = new UsageTracker();
            listViewContextMenuStrip = new ContextMenuStrip(components);
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
            detailsToolStripMenuItem = new ToolStripMenuItem();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((ISupportInitialize)objectListViewMain).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            listViewContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // exportDialog
            // 
            exportDialog.DefaultExt = "txt";
            exportDialog.FileName = "New BCUninstaller Junk Export";
            resources.ApplyResources(exportDialog, "exportDialog");
            exportDialog.RestoreDirectory = true;
            exportDialog.FileOk += exportDialog_FileOk;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonAccept
            // 
            resources.ApplyResources(buttonAccept, "buttonAccept");
            buttonAccept.Name = "buttonAccept";
            buttonAccept.UseVisualStyleBackColor = true;
            buttonAccept.Click += buttonAccept_Click;
            // 
            // buttonExport
            // 
            resources.ApplyResources(buttonExport, "buttonExport");
            buttonExport.Name = "buttonExport";
            buttonExport.UseVisualStyleBackColor = true;
            buttonExport.Click += buttonExport_Click;
            // 
            // comboBoxChecker
            // 
            comboBoxChecker.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxChecker.FormattingEnabled = true;
            resources.ApplyResources(comboBoxChecker, "comboBoxChecker");
            comboBoxChecker.Name = "comboBoxChecker";
            comboBoxChecker.DropDown += comboBoxChecker_DropDown;
            comboBoxChecker.SelectedIndexChanged += comboBoxChecker_SelectedIndexChanged;
            comboBoxChecker.DropDownClosed += comboBoxChecker_DropDownClosed;
            // 
            // checkBoxHideLowConfidence
            // 
            resources.ApplyResources(checkBoxHideLowConfidence, "checkBoxHideLowConfidence");
            checkBoxHideLowConfidence.Checked = true;
            checkBoxHideLowConfidence.CheckState = CheckState.Checked;
            checkBoxHideLowConfidence.Name = "checkBoxHideLowConfidence";
            checkBoxHideLowConfidence.UseVisualStyleBackColor = true;
            checkBoxHideLowConfidence.CheckedChanged += checkBoxHideLowConfidence_CheckedChanged;
            checkBoxHideLowConfidence.Click += checkBoxHideLowConfidence_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonAccept);
            panel1.Controls.Add(buttonCancel);
            panel1.Controls.Add(checkBoxHideLowConfidence);
            panel1.Controls.Add(comboBoxChecker);
            panel1.Controls.Add(buttonExport);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(objectListViewMain);
            groupBox1.Controls.Add(flowLayoutPanel1);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // objectListViewMain
            // 
            objectListViewMain.AllColumns.Add(olvColumnPath);
            objectListViewMain.AllColumns.Add(olvColumnSafety);
            objectListViewMain.AllColumns.Add(olvColumnUninstallerName);
            objectListViewMain.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
            objectListViewMain.CellEditUseWholeCell = false;
            objectListViewMain.CheckBoxes = true;
            objectListViewMain.Columns.AddRange(new ColumnHeader[] { olvColumnPath, olvColumnSafety, olvColumnUninstallerName });
            resources.ApplyResources(objectListViewMain, "objectListViewMain");
            objectListViewMain.FullRowSelect = true;
            objectListViewMain.GridLines = true;
            objectListViewMain.Name = "objectListViewMain";
            objectListViewMain.ShowItemToolTips = true;
            objectListViewMain.UseCompatibleStateImageBehavior = false;
            objectListViewMain.View = View.Details;
            objectListViewMain.CellEditStarting += objectListViewMain_CellEditStarting;
            objectListViewMain.CellRightClick += objectListViewMain_CellRightClick;
            // 
            // olvColumnPath
            // 
            olvColumnPath.AspectName = "";
            resources.ApplyResources(olvColumnPath, "olvColumnPath");
            // 
            // olvColumnSafety
            // 
            olvColumnSafety.AspectName = "";
            resources.ApplyResources(olvColumnSafety, "olvColumnSafety");
            // 
            // olvColumnUninstallerName
            // 
            olvColumnUninstallerName.AspectName = "";
            resources.ApplyResources(olvColumnUninstallerName, "olvColumnUninstallerName");
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(headerIntro);
            flowLayoutPanel1.Controls.Add(headerConfTitle);
            flowLayoutPanel1.Controls.Add(headerConfInfo);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // headerIntro
            // 
            resources.ApplyResources(headerIntro, "headerIntro");
            headerIntro.Name = "headerIntro";
            // 
            // headerConfTitle
            // 
            resources.ApplyResources(headerConfTitle, "headerConfTitle");
            headerConfTitle.Name = "headerConfTitle";
            // 
            // headerConfInfo
            // 
            resources.ApplyResources(headerConfInfo, "headerConfInfo");
            headerConfInfo.Name = "headerConfInfo";
            // 
            // usageTracker1
            // 
            usageTracker1.ContainerControl = this;
            // 
            // listViewContextMenuStrip
            // 
            listViewContextMenuStrip.Items.AddRange(new ToolStripItem[] { openToolStripMenuItem, toolStripSeparator1, copyToClipboardToolStripMenuItem, detailsToolStripMenuItem });
            listViewContextMenuStrip.Name = "listViewContextMenuStrip";
            resources.ApplyResources(listViewContextMenuStrip, "listViewContextMenuStrip");
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(openToolStripMenuItem, "openToolStripMenuItem");
            openToolStripMenuItem.Image = Properties.Resources.folderopen;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // copyToClipboardToolStripMenuItem
            // 
            copyToClipboardToolStripMenuItem.Image = Properties.Resources.pagecopy;
            copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            resources.ApplyResources(copyToClipboardToolStripMenuItem, "copyToClipboardToolStripMenuItem");
            copyToClipboardToolStripMenuItem.Click += copyToClipboardToolStripMenuItem_Click;
            // 
            // detailsToolStripMenuItem
            // 
            detailsToolStripMenuItem.Image = Properties.Resources.magnifybrowse;
            detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            resources.ApplyResources(detailsToolStripMenuItem, "detailsToolStripMenuItem");
            detailsToolStripMenuItem.Click += detailsToolStripMenuItem_Click;
            // 
            // JunkRemoveWindow
            // 
            AcceptButton = buttonAccept;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            Controls.Add(groupBox1);
            Controls.Add(panel1);
            Name = "JunkRemoveWindow";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((ISupportInitialize)objectListViewMain).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            listViewContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private SaveFileDialog exportDialog;
        private Button buttonCancel;
        private Button buttonAccept;
        private Button buttonExport;
        private ComboBox comboBoxChecker;
        private CheckBox checkBoxHideLowConfidence;
        private Panel panel1;
        private GroupBox groupBox1;
        private ObjectListView objectListViewMain;
        private OLVColumn olvColumnPath;
        private OLVColumn olvColumnSafety;
        private OLVColumn olvColumnUninstallerName;
        private UsageTracker usageTracker1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label headerConfInfo;
        private Label headerIntro;
        private Label headerConfTitle;
        private ContextMenuStrip listViewContextMenuStrip;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private ToolStripMenuItem detailsToolStripMenuItem;
    }
}