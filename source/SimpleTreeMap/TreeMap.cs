using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleTreeMap
{
    public partial class TreeMap : UserControl
    {
        public class SliceEventArgs : EventArgs
        {
            public ICollection<object> Objects { get; }

            public SliceRectangle<object> Rectangle { get; }

            public SliceEventArgs(SliceRectangle<object> rectangle)
                : this(rectangle, rectangle.Slice.Elements.Select(x => x.Object).ToList())
            {
            }

            public SliceEventArgs(SliceRectangle<object> rectangle, ICollection<object> o)
            {
                Rectangle = rectangle;
                Objects = o;
            }
        }

        public class SliceClickedEventArgs : SliceEventArgs
        {
            public bool AddToSelection { get; }

            public SliceClickedEventArgs(SliceRectangle<object> rectangle, bool addToSelection)
                : base(rectangle)
            {
                AddToSelection = addToSelection;
            }

            public SliceClickedEventArgs(SliceRectangle<object> rectangle, ICollection<object> o, bool addToSelection)
                : base(rectangle, o)
            {
                AddToSelection = addToSelection;
            }
        }

        public event EventHandler<SliceClickedEventArgs> SliceClicked;
        public event EventHandler<SliceEventArgs> SliceHovered;
        public event EventHandler<SliceClickedEventArgs> SliceRightClicked;

        private const double MinSliceRatio = 0.35;
        private Slice<object> _currentSlice;
        private List<SliceRectangle<object>> _rectangles;
        private HashSet<object> _selectedObjects;
        private SliceRectangle<object> _currentHoveredRectangle;
        private static readonly SolidBrush SelectedRectBrush = new(Color.DodgerBlue);
        readonly Dictionary<Color, Brush> _brushCache = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ReadOnly(true)]
        public Func<object, Color> ObjectColorGetter { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ReadOnly(true)]
        public Func<object, string> ObjectNameGetter { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ReadOnly(true)]
        public Func<object, double> ObjectValueGetter { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowToolTip { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseLogValueScaling { get; set; }

        public TreeMap()
        {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        /// <summary>
        /// Calculate area to be drawn for this rectangle.
        /// Make sure to leave a 1px border around the map and between other rectangles.
        /// </summary>
        private void CalculatePaintRect(SliceRectangle<object> r)
        {
            r.PaintRect = new Rectangle(r.X == 0 ? 1 : r.X, r.Y == 0 ? 1 : r.Y, r.Width - (r.X == 0 ? 2 : 1), r.Height - (r.Y == 0 ? 2 : 1));

            r.PaintRect.Offset(ClientRectangle.Location);

            // Avoid unnecessary drawing
            if (r.PaintRect.Height < 2 || r.PaintRect.Width < 2)
                r.PaintRect = Rectangle.Empty;
        }

        private SliceRectangle<object> GetRectangleUnderMouse()
        {
            if (_rectangles == null) return null;
            var mousePos = PointToClient(MousePosition);
            var hoveredItem = _rectangles.FirstOrDefault(x => x.Contains(mousePos));
            return hoveredItem;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (ShowToolTip) toolTip1.Hide(this);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (ShowToolTip) toolTip1.Hide(this);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (ShowToolTip) toolTip1.Hide(this);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            var r = GetRectangleUnderMouse();

            if (r != null)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        OnSliceClicked(new SliceClickedEventArgs(r, (ModifierKeys & Keys.Control) != 0));
                        break;
                    case MouseButtons.Right:
                        OnSliceRightClicked(new SliceClickedEventArgs(r, (ModifierKeys & Keys.Control) != 0));
                        break;
                }
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            OnSliceHovered();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            OnSliceHovered();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // todo draw whole thing based on input, input is set by populate

            //var font = new Font("Arial", 8);
            var gfx = e.Graphics;

            gfx.FillRectangle(new SolidBrush(Color.Black), ClientRectangle);

            if (_rectangles == null)
            {
                if (DesignMode)
                    PopulateWithDemoData();
                return;
            }

            foreach (var r in _rectangles)
            {
                if (r.PaintRect.IsEmpty)
                    continue;

                if (_selectedObjects != null && r.Slice.Elements.Any(x => _selectedObjects.Contains(x.Object)))
                    gfx.FillRectangle(SelectedRectBrush, r.PaintRect);
                else
                    gfx.FillRectangle(_brushCache[r.Slice.Elements.First().Color], r.PaintRect);

                //gfx.DrawString(r.Slice.Elements.First().Object.ToString(), font,
                //    new SolidBrush(Control.DefaultForeColor), r.X, r.Y);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_currentSlice != null)
                UpdateRectangles();
        }

        protected virtual void OnSliceClicked(SliceClickedEventArgs e)
        {
            SliceClicked?.Invoke(this, e);
        }

        protected virtual void OnSliceHovered()
        {
            if (Disposing || IsDisposed) return;

            var hoveredItem = GetRectangleUnderMouse();

            if (ShowToolTip)
            {
                if (hoveredItem == null)
                    toolTip1.Hide(this);
                else if (!toolTip1.Active || _currentHoveredRectangle != hoveredItem)
                    toolTip1.Show(hoveredItem.Slice.ToElementNames(), this, new Point(0, Height + 2));
            }

            if (_currentHoveredRectangle != hoveredItem && hoveredItem != null)
                SliceHovered?.Invoke(this, new SliceEventArgs(hoveredItem));

            _currentHoveredRectangle = hoveredItem;
        }

        private void OnSliceRightClicked(SliceClickedEventArgs e)
        {
            SliceRightClicked?.Invoke(this, e);
        }

        public void Populate(IEnumerable<object> objects)
        {
            if (Disposing || IsDisposed) return;

            if (ObjectValueGetter == null)
                throw new InvalidOperationException(nameof(ObjectValueGetter) + " can't be null");
            if (ObjectNameGetter == null)
                throw new InvalidOperationException(nameof(ObjectNameGetter) + " can't be null");

            var elements = objects.Select(x =>
            {
                var element = new Element<object>
                {
                    Object = x,
                    Text = ObjectNameGetter(x),
                    Value = ObjectValueGetter(x)
                };

                if (ObjectColorGetter != null)
                {
                    element.Color = ObjectColorGetter.Invoke(x);
                    if (element.Color.IsEmpty)
                        element.Color = BackColor;
                }
                else
                {
                    element.Color = BackColor;
                }

                return element;
            }).Where(x => x.Value > 0.0001).OrderByDescending(x => x.Value).ToList();

            if (UseLogValueScaling)
                ScaleValuesLog(elements);

            _currentSlice = SliceMaker.GetSlice(elements, 1, MinSliceRatio);

            foreach (var c in elements.Select(x => x.Color).Distinct())
                _brushCache[c] = new SolidBrush(c);

            UpdateRectangles();

            Refresh();
        }

        private void PopulateWithDemoData()
        {
            ObjectNameGetter = o => o.ToString();
            ObjectValueGetter = o => (int)o;
            Populate(new[] { 10, 9, 8, 7, 6, 5, 3, 3, 3, 1 }.Cast<object>());
        }

        private void ScaleValuesLog(ICollection<Element<object>> elements)
        {
            var max = elements.Max(x => x.Value);

            foreach (var element in elements)
            {
                element.Value = (Math.Log10(element.Value / max + 0.3174) + 0.5) * 1.6;
            }
        }

        public void SetSelectedObjects(IEnumerable<object> objects)
        {
            _selectedObjects = new HashSet<object>(objects);
            Refresh();
        }

        private void UpdateRectangles()
        {
            if (Disposing || IsDisposed) return;

            _rectangles = SliceMaker.GetRectangles(_currentSlice, ClientSize.Width, ClientSize.Height).ToList();
            foreach (var r in _rectangles)
                CalculatePaintRect(r);
        }
    }
}
