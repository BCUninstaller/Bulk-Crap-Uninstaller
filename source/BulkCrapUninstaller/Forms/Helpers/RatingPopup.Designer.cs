namespace BulkCrapUninstaller.Forms
{
    partial class RatingPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RatingPopup));
            buttonGood = new System.Windows.Forms.Button();
            buttonBad = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            buttonNormal = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // buttonGood
            // 
            buttonGood.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            resources.ApplyResources(buttonGood, "buttonGood");
            buttonGood.ForeColor = System.Drawing.Color.Black;
            buttonGood.Name = "buttonGood";
            buttonGood.UseVisualStyleBackColor = false;
            buttonGood.Click += buttonGood_Click;
            // 
            // buttonBad
            // 
            buttonBad.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);
            resources.ApplyResources(buttonBad, "buttonBad");
            buttonBad.ForeColor = System.Drawing.Color.Black;
            buttonBad.Name = "buttonBad";
            buttonBad.UseVisualStyleBackColor = false;
            buttonBad.Click += buttonBad_Click;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(buttonBad);
            groupBox1.Controls.Add(buttonNormal);
            groupBox1.Controls.Add(buttonGood);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // buttonNormal
            // 
            buttonNormal.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
            resources.ApplyResources(buttonNormal, "buttonNormal");
            buttonNormal.ForeColor = System.Drawing.Color.Black;
            buttonNormal.Name = "buttonNormal";
            buttonNormal.UseVisualStyleBackColor = false;
            buttonNormal.Click += buttonNormal_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonCancel);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // RatingPopup
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            Controls.Add(groupBox1);
            Controls.Add(panel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "RatingPopup";
            ShowIcon = false;
            ShowInTaskbar = false;
            groupBox1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonGood;
        private System.Windows.Forms.Button buttonBad;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonNormal;
        private System.Windows.Forms.Panel panel1;
    }
}