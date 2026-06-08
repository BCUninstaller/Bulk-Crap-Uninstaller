namespace UninstallerAutomatizer
{
    partial class MainWindow
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            textBoxStatus = new System.Windows.Forms.TextBox();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            panel1 = new System.Windows.Forms.Panel();
            checkBoxUninstallerVisible = new System.Windows.Forms.CheckBox();
            buttonResume = new System.Windows.Forms.Button();
            buttonPause = new System.Windows.Forms.Button();
            panel2 = new System.Windows.Forms.Panel();
            buttonAbort = new System.Windows.Forms.Button();
            timerClose = new System.Windows.Forms.Timer(components);
            timerOpacity = new System.Windows.Forms.Timer(components);
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxStatus
            // 
            textBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(textBoxStatus, "textBoxStatus");
            textBoxStatus.Name = "textBoxStatus";
            textBoxStatus.ReadOnly = true;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBoxStatus);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(checkBoxUninstallerVisible);
            panel1.Controls.Add(buttonResume);
            panel1.Controls.Add(buttonPause);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(buttonAbort);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // checkBoxUninstallerVisible
            // 
            resources.ApplyResources(checkBoxUninstallerVisible, "checkBoxUninstallerVisible");
            checkBoxUninstallerVisible.Name = "checkBoxUninstallerVisible";
            checkBoxUninstallerVisible.UseVisualStyleBackColor = true;
            checkBoxUninstallerVisible.CheckedChanged += checkBoxUninstallerVisible_CheckedChanged;
            // 
            // buttonResume
            // 
            resources.ApplyResources(buttonResume, "buttonResume");
            buttonResume.Name = "buttonResume";
            buttonResume.UseVisualStyleBackColor = true;
            buttonResume.Click += buttonResume_Click;
            // 
            // buttonPause
            // 
            resources.ApplyResources(buttonPause, "buttonPause");
            buttonPause.Name = "buttonPause";
            buttonPause.UseVisualStyleBackColor = true;
            buttonPause.Click += buttonPause_Click;
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.Name = "panel2";
            // 
            // buttonAbort
            // 
            resources.ApplyResources(buttonAbort, "buttonAbort");
            buttonAbort.Name = "buttonAbort";
            buttonAbort.UseVisualStyleBackColor = true;
            buttonAbort.Click += buttonAbort_Click;
            // 
            // timerClose
            // 
            timerClose.Tick += timerClose_Tick;
            // 
            // timerOpacity
            // 
            timerOpacity.Interval = 30;
            timerOpacity.Tick += timerOpacity_Tick;
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Controls.Add(panel1);
            Controls.Add(flowLayoutPanel1);
            Name = "MainWindow";
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Button buttonResume;
        private System.Windows.Forms.Timer timerClose;
        private System.Windows.Forms.CheckBox checkBoxUninstallerVisible;
        private System.Windows.Forms.Timer timerOpacity;
    }
}