using System.ComponentModel;
using System.Windows.Forms;

namespace Klocman.Forms
{
    sealed partial class LoadingDialog
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
            progressBar = new ProgressBar();
            label1 = new Label();
            panel1 = new Panel();
            progressBar2 = new ProgressBar();
            label2 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Top;
            progressBar.Location = new System.Drawing.Point(7, 28);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(392, 24);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 0;
            progressBar.Value = 100;
            // 
            // label1
            // 
            label1.AutoEllipsis = true;
            label1.AutoSize = true;
            label1.Dock = DockStyle.Top;
            label1.Location = new System.Drawing.Point(7, 7);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Padding = new Padding(0, 0, 0, 6);
            label1.Size = new System.Drawing.Size(59, 21);
            label1.TabIndex = 1;
            label1.Text = "Loading...";
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(progressBar2);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(progressBar);
            panel1.Controls.Add(label1);
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.MinimumSize = new System.Drawing.Size(408, 12);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(7, 7, 7, 7);
            panel1.Size = new System.Drawing.Size(408, 113);
            panel1.TabIndex = 2;
            panel1.Resize += panel1_Resize;
            // 
            // progressBar2
            // 
            progressBar2.Dock = DockStyle.Top;
            progressBar2.Location = new System.Drawing.Point(7, 80);
            progressBar2.Margin = new Padding(4, 3, 4, 3);
            progressBar2.Name = "progressBar2";
            progressBar2.Size = new System.Drawing.Size(392, 24);
            progressBar2.Style = ProgressBarStyle.Marquee;
            progressBar2.TabIndex = 2;
            progressBar2.Value = 100;
            progressBar2.Visible = false;
            // 
            // label2
            // 
            label2.AutoEllipsis = true;
            label2.AutoSize = true;
            label2.Dock = DockStyle.Top;
            label2.Location = new System.Drawing.Point(7, 52);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Padding = new Padding(0, 7, 0, 6);
            label2.Size = new System.Drawing.Size(59, 28);
            label2.TabIndex = 3;
            label2.Text = "Loading...";
            label2.Visible = false;
            // 
            // LoadingDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size(464, 290);
            ControlBox = false;
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoadingDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label label1;
        private Panel panel1;
        private ProgressBar progressBar;
        private ProgressBar progressBar2;
        private Label label2;
    }
}