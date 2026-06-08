namespace BulkCrapUninstaller.Forms
{
    partial class AdvancedClipboardCopyWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedClipboardCopyWindow));
            panel1 = new System.Windows.Forms.Panel();
            buttonClose = new System.Windows.Forms.Button();
            buttonCopyAll = new System.Windows.Forms.Button();
            advancedClipboardCopy1 = new BulkCrapUninstaller.Controls.AdvancedClipboardCopy();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonClose);
            panel1.Controls.Add(buttonCopyAll);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // buttonClose
            // 
            resources.ApplyResources(buttonClose, "buttonClose");
            buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonClose.Name = "buttonClose";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += button2_Click;
            // 
            // buttonCopyAll
            // 
            resources.ApplyResources(buttonCopyAll, "buttonCopyAll");
            buttonCopyAll.Name = "buttonCopyAll";
            buttonCopyAll.UseVisualStyleBackColor = true;
            buttonCopyAll.Click += button1_Click;
            // 
            // advancedClipboardCopy1
            // 
            resources.ApplyResources(advancedClipboardCopy1, "advancedClipboardCopy1");
            advancedClipboardCopy1.Name = "advancedClipboardCopy1";
            // 
            // AdvancedClipboardCopyWindow
            // 
            AcceptButton = buttonCopyAll;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonClose;
            Controls.Add(advancedClipboardCopy1);
            Controls.Add(panel1);
            Name = "AdvancedClipboardCopyWindow";
            Shown += AdvancedClipboardCopyWindow_Shown;
            panel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private Controls.AdvancedClipboardCopy advancedClipboardCopy1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonCopyAll;
    }
}