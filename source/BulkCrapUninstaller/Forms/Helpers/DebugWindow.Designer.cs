using System.ComponentModel;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Forms
{
    partial class DebugWindow
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
            groupBox1 = new GroupBox();
            checkBox1 = new CheckBox();
            button2 = new Button();
            button1 = new Button();
            button5 = new Button();
            groupBox3 = new GroupBox();
            panel1 = new Panel();
            panel2 = new Panel();
            buttonGoMessages = new Button();
            numericUpDownMessages = new NumericUpDown();
            textBoxMessages = new TextBox();
            comboBoxMessages = new ComboBox();
            groupBox4 = new GroupBox();
            labelVersion = new Label();
            button3 = new Button();
            button4 = new Button();
            groupBox5 = new GroupBox();
            checkBoxDebug = new CheckBox();
            checkBox2 = new CheckBox();
            groupBox6 = new GroupBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            button10 = new Button();
            button11 = new Button();
            button12 = new Button();
            button13 = new Button();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            button14 = new Button();
            checkBoxSysRestoreAvail = new CheckBox();
            button15 = new Button();
            button16 = new Button();
            button17 = new Button();
            button18 = new Button();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((ISupportInitialize)numericUpDownMessages).BeginInit();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(button5);
            groupBox1.Dock = DockStyle.Bottom;
            groupBox1.Location = new System.Drawing.Point(8, 403);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(7);
            groupBox1.Size = new System.Drawing.Size(571, 58);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Settings";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Dock = DockStyle.Left;
            checkBox1.Location = new System.Drawing.Point(383, 23);
            checkBox1.Margin = new Padding(4, 3, 4, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(129, 28);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "Test setting binding";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.AutoSize = true;
            button2.Dock = DockStyle.Left;
            button2.Location = new System.Drawing.Point(265, 23);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(118, 28);
            button2.TabIndex = 0;
            button2.Text = "External change";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.AutoSize = true;
            button1.Dock = DockStyle.Left;
            button1.Location = new System.Drawing.Point(165, 23);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(100, 28);
            button1.TabIndex = 2;
            button1.Text = "Force update";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // button5
            // 
            button5.AutoSize = true;
            button5.Dock = DockStyle.Left;
            button5.Location = new System.Drawing.Point(7, 23);
            button5.Margin = new Padding(4, 3, 4, 3);
            button5.Name = "button5";
            button5.Size = new System.Drawing.Size(158, 28);
            button5.TabIndex = 3;
            button5.Text = "Open settings window";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(panel1);
            groupBox3.Controls.Add(comboBoxMessages);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new System.Drawing.Point(8, 168);
            groupBox3.Margin = new Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(7);
            groupBox3.Size = new System.Drawing.Size(571, 177);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Test message boxes";
            // 
            // panel1
            // 
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(textBoxMessages);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(7, 46);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(0, 7, 0, 0);
            panel1.Size = new System.Drawing.Size(557, 124);
            panel1.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.Controls.Add(buttonGoMessages);
            panel2.Controls.Add(numericUpDownMessages);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(462, 7);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(7, 0, 0, 0);
            panel2.Size = new System.Drawing.Size(95, 117);
            panel2.TabIndex = 3;
            // 
            // buttonGoMessages
            // 
            buttonGoMessages.Dock = DockStyle.Fill;
            buttonGoMessages.Location = new System.Drawing.Point(7, 23);
            buttonGoMessages.Margin = new Padding(4, 3, 4, 3);
            buttonGoMessages.Name = "buttonGoMessages";
            buttonGoMessages.Size = new System.Drawing.Size(88, 94);
            buttonGoMessages.TabIndex = 1;
            buttonGoMessages.Text = "Open messagebox";
            buttonGoMessages.UseVisualStyleBackColor = true;
            buttonGoMessages.Click += buttonGoMessages_Click;
            // 
            // numericUpDownMessages
            // 
            numericUpDownMessages.Dock = DockStyle.Top;
            numericUpDownMessages.Location = new System.Drawing.Point(7, 0);
            numericUpDownMessages.Margin = new Padding(4, 3, 4, 3);
            numericUpDownMessages.Name = "numericUpDownMessages";
            numericUpDownMessages.Size = new System.Drawing.Size(88, 23);
            numericUpDownMessages.TabIndex = 3;
            // 
            // textBoxMessages
            // 
            textBoxMessages.Dock = DockStyle.Left;
            textBoxMessages.Location = new System.Drawing.Point(0, 7);
            textBoxMessages.Margin = new Padding(4, 3, 4, 3);
            textBoxMessages.Multiline = true;
            textBoxMessages.Name = "textBoxMessages";
            textBoxMessages.Size = new System.Drawing.Size(462, 117);
            textBoxMessages.TabIndex = 2;
            textBoxMessages.Text = "Text to send to the messagebox\r\nSecond line if the box takes multiple lines\r\nIf messagebox needs a number it takes it from the number box on right";
            // 
            // comboBoxMessages
            // 
            comboBoxMessages.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxMessages.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBoxMessages.Dock = DockStyle.Top;
            comboBoxMessages.FormattingEnabled = true;
            comboBoxMessages.Location = new System.Drawing.Point(7, 23);
            comboBoxMessages.Margin = new Padding(4, 3, 4, 3);
            comboBoxMessages.Name = "comboBoxMessages";
            comboBoxMessages.Size = new System.Drawing.Size(557, 23);
            comboBoxMessages.TabIndex = 0;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(labelVersion);
            groupBox4.Controls.Add(button3);
            groupBox4.Controls.Add(button4);
            groupBox4.Dock = DockStyle.Bottom;
            groupBox4.Location = new System.Drawing.Point(8, 345);
            groupBox4.Margin = new Padding(4, 3, 4, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(7);
            groupBox4.Size = new System.Drawing.Size(571, 58);
            groupBox4.TabIndex = 4;
            groupBox4.TabStop = false;
            groupBox4.Text = "Update system";
            // 
            // labelVersion
            // 
            labelVersion.Dock = DockStyle.Fill;
            labelVersion.Location = new System.Drawing.Point(227, 23);
            labelVersion.Margin = new Padding(4, 0, 4, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new System.Drawing.Size(337, 28);
            labelVersion.TabIndex = 3;
            labelVersion.Text = "remote version";
            labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button3
            // 
            button3.AutoSize = true;
            button3.Dock = DockStyle.Left;
            button3.Location = new System.Drawing.Point(139, 23);
            button3.Margin = new Padding(4, 3, 4, 3);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(88, 28);
            button3.TabIndex = 2;
            button3.Text = "Download";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.AutoSize = true;
            button4.Dock = DockStyle.Left;
            button4.Location = new System.Drawing.Point(7, 23);
            button4.Margin = new Padding(4, 3, 4, 3);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(132, 28);
            button4.TabIndex = 0;
            button4.Text = "Check for updates";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(checkBoxDebug);
            groupBox5.Controls.Add(checkBox2);
            groupBox5.Dock = DockStyle.Bottom;
            groupBox5.Location = new System.Drawing.Point(8, 461);
            groupBox5.Margin = new Padding(4, 3, 4, 3);
            groupBox5.Name = "groupBox5";
            groupBox5.Padding = new Padding(7);
            groupBox5.Size = new System.Drawing.Size(571, 58);
            groupBox5.TabIndex = 5;
            groupBox5.TabStop = false;
            groupBox5.Text = "Misc";
            // 
            // checkBoxDebug
            // 
            checkBoxDebug.AutoSize = true;
            checkBoxDebug.Dock = DockStyle.Left;
            checkBoxDebug.Location = new System.Drawing.Point(162, 23);
            checkBoxDebug.Margin = new Padding(4, 3, 4, 3);
            checkBoxDebug.Name = "checkBoxDebug";
            checkBoxDebug.Size = new System.Drawing.Size(132, 28);
            checkBoxDebug.TabIndex = 1;
            checkBoxDebug.Text = "Enable debug mode";
            checkBoxDebug.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Dock = DockStyle.Left;
            checkBox2.Location = new System.Drawing.Point(7, 23);
            checkBox2.Margin = new Padding(4, 3, 4, 3);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new System.Drawing.Size(155, 28);
            checkBox2.TabIndex = 0;
            checkBox2.Text = "Override BCU is installed";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(flowLayoutPanel1);
            groupBox6.Dock = DockStyle.Top;
            groupBox6.Location = new System.Drawing.Point(8, 8);
            groupBox6.Margin = new Padding(4, 3, 4, 3);
            groupBox6.Name = "groupBox6";
            groupBox6.Padding = new Padding(7);
            groupBox6.Size = new System.Drawing.Size(571, 160);
            groupBox6.TabIndex = 6;
            groupBox6.TabStop = false;
            groupBox6.Text = "Test methods";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(button10);
            flowLayoutPanel1.Controls.Add(button11);
            flowLayoutPanel1.Controls.Add(button12);
            flowLayoutPanel1.Controls.Add(button13);
            flowLayoutPanel1.Controls.Add(button6);
            flowLayoutPanel1.Controls.Add(button7);
            flowLayoutPanel1.Controls.Add(button8);
            flowLayoutPanel1.Controls.Add(button9);
            flowLayoutPanel1.Controls.Add(button14);
            flowLayoutPanel1.Controls.Add(checkBoxSysRestoreAvail);
            flowLayoutPanel1.Controls.Add(button15);
            flowLayoutPanel1.Controls.Add(button16);
            flowLayoutPanel1.Controls.Add(button17);
            flowLayoutPanel1.Controls.Add(button18);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(7, 23);
            flowLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(557, 130);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // button10
            // 
            button10.AutoSize = true;
            button10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button10.Location = new System.Drawing.Point(4, 3);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new System.Drawing.Size(110, 25);
            button10.TabIndex = 7;
            button10.Text = "Test junk searcher";
            button10.UseVisualStyleBackColor = true;
            button10.Click += TestJunkSearcher;
            // 
            // button11
            // 
            button11.AutoSize = true;
            button11.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button11.Location = new System.Drawing.Point(122, 3);
            button11.Margin = new Padding(4, 3, 4, 3);
            button11.Name = "button11";
            button11.Size = new System.Drawing.Size(151, 25);
            button11.TabIndex = 6;
            button11.Text = "Crash background thread";
            button11.UseVisualStyleBackColor = true;
            button11.Click += TestCrashBackgroundThread;
            // 
            // button12
            // 
            button12.AutoSize = true;
            button12.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button12.Location = new System.Drawing.Point(281, 3);
            button12.Margin = new Padding(4, 3, 4, 3);
            button12.Name = "button12";
            button12.Size = new System.Drawing.Size(97, 25);
            button12.TabIndex = 5;
            button12.Text = "Crash ui thread";
            button12.UseVisualStyleBackColor = true;
            button12.Click += TestCrashUiThread;
            // 
            // button13
            // 
            button13.AutoSize = true;
            button13.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button13.Location = new System.Drawing.Point(386, 3);
            button13.Margin = new Padding(4, 3, 4, 3);
            button13.Name = "button13";
            button13.Size = new System.Drawing.Size(130, 25);
            button13.TabIndex = 4;
            button13.Text = "Finish collecting stats";
            button13.UseVisualStyleBackColor = true;
            button13.Click += button13_Click;
            // 
            // button6
            // 
            button6.AutoSize = true;
            button6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button6.Location = new System.Drawing.Point(4, 34);
            button6.Margin = new Padding(4, 3, 4, 3);
            button6.Name = "button6";
            button6.Size = new System.Drawing.Size(70, 25);
            button6.TabIndex = 8;
            button6.Text = "Send stats";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button7
            // 
            button7.AutoSize = true;
            button7.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button7.Location = new System.Drawing.Point(82, 34);
            button7.Margin = new Padding(4, 3, 4, 3);
            button7.Name = "button7";
            button7.Size = new System.Drawing.Size(76, 25);
            button7.TabIndex = 9;
            button7.Text = "Restart app";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // button8
            // 
            button8.AutoSize = true;
            button8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button8.Location = new System.Drawing.Point(166, 34);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new System.Drawing.Size(92, 25);
            button8.TabIndex = 10;
            button8.Text = "Get hotkey list";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button9
            // 
            button9.AutoSize = true;
            button9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button9.Location = new System.Drawing.Point(266, 34);
            button9.Margin = new Padding(4, 3, 4, 3);
            button9.Name = "button9";
            button9.Size = new System.Drawing.Size(69, 25);
            button9.TabIndex = 11;
            button9.Text = "Soft crash";
            button9.UseVisualStyleBackColor = true;
            button9.Click += SoftCrash;
            // 
            // button14
            // 
            button14.AutoSize = true;
            button14.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.SetFlowBreak(button14, true);
            button14.Location = new System.Drawing.Point(343, 34);
            button14.Margin = new Padding(4, 3, 4, 3);
            button14.Name = "button14";
            button14.Size = new System.Drawing.Size(90, 25);
            button14.TabIndex = 12;
            button14.Text = "Feedback box";
            button14.UseVisualStyleBackColor = true;
            button14.Click += button14_Click;
            // 
            // checkBoxSysRestoreAvail
            // 
            checkBoxSysRestoreAvail.AutoSize = true;
            checkBoxSysRestoreAvail.Dock = DockStyle.Left;
            checkBoxSysRestoreAvail.Enabled = false;
            checkBoxSysRestoreAvail.Location = new System.Drawing.Point(4, 65);
            checkBoxSysRestoreAvail.Margin = new Padding(4, 3, 4, 3);
            checkBoxSysRestoreAvail.Name = "checkBoxSysRestoreAvail";
            checkBoxSysRestoreAvail.Size = new System.Drawing.Size(131, 25);
            checkBoxSysRestoreAvail.TabIndex = 16;
            checkBoxSysRestoreAvail.Text = "SysRestore available";
            checkBoxSysRestoreAvail.UseVisualStyleBackColor = true;
            // 
            // button15
            // 
            button15.AutoSize = true;
            button15.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button15.Location = new System.Drawing.Point(143, 65);
            button15.Margin = new Padding(4, 3, 4, 3);
            button15.Name = "button15";
            button15.Size = new System.Drawing.Size(157, 25);
            button15.TabIndex = 13;
            button15.Text = "Start restore point creation";
            button15.UseVisualStyleBackColor = true;
            button15.Click += button15_Click;
            // 
            // button16
            // 
            button16.AutoSize = true;
            button16.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button16.Location = new System.Drawing.Point(308, 65);
            button16.Margin = new Padding(4, 3, 4, 3);
            button16.Name = "button16";
            button16.Size = new System.Drawing.Size(76, 25);
            button16.TabIndex = 14;
            button16.Text = "EndRestore";
            button16.UseVisualStyleBackColor = true;
            button16.Click += button16_Click;
            // 
            // button17
            // 
            button17.AutoSize = true;
            button17.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button17.Location = new System.Drawing.Point(392, 65);
            button17.Margin = new Padding(4, 3, 4, 3);
            button17.Name = "button17";
            button17.Size = new System.Drawing.Size(92, 25);
            button17.TabIndex = 15;
            button17.Text = "CancelRestore";
            button17.UseVisualStyleBackColor = true;
            button17.Click += button17_Click;
            // 
            // button18
            // 
            button18.AutoSize = true;
            button18.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button18.Location = new System.Drawing.Point(4, 96);
            button18.Margin = new Padding(4, 3, 4, 3);
            button18.Name = "button18";
            button18.Size = new System.Drawing.Size(134, 25);
            button18.TabIndex = 17;
            button18.Text = "Simulate outdated W7";
            button18.UseVisualStyleBackColor = true;
            button18.Click += button18_Click;
            // 
            // DebugWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(587, 527);
            Controls.Add(groupBox3);
            Controls.Add(groupBox4);
            Controls.Add(groupBox1);
            Controls.Add(groupBox6);
            Controls.Add(groupBox5);
            Margin = new Padding(4, 3, 4, 3);
            Name = "DebugWindow";
            Padding = new Padding(8);
            Text = "DebugWindow (here be dragons)";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ((ISupportInitialize)numericUpDownMessages).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private GroupBox groupBox1;
        private CheckBox checkBox1;
        private Button button2;
        private Button button1;
        private GroupBox groupBox3;
        private TextBox textBoxMessages;
        private Button buttonGoMessages;
        private ComboBox comboBoxMessages;
        private NumericUpDown numericUpDownMessages;
        private GroupBox groupBox4;
        private Label labelVersion;
        private Button button3;
        private Button button4;
        private Button button5;
        private GroupBox groupBox5;
        private CheckBox checkBox2;
        private GroupBox groupBox6;
        private Panel panel1;
        private Panel panel2;
        private CheckBox checkBoxDebug;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button button10;
        private Button button11;
        private Button button12;
        private Button button13;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button14;
        private Button button15;
        private Button button16;
        private Button button17;
        private CheckBox checkBoxSysRestoreAvail;
        private Button button18;
    }
}