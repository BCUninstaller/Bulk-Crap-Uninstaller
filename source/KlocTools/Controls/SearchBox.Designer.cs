using System.ComponentModel;
using System.Windows.Forms;

namespace Klocman.Controls
{
    partial class SearchBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SearchBox));
            filteringTextBox = new TextBox();
            SuspendLayout();
            // 
            // filteringTextBox
            // 
            resources.ApplyResources(filteringTextBox, "filteringTextBox");
            filteringTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            filteringTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            filteringTextBox.BackColor = System.Drawing.SystemColors.Window;
            filteringTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            filteringTextBox.Name = "filteringTextBox";
            filteringTextBox.TextChanged += filteringTextBox_TextChanged;
            filteringTextBox.Enter += filteringTextBox_Enter;
            filteringTextBox.KeyDown += filteringTextBox_KeyDown;
            filteringTextBox.KeyUp += filteringTextBox_KeyUp;
            filteringTextBox.Leave += filteringTextBox_Leave;
            // 
            // SearchBox
            // 
            AutoScaleMode = AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            Controls.Add(filteringTextBox);
            Name = "SearchBox";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private TextBox filteringTextBox;
    }
}
