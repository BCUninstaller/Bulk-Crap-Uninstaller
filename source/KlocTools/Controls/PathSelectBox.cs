/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Klocman.Controls
{
    /// <summary>
    ///     Path selection box with validation and filtering of the supplied path.
    ///     Browsing to find desired file is possible with a dedicated button.
    /// </summary>
    public sealed partial class PathSelectBox : UserControl
    {
        private readonly Regex _r;
        
        public PathSelectBox()
        {
            InitializeComponent();

            var regexSearch = new string(Path.GetInvalidPathChars());
            _r = new Regex($"[{Regex.Escape(regexSearch)}]");
        }
        
        public event EventHandler FileNameChanged;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string FileName
        {
            get { return textBox1.Text; }
            set
            {
                if (textBox1.Text != value)
                {
                    textBox1.Text = value;
                    OnFileNameChanged();
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue("")]
        public string Filter
        {
            get { return openFileDialog.Filter; }
            set { openFileDialog.Filter = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void OnFileNameChanged()
        {
            FileNameChanged?.Invoke(this, EventArgs.Empty);
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog.FileName;
        }

        private void textBox1_Changed(object sender, EventArgs e)
        {
            FileName = _r.Replace(FileName, string.Empty);
            OnFileNameChanged();
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Link;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            var dir = files.FirstOrDefault(File.Exists);

            if (!string.IsNullOrEmpty(dir))
                FileName = dir;
        }
    }
}