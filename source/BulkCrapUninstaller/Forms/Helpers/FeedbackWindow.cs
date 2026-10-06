/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Theming;

namespace BulkCrapUninstaller.Forms
{
    internal partial class FeedbackWindow : Form
    {
        public FeedbackWindow()
        {
            InitializeComponent();
            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
            webBrowser.ScrollBarsEnabled = false;
            webBrowser.TabStop = true;
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
            if (webBrowser.Url != e.Url)
                return;

            webBrowser.DocumentCompleted -= webBrowser_DocumentCompleted;

            try
            {
                ApplyDocumentStyle(webBrowser.Document);
            }
            catch
            {
                //Error while parsing, probably couldn't connect. Let the browser show the error.
            }

            loadingLabel.Visible = false;
            webBrowser.Visible = true;

            if (webBrowser.CanFocus)
                webBrowser.Focus();
        }

        private void FeedbackWindow_Shown(object sender, EventArgs e)
        {
            webBrowser.Navigate(new Uri(@"https://klocmansoftware.weebly.com/contact.html"));
        }

        private static void ApplyDocumentStyle(HtmlDocument document)
        {
            var body = document?.Body;
            if (body == null) return;

            var container = document.GetElementById("container");
            var content = document.GetElementById("content");
            if (container != null && content != null)
            {
                container.InnerHtml = content.InnerHtml;
            }
            else
            {
                content = document.GetElementById("wsite-content");
                if (content != null)
                {
                    var contentHtml = content.InnerHtml;
                    body.InnerHtml = "<div id=\"bcu-feedback-content\"></div>";
                    container = document.GetElementById("bcu-feedback-content");
                    if (container != null)
                        container.InnerHtml = contentHtml;
                }
            }

            body.Style = BuildBodyStyle();
            if (container != null)
                container.Style = BuildContainerStyle();

            ApplyDarkDocumentStyle(document);
        }

        private static void ApplyDarkDocumentStyle(HtmlDocument document)
        {
            if (!ThemeManager.IsEnabled) return;

            var heads = document.GetElementsByTagName("head");
            if (heads.Count == 0) return;

            var background = ToCssColor(SystemColors.Window);
            var foreground = ToCssColor(SystemColors.WindowText);
            var link = ToCssColor(Color.FromArgb(139, 194, 255));
            var css = $"html,body,body * {{ background-color:{background} !important; color:{foreground} !important; }} " +
                      $"body a,body a * {{ color:{link} !important; }}";
            var style = document.CreateElement("style");
            style.SetAttribute("type", "text/css");
            heads[0].AppendChild(style);
            dynamic styleElement = style.DomElement;
            styleElement.styleSheet.cssText = css;
        }

        private static string BuildBodyStyle()
        {
            if (!ThemeManager.IsEnabled) return "padding:0px;";

            var background = ToCssColor(SystemColors.Window);
            var foreground = ToCssColor(SystemColors.WindowText);
            return $"background-color:{background}; color:{foreground}; padding:0px;";
        }

        private static string BuildContainerStyle()
        {
            if (!ThemeManager.IsEnabled)
                return "width:422px; margin:10px auto; padding:10px; align:center;";

            var background = ToCssColor(SystemColors.Window);
            var foreground = ToCssColor(SystemColors.WindowText);
            return $"width:422px; margin:10px auto; padding:10px; align:center; " +
                   $"background-color:{background}; color:{foreground};";
        }

        private static string ToCssColor(Color color) =>
            ColorTranslator.ToHtml(Color.FromArgb(color.R, color.G, color.B));
    }
}
