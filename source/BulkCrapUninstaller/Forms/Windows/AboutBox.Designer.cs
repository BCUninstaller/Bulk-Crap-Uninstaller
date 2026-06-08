using System.ComponentModel;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.Tracking;

namespace BulkCrapUninstaller.Forms
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(AboutBox));
            labelName = new Label();
            labelVersion = new Label();
            labelCopyright = new Label();
            labelCompanyName = new Label();
            groupBox1 = new GroupBox();
            panel1 = new Panel();
            labelis64 = new Label();
            labelPortable = new Label();
            panel4 = new Panel();
            labelArchitecture = new Label();
            groupBox2 = new GroupBox();
            panel5 = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            linkLabel6 = new LinkLabel();
            linkLabel9 = new LinkLabel();
            linkLabel3 = new LinkLabel();
            linkLabel1 = new LinkLabel();
            linkLabel5 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            linkLabel7 = new LinkLabel();
            linkLabel8 = new LinkLabel();
            button1 = new Button();
            linkLabel4 = new LinkLabel();
            groupBox3 = new GroupBox();
            panel3 = new Panel();
            imageBox = new Label();
            panel2 = new Panel();
            groupBox4 = new GroupBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            usageTracker1 = new UsageTracker();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            panel4.SuspendLayout();
            groupBox2.SuspendLayout();
            panel5.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            groupBox3.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // labelName
            // 
            resources.ApplyResources(labelName, "labelName");
            labelName.Name = "labelName";
            // 
            // labelVersion
            // 
            resources.ApplyResources(labelVersion, "labelVersion");
            labelVersion.Name = "labelVersion";
            // 
            // labelCopyright
            // 
            resources.ApplyResources(labelCopyright, "labelCopyright");
            labelCopyright.Name = "labelCopyright";
            // 
            // labelCompanyName
            // 
            resources.ApplyResources(labelCompanyName, "labelCompanyName");
            labelCompanyName.Name = "labelCompanyName";
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(panel1);
            groupBox1.Controls.Add(panel4);
            groupBox1.Controls.Add(labelName);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(labelis64);
            panel1.Controls.Add(labelPortable);
            panel1.Name = "panel1";
            // 
            // labelis64
            // 
            resources.ApplyResources(labelis64, "labelis64");
            labelis64.Name = "labelis64";
            // 
            // labelPortable
            // 
            resources.ApplyResources(labelPortable, "labelPortable");
            labelPortable.Name = "labelPortable";
            // 
            // panel4
            // 
            resources.ApplyResources(panel4, "panel4");
            panel4.Controls.Add(labelVersion);
            panel4.Controls.Add(labelArchitecture);
            panel4.Name = "panel4";
            // 
            // labelArchitecture
            // 
            resources.ApplyResources(labelArchitecture, "labelArchitecture");
            labelArchitecture.Name = "labelArchitecture";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(panel5);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // panel5
            // 
            panel5.Controls.Add(flowLayoutPanel1);
            resources.ApplyResources(panel5, "panel5");
            panel5.Name = "panel5";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(linkLabel6);
            flowLayoutPanel1.Controls.Add(linkLabel9);
            flowLayoutPanel1.Controls.Add(linkLabel3);
            flowLayoutPanel1.Controls.Add(linkLabel1);
            flowLayoutPanel1.Controls.Add(linkLabel5);
            flowLayoutPanel1.Controls.Add(linkLabel2);
            flowLayoutPanel1.Controls.Add(linkLabel7);
            flowLayoutPanel1.Controls.Add(linkLabel8);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // linkLabel6
            // 
            resources.ApplyResources(linkLabel6, "linkLabel6");
            linkLabel6.Name = "linkLabel6";
            linkLabel6.TabStop = true;
            linkLabel6.LinkClicked += linkLabel6_LinkClicked;
            // 
            // linkLabel9
            // 
            resources.ApplyResources(linkLabel9, "linkLabel9");
            linkLabel9.Name = "linkLabel9";
            linkLabel9.TabStop = true;
            linkLabel9.LinkClicked += linkLabel9_LinkClicked;
            // 
            // linkLabel3
            // 
            resources.ApplyResources(linkLabel3, "linkLabel3");
            linkLabel3.Name = "linkLabel3";
            linkLabel3.TabStop = true;
            linkLabel3.LinkClicked += linkLabel3_LinkClicked;
            // 
            // linkLabel1
            // 
            resources.ApplyResources(linkLabel1, "linkLabel1");
            linkLabel1.Name = "linkLabel1";
            linkLabel1.TabStop = true;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkLabel5
            // 
            resources.ApplyResources(linkLabel5, "linkLabel5");
            linkLabel5.Name = "linkLabel5";
            linkLabel5.TabStop = true;
            linkLabel5.LinkClicked += linkLabel5_LinkClicked;
            // 
            // linkLabel2
            // 
            resources.ApplyResources(linkLabel2, "linkLabel2");
            linkLabel2.Name = "linkLabel2";
            linkLabel2.TabStop = true;
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // linkLabel7
            // 
            resources.ApplyResources(linkLabel7, "linkLabel7");
            linkLabel7.Name = "linkLabel7";
            linkLabel7.TabStop = true;
            linkLabel7.LinkClicked += linkLabel7_LinkClicked;
            // 
            // linkLabel8
            // 
            resources.ApplyResources(linkLabel8, "linkLabel8");
            linkLabel8.Name = "linkLabel8";
            linkLabel8.TabStop = true;
            linkLabel8.LinkClicked += linkLabel8_LinkClicked;
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.DialogResult = DialogResult.Cancel;
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // linkLabel4
            // 
            resources.ApplyResources(linkLabel4, "linkLabel4");
            linkLabel4.Name = "linkLabel4";
            linkLabel4.TabStop = true;
            linkLabel4.LinkClicked += linkLabel4_LinkClicked;
            // 
            // groupBox3
            // 
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Controls.Add(linkLabel4);
            groupBox3.Controls.Add(panel3);
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // panel3
            // 
            resources.ApplyResources(panel3, "panel3");
            panel3.Controls.Add(labelCopyright);
            panel3.Controls.Add(labelCompanyName);
            panel3.Name = "panel3";
            // 
            // imageBox
            // 
            imageBox.BorderStyle = BorderStyle.Fixed3D;
            resources.ApplyResources(imageBox, "imageBox");
            imageBox.Image = Properties.Resources.bigImage;
            imageBox.Name = "imageBox";
            // 
            // panel2
            // 
            panel2.Controls.Add(groupBox4);
            panel2.Controls.Add(groupBox2);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(groupBox3);
            panel2.Controls.Add(groupBox1);
            resources.ApplyResources(panel2, "panel2");
            panel2.Name = "panel2";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(flowLayoutPanel2);
            resources.ApplyResources(groupBox4, "groupBox4");
            groupBox4.Name = "groupBox4";
            groupBox4.TabStop = false;
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(flowLayoutPanel2, "flowLayoutPanel2");
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // usageTracker1
            // 
            usageTracker1.ContainerControl = this;
            // 
            // AboutBox
            // 
            AcceptButton = button1;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = button1;
            Controls.Add(panel2);
            Controls.Add(imageBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutBox";
            ShowIcon = false;
            ShowInTaskbar = false;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            groupBox2.ResumeLayout(false);
            panel5.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private Label labelName;
        private Label labelVersion;
        private Label labelCopyright;
        private Label labelCompanyName;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private LinkLabel linkLabel3;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel4;
        private Button button1;
        private Label labelArchitecture;
        private GroupBox groupBox3;
        private Label labelis64;
        private LinkLabel linkLabel6;
        private LinkLabel linkLabel5;
        private Label labelPortable;
        private UsageTracker usageTracker1;
        private Label imageBox;
        private Panel panel1;
        private Panel panel3;
        private Panel panel2;
        private Panel panel4;
        private GroupBox groupBox4;
        private FlowLayoutPanel flowLayoutPanel1;
        private LinkLabel linkLabel7;
        private FlowLayoutPanel flowLayoutPanel2;
        private Panel panel5;
        private LinkLabel linkLabel8;
        private LinkLabel linkLabel9;
    }
}
