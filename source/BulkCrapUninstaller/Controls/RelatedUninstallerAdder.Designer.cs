namespace BulkCrapUninstaller.Forms
{
    partial class RelatedUninstallerAdder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RelatedUninstallerAdder));
            objectListView1 = new BrightIdeasSoftware.ObjectListView();
            olvColumnName = new BrightIdeasSoftware.OLVColumn();
            olvColumnEnabled = new BrightIdeasSoftware.OLVColumn();
            olvColumnRelatedApps = new BrightIdeasSoftware.OLVColumn();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            labelInfo = new System.Windows.Forms.Label();
            labelInfo2 = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)objectListView1).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // objectListView1
            // 
            objectListView1.AllColumns.Add(olvColumnName);
            objectListView1.AllColumns.Add(olvColumnEnabled);
            objectListView1.AllColumns.Add(olvColumnRelatedApps);
            objectListView1.CellEditUseWholeCell = false;
            objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { olvColumnName, olvColumnEnabled, olvColumnRelatedApps });
            resources.ApplyResources(objectListView1, "objectListView1");
            objectListView1.FullRowSelect = true;
            objectListView1.GridLines = true;
            objectListView1.MultiSelect = false;
            objectListView1.Name = "objectListView1";
            objectListView1.ShowGroups = false;
            objectListView1.ShowItemToolTips = true;
            objectListView1.UseCompatibleStateImageBehavior = false;
            objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnName
            // 
            resources.ApplyResources(olvColumnName, "olvColumnName");
            // 
            // olvColumnEnabled
            // 
            olvColumnEnabled.CheckBoxes = true;
            olvColumnEnabled.Hideable = false;
            resources.ApplyResources(olvColumnEnabled, "olvColumnEnabled");
            // 
            // olvColumnRelatedApps
            // 
            olvColumnRelatedApps.FillsFreeSpace = true;
            resources.ApplyResources(olvColumnRelatedApps, "olvColumnRelatedApps");
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(labelInfo);
            flowLayoutPanel1.Controls.Add(labelInfo2);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // labelInfo
            // 
            resources.ApplyResources(labelInfo, "labelInfo");
            labelInfo.Name = "labelInfo";
            // 
            // labelInfo2
            // 
            resources.ApplyResources(labelInfo2, "labelInfo2");
            labelInfo2.Name = "labelInfo2";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.warning;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // RelatedUninstallerAdder
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(pictureBox1);
            Controls.Add(objectListView1);
            Controls.Add(flowLayoutPanel1);
            Name = "RelatedUninstallerAdder";
            ((System.ComponentModel.ISupportInitialize)objectListView1).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelInfo;
        private BrightIdeasSoftware.OLVColumn olvColumnEnabled;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnRelatedApps;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelInfo2;
    }
}