namespace BulkCrapUninstaller.Controls
{
    partial class ListLegend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListLegend));
            labelWinFeature = new System.Windows.Forms.Label();
            labelOrphaned = new System.Windows.Forms.Label();
            labelInvalid = new System.Windows.Forms.Label();
            labelUnverified = new System.Windows.Forms.Label();
            labelVerified = new System.Windows.Forms.Label();
            labelLegend = new System.Windows.Forms.Label();
            labelStoreApp = new System.Windows.Forms.Label();
            flowLayoutPanellabelVerified = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanellabelUnverified = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanellabelInvalid = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanellabelOrphaned = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanellabelWinFeature = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanellabelStoreApp = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanellabelVerified.SuspendLayout();
            flowLayoutPanellabelUnverified.SuspendLayout();
            flowLayoutPanellabelInvalid.SuspendLayout();
            flowLayoutPanellabelOrphaned.SuspendLayout();
            flowLayoutPanellabelWinFeature.SuspendLayout();
            flowLayoutPanellabelStoreApp.SuspendLayout();
            SuspendLayout();
            // 
            // labelWinFeature
            // 
            resources.ApplyResources(labelWinFeature, "labelWinFeature");
            labelWinFeature.Cursor = System.Windows.Forms.Cursors.Hand;
            labelWinFeature.Name = "labelWinFeature";
            labelWinFeature.MouseDown += OnMouseDown;
            // 
            // labelOrphaned
            // 
            resources.ApplyResources(labelOrphaned, "labelOrphaned");
            labelOrphaned.Cursor = System.Windows.Forms.Cursors.Hand;
            labelOrphaned.Name = "labelOrphaned";
            labelOrphaned.MouseDown += OnMouseDown;
            // 
            // labelInvalid
            // 
            resources.ApplyResources(labelInvalid, "labelInvalid");
            labelInvalid.Cursor = System.Windows.Forms.Cursors.Hand;
            labelInvalid.Name = "labelInvalid";
            labelInvalid.MouseDown += OnMouseDown;
            // 
            // labelUnverified
            // 
            resources.ApplyResources(labelUnverified, "labelUnverified");
            labelUnverified.Cursor = System.Windows.Forms.Cursors.Hand;
            labelUnverified.Name = "labelUnverified";
            labelUnverified.MouseDown += OnMouseDown;
            // 
            // labelVerified
            // 
            resources.ApplyResources(labelVerified, "labelVerified");
            labelVerified.Cursor = System.Windows.Forms.Cursors.Hand;
            labelVerified.Name = "labelVerified";
            labelVerified.MouseDown += OnMouseDown;
            // 
            // labelLegend
            // 
            labelLegend.BackColor = System.Drawing.SystemColors.Control;
            labelLegend.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(labelLegend, "labelLegend");
            labelLegend.Name = "labelLegend";
            labelLegend.MouseDown += OnMouseDown;
            // 
            // labelStoreApp
            // 
            resources.ApplyResources(labelStoreApp, "labelStoreApp");
            labelStoreApp.Cursor = System.Windows.Forms.Cursors.Hand;
            labelStoreApp.Name = "labelStoreApp";
            // 
            // flowLayoutPanellabelVerified
            // 
            resources.ApplyResources(flowLayoutPanellabelVerified, "flowLayoutPanellabelVerified");
            flowLayoutPanellabelVerified.BackColor = System.Drawing.Color.PaleGreen;
            flowLayoutPanellabelVerified.Controls.Add(labelVerified);
            flowLayoutPanellabelVerified.Name = "flowLayoutPanellabelVerified";
            flowLayoutPanellabelVerified.MouseDown += OnMouseDown;
            // 
            // flowLayoutPanellabelUnverified
            // 
            resources.ApplyResources(flowLayoutPanellabelUnverified, "flowLayoutPanellabelUnverified");
            flowLayoutPanellabelUnverified.BackColor = System.Drawing.Color.Aquamarine;
            flowLayoutPanellabelUnverified.Controls.Add(labelUnverified);
            flowLayoutPanellabelUnverified.Name = "flowLayoutPanellabelUnverified";
            flowLayoutPanellabelUnverified.MouseDown += OnMouseDown;
            // 
            // flowLayoutPanellabelInvalid
            // 
            resources.ApplyResources(flowLayoutPanellabelInvalid, "flowLayoutPanellabelInvalid");
            flowLayoutPanellabelInvalid.BackColor = System.Drawing.Color.LightSteelBlue;
            flowLayoutPanellabelInvalid.Controls.Add(labelInvalid);
            flowLayoutPanellabelInvalid.Name = "flowLayoutPanellabelInvalid";
            flowLayoutPanellabelInvalid.MouseDown += OnMouseDown;
            // 
            // flowLayoutPanellabelOrphaned
            // 
            resources.ApplyResources(flowLayoutPanellabelOrphaned, "flowLayoutPanellabelOrphaned");
            flowLayoutPanellabelOrphaned.BackColor = System.Drawing.Color.LightPink;
            flowLayoutPanellabelOrphaned.Controls.Add(labelOrphaned);
            flowLayoutPanellabelOrphaned.Name = "flowLayoutPanellabelOrphaned";
            flowLayoutPanellabelOrphaned.MouseDown += OnMouseDown;
            // 
            // flowLayoutPanellabelWinFeature
            // 
            resources.ApplyResources(flowLayoutPanellabelWinFeature, "flowLayoutPanellabelWinFeature");
            flowLayoutPanellabelWinFeature.BackColor = System.Drawing.Color.SlateBlue;
            flowLayoutPanellabelWinFeature.Controls.Add(labelWinFeature);
            flowLayoutPanellabelWinFeature.Name = "flowLayoutPanellabelWinFeature";
            flowLayoutPanellabelWinFeature.MouseDown += OnMouseDown;
            // 
            // flowLayoutPanellabelStoreApp
            // 
            resources.ApplyResources(flowLayoutPanellabelStoreApp, "flowLayoutPanellabelStoreApp");
            flowLayoutPanellabelStoreApp.BackColor = System.Drawing.Color.DeepSkyBlue;
            flowLayoutPanellabelStoreApp.Controls.Add(labelStoreApp);
            flowLayoutPanellabelStoreApp.Name = "flowLayoutPanellabelStoreApp";
            flowLayoutPanellabelStoreApp.MouseDown += OnMouseDown;
            // 
            // ListLegend
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(flowLayoutPanellabelStoreApp);
            Controls.Add(flowLayoutPanellabelWinFeature);
            Controls.Add(flowLayoutPanellabelOrphaned);
            Controls.Add(flowLayoutPanellabelInvalid);
            Controls.Add(flowLayoutPanellabelUnverified);
            Controls.Add(flowLayoutPanellabelVerified);
            Controls.Add(labelLegend);
            Cursor = System.Windows.Forms.Cursors.Hand;
            Name = "ListLegend";
            EnabledChanged += ThisEnabledChanged;
            MouseDown += OnMouseDown;
            flowLayoutPanellabelVerified.ResumeLayout(false);
            flowLayoutPanellabelVerified.PerformLayout();
            flowLayoutPanellabelUnverified.ResumeLayout(false);
            flowLayoutPanellabelUnverified.PerformLayout();
            flowLayoutPanellabelInvalid.ResumeLayout(false);
            flowLayoutPanellabelInvalid.PerformLayout();
            flowLayoutPanellabelOrphaned.ResumeLayout(false);
            flowLayoutPanellabelOrphaned.PerformLayout();
            flowLayoutPanellabelWinFeature.ResumeLayout(false);
            flowLayoutPanellabelWinFeature.PerformLayout();
            flowLayoutPanellabelStoreApp.ResumeLayout(false);
            flowLayoutPanellabelStoreApp.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelWinFeature;
        private System.Windows.Forms.Label labelOrphaned;
        private System.Windows.Forms.Label labelInvalid;
        private System.Windows.Forms.Label labelUnverified;
        private System.Windows.Forms.Label labelVerified;
        private System.Windows.Forms.Label labelLegend;
        private System.Windows.Forms.Label labelStoreApp;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelVerified;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelUnverified;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelInvalid;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelOrphaned;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelWinFeature;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelStoreApp;
    }
}
