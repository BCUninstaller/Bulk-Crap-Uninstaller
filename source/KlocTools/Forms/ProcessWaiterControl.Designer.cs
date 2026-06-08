namespace Klocman.Forms
{
    partial class ProcessWaiterControl
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
            _timer?.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessWaiterControl));
            treeView1 = new System.Windows.Forms.TreeView();
            label1 = new System.Windows.Forms.Label();
            buttonCancel = new System.Windows.Forms.Button();
            buttonIgnore = new System.Windows.Forms.Button();
            buttonKillAll = new System.Windows.Forms.Button();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            buttonKill = new System.Windows.Forms.Button();
            panel3 = new System.Windows.Forms.Panel();
            panel2c = new System.Windows.Forms.Panel();
            panel4c = new System.Windows.Forms.Panel();
            flowLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // treeView1
            // 
            resources.ApplyResources(treeView1, "treeView1");
            treeView1.Name = "treeView1";
            treeView1.ShowNodeToolTips = true;
            treeView1.AfterSelect += treeView1_AfterSelect;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonIgnore
            // 
            resources.ApplyResources(buttonIgnore, "buttonIgnore");
            buttonIgnore.Name = "buttonIgnore";
            buttonIgnore.UseVisualStyleBackColor = true;
            buttonIgnore.Click += buttonIgnore_Click;
            // 
            // buttonKillAll
            // 
            resources.ApplyResources(buttonKillAll, "buttonKillAll");
            buttonKillAll.Name = "buttonKillAll";
            buttonKillAll.UseVisualStyleBackColor = true;
            buttonKillAll.Click += buttonKillAll_Click;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonKill);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(buttonKillAll);
            panel1.Controls.Add(panel2c);
            panel1.Controls.Add(buttonIgnore);
            panel1.Controls.Add(panel4c);
            panel1.Controls.Add(buttonCancel);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // buttonKill
            // 
            resources.ApplyResources(buttonKill, "buttonKill");
            buttonKill.Name = "buttonKill";
            buttonKill.UseVisualStyleBackColor = true;
            buttonKill.Click += buttonKill_Click;
            // 
            // panel3
            // 
            resources.ApplyResources(panel3, "panel3");
            panel3.Name = "panel3";
            // 
            // panel2c
            // 
            resources.ApplyResources(panel2c, "panel2c");
            panel2c.Name = "panel2c";
            // 
            // panel4c
            // 
            resources.ApplyResources(panel4c, "panel4c");
            panel4c.Name = "panel4c";
            // 
            // ProcessWaiterControl
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(treeView1);
            Controls.Add(panel1);
            Controls.Add(flowLayoutPanel1);
            Name = "ProcessWaiterControl";
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonIgnore;
        private System.Windows.Forms.Button buttonKillAll;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonKill;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2c;
        private System.Windows.Forms.Panel panel4c;
    }
}