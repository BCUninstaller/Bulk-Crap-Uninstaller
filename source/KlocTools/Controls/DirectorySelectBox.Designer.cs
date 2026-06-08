using System.ComponentModel;
using System.Windows.Forms;

namespace Klocman.Controls
{
    partial class DirectorySelectBox
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(DirectorySelectBox));
            textBox1 = new TextBox();
            button1 = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.AllowDrop = true;
            resources.ApplyResources(textBox1, "textBox1");
            textBox1.Name = "textBox1";
            textBox1.DragDrop += OnDragDrop;
            textBox1.DragEnter += OnDragEnter;
            textBox1.Validating += textBox1_Validating;
            // 
            // button1
            // 
            button1.AllowDrop = true;
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            button1.DragDrop += OnDragDrop;
            button1.DragEnter += OnDragEnter;
            // 
            // panel1
            // 
            panel1.Controls.Add(textBox1);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // DirectorySelectBox
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(button1);
            Name = "DirectorySelectBox";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private TextBox textBox1;
        private Button button1;
        private FolderBrowserDialog folderBrowserDialog;
        private Panel panel1;
    }
}
