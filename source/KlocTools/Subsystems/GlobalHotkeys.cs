/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Klocman.Forms.Tools;

namespace Klocman.Subsystems
{
    public sealed class GlobalHotkeys : ReferencedComponent, ICollection<HotkeyEntry>
    {
        private readonly List<HotkeyEntry> _registeredHotkeys = new();
        private Form _parentForm;

        [Browsable(false)]
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Form ParentForm
        {
            get { return _parentForm; }
            set
            {
                if (_parentForm != null)
                {
                    _parentForm.KeyPreview = false;
                    _parentForm.KeyDown -= KeyDown_Handler;
                }

                if (value != null)
                {
                    value.KeyPreview = true;
                    value.KeyDown += KeyDown_Handler;
                }

                _parentForm = value;
            }
        }

        /// <summary>
        ///     Stop responding to keystrokes when the parent form is disabled
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool StopWhenFormIsDisabled { get; set; }

        /// <summary>
        ///     Set SuppressKeyPress to true in the keypress eventargs if a hotkey was pressed.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SuppressKeyPresses { get; set; } = true;

        [Browsable(false)]
        public int Count => _registeredHotkeys.Count;

        [Browsable(false)]
        public bool IsReadOnly => false;

        /// <exception cref="ArgumentNullException">The value of 'item' cannot be null. </exception>
        public void Add(HotkeyEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Master != null)
                item.Master.ShortcutKeyDisplayString = item.ToString();

            _registeredHotkeys.Add(item);
        }

        public void Clear()
        {
            _registeredHotkeys.ForEach(x => x.Dispose());
            _registeredHotkeys.Clear();
        }

        public bool Contains(HotkeyEntry item)
        {
            return _registeredHotkeys.Contains(item);
        }

        void ICollection<HotkeyEntry>.CopyTo(HotkeyEntry[] array, int arrayIndex)
        {
            throw new InvalidOperationException();
        }

        public IEnumerator<HotkeyEntry> GetEnumerator()
        {
            return _registeredHotkeys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _registeredHotkeys.GetEnumerator();
        }

        public bool Remove(HotkeyEntry item)
        {
            if (item == null)
                return false;
            item.Dispose();
            return _registeredHotkeys.Remove(item);
        }

        protected override void OnContainerInitialized(object obj, EventArgs args)
        {
            if (ContainerControl is Form form)
                ParentForm = form;
            else if (ContainerControl.ParentForm != null)
                ParentForm = ContainerControl.ParentForm;
            else
                throw new InvalidOperationException("Could not find the parent form");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ParentForm = null;
                Clear();
            }
            base.Dispose(disposing);
        }

        private void KeyDown_Handler(object sender, KeyEventArgs e)
        {
            if (_parentForm == null)
                throw new InvalidOperationException("Hotkey handler called when parent form was null");

            foreach (var hotkey in _registeredHotkeys)
            {
                if (hotkey.Alt == e.Alt && hotkey.Ctrl == e.Control && hotkey.Shift == e.Shift &&
                    hotkey.KeyCode == e.KeyCode)
                {
                    if (hotkey.EventHandler == null || !hotkey.IsEnabled ||
                        (StopWhenFormIsDisabled && !ParentForm.Enabled))
                        continue;

                    hotkey.EventHandler(_parentForm, EventArgs.Empty);

                    // Stops default windows "wtfding" sound and prevents event bubbling
                    if (SuppressKeyPresses)
                        e.SuppressKeyPress = true;

                    // Do not process any more hotkeys
                    return;
                }
            }
        }

        //TODO does not work very well, need to ask for description at hotkey creation
        public HotkeyInfo[] GetHotkeyList()
        {
            var query = from hotkey in _registeredHotkeys
                group hotkey by hotkey.EventHandler
                into groupedHotkeys
                select new HotkeyInfo
                {
                    Master = (groupedHotkeys.FirstOrDefault(x => x.Master != null) ?? groupedHotkeys.First()).Master,
                    Hotkeys = groupedHotkeys.Select(x => x.ToString()).OrderBy(x => x).ToArray()
                };

            return query.OrderBy(x => x.Name).ToArray();
        }

        //TODO does not work very well, need to ask for description at hotkey creation
        public class HotkeyInfo
        {
            public string[] Hotkeys;
            internal ToolStripMenuItem Master;

            public string Name
            {
                get
                {
                    return (Master == null) ? string.Empty : new string(Master.Text.Where(x => !x.Equals('&')).ToArray());
                }
            }

            public override string ToString()
            {
                return $"Name: {Name}, Hotkeys: {string.Join(", ", Hotkeys)}";
            }
        }
    }
}