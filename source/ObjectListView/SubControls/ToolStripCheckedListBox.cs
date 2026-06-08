/*
 * ToolStripCheckedListBox - Puts a CheckedListBox into a tool strip menu item
 *
 * Author: Phillip Piper
 * Date: 4-March-2011 11:59 pm
 *
 * Change log:
 * 2011-03-04  JPP  - First version
 * 
 * Copyright (C) 2011-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace BrightIdeasSoftware {

    /// <summary>
    /// Instances of this class put a CheckedListBox into a tool strip menu item.
    /// </summary>
    public class ToolStripCheckedListBox : ToolStripControlHost {

        /// <summary>
        /// Create a ToolStripCheckedListBox
        /// </summary>
        public ToolStripCheckedListBox()
            : base(new CheckedListBox()) {
            CheckedListBoxControl.MaximumSize = new Size(400, 700);
            CheckedListBoxControl.ThreeDCheckBoxes = true;
            CheckedListBoxControl.CheckOnClick = true;
            CheckedListBoxControl.SelectionMode = SelectionMode.One;
        }

        /// <summary>
        /// Gets the control embedded in the menu
        /// </summary>
        public CheckedListBox CheckedListBoxControl {
            get {
                return Control as CheckedListBox;
            }
        }

        /// <summary>
        /// Gets the items shown in the checkedlistbox
        /// </summary>
        public CheckedListBox.ObjectCollection Items {
            get {
                return CheckedListBoxControl.Items;
            }
        }

        /// <summary>
        /// Gets or sets whether an item should be checked when it is clicked
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CheckedOnClick
        {
            get {
                return CheckedListBoxControl.CheckOnClick;
            }
            set {
                CheckedListBoxControl.CheckOnClick = value;
            }
        }

        /// <summary>
        /// Gets a collection of the checked items
        /// </summary>
        public CheckedListBox.CheckedItemCollection CheckedItems {
            get {
                return CheckedListBoxControl.CheckedItems;
            }
        }

        /// <summary>
        /// Add a possibly checked item to the control
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isChecked"></param>
        public void AddItem(object item, bool isChecked) {
            Items.Add(item);
            if (isChecked)
                CheckedListBoxControl.SetItemChecked(Items.Count - 1, true);
        }

        /// <summary>
        /// Add an item with the given state to the control
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        public void AddItem(object item, CheckState state) {
            Items.Add(item);
            CheckedListBoxControl.SetItemCheckState(Items.Count - 1, state);
        }

        /// <summary>
        /// Gets the checkedness of the i'th item
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CheckState GetItemCheckState(int i) {
            return CheckedListBoxControl.GetItemCheckState(i);
        }

        /// <summary>
        /// Set the checkedness of the i'th item
        /// </summary>
        /// <param name="i"></param>
        /// <param name="checkState"></param>
        public void SetItemState(int i, CheckState checkState) {
            if (i >= 0 && i < Items.Count)
                CheckedListBoxControl.SetItemCheckState(i, checkState);
        }

        /// <summary>
        /// Check all the items in the control
        /// </summary>
        public void CheckAll() {
            for (int i = 0; i < Items.Count; i++)
                CheckedListBoxControl.SetItemChecked(i, true);
        }

        /// <summary>
        /// Unchecked all the items in the control
        /// </summary>
        public void UncheckAll() {
            for (int i = 0; i < Items.Count; i++)
                CheckedListBoxControl.SetItemChecked(i, false);
        }

        #region Events

        /// <summary>
        /// Listen for events on the underlying control
        /// </summary>
        /// <param name="c"></param>
        protected override void OnSubscribeControlEvents(Control c) {
            base.OnSubscribeControlEvents(c);

            CheckedListBox control = (CheckedListBox)c;
            control.ItemCheck += new ItemCheckEventHandler(OnItemCheck);
        }

        /// <summary>
        /// Stop listening for events on the underlying control
        /// </summary>
        /// <param name="c"></param>
        protected override void OnUnsubscribeControlEvents(Control c) {
            base.OnUnsubscribeControlEvents(c);

            CheckedListBox control = (CheckedListBox)c;
            control.ItemCheck -= new ItemCheckEventHandler(OnItemCheck);
        }

        /// <summary>
        /// Tell the world that an item was checked
        /// </summary>
        public event ItemCheckEventHandler ItemCheck;

        /// <summary>
        /// Trigger the ItemCheck event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemCheck(object sender, ItemCheckEventArgs e) {
            if (ItemCheck != null) {
                ItemCheck(this, e);
            }
        }

        #endregion
    }
}
