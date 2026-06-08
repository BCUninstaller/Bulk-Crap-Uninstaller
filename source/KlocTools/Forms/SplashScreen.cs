/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Klocman.Forms.Tools;

namespace Klocman.Forms
{
    /// <summary>
    ///     A Windows 10 Store App styled splash screen. It will draw over form's client area and fade out when needed.
    /// </summary>
    public class SplashScreen : ReferencedComponent
    {
        private OverlaySplashScreen _splash;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image SplashScreenImage { get; set; }

        [DefaultValue(true)]
        public bool AutomaticallyClose { get; set; } = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "<Pending>")]
        private Form _ownerForm;

        protected override void OnContainerControlChanged(object orig, EventArgs args)
        {
            _ownerForm = ContainerControl.FindForm();

            if (_ownerForm != null)
            {
                _ownerForm.Load += OnLoad;
                _ownerForm.Shown += OnShown;
            }
        }

        public void CloseSplashScreen()
        {
            if (_splash == null || _splash.IsDisposed) return;

            _splash.FadeOut();
            
            SetOwnerEnabled(true);
        }

        private void SetOwnerEnabled(bool enabled)
        {
            // Don't disable the entire main form to avoid it being unmovable
            foreach (Control control in ContainerControl.Controls)
                control.Enabled = enabled;
        }

        protected override void Dispose(bool disposing)
        {
            _splash?.Dispose();

            base.Dispose(disposing);
        }

        private void OnLoad(object orig, EventArgs e)
        {
            _ownerForm.Opacity = 0;
            _splash = new OverlaySplashScreen(_ownerForm, SplashScreenImage);
            _splash.Disposed += (sender, args) => _splash = null;
        }

        private void OnShown(object orig, EventArgs e)
        {
            var canSplash = _splash != null && !_splash.IsDisposed;
            if (canSplash)
            {
                SetOwnerEnabled(false);

                _splash.Opacity = 0;
                _splash.Show(ContainerControl);
                _splash.Refresh();
                _splash.Opacity = 1;
            }

            _ownerForm.Opacity = 1;

            if(canSplash && AutomaticallyClose)
                CloseSplashScreen();
        }
    }
}