namespace BulkCrapUninstaller.Controls
{
    partial class FileTargeter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileTargeter));
            panel1 = new System.Windows.Forms.Panel();
            button1 = new System.Windows.Forms.Button();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            panel2 = new System.Windows.Forms.Panel();
            button2 = new System.Windows.Forms.Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(button1);
            panel1.Name = "panel1";
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.layerdown;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // openFileDialog1
            // 
            openFileDialog1.DereferenceLinks = false;
            openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(openFileDialog1, "openFileDialog1");
            openFileDialog1.Multiselect = true;
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.Controls.Add(button2);
            panel2.Name = "panel2";
            // 
            // button2
            // 
            resources.ApplyResources(button2, "button2");
            button2.Name = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // FileTargeter
            // 
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(panel2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(pictureBox1);
            Name = "FileTargeter";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button2;
    }
}
