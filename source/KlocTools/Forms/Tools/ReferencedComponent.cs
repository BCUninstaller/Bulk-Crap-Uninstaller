/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Klocman.Forms.Tools
{
    public abstract class ReferencedComponent : Component 
    {
        private ContainerControl _containerControl;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContainerControl ContainerControl
        {
            get { return _containerControl; }
            set
            {
                if (!DesignMode)
                {
                    if (_containerControl != null)
                        throw new InvalidOperationException("ContainerControl can be set only once");

                    _containerControl = value;

                    if (_containerControl.Visible)
                        ContainerVisibleChanged(this, EventArgs.Empty);
                    else
                        _containerControl.VisibleChanged += ContainerVisibleChanged;
                }
                else
                {
                    _containerControl = value;
                }

                OnContainerControlChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Automatically populate ContainerForm when added using designer
        /// </summary>
        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;
                if (value == null)
                    return;

                var host = value.GetService(typeof (IDesignerHost)) as IDesignerHost;
                if (host?.RootComponent is ContainerControl control)
                    ContainerControl = control;
            }
        }

        private void ContainerVisibleChanged(object obj, EventArgs args)
        {
            if (!_containerControl.Visible) return;
            _containerControl.VisibleChanged -= ContainerVisibleChanged;

            OnContainerInitialized(obj, args);
        }

        protected virtual void OnContainerControlChanged(object obj, EventArgs args)
        {
        }

        protected virtual void OnContainerInitialized(object obj, EventArgs args)
        {
        }
    }
}