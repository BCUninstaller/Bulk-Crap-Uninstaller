namespace BulkCrapUninstaller.Forms
{
    partial class FeedbackWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeedbackWindow));
            webBrowser = new System.Windows.Forms.WebBrowser();
            loadingLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // webBrowser
            // 
            webBrowser.AllowWebBrowserDrop = false;
            resources.ApplyResources(webBrowser, "webBrowser");
            webBrowser.IsWebBrowserContextMenuEnabled = false;
            webBrowser.Name = "webBrowser";
            // 
            // loadingLabel
            // 
            resources.ApplyResources(loadingLabel, "loadingLabel");
            loadingLabel.Name = "loadingLabel";
            loadingLabel.UseWaitCursor = true;
            // 
            // FeedbackWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(loadingLabel);
            Controls.Add(webBrowser);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FeedbackWindow";
            Shown += FeedbackWindow_Shown;
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Label loadingLabel;
    }
}