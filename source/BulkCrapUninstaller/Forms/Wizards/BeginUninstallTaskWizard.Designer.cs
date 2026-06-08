using BulkCrapUninstaller.Controls;

namespace BulkCrapUninstaller.Forms
{
    partial class BeginUninstallTaskWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeginUninstallTaskWizard));
            panelNavigation = new System.Windows.Forms.Panel();
            labelProgress = new System.Windows.Forms.Label();
            buttonExit = new System.Windows.Forms.Button();
            buttonPrev = new System.Windows.Forms.Button();
            panel4 = new System.Windows.Forms.Panel();
            buttonNext = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            tabControl1 = new TabControlWithoutHeader();
            tabPage1 = new System.Windows.Forms.TabPage();
            relatedUninstallerAdder1 = new RelatedUninstallerAdder();
            flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            p1Heading = new System.Windows.Forms.Label();
            tabPage2 = new System.Windows.Forms.TabPage();
            uninstallConfirmation1 = new UninstallConfirmation();
            flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            tabPage3 = new System.Windows.Forms.TabPage();
            processWaiterControl1 = new Klocman.Forms.ProcessWaiterControl();
            flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            label2 = new System.Windows.Forms.Label();
            tabPage4 = new System.Windows.Forms.TabPage();
            uninstallationSettings1 = new UninstallationSettings();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            label5 = new System.Windows.Forms.Label();
            tabPage5 = new System.Windows.Forms.TabPage();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            labelOther = new System.Windows.Forms.Label();
            label19 = new System.Windows.Forms.Label();
            labelConcurrentEnabled = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            labelWillBeSilent = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            labelRestorePointCreated = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            labelFilesStillUsed = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            labelTotalSize = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            labelApps = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            button2 = new System.Windows.Forms.Button();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            label6 = new System.Windows.Forms.Label();
            panelNavigation.SuspendLayout();
            panel1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            flowLayoutPanel5.SuspendLayout();
            tabPage2.SuspendLayout();
            flowLayoutPanel4.SuspendLayout();
            tabPage3.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
            tabPage4.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tabPage5.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // panelNavigation
            // 
            panelNavigation.BackColor = System.Drawing.SystemColors.Control;
            panelNavigation.Controls.Add(labelProgress);
            panelNavigation.Controls.Add(buttonExit);
            panelNavigation.Controls.Add(buttonPrev);
            panelNavigation.Controls.Add(panel4);
            panelNavigation.Controls.Add(buttonNext);
            resources.ApplyResources(panelNavigation, "panelNavigation");
            panelNavigation.Name = "panelNavigation";
            // 
            // labelProgress
            // 
            resources.ApplyResources(labelProgress, "labelProgress");
            labelProgress.ForeColor = System.Drawing.SystemColors.GrayText;
            labelProgress.Name = "labelProgress";
            // 
            // buttonExit
            // 
            resources.ApplyResources(buttonExit, "buttonExit");
            buttonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonExit.Name = "buttonExit";
            buttonExit.UseVisualStyleBackColor = true;
            // 
            // buttonPrev
            // 
            resources.ApplyResources(buttonPrev, "buttonPrev");
            buttonPrev.Name = "buttonPrev";
            buttonPrev.UseVisualStyleBackColor = true;
            buttonPrev.Click += buttonPrev_Click;
            // 
            // panel4
            // 
            resources.ApplyResources(panel4, "panel4");
            panel4.Name = "panel4";
            // 
            // buttonNext
            // 
            resources.ApplyResources(buttonNext, "buttonNext");
            buttonNext.Name = "buttonNext";
            buttonNext.UseVisualStyleBackColor = true;
            buttonNext.Click += buttonNext_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(tabControl1);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(relatedUninstallerAdder1);
            tabPage1.Controls.Add(flowLayoutPanel5);
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Name = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // relatedUninstallerAdder1
            // 
            resources.ApplyResources(relatedUninstallerAdder1, "relatedUninstallerAdder1");
            relatedUninstallerAdder1.Name = "relatedUninstallerAdder1";
            // 
            // flowLayoutPanel5
            // 
            resources.ApplyResources(flowLayoutPanel5, "flowLayoutPanel5");
            flowLayoutPanel5.Controls.Add(p1Heading);
            flowLayoutPanel5.Name = "flowLayoutPanel5";
            // 
            // p1Heading
            // 
            resources.ApplyResources(p1Heading, "p1Heading");
            p1Heading.Name = "p1Heading";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(uninstallConfirmation1);
            tabPage2.Controls.Add(flowLayoutPanel4);
            resources.ApplyResources(tabPage2, "tabPage2");
            tabPage2.Name = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // uninstallConfirmation1
            // 
            resources.ApplyResources(uninstallConfirmation1, "uninstallConfirmation1");
            uninstallConfirmation1.Name = "uninstallConfirmation1";
            // 
            // flowLayoutPanel4
            // 
            resources.ApplyResources(flowLayoutPanel4, "flowLayoutPanel4");
            flowLayoutPanel4.Controls.Add(label1);
            flowLayoutPanel4.Name = "flowLayoutPanel4";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(processWaiterControl1);
            tabPage3.Controls.Add(flowLayoutPanel3);
            resources.ApplyResources(tabPage3, "tabPage3");
            tabPage3.Name = "tabPage3";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // processWaiterControl1
            // 
            resources.ApplyResources(processWaiterControl1, "processWaiterControl1");
            processWaiterControl1.Name = "processWaiterControl1";
            processWaiterControl1.ShowIgnoreAndCancel = false;
            processWaiterControl1.AllProcessesClosed += processWaiterControl1_AllProcessesClosed;
            // 
            // flowLayoutPanel3
            // 
            resources.ApplyResources(flowLayoutPanel3, "flowLayoutPanel3");
            flowLayoutPanel3.Controls.Add(label2);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // tabPage4
            // 
            resources.ApplyResources(tabPage4, "tabPage4");
            tabPage4.Controls.Add(uninstallationSettings1);
            tabPage4.Controls.Add(flowLayoutPanel1);
            tabPage4.Name = "tabPage4";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // uninstallationSettings1
            // 
            resources.ApplyResources(uninstallationSettings1, "uninstallationSettings1");
            uninstallationSettings1.Name = "uninstallationSettings1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(label5);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            flowLayoutPanel1.SetFlowBreak(label5, true);
            label5.Name = "label5";
            // 
            // tabPage5
            // 
            resources.ApplyResources(tabPage5, "tabPage5");
            tabPage5.Controls.Add(tableLayoutPanel1);
            tabPage5.Controls.Add(button2);
            tabPage5.Controls.Add(flowLayoutPanel2);
            tabPage5.Name = "tabPage5";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
            tableLayoutPanel1.Controls.Add(labelOther, 1, 6);
            tableLayoutPanel1.Controls.Add(label19, 0, 6);
            tableLayoutPanel1.Controls.Add(labelConcurrentEnabled, 1, 5);
            tableLayoutPanel1.Controls.Add(label17, 0, 5);
            tableLayoutPanel1.Controls.Add(labelWillBeSilent, 1, 4);
            tableLayoutPanel1.Controls.Add(label15, 0, 4);
            tableLayoutPanel1.Controls.Add(labelRestorePointCreated, 1, 3);
            tableLayoutPanel1.Controls.Add(label13, 0, 3);
            tableLayoutPanel1.Controls.Add(labelFilesStillUsed, 1, 2);
            tableLayoutPanel1.Controls.Add(label11, 0, 2);
            tableLayoutPanel1.Controls.Add(labelTotalSize, 1, 1);
            tableLayoutPanel1.Controls.Add(label9, 0, 1);
            tableLayoutPanel1.Controls.Add(labelApps, 1, 0);
            tableLayoutPanel1.Controls.Add(label7, 0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelOther
            // 
            resources.ApplyResources(labelOther, "labelOther");
            labelOther.Name = "labelOther";
            // 
            // label19
            // 
            resources.ApplyResources(label19, "label19");
            label19.Name = "label19";
            // 
            // labelConcurrentEnabled
            // 
            resources.ApplyResources(labelConcurrentEnabled, "labelConcurrentEnabled");
            labelConcurrentEnabled.Name = "labelConcurrentEnabled";
            // 
            // label17
            // 
            resources.ApplyResources(label17, "label17");
            label17.Name = "label17";
            // 
            // labelWillBeSilent
            // 
            resources.ApplyResources(labelWillBeSilent, "labelWillBeSilent");
            labelWillBeSilent.Name = "labelWillBeSilent";
            // 
            // label15
            // 
            resources.ApplyResources(label15, "label15");
            label15.Name = "label15";
            // 
            // labelRestorePointCreated
            // 
            resources.ApplyResources(labelRestorePointCreated, "labelRestorePointCreated");
            labelRestorePointCreated.Name = "labelRestorePointCreated";
            // 
            // label13
            // 
            resources.ApplyResources(label13, "label13");
            label13.Name = "label13";
            // 
            // labelFilesStillUsed
            // 
            resources.ApplyResources(labelFilesStillUsed, "labelFilesStillUsed");
            labelFilesStillUsed.Name = "labelFilesStillUsed";
            // 
            // label11
            // 
            resources.ApplyResources(label11, "label11");
            label11.Name = "label11";
            // 
            // labelTotalSize
            // 
            resources.ApplyResources(labelTotalSize, "labelTotalSize");
            labelTotalSize.Name = "labelTotalSize";
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.Name = "label9";
            // 
            // labelApps
            // 
            labelApps.AutoEllipsis = true;
            resources.ApplyResources(labelApps, "labelApps");
            labelApps.Name = "labelApps";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // button2
            // 
            resources.ApplyResources(button2, "button2");
            button2.Name = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(flowLayoutPanel2, "flowLayoutPanel2");
            flowLayoutPanel2.Controls.Add(label6);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            flowLayoutPanel2.SetFlowBreak(label6, true);
            label6.Name = "label6";
            // 
            // BeginUninstallTaskWizard
            // 
            AcceptButton = button2;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            CancelButton = buttonExit;
            Controls.Add(panel1);
            Controls.Add(panelNavigation);
            Name = "BeginUninstallTaskWizard";
            FormClosed += BeginUninstallTaskWizard_FormClosed;
            panelNavigation.ResumeLayout(false);
            panelNavigation.PerformLayout();
            panel1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            flowLayoutPanel5.ResumeLayout(false);
            flowLayoutPanel5.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            flowLayoutPanel4.ResumeLayout(false);
            flowLayoutPanel4.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            flowLayoutPanel3.ResumeLayout(false);
            flowLayoutPanel3.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelNavigation;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private TabControlWithoutHeader tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private RelatedUninstallerAdder relatedUninstallerAdder1;
        private UninstallConfirmation uninstallConfirmation1;
        private System.Windows.Forms.TabPage tabPage3;
        private Klocman.Forms.ProcessWaiterControl processWaiterControl1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private UninstallationSettings uninstallationSettings1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelOther;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label labelConcurrentEnabled;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label labelWillBeSilent;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label labelRestorePointCreated;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelFilesStillUsed;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelTotalSize;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelApps;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label p1Heading;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Label label1;
    }
}