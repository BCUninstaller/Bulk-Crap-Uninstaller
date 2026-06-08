using System.ComponentModel;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Forms
{
    partial class PropertiesWindow
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PropertiesWindow));
            contextMenuStrip1 = new ContextMenuStrip(components);
            copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
            saveToFileToolStripMenuItem = new ToolStripMenuItem();
            saveFileDialog1 = new SaveFileDialog();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            dataGridView1 = new DataGridView();
            tabControl2 = new TabControl();
            tabPageOverview = new TabPage();
            tabPageFileInfo = new TabPage();
            tabPageRegistry = new TabPage();
            tabPageCertificate = new TabPage();
            contextMenuStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            ((ISupportInitialize)dataGridView1).BeginInit();
            tabControl2.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { copyToClipboardToolStripMenuItem, saveToFileToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(contextMenuStrip1, "contextMenuStrip1");
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // copyToClipboardToolStripMenuItem
            // 
            copyToClipboardToolStripMenuItem.Image = Properties.Resources.pagecopy;
            copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            resources.ApplyResources(copyToClipboardToolStripMenuItem, "copyToClipboardToolStripMenuItem");
            copyToClipboardToolStripMenuItem.Click += copyToClipboardToolStripMenuItem_Click;
            // 
            // saveToFileToolStripMenuItem
            // 
            saveToFileToolStripMenuItem.Image = Properties.Resources.save;
            saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            resources.ApplyResources(saveToFileToolStripMenuItem, "saveToFileToolStripMenuItem");
            saveToFileToolStripMenuItem.Click += saveToFileToolStripMenuItem_Click;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.DefaultExt = "txt";
            resources.ApplyResources(saveFileDialog1, "saveFileDialog1");
            saveFileDialog1.FileOk += saveFileDialog1_FileOk;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.SelectedIndexChanged += OnSelectedTabChanged;
            // 
            // tabPage1
            // 
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Name = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            resources.ApplyResources(tabPage2, "tabPage2");
            tabPage2.Name = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            resources.ApplyResources(dataGridView1, "dataGridView1");
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.KeyUp += dataGridView1_KeyUp;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabPageOverview);
            tabControl2.Controls.Add(tabPageFileInfo);
            tabControl2.Controls.Add(tabPageRegistry);
            tabControl2.Controls.Add(tabPageCertificate);
            resources.ApplyResources(tabControl2, "tabControl2");
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.SelectedIndexChanged += OnSelectedTabChanged;
            // 
            // tabPageOverview
            // 
            resources.ApplyResources(tabPageOverview, "tabPageOverview");
            tabPageOverview.Name = "tabPageOverview";
            tabPageOverview.UseVisualStyleBackColor = true;
            // 
            // tabPageFileInfo
            // 
            resources.ApplyResources(tabPageFileInfo, "tabPageFileInfo");
            tabPageFileInfo.Name = "tabPageFileInfo";
            tabPageFileInfo.UseVisualStyleBackColor = true;
            // 
            // tabPageRegistry
            // 
            resources.ApplyResources(tabPageRegistry, "tabPageRegistry");
            tabPageRegistry.Name = "tabPageRegistry";
            tabPageRegistry.UseVisualStyleBackColor = true;
            // 
            // tabPageCertificate
            // 
            resources.ApplyResources(tabPageCertificate, "tabPageCertificate");
            tabPageCertificate.Name = "tabPageCertificate";
            tabPageCertificate.UseVisualStyleBackColor = true;
            // 
            // PropertiesWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dataGridView1);
            Controls.Add(tabControl2);
            Controls.Add(tabControl1);
            KeyPreview = true;
            Name = "PropertiesWindow";
            KeyDown += PropertiesWindow_KeyDown;
            contextMenuStrip1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ((ISupportInitialize)dataGridView1).EndInit();
            tabControl2.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private ToolStripMenuItem saveToFileToolStripMenuItem;
        private SaveFileDialog saveFileDialog1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView dataGridView1;
        private TabControl tabControl2;
        private TabPage tabPageOverview;
        private TabPage tabPageFileInfo;
        private TabPage tabPageRegistry;
        private TabPage tabPageCertificate;
    }
}