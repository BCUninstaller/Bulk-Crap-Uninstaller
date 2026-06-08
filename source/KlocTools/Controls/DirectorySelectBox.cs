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
    public sealed partial class DirectorySelectBox : UserControl
    {
        #region Fields

        private readonly Regex _r;

        #endregion Fields

        #region Constructors

        public DirectorySelectBox()
        {
            InitializeComponent();

            var regexSearch = new string(Path.GetInvalidPathChars());
            _r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        }

        #endregion Constructors

        #region Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string DirectoryPath
        {
            get { return textBox1.Text; }
            set
            {
                if (textBox1.Text != value)
                {
                    textBox1.Text = value;
                    OnDirectoryPathChanged();
                }
            }
        }

        #endregion Properties

        #region Events

        public event EventHandler DirectoryPathChanged;

        #endregion Events

        #region Methods

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            DirectoryPath = folderBrowserDialog.SelectedPath;
        }

        private void OnDirectoryPathChanged()
        {
            DirectoryPathChanged?.Invoke(this, EventArgs.Empty);
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            DirectoryPath = _r.Replace(DirectoryPath, string.Empty);
            OnDirectoryPathChanged();
        }

        #endregion Methods

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Link;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            var dir = files.FirstOrDefault(Directory.Exists);

            if (!string.IsNullOrEmpty(dir))
                DirectoryPath = dir;
        }
    }
}