namespace Klocman.Controls
{
    partial class WindowTargeter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowTargeter));
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoEllipsis = true;
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            label2.AutoEllipsis = true;
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // label3
            // 
            label3.AutoEllipsis = true;
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.centerline;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // WindowTargeter
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Name = "WindowTargeter";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
