namespace Klocman.Forms
{
    partial class WindowTargeterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowTargeterDialog));
            windowTargeter1 = new Klocman.Controls.WindowTargeter();
            SuspendLayout();
            // 
            // windowTargeter1
            // 
            resources.ApplyResources(windowTargeter1, "windowTargeter1");
            windowTargeter1.Name = "windowTargeter1";
            // 
            // WindowTargeterDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(windowTargeter1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "WindowTargeterDialog";
            ResumeLayout(false);

        }

        #endregion

        private Controls.WindowTargeter windowTargeter1;
    }
}