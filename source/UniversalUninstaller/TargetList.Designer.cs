namespace UniversalUninstaller
{
    partial class TargetList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TargetList));
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripButtonSelAll = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonExpand = new System.Windows.Forms.ToolStripButton();
            toolStripButtonCollapse = new System.Windows.Forms.ToolStripButton();
            treeListView1 = new BrightIdeasSoftware.TreeListView();
            olvColumnName = new BrightIdeasSoftware.OLVColumn();
            olvColumnSize = new BrightIdeasSoftware.OLVColumn();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)treeListView1).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(22, 22);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonSelAll, toolStripSeparator1, toolStripButtonExpand, toolStripButtonCollapse });
            resources.ApplyResources(toolStrip1, "toolStrip1");
            toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonSelAll
            // 
            resources.ApplyResources(toolStripButtonSelAll, "toolStripButtonSelAll");
            toolStripButtonSelAll.Name = "toolStripButtonSelAll";
            toolStripButtonSelAll.Click += toolStripButtonSelAll_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonExpand
            // 
            toolStripButtonExpand.Image = Properties.Resources.section_expand_all;
            resources.ApplyResources(toolStripButtonExpand, "toolStripButtonExpand");
            toolStripButtonExpand.Name = "toolStripButtonExpand";
            toolStripButtonExpand.Click += expand_Click;
            // 
            // toolStripButtonCollapse
            // 
            toolStripButtonCollapse.Image = Properties.Resources.section_collapse_all;
            resources.ApplyResources(toolStripButtonCollapse, "toolStripButtonCollapse");
            toolStripButtonCollapse.Name = "toolStripButtonCollapse";
            toolStripButtonCollapse.Click += collapse_Click;
            // 
            // treeListView1
            // 
            treeListView1.AllColumns.Add(olvColumnName);
            treeListView1.AllColumns.Add(olvColumnSize);
            treeListView1.CellEditUseWholeCell = false;
            treeListView1.CheckBoxes = true;
            treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { olvColumnName, olvColumnSize });
            resources.ApplyResources(treeListView1, "treeListView1");
            treeListView1.FullRowSelect = true;
            treeListView1.GridLines = true;
            treeListView1.Name = "treeListView1";
            treeListView1.ShowGroups = false;
            treeListView1.ShowImagesOnSubItems = true;
            treeListView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            treeListView1.UseCompatibleStateImageBehavior = false;
            treeListView1.View = System.Windows.Forms.View.Details;
            treeListView1.VirtualMode = true;
            treeListView1.ItemChecked += treeListView1_ItemChecked;
            treeListView1.MouseDoubleClick += treeListView1_MouseDoubleClick;
            // 
            // olvColumnName
            // 
            olvColumnName.AspectName = "";
            olvColumnName.FillsFreeSpace = true;
            resources.ApplyResources(olvColumnName, "olvColumnName");
            // 
            // olvColumnSize
            // 
            resources.ApplyResources(olvColumnSize, "olvColumnSize");
            // 
            // TargetList
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(treeListView1);
            Controls.Add(toolStrip1);
            Name = "TargetList";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)treeListView1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelAll;
        private BrightIdeasSoftware.TreeListView treeListView1;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private System.Windows.Forms.ToolStripButton toolStripButtonExpand;
        private System.Windows.Forms.ToolStripButton toolStripButtonCollapse;
        private BrightIdeasSoftware.OLVColumn olvColumnSize;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
