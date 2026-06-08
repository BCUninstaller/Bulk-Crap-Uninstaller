using System.ComponentModel;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.Tracking;

namespace BulkCrapUninstaller.Forms
{
    partial class SettingsWindow
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SettingsWindow));
            splitContainer1 = new SplitContainer();
            textBoxPreUninstall = new TextBox();
            label5 = new Label();
            textBoxPostUninstall = new TextBox();
            label6 = new Label();
            groupBoxMisc = new GroupBox();
            flowLayoutPanel3 = new FlowLayoutPanel();
            checkBoxColorblind = new CheckBox();
            checkBoxDpiaware = new CheckBox();
            panel5 = new Panel();
            comboBoxDoubleClick = new ComboBox();
            label3 = new Label();
            checkBoxAutoLoad = new CheckBox();
            checkBoxRatings = new CheckBox();
            checkBoxUpdateSearch = new CheckBox();
            checkBoxSendStats = new CheckBox();
            panel3 = new Panel();
            comboBoxLanguage = new ComboBox();
            label9 = new Label();
            label10 = new Label();
            groupBoxMessages = new GroupBox();
            groupBoxBackup = new GroupBox();
            directorySelectBoxBackup = new Klocman.Controls.DirectorySelectBox();
            flowLayoutPanel9 = new FlowLayoutPanel();
            radioButtonBackupAsk = new RadioButton();
            radioButtonBackupNever = new RadioButton();
            radioButtonBackupAuto = new RadioButton();
            flowLayoutPanel1 = new FlowLayoutPanel();
            checkBoxShowAllBadJunk = new CheckBox();
            checkBoxLoud = new CheckBox();
            checkBoxNeverFeedback = new CheckBox();
            panel1 = new Panel();
            comboBoxJunk = new ComboBox();
            label1 = new Label();
            panel2 = new Panel();
            comboBoxRestore = new ComboBox();
            label2 = new Label();
            groupBoxExternal = new GroupBox();
            checkBoxEnableExternal = new CheckBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            label7 = new Label();
            button2 = new Button();
            tabControl = new TabControl();
            tabPageGeneral = new TabPage();
            propertiesSidebar1 = new BulkCrapUninstaller.Controls.PropertiesSidebar();
            tabPageInterface = new TabPage();
            groupBoxLanguage = new GroupBox();
            tabPageUninstallation = new TabPage();
            uninstallationSettings1 = new BulkCrapUninstaller.Controls.UninstallationSettings();
            tabPageDetection = new TabPage();
            groupBoxAppStores = new GroupBox();
            labelWinUpdateInfo = new Label();
            checkBoxScanWinUpdates = new CheckBox();
            labelWinFeatureInfo = new Label();
            checkBoxScanWinFeatures = new CheckBox();
            checkBoxScanStoreApps = new CheckBox();
            checkBoxScanSteam = new CheckBox();
            checkBoxScoop = new CheckBox();
            checkBoxOculus = new CheckBox();
            checkBoxChoco = new CheckBox();
            groupBox1 = new GroupBox();
            flowLayoutPanel7 = new FlowLayoutPanel();
            checkBoxScanRegistry = new CheckBox();
            checkBoxScanDrives = new CheckBox();
            checkBoxPreDefined = new CheckBox();
            tabPageExternal = new TabPage();
            tabPageFolders = new TabPage();
            groupBoxProgramFolders = new GroupBox();
            textBoxProgramFolders = new TextBox();
            checkBoxRemovable = new CheckBox();
            checkBoxAutoInstallFolderDetect = new CheckBox();
            labelProgramFolders = new Label();
            tabPageMisc = new TabPage();
            cacheSettings1 = new BulkCrapUninstaller.Controls.Settings.CacheSettings();
            groupBox2 = new GroupBox();
            flowLayoutPanel4 = new FlowLayoutPanel();
            groupBox3 = new GroupBox();
            flowLayoutPanel10 = new FlowLayoutPanel();
            panel4 = new Panel();
            usageTracker1 = new UsageTracker();
            ((ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBoxMisc.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
            panel5.SuspendLayout();
            panel3.SuspendLayout();
            groupBoxMessages.SuspendLayout();
            groupBoxBackup.SuspendLayout();
            flowLayoutPanel9.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            groupBoxExternal.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            tabControl.SuspendLayout();
            tabPageGeneral.SuspendLayout();
            tabPageInterface.SuspendLayout();
            groupBoxLanguage.SuspendLayout();
            tabPageUninstallation.SuspendLayout();
            tabPageDetection.SuspendLayout();
            groupBoxAppStores.SuspendLayout();
            groupBox1.SuspendLayout();
            flowLayoutPanel7.SuspendLayout();
            tabPageExternal.SuspendLayout();
            tabPageFolders.SuspendLayout();
            groupBoxProgramFolders.SuspendLayout();
            tabPageMisc.SuspendLayout();
            groupBox2.SuspendLayout();
            flowLayoutPanel4.SuspendLayout();
            groupBox3.SuspendLayout();
            flowLayoutPanel10.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            resources.ApplyResources(splitContainer1, "splitContainer1");
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(textBoxPreUninstall);
            splitContainer1.Panel1.Controls.Add(label5);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(textBoxPostUninstall);
            splitContainer1.Panel2.Controls.Add(label6);
            // 
            // textBoxPreUninstall
            // 
            resources.ApplyResources(textBoxPreUninstall, "textBoxPreUninstall");
            textBoxPreUninstall.Name = "textBoxPreUninstall";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // textBoxPostUninstall
            // 
            resources.ApplyResources(textBoxPostUninstall, "textBoxPostUninstall");
            textBoxPostUninstall.Name = "textBoxPostUninstall";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // groupBoxMisc
            // 
            resources.ApplyResources(groupBoxMisc, "groupBoxMisc");
            groupBoxMisc.Controls.Add(flowLayoutPanel3);
            groupBoxMisc.Controls.Add(panel5);
            groupBoxMisc.Name = "groupBoxMisc";
            groupBoxMisc.TabStop = false;
            // 
            // flowLayoutPanel3
            // 
            resources.ApplyResources(flowLayoutPanel3, "flowLayoutPanel3");
            flowLayoutPanel3.Controls.Add(checkBoxColorblind);
            flowLayoutPanel3.Controls.Add(checkBoxDpiaware);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            // 
            // checkBoxColorblind
            // 
            resources.ApplyResources(checkBoxColorblind, "checkBoxColorblind");
            checkBoxColorblind.Name = "checkBoxColorblind";
            checkBoxColorblind.UseVisualStyleBackColor = true;
            // 
            // checkBoxDpiaware
            // 
            resources.ApplyResources(checkBoxDpiaware, "checkBoxDpiaware");
            checkBoxDpiaware.Name = "checkBoxDpiaware";
            checkBoxDpiaware.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            resources.ApplyResources(panel5, "panel5");
            panel5.Controls.Add(comboBoxDoubleClick);
            panel5.Controls.Add(label3);
            panel5.Name = "panel5";
            // 
            // comboBoxDoubleClick
            // 
            resources.ApplyResources(comboBoxDoubleClick, "comboBoxDoubleClick");
            comboBoxDoubleClick.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDoubleClick.FormattingEnabled = true;
            comboBoxDoubleClick.Name = "comboBoxDoubleClick";
            comboBoxDoubleClick.SelectedIndexChanged += comboBoxDoubleClick_SelectedIndexChanged;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // checkBoxAutoLoad
            // 
            resources.ApplyResources(checkBoxAutoLoad, "checkBoxAutoLoad");
            checkBoxAutoLoad.Name = "checkBoxAutoLoad";
            checkBoxAutoLoad.UseVisualStyleBackColor = true;
            // 
            // checkBoxRatings
            // 
            resources.ApplyResources(checkBoxRatings, "checkBoxRatings");
            checkBoxRatings.Name = "checkBoxRatings";
            checkBoxRatings.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdateSearch
            // 
            resources.ApplyResources(checkBoxUpdateSearch, "checkBoxUpdateSearch");
            checkBoxUpdateSearch.Name = "checkBoxUpdateSearch";
            checkBoxUpdateSearch.UseVisualStyleBackColor = true;
            // 
            // checkBoxSendStats
            // 
            resources.ApplyResources(checkBoxSendStats, "checkBoxSendStats");
            checkBoxSendStats.Name = "checkBoxSendStats";
            checkBoxSendStats.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            resources.ApplyResources(panel3, "panel3");
            panel3.Controls.Add(comboBoxLanguage);
            panel3.Controls.Add(label9);
            panel3.Name = "panel3";
            // 
            // comboBoxLanguage
            // 
            resources.ApplyResources(comboBoxLanguage, "comboBoxLanguage");
            comboBoxLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLanguage.FormattingEnabled = true;
            comboBoxLanguage.Name = "comboBoxLanguage";
            comboBoxLanguage.SelectedIndexChanged += comboBoxLanguage_SelectedIndexChanged;
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(label10, "label10");
            label10.Name = "label10";
            // 
            // groupBoxMessages
            // 
            resources.ApplyResources(groupBoxMessages, "groupBoxMessages");
            groupBoxMessages.Controls.Add(groupBoxBackup);
            groupBoxMessages.Controls.Add(flowLayoutPanel1);
            groupBoxMessages.Controls.Add(panel1);
            groupBoxMessages.Controls.Add(panel2);
            groupBoxMessages.Name = "groupBoxMessages";
            groupBoxMessages.TabStop = false;
            // 
            // groupBoxBackup
            // 
            resources.ApplyResources(groupBoxBackup, "groupBoxBackup");
            groupBoxBackup.Controls.Add(directorySelectBoxBackup);
            groupBoxBackup.Controls.Add(flowLayoutPanel9);
            groupBoxBackup.Name = "groupBoxBackup";
            groupBoxBackup.TabStop = false;
            // 
            // directorySelectBoxBackup
            // 
            directorySelectBoxBackup.DirectoryPath = "";
            resources.ApplyResources(directorySelectBoxBackup, "directorySelectBoxBackup");
            directorySelectBoxBackup.Name = "directorySelectBoxBackup";
            // 
            // flowLayoutPanel9
            // 
            resources.ApplyResources(flowLayoutPanel9, "flowLayoutPanel9");
            flowLayoutPanel9.Controls.Add(radioButtonBackupAsk);
            flowLayoutPanel9.Controls.Add(radioButtonBackupNever);
            flowLayoutPanel9.Controls.Add(radioButtonBackupAuto);
            flowLayoutPanel9.Name = "flowLayoutPanel9";
            // 
            // radioButtonBackupAsk
            // 
            resources.ApplyResources(radioButtonBackupAsk, "radioButtonBackupAsk");
            flowLayoutPanel9.SetFlowBreak(radioButtonBackupAsk, true);
            radioButtonBackupAsk.Name = "radioButtonBackupAsk";
            radioButtonBackupAsk.TabStop = true;
            radioButtonBackupAsk.UseVisualStyleBackColor = true;
            radioButtonBackupAsk.CheckedChanged += radioButtonBackup_CheckedChanged;
            // 
            // radioButtonBackupNever
            // 
            resources.ApplyResources(radioButtonBackupNever, "radioButtonBackupNever");
            flowLayoutPanel9.SetFlowBreak(radioButtonBackupNever, true);
            radioButtonBackupNever.Name = "radioButtonBackupNever";
            radioButtonBackupNever.TabStop = true;
            radioButtonBackupNever.UseVisualStyleBackColor = true;
            radioButtonBackupNever.CheckedChanged += radioButtonBackup_CheckedChanged;
            // 
            // radioButtonBackupAuto
            // 
            resources.ApplyResources(radioButtonBackupAuto, "radioButtonBackupAuto");
            flowLayoutPanel9.SetFlowBreak(radioButtonBackupAuto, true);
            radioButtonBackupAuto.Name = "radioButtonBackupAuto";
            radioButtonBackupAuto.TabStop = true;
            radioButtonBackupAuto.UseVisualStyleBackColor = true;
            radioButtonBackupAuto.CheckedChanged += radioButtonBackup_CheckedChanged;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(checkBoxShowAllBadJunk);
            flowLayoutPanel1.Controls.Add(checkBoxLoud);
            flowLayoutPanel1.Controls.Add(checkBoxNeverFeedback);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // checkBoxShowAllBadJunk
            // 
            resources.ApplyResources(checkBoxShowAllBadJunk, "checkBoxShowAllBadJunk");
            checkBoxShowAllBadJunk.Name = "checkBoxShowAllBadJunk";
            checkBoxShowAllBadJunk.UseVisualStyleBackColor = true;
            // 
            // checkBoxLoud
            // 
            resources.ApplyResources(checkBoxLoud, "checkBoxLoud");
            checkBoxLoud.Name = "checkBoxLoud";
            checkBoxLoud.UseVisualStyleBackColor = true;
            // 
            // checkBoxNeverFeedback
            // 
            resources.ApplyResources(checkBoxNeverFeedback, "checkBoxNeverFeedback");
            checkBoxNeverFeedback.Name = "checkBoxNeverFeedback";
            checkBoxNeverFeedback.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(comboBoxJunk);
            panel1.Controls.Add(label1);
            panel1.Name = "panel1";
            // 
            // comboBoxJunk
            // 
            resources.ApplyResources(comboBoxJunk, "comboBoxJunk");
            comboBoxJunk.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxJunk.FormattingEnabled = true;
            comboBoxJunk.Name = "comboBoxJunk";
            comboBoxJunk.SelectedIndexChanged += comboBoxJunk_SelectedIndexChanged;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.Controls.Add(comboBoxRestore);
            panel2.Controls.Add(label2);
            panel2.Name = "panel2";
            // 
            // comboBoxRestore
            // 
            resources.ApplyResources(comboBoxRestore, "comboBoxRestore");
            comboBoxRestore.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxRestore.FormattingEnabled = true;
            comboBoxRestore.Name = "comboBoxRestore";
            comboBoxRestore.SelectedIndexChanged += comboBoxRestore_SelectedIndexChanged;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // groupBoxExternal
            // 
            resources.ApplyResources(groupBoxExternal, "groupBoxExternal");
            groupBoxExternal.Controls.Add(splitContainer1);
            groupBoxExternal.Controls.Add(checkBoxEnableExternal);
            groupBoxExternal.Controls.Add(flowLayoutPanel2);
            groupBoxExternal.Name = "groupBoxExternal";
            groupBoxExternal.TabStop = false;
            // 
            // checkBoxEnableExternal
            // 
            resources.ApplyResources(checkBoxEnableExternal, "checkBoxEnableExternal");
            checkBoxEnableExternal.Checked = true;
            checkBoxEnableExternal.CheckState = CheckState.Checked;
            checkBoxEnableExternal.Name = "checkBoxEnableExternal";
            checkBoxEnableExternal.UseVisualStyleBackColor = true;
            checkBoxEnableExternal.CheckedChanged += checkBoxEnableExternal_CheckedChanged;
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(flowLayoutPanel2, "flowLayoutPanel2");
            flowLayoutPanel2.Controls.Add(label7);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // button2
            // 
            resources.ApplyResources(button2, "button2");
            button2.DialogResult = DialogResult.Cancel;
            button2.Name = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPageGeneral);
            tabControl.Controls.Add(tabPageInterface);
            tabControl.Controls.Add(tabPageUninstallation);
            tabControl.Controls.Add(tabPageDetection);
            tabControl.Controls.Add(tabPageExternal);
            tabControl.Controls.Add(tabPageFolders);
            tabControl.Controls.Add(tabPageMisc);
            resources.ApplyResources(tabControl, "tabControl");
            tabControl.Multiline = true;
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.SizeMode = TabSizeMode.FillToRight;
            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
            // 
            // tabPageGeneral
            // 
            tabPageGeneral.Controls.Add(propertiesSidebar1);
            resources.ApplyResources(tabPageGeneral, "tabPageGeneral");
            tabPageGeneral.Name = "tabPageGeneral";
            tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // propertiesSidebar1
            // 
            resources.ApplyResources(propertiesSidebar1, "propertiesSidebar1");
            propertiesSidebar1.Name = "propertiesSidebar1";
            // 
            // tabPageInterface
            // 
            resources.ApplyResources(tabPageInterface, "tabPageInterface");
            tabPageInterface.Controls.Add(groupBoxMisc);
            tabPageInterface.Controls.Add(groupBoxMessages);
            tabPageInterface.Controls.Add(groupBoxLanguage);
            tabPageInterface.Name = "tabPageInterface";
            tabPageInterface.UseVisualStyleBackColor = true;
            // 
            // groupBoxLanguage
            // 
            resources.ApplyResources(groupBoxLanguage, "groupBoxLanguage");
            groupBoxLanguage.Controls.Add(panel3);
            groupBoxLanguage.Controls.Add(label10);
            groupBoxLanguage.Name = "groupBoxLanguage";
            groupBoxLanguage.TabStop = false;
            // 
            // tabPageUninstallation
            // 
            resources.ApplyResources(tabPageUninstallation, "tabPageUninstallation");
            tabPageUninstallation.Controls.Add(uninstallationSettings1);
            tabPageUninstallation.Name = "tabPageUninstallation";
            tabPageUninstallation.UseVisualStyleBackColor = true;
            // 
            // uninstallationSettings1
            // 
            resources.ApplyResources(uninstallationSettings1, "uninstallationSettings1");
            uninstallationSettings1.Name = "uninstallationSettings1";
            // 
            // tabPageDetection
            // 
            resources.ApplyResources(tabPageDetection, "tabPageDetection");
            tabPageDetection.Controls.Add(groupBoxAppStores);
            tabPageDetection.Controls.Add(groupBox1);
            tabPageDetection.Name = "tabPageDetection";
            tabPageDetection.UseVisualStyleBackColor = true;
            // 
            // groupBoxAppStores
            // 
            resources.ApplyResources(groupBoxAppStores, "groupBoxAppStores");
            groupBoxAppStores.Controls.Add(labelWinUpdateInfo);
            groupBoxAppStores.Controls.Add(checkBoxScanWinUpdates);
            groupBoxAppStores.Controls.Add(labelWinFeatureInfo);
            groupBoxAppStores.Controls.Add(checkBoxScanWinFeatures);
            groupBoxAppStores.Controls.Add(checkBoxScanStoreApps);
            groupBoxAppStores.Controls.Add(checkBoxScanSteam);
            groupBoxAppStores.Controls.Add(checkBoxScoop);
            groupBoxAppStores.Controls.Add(checkBoxOculus);
            groupBoxAppStores.Controls.Add(checkBoxChoco);
            groupBoxAppStores.Name = "groupBoxAppStores";
            groupBoxAppStores.TabStop = false;
            // 
            // labelWinUpdateInfo
            // 
            resources.ApplyResources(labelWinUpdateInfo, "labelWinUpdateInfo");
            labelWinUpdateInfo.Name = "labelWinUpdateInfo";
            // 
            // checkBoxScanWinUpdates
            // 
            resources.ApplyResources(checkBoxScanWinUpdates, "checkBoxScanWinUpdates");
            checkBoxScanWinUpdates.Name = "checkBoxScanWinUpdates";
            checkBoxScanWinUpdates.UseVisualStyleBackColor = true;
            // 
            // labelWinFeatureInfo
            // 
            resources.ApplyResources(labelWinFeatureInfo, "labelWinFeatureInfo");
            labelWinFeatureInfo.Name = "labelWinFeatureInfo";
            // 
            // checkBoxScanWinFeatures
            // 
            resources.ApplyResources(checkBoxScanWinFeatures, "checkBoxScanWinFeatures");
            checkBoxScanWinFeatures.Name = "checkBoxScanWinFeatures";
            checkBoxScanWinFeatures.UseVisualStyleBackColor = true;
            // 
            // checkBoxScanStoreApps
            // 
            resources.ApplyResources(checkBoxScanStoreApps, "checkBoxScanStoreApps");
            checkBoxScanStoreApps.Name = "checkBoxScanStoreApps";
            checkBoxScanStoreApps.UseVisualStyleBackColor = true;
            // 
            // checkBoxScanSteam
            // 
            resources.ApplyResources(checkBoxScanSteam, "checkBoxScanSteam");
            checkBoxScanSteam.Name = "checkBoxScanSteam";
            checkBoxScanSteam.UseVisualStyleBackColor = true;
            // 
            // checkBoxScoop
            // 
            resources.ApplyResources(checkBoxScoop, "checkBoxScoop");
            checkBoxScoop.Name = "checkBoxScoop";
            checkBoxScoop.UseVisualStyleBackColor = true;
            // 
            // checkBoxOculus
            // 
            resources.ApplyResources(checkBoxOculus, "checkBoxOculus");
            checkBoxOculus.Name = "checkBoxOculus";
            checkBoxOculus.UseVisualStyleBackColor = true;
            // 
            // checkBoxChoco
            // 
            resources.ApplyResources(checkBoxChoco, "checkBoxChoco");
            checkBoxChoco.Name = "checkBoxChoco";
            checkBoxChoco.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(flowLayoutPanel7);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // flowLayoutPanel7
            // 
            resources.ApplyResources(flowLayoutPanel7, "flowLayoutPanel7");
            flowLayoutPanel7.Controls.Add(checkBoxScanRegistry);
            flowLayoutPanel7.Controls.Add(checkBoxScanDrives);
            flowLayoutPanel7.Controls.Add(checkBoxPreDefined);
            flowLayoutPanel7.Name = "flowLayoutPanel7";
            // 
            // checkBoxScanRegistry
            // 
            resources.ApplyResources(checkBoxScanRegistry, "checkBoxScanRegistry");
            flowLayoutPanel7.SetFlowBreak(checkBoxScanRegistry, true);
            checkBoxScanRegistry.Name = "checkBoxScanRegistry";
            checkBoxScanRegistry.UseVisualStyleBackColor = true;
            // 
            // checkBoxScanDrives
            // 
            resources.ApplyResources(checkBoxScanDrives, "checkBoxScanDrives");
            flowLayoutPanel7.SetFlowBreak(checkBoxScanDrives, true);
            checkBoxScanDrives.Name = "checkBoxScanDrives";
            checkBoxScanDrives.UseVisualStyleBackColor = true;
            // 
            // checkBoxPreDefined
            // 
            resources.ApplyResources(checkBoxPreDefined, "checkBoxPreDefined");
            flowLayoutPanel7.SetFlowBreak(checkBoxPreDefined, true);
            checkBoxPreDefined.Name = "checkBoxPreDefined";
            checkBoxPreDefined.UseVisualStyleBackColor = true;
            // 
            // tabPageExternal
            // 
            tabPageExternal.Controls.Add(groupBoxExternal);
            resources.ApplyResources(tabPageExternal, "tabPageExternal");
            tabPageExternal.Name = "tabPageExternal";
            tabPageExternal.UseVisualStyleBackColor = true;
            // 
            // tabPageFolders
            // 
            tabPageFolders.Controls.Add(groupBoxProgramFolders);
            resources.ApplyResources(tabPageFolders, "tabPageFolders");
            tabPageFolders.Name = "tabPageFolders";
            tabPageFolders.UseVisualStyleBackColor = true;
            // 
            // groupBoxProgramFolders
            // 
            resources.ApplyResources(groupBoxProgramFolders, "groupBoxProgramFolders");
            groupBoxProgramFolders.Controls.Add(textBoxProgramFolders);
            groupBoxProgramFolders.Controls.Add(checkBoxRemovable);
            groupBoxProgramFolders.Controls.Add(checkBoxAutoInstallFolderDetect);
            groupBoxProgramFolders.Controls.Add(labelProgramFolders);
            groupBoxProgramFolders.Name = "groupBoxProgramFolders";
            groupBoxProgramFolders.TabStop = false;
            // 
            // textBoxProgramFolders
            // 
            resources.ApplyResources(textBoxProgramFolders, "textBoxProgramFolders");
            textBoxProgramFolders.Name = "textBoxProgramFolders";
            // 
            // checkBoxRemovable
            // 
            resources.ApplyResources(checkBoxRemovable, "checkBoxRemovable");
            checkBoxRemovable.Name = "checkBoxRemovable";
            checkBoxRemovable.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoInstallFolderDetect
            // 
            resources.ApplyResources(checkBoxAutoInstallFolderDetect, "checkBoxAutoInstallFolderDetect");
            checkBoxAutoInstallFolderDetect.Name = "checkBoxAutoInstallFolderDetect";
            checkBoxAutoInstallFolderDetect.UseVisualStyleBackColor = true;
            // 
            // labelProgramFolders
            // 
            resources.ApplyResources(labelProgramFolders, "labelProgramFolders");
            labelProgramFolders.Name = "labelProgramFolders";
            // 
            // tabPageMisc
            // 
            tabPageMisc.Controls.Add(cacheSettings1);
            tabPageMisc.Controls.Add(groupBox2);
            tabPageMisc.Controls.Add(groupBox3);
            resources.ApplyResources(tabPageMisc, "tabPageMisc");
            tabPageMisc.Name = "tabPageMisc";
            tabPageMisc.UseVisualStyleBackColor = true;
            // 
            // cacheSettings1
            // 
            resources.ApplyResources(cacheSettings1, "cacheSettings1");
            cacheSettings1.Name = "cacheSettings1";
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(flowLayoutPanel4);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // flowLayoutPanel4
            // 
            resources.ApplyResources(flowLayoutPanel4, "flowLayoutPanel4");
            flowLayoutPanel4.Controls.Add(checkBoxAutoLoad);
            flowLayoutPanel4.Name = "flowLayoutPanel4";
            // 
            // groupBox3
            // 
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Controls.Add(flowLayoutPanel10);
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // flowLayoutPanel10
            // 
            resources.ApplyResources(flowLayoutPanel10, "flowLayoutPanel10");
            flowLayoutPanel10.Controls.Add(checkBoxUpdateSearch);
            flowLayoutPanel10.Controls.Add(checkBoxSendStats);
            flowLayoutPanel10.Controls.Add(checkBoxRatings);
            flowLayoutPanel10.Name = "flowLayoutPanel10";
            // 
            // panel4
            // 
            panel4.Controls.Add(button2);
            resources.ApplyResources(panel4, "panel4");
            panel4.Name = "panel4";
            // 
            // usageTracker1
            // 
            usageTracker1.ContainerControl = this;
            // 
            // SettingsWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = button2;
            Controls.Add(tabControl);
            Controls.Add(panel4);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsWindow";
            FormClosing += SettingsWindow_FormClosing;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxMisc.ResumeLayout(false);
            groupBoxMisc.PerformLayout();
            flowLayoutPanel3.ResumeLayout(false);
            flowLayoutPanel3.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            groupBoxMessages.ResumeLayout(false);
            groupBoxMessages.PerformLayout();
            groupBoxBackup.ResumeLayout(false);
            groupBoxBackup.PerformLayout();
            flowLayoutPanel9.ResumeLayout(false);
            flowLayoutPanel9.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            groupBoxExternal.ResumeLayout(false);
            groupBoxExternal.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            tabControl.ResumeLayout(false);
            tabPageGeneral.ResumeLayout(false);
            tabPageGeneral.PerformLayout();
            tabPageInterface.ResumeLayout(false);
            tabPageInterface.PerformLayout();
            groupBoxLanguage.ResumeLayout(false);
            groupBoxLanguage.PerformLayout();
            tabPageUninstallation.ResumeLayout(false);
            tabPageUninstallation.PerformLayout();
            tabPageDetection.ResumeLayout(false);
            tabPageDetection.PerformLayout();
            groupBoxAppStores.ResumeLayout(false);
            groupBoxAppStores.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            flowLayoutPanel7.ResumeLayout(false);
            flowLayoutPanel7.PerformLayout();
            tabPageExternal.ResumeLayout(false);
            tabPageExternal.PerformLayout();
            tabPageFolders.ResumeLayout(false);
            tabPageFolders.PerformLayout();
            groupBoxProgramFolders.ResumeLayout(false);
            groupBoxProgramFolders.PerformLayout();
            tabPageMisc.ResumeLayout(false);
            tabPageMisc.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            flowLayoutPanel4.ResumeLayout(false);
            flowLayoutPanel4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            flowLayoutPanel10.ResumeLayout(false);
            flowLayoutPanel10.PerformLayout();
            panel4.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBoxMessages;
        private CheckBox checkBoxLoud;
        private ComboBox comboBoxRestore;
        private ComboBox comboBoxJunk;
        private Label label2;
        private Label label1;
        private Button button2;
        private GroupBox groupBoxMisc;
        private CheckBox checkBoxUpdateSearch;
        private UsageTracker usageTracker1;
        private GroupBox groupBoxExternal;
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel panel1;
        private Panel panel2;
        private FlowLayoutPanel flowLayoutPanel3;
        private CheckBox checkBoxSendStats;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label label5;
        private TextBox textBoxPreUninstall;
        private Label label6;
        private TextBox textBoxPostUninstall;
        private Label label7;
        private CheckBox checkBoxEnableExternal;
        private Panel panel3;
        private ComboBox comboBoxLanguage;
        private Label label9;
        private Label label10;
        private SplitContainer splitContainer1;
        private TabControl tabControl;
        private TabPage tabPageMisc;
        private TabPage tabPageExternal;
        private Panel panel4;
        private TabPage tabPageGeneral;
        private Controls.PropertiesSidebar propertiesSidebar1;
        private TabPage tabPageFolders;
        private GroupBox groupBoxProgramFolders;
        private Label labelProgramFolders;
        private TextBox textBoxProgramFolders;
        private CheckBox checkBoxAutoLoad;
        private CheckBox checkBoxRatings;
        private TabPage tabPageUninstallation;
        private Controls.UninstallationSettings uninstallationSettings1;
        private CheckBox checkBoxShowAllBadJunk;
        private CheckBox checkBoxNeverFeedback;
        private TabPage tabPageDetection;
        private GroupBox groupBoxAppStores;
        private CheckBox checkBoxScanSteam;
        private CheckBox checkBoxScanStoreApps;
        private CheckBox checkBoxScanWinFeatures;
        private CheckBox checkBoxScanWinUpdates;
        private Label labelWinFeatureInfo;
        private Label labelWinUpdateInfo;
        private CheckBox checkBoxAutoInstallFolderDetect;
        private GroupBox groupBox1;
        private FlowLayoutPanel flowLayoutPanel7;
        private CheckBox checkBoxScanRegistry;
        private CheckBox checkBoxScanDrives;
        private CheckBox checkBoxPreDefined;
        private TabPage tabPageInterface;
        private GroupBox groupBoxLanguage;
        private GroupBox groupBoxBackup;
        private FlowLayoutPanel flowLayoutPanel9;
        private RadioButton radioButtonBackupAsk;
        private RadioButton radioButtonBackupNever;
        private RadioButton radioButtonBackupAuto;
        private Klocman.Controls.DirectorySelectBox directorySelectBoxBackup;
        private Controls.Settings.CacheSettings cacheSettings1;
        private GroupBox groupBox3;
        private FlowLayoutPanel flowLayoutPanel10;
        private CheckBox checkBoxChoco;
        private CheckBox checkBoxOculus;
        private CheckBox checkBoxRemovable;
        private CheckBox checkBoxColorblind;
        private CheckBox checkBoxScoop;
        private CheckBox checkBoxDpiaware;
        private GroupBox groupBox2;
        private FlowLayoutPanel flowLayoutPanel4;
        private Panel panel5;
        private ComboBox comboBoxDoubleClick;
        private Label label3;
    }
}