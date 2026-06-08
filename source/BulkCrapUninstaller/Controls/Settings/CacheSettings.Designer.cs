namespace BulkCrapUninstaller.Controls.Settings
{
    partial class CacheSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CacheSettings));
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            checkBoxCerts = new System.Windows.Forms.CheckBox();
            checkBoxInfo = new System.Windows.Forms.CheckBox();
            button1 = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Controls.Add(panel1);
            flowLayoutPanel1.Controls.Add(checkBoxCerts);
            flowLayoutPanel1.Controls.Add(checkBoxInfo);
            flowLayoutPanel1.Controls.Add(button1);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // checkBoxCerts
            // 
            resources.ApplyResources(checkBoxCerts, "checkBoxCerts");
            checkBoxCerts.Name = "checkBoxCerts";
            checkBoxCerts.UseVisualStyleBackColor = true;
            // 
            // checkBoxInfo
            // 
            resources.ApplyResources(checkBoxInfo, "checkBoxInfo");
            checkBoxInfo.Name = "checkBoxInfo";
            checkBoxInfo.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(flowLayoutPanel1);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // CacheSettings
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Name = "CacheSettings";
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxCerts;
        private System.Windows.Forms.CheckBox checkBoxInfo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}
