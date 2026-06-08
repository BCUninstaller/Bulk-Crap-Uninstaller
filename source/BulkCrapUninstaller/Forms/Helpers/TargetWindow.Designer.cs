namespace BulkCrapUninstaller.Forms
{
    partial class TargetWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TargetWindow));
            groupBox1 = new System.Windows.Forms.GroupBox();
            windowTargeter1 = new Klocman.Controls.WindowTargeter();
            groupBox2 = new System.Windows.Forms.GroupBox();
            fileTargeter1 = new BulkCrapUninstaller.Controls.FileTargeter();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            label2 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(windowTargeter1);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // windowTargeter1
            // 
            resources.ApplyResources(windowTargeter1, "windowTargeter1");
            windowTargeter1.Name = "windowTargeter1";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(fileTargeter1);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // fileTargeter1
            // 
            fileTargeter1.AllowDrop = true;
            resources.ApplyResources(fileTargeter1, "fileTargeter1");
            fileTargeter1.Name = "fileTargeter1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(flowLayoutPanel2, "flowLayoutPanel2");
            flowLayoutPanel2.Controls.Add(label2);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.Name = "panel2";
            // 
            // TargetWindow
            // 
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(groupBox2);
            Controls.Add(panel2);
            Controls.Add(groupBox1);
            Controls.Add(flowLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TargetWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Klocman.Controls.WindowTargeter windowTargeter1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Controls.FileTargeter fileTargeter1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}