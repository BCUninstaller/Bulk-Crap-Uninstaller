using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Klocman.Extensions;
using Klocman.Tools;

namespace Klocman.Forms
{
    public sealed partial class CustomMessageBox : Form
    {
        /// <summary>
        /// Optional application default, set before showing dialogs. Per-dialog settings take precedence.
        /// </summary>
        public static Color? DefaultHeadingColor { get; set; }

        public enum PressedButton
        {
            None,
            Right,
            Middle,
            Left
        }

        private readonly CmbCheckboxSettings _checkboxSettings;
        private readonly Icon _overrideIcon;
        private int _topWidth, _topHeigth;

        private CustomMessageBox(CmbBasicSettings settings, CmbCheckboxSettings checkboxSettings)
        {
            Opacity = 0;

            Result = PressedButton.None;
            if (SystemFonts.MessageBoxFont != null)
                Font = SystemFonts.MessageBoxFont;
            ForeColor = SystemColors.WindowText;

            InitializeComponent();

            StartPosition = settings.StartPosition;

            if (checkboxSettings != null)
            {
                _checkboxSettings = checkboxSettings;

                if (checkboxSettings.Text.IsNotEmpty())
                    checkBox1.Text = checkboxSettings.Text;
                checkBox1.Checked = checkboxSettings.InitialState;
                checkBox1.Visible = true;
            }
            else
            {
                checkBox1.Visible = false;
            }

            if (!string.IsNullOrEmpty(settings.SmallExplanation) && SystemFonts.MessageBoxFont != null)
            {
                if (SystemFonts.MessageBoxFont.FontFamily.Name == "Segoe UI"
                    && SystemFonts.MessageBoxFont.FontFamily.IsStyleAvailable(FontStyle.Regular))
                {
                    // use the special, windows-vista font style if we are running vista (using Segoe UI).
                    label1.ForeColor = Color.FromArgb(0, 51, 153); // [ColorTranslator.FromHtml("#003399")]
                    label1.Font = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Point);
                }
                else
                {
                    // might want to special case the old, MS Sans Serif font.
                    // use the regular font but bold it for XP, etc.
                    label1.Font = new Font(SystemFonts.MessageBoxFont.FontFamily.Name, 8.0f,
                        FontStyle.Bold, GraphicsUnit.Point);
                }
            }

            var headingColor = settings.HeadingColor ?? DefaultHeadingColor;
            if (headingColor.HasValue)
                label1.ForeColor = headingColor.Value;

            Text = settings.Title;
            label1.Text = settings.LargeHeading;
            label2.Text = string.IsNullOrEmpty(settings.SmallExplanation) ? string.Empty : settings.SmallExplanation;

            if (!string.IsNullOrEmpty(settings.LeftButton))
            {
                // if we have the button, we are fine
                buttonLeft.Text = settings.LeftButton;
            }
            else
            {
                // move settings to right button if we don't have the left button
                AcceptButton = buttonMiddle;
                buttonLeft.Visible = false;
                panelLeft.Visible = false;
            }

            if (!string.IsNullOrEmpty(settings.MiddleButton))
            {
                // if we have the button, we are fine
                buttonMiddle.Text = settings.MiddleButton;
            }
            else
            {
                // move settings to right button if we don't have the left button
                AcceptButton = buttonRight;
                buttonMiddle.Visible = false;
                panelMiddle.Visible = false;
            }

            // this button must always be present
            buttonRight.Text = settings.RightButton;

            if (settings.IconSet != null)
            {
                try
                {
                    pictureBox1.Image = settings.IconSet.ToBitmap();
                    settings.IconSet.PlayCorrespondingSystemSound();
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e);
                }
            }

            TopMost = settings.AlwaysOnTop;

            _overrideIcon = settings.WindowIcon;
        }

        internal bool Checked => checkBox1.Checked;

        public PressedButton Result { get; private set; }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            Result = PressedButton.Left;
            Close();
        }

        private void buttonMiddle_Click(object sender, EventArgs e)
        {
            Result = PressedButton.Middle;
            Close();
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            Result = PressedButton.Right;
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (_checkboxSettings != null)
            {
                var enable = !checkBox1.Checked;

                if (_checkboxSettings.DisableRightButton)
                    buttonRight.Enabled = enable;

                if (_checkboxSettings.DisableMiddleButton)
                    buttonMiddle.Enabled = enable;

                if (_checkboxSettings.DisableLeftButton)
                    buttonLeft.Enabled = enable;
            }
        }

        private void CustomMessageBox_SizeChanged(object sender, EventArgs e)
        {
            // Needed to prevent the size from shrinking by itself
            Width = _topWidth;
            Height = _topHeigth;
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            SetHeight();
            SetWidth();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (_overrideIcon != null)
                Icon = _overrideIcon;
            else if (ParentForm?.Icon != null)
                Icon = ParentForm.Icon;
            else
            {
                try
                {
                    Icon = ProcessTools.GetIconFromEntryExe();
                }
                catch
                {
                    /* Fall back to the default icon */
                }
            }

            SetHeight();
            SetWidth();

            if (StartPosition == FormStartPosition.CenterParent && Owner != null && Owner.Visible)
                this.CenterToForm(Owner);

            FocusDefaultAction();

            Opacity = 1;
        }

        private void FocusDefaultAction()
        {
            if (buttonLeft.Visible && buttonLeft.Enabled)
                buttonLeft.Focus();
            else if (buttonMiddle.Visible && buttonMiddle.Enabled)
                buttonMiddle.Focus();
            else if (buttonRight.Visible && buttonRight.Enabled)
                buttonRight.Focus();
            else if (buttonLeft.Visible)
                buttonLeft.Focus();
            else if (buttonMiddle.Visible)
                buttonMiddle.Focus();
            else
                buttonRight.Focus();
        }

        private void SetHeight()
        {
            var border = Height - ClientSize.Height;
            _topHeigth = Math.Max(panelButtons.Location.Y + panelButtons.Height + border, _topHeigth);
            Height = _topHeigth;
        }

        private void SetWidth()
        {
            var border = Width - ClientSize.Width;
            var buttonWidth = panelButtons.Controls.Cast<Control>().Where(x => x.Visible).Sum(x => x.Width)
                              + panelButtons.Padding.Left + panelButtons.Padding.Right + border;
            _topWidth = Math.Max(buttonWidth, _topWidth);
            Width = _topWidth;
        }

        public static CustomMessageBox Show(Form owner, CmbBasicSettings settings)
        {
            return Show(owner, settings, null);
        }

        public static CustomMessageBox Show(Form owner, CmbBasicSettings settings, CmbCheckboxSettings checkboxSettings)
        {
            if (owner.IsDisposed || owner.Disposing || !owner.Visible) owner = null;

            var dialog = new CustomMessageBox(settings, checkboxSettings) { Owner = owner }; //, hyperlinkSettings))

            dialog.Show(owner);

            if (dialog.Result != PressedButton.None && checkboxSettings != null)
                checkboxSettings.Result = dialog.Checked;

            return dialog;
        }

        public static PressedButton ShowDialog(Form owner, CmbBasicSettings settings,
            CmbCheckboxSettings checkboxSettings)
        {
            return ShowDialog(owner, settings, checkboxSettings, null);
        }

        public static PressedButton ShowDialog(Form owner, CmbBasicSettings settings,
            CmbHyperlinkSettings hyperlinkSettings)
        {
            return ShowDialog(owner, settings, null, hyperlinkSettings);
        }

        public static PressedButton ShowDialog(Form owner, CmbBasicSettings settings)
        {
            var result = PressedButton.None;

            void ShowDialogLocal() { result = ShowDialog(owner, settings, null, null); }

            if (owner != null)
                owner.SafeInvoke(ShowDialogLocal);
            else
                ShowDialogLocal();

            return result;
        }

        public static PressedButton ShowDialog(Form owner, CmbBasicSettings settings,
            CmbCheckboxSettings checkboxSettings,
            CmbHyperlinkSettings hyperlinkSettings)
        {
            using (var dialog = new CustomMessageBox(settings, checkboxSettings)) //, hyperlinkSettings))
            {
                if (owner != null && !owner.IsDisposed && !owner.Disposing)
                {
                    if (owner.Visible)
                    {
                        dialog.Owner = owner;
                        owner.Select();
                        dialog.ShowDialog(owner);
                    }
                    else
                    {
                        dialog.CenterToForm(owner);
                        dialog.ShowDialog();
                    }
                }
                else
                {
                    dialog.StartPosition = FormStartPosition.CenterScreen;
                    dialog.ShowDialog();
                }

                if (dialog.Result != PressedButton.None && checkboxSettings != null)
                    checkboxSettings.Result = dialog.Checked;

                return dialog.Result;
            }
        }
    }
}
