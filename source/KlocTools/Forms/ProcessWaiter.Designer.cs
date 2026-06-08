namespace Klocman.Forms
{
    partial class ProcessWaiter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessWaiter));
            processWaiterControl1 = new ProcessWaiterControl();
            SuspendLayout();
            // 
            // processWaiterControl1
            // 
            resources.ApplyResources(processWaiterControl1, "processWaiterControl1");
            processWaiterControl1.Name = "processWaiterControl1";
            processWaiterControl1.ShowIgnoreAndCancel = true;
            // 
            // ProcessWaiter
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ControlBox = false;
            Controls.Add(processWaiterControl1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProcessWaiter";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Shown += ProcessWaiter_Shown;
            ResumeLayout(false);

        }

        #endregion

        private ProcessWaiterControl processWaiterControl1;
    }
}