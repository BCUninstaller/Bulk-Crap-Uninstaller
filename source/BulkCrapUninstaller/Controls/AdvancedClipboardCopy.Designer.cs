namespace BulkCrapUninstaller.Controls
{
    partial class AdvancedClipboardCopy
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedClipboardCopy));
            groupBox1 = new System.Windows.Forms.GroupBox();
            panel2 = new System.Windows.Forms.Panel();
            comboBoxInsert = new System.Windows.Forms.ComboBox();
            panel3 = new System.Windows.Forms.Panel();
            checkBoxUnescape = new System.Windows.Forms.CheckBox();
            buttonHelp = new System.Windows.Forms.Button();
            textBoxPatternInput = new System.Windows.Forms.TextBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            textBoxResults = new System.Windows.Forms.TextBox();
            panel1 = new System.Windows.Forms.Panel();
            groupBox1.SuspendLayout();
            panel2.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(panel2);
            groupBox1.Controls.Add(textBoxPatternInput);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.Controls.Add(comboBoxInsert);
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(checkBoxUnescape);
            panel2.Controls.Add(buttonHelp);
            panel2.Name = "panel2";
            // 
            // comboBoxInsert
            // 
            resources.ApplyResources(comboBoxInsert, "comboBoxInsert");
            comboBoxInsert.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxInsert.FormattingEnabled = true;
            comboBoxInsert.Items.AddRange(new object[] { resources.GetString("comboBoxInsert.Items") });
            comboBoxInsert.Name = "comboBoxInsert";
            comboBoxInsert.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // panel3
            // 
            resources.ApplyResources(panel3, "panel3");
            panel3.Name = "panel3";
            // 
            // checkBoxUnescape
            // 
            resources.ApplyResources(checkBoxUnescape, "checkBoxUnescape");
            checkBoxUnescape.Name = "checkBoxUnescape";
            checkBoxUnescape.UseVisualStyleBackColor = true;
            checkBoxUnescape.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // buttonHelp
            // 
            resources.ApplyResources(buttonHelp, "buttonHelp");
            buttonHelp.Name = "buttonHelp";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += buttonHelp_Click;
            // 
            // textBoxPatternInput
            // 
            resources.ApplyResources(textBoxPatternInput, "textBoxPatternInput");
            textBoxPatternInput.Name = "textBoxPatternInput";
            textBoxPatternInput.TextChanged += RefreshResult;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(textBoxResults);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // textBoxResults
            // 
            textBoxResults.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(textBoxResults, "textBoxResults");
            textBoxResults.Name = "textBoxResults";
            textBoxResults.ReadOnly = true;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // AdvancedClipboardCopy
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(groupBox2);
            Controls.Add(panel1);
            Controls.Add(groupBox1);
            Name = "AdvancedClipboardCopy";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxPatternInput;
        private System.Windows.Forms.TextBox textBoxResults;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.ComboBox comboBoxInsert;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBoxUnescape;
    }
}
