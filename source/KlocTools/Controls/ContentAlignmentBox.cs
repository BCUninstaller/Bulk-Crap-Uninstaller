/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Klocman.Controls
{
    public partial class ContentAlignmentBox : UserControl
    {
        private readonly IEnumerable<CheckBox> _checkBoxes;
        private readonly object _lock = new();
        private bool _suppress;

        public ContentAlignmentBox()
        {
            InitializeComponent();
            _checkBoxes = tableLayoutPanel2.Controls.Cast<Control>().OfType<CheckBox>();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment SelectedContentAlignment
        {
            get
            {
                lock (_lock)
                {
                    var box = _checkBoxes.Single(x => x.Checked);
                    if (box == checkBox1)
                        return ContentAlignment.TopLeft;
                    if (box == checkBox2)
                        return ContentAlignment.TopCenter;
                    if (box == checkBox3)
                        return ContentAlignment.TopRight;
                    if (box == checkBox4)
                        return ContentAlignment.MiddleLeft;
                    if (box == checkBox5)
                        return ContentAlignment.MiddleCenter;
                    if (box == checkBox6)
                        return ContentAlignment.MiddleRight;
                    if (box == checkBox7)
                        return ContentAlignment.BottomLeft;
                    if (box == checkBox8)
                        return ContentAlignment.BottomCenter;
                    if (box == checkBox9)
                        return ContentAlignment.BottomRight;

                    throw new InvalidOperationException();
                }
            }
            set
            {
                lock (_lock)
                {
                    switch (value)
                    {
                        case ContentAlignment.TopLeft:
                            checkBox1.Checked = true;
                            break;
                        case ContentAlignment.TopCenter:
                            checkBox2.Checked = true;
                            break;
                        case ContentAlignment.TopRight:
                            checkBox3.Checked = true;
                            break;
                        case ContentAlignment.MiddleLeft:
                            checkBox4.Checked = true;
                            break;
                        case ContentAlignment.MiddleCenter:
                            checkBox5.Checked = true;
                            break;
                        case ContentAlignment.MiddleRight:
                            checkBox6.Checked = true;
                            break;
                        case ContentAlignment.BottomLeft:
                            checkBox7.Checked = true;
                            break;
                        case ContentAlignment.BottomCenter:
                            checkBox8.Checked = true;
                            break;
                        case ContentAlignment.BottomRight:
                            checkBox9.Checked = true;
                            break;

                        default:
                            throw new InvalidEnumArgumentException();
                    }
                }
            }
        }

        public event EventHandler SelectedContentAlignmentChanged;

        private void CheckedChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (_suppress) return;
                _suppress = true;

                var box = (CheckBox) sender;
                var changed = true;

                if (box.Checked)
                {
                    foreach (var checkedBox in _checkBoxes.Where(x => x != box))
                    {
                        checkedBox.Checked = false;
                    }
                }
                else if (_checkBoxes.All(x => !x.Checked))
                {
                    box.Checked = true;
                    changed = false;
                }

                _suppress = false;

                if (changed)
                    SelectedContentAlignmentChanged?.Invoke(sender, e);
            }
        }
    }
}