/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Forms
{
    internal partial class FeedbackWindow : Form
    {
        public FeedbackWindow()
        {
            InitializeComponent();
            ModernStyleController.ApplyOnLoad(this);
            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
            webBrowser.ScrollBarsEnabled = false;
            //webBrowser.Visible = false;
        }

        public static void ShowFeedbackDialog()
        {
            using (var fw = new FeedbackWindow())
            {
                fw.ShowDialog();
            }
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser.DocumentCompleted -= webBrowser_DocumentCompleted;

            try
            {
                var container = webBrowser.Document!.GetElementById("container");
                container!.InnerHtml = webBrowser.Document.GetElementById("content")!.InnerHtml;
                container.Style = "width:422px; margin:10px auto; padding:10px; align:center;";
                webBrowser.Document.Body!.Style = "padding:0px;";
            }
            catch
            {
                //Error while parsing, probably couldn't connect. Let the browser show the error.
            }

            loadingLabel.Visible = false;
            webBrowser.Visible = true;
        }

        private void FeedbackWindow_Shown(object sender, EventArgs e)
        {
            webBrowser.Navigate(new Uri(@"http://klocmansoftware.weebly.com/feedback--contact.html"));
        }
    }
}