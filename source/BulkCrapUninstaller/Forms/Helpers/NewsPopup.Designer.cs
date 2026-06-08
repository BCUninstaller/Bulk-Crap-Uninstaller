namespace BulkCrapUninstaller.Forms
{
    partial class NewsPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewsPopup));
            labelTitle = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            linkLabel2 = new System.Windows.Forms.LinkLabel();
            linkLabel3 = new System.Windows.Forms.LinkLabel();
            linkLabel4 = new System.Windows.Forms.LinkLabel();
            linkLabel5 = new System.Windows.Forms.LinkLabel();
            linkLabel6 = new System.Windows.Forms.LinkLabel();
            linkLabel7 = new System.Windows.Forms.LinkLabel();
            label1 = new System.Windows.Forms.Label();
            checkBoxNeverShow = new System.Windows.Forms.CheckBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // labelTitle
            // 
            resources.ApplyResources(labelTitle, "labelTitle");
            labelTitle.Name = "labelTitle";
            labelTitle.Click += Close;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            label3.Click += Close;
            // 
            // linkLabel1
            // 
            resources.ApplyResources(linkLabel1, "linkLabel1");
            linkLabel1.Name = "linkLabel1";
            linkLabel1.TabStop = true;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkLabel2
            // 
            resources.ApplyResources(linkLabel2, "linkLabel2");
            linkLabel2.Name = "linkLabel2";
            linkLabel2.TabStop = true;
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // linkLabel3
            // 
            resources.ApplyResources(linkLabel3, "linkLabel3");
            linkLabel3.Name = "linkLabel3";
            linkLabel3.TabStop = true;
            linkLabel3.LinkClicked += linkLabel3_LinkClicked;
            // 
            // linkLabel4
            // 
            resources.ApplyResources(linkLabel4, "linkLabel4");
            linkLabel4.Name = "linkLabel4";
            linkLabel4.TabStop = true;
            linkLabel4.LinkClicked += linkLabel4_LinkClicked;
            // 
            // linkLabel5
            // 
            resources.ApplyResources(linkLabel5, "linkLabel5");
            linkLabel5.Name = "linkLabel5";
            linkLabel5.TabStop = true;
            linkLabel5.LinkClicked += linkLabel5_LinkClicked;
            // 
            // linkLabel6
            // 
            resources.ApplyResources(linkLabel6, "linkLabel6");
            linkLabel6.Name = "linkLabel6";
            linkLabel6.TabStop = true;
            linkLabel6.LinkClicked += linkLabel6_LinkClicked;
            // 
            // linkLabel7
            // 
            resources.ApplyResources(linkLabel7, "linkLabel7");
            linkLabel7.Name = "linkLabel7";
            linkLabel7.TabStop = true;
            linkLabel7.LinkClicked += linkLabel7_LinkClicked;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            label1.Click += Close;
            // 
            // checkBoxNeverShow
            // 
            resources.ApplyResources(checkBoxNeverShow, "checkBoxNeverShow");
            checkBoxNeverShow.Name = "checkBoxNeverShow";
            checkBoxNeverShow.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(checkBoxNeverShow, 1, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // NewsPopup
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            Controls.Add(tableLayoutPanel1);
            Controls.Add(label3);
            Controls.Add(linkLabel7);
            Controls.Add(linkLabel4);
            Controls.Add(linkLabel3);
            Controls.Add(linkLabel2);
            Controls.Add(linkLabel6);
            Controls.Add(linkLabel5);
            Controls.Add(linkLabel1);
            Controls.Add(labelTitle);
            ForeColor = System.Drawing.Color.Black;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewsPopup";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Click += Close;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.LinkLabel linkLabel7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxNeverShow;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}