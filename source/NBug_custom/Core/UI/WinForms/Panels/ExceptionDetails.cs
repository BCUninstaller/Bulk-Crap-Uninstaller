// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionDetails.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NBug.Core.Util.Serialization;

namespace NBug.Core.UI.WinForms.Panels
{
    internal partial class ExceptionDetails : UserControl
    {
        private readonly Dictionary<TreeNode, SerializableException> exceptionDetailsList = new();

        public ExceptionDetails()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int InformationColumnWidth
        {
            get { return exceptionDetailsListView.Columns[1].Width; }

            set { exceptionDetailsListView.Columns[1].Width = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int PropertyColumnWidth
        {
            get { return exceptionDetailsListView.Columns[0].Width; }

            set { exceptionDetailsListView.Columns[0].Width = value; }
        }

        internal void Initialize(SerializableException exception)
        {
            exceptionDetailsList.Add(exceptionTreeView.Nodes.Add(exception.Type), exception);

            if (exception.InnerException != null)
            {
                FillInnerExceptionTree(exception.InnerException, exceptionTreeView.Nodes[0]);
            }

            if (exception.InnerExceptions != null)
            {
                foreach (var innerException in exception.InnerExceptions)
                {
                    FillInnerExceptionTree(innerException, exceptionTreeView.Nodes[0]);
                }
            }

            exceptionTreeView.ExpandAll();
            DisplayExceptionDetails(exceptionTreeView.Nodes[0]);
        }

        private void DisplayExceptionDetails(TreeNode node)
        {
            var exception = exceptionDetailsList[node];
            exceptionDetailsListView.SuspendLayout();
            exceptionDetailsListView.Items.Clear();

            if (exception.Type != null)
            {
                exceptionDetailsListView.Items.Add("Exception").SubItems.Add(exception.Type);
            }

            if (exception.Message != null)
            {
                exceptionDetailsListView.Items.Add("Message").SubItems.Add(exception.Message);
            }

            if (exception.TargetSite != null)
            {
                exceptionDetailsListView.Items.Add("Target Site").SubItems.Add(exception.TargetSite);
            }

            if (exception.InnerException != null)
            {
                exceptionDetailsListView.Items.Add("Inner Exception").SubItems.Add(exception.InnerException.Type);
            }

            if (exception.Source != null)
            {
                exceptionDetailsListView.Items.Add("Source").SubItems.Add(exception.Source);
            }

            if (exception.HelpLink != null)
            {
                exceptionDetailsListView.Items.Add("Help Link").SubItems.Add(exception.HelpLink);
            }

            if (exception.StackTrace != null)
            {
                exceptionDetailsListView.Items.Add("Stack Trace").SubItems.Add(exception.StackTrace);
            }

            if (exception.Data != null)
            {
                foreach (var pair in exception.Data)
                {
                    exceptionDetailsListView.Items.Add(string.Format("Data[\"{0}\"]", pair.Key))
                        .SubItems.Add(pair.Value.ToString());
                }
            }

            if (exception.ExtendedInformation != null)
            {
                foreach (var info in exception.ExtendedInformation)
                {
                    var item = exceptionDetailsListView.Items.Add(info.Key);
                    item.UseItemStyleForSubItems = false;
                    item.Font = new Font(Font, FontStyle.Bold);
                    item.SubItems.Add(info.Value.ToString());
                }
            }

            exceptionDetailsListView.ResumeLayout();
        }

        private void ExceptionDetailsListView_DoubleClick(object sender, EventArgs e)
        {
            using (var detailView = new ExceptionDetailView())
            {
                detailView.ShowDialog(exceptionDetailsListView.SelectedItems[0].Text,
                    exceptionDetailsListView.SelectedItems[0].SubItems[1].Text);
            }
        }

        private void ExceptionDetailsListView_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            toolTip.RemoveAll();
            toolTip.Show(e.Item.SubItems[1].Text, exceptionDetailsListView);
        }

        private void ExceptionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DisplayExceptionDetails(e.Node);
        }

        private void FillInnerExceptionTree(SerializableException innerException, TreeNode innerNode)
        {
            exceptionDetailsList.Add(innerNode.Nodes.Add(innerException.Type), innerException);

            if (innerException.InnerException != null)
            {
                FillInnerExceptionTree(innerException.InnerException, innerNode.Nodes[0]);
            }
        }
    }
}