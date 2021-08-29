using System.Drawing;
using System.Windows.Forms;

namespace Tools
{
    public partial class LoggingView : ListBox
    {
        public LoggingView()
        {
            InitializeComponent();
                        SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            DrawMode = DrawMode.OwnerDrawFixed;
            FollowLastItem = true;
            MaxEntriesInListBox = 3000;
        }

        public bool FollowLastItem{ get; set; }
        public int MaxEntriesInListBox { get; set; }


        public void AddEntry(object item)
        {
            BeginUpdate();
            Items.Add(item);

            if (Items.Count > MaxEntriesInListBox)
            {
                Items.RemoveAt(0);
            }

            if (FollowLastItem) TopIndex = Items.Count - 1;
            EndUpdate();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (Items.Count > 0)
            {
                e.DrawBackground();
                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            base.OnDrawItem(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(BackColor), iRegion);
            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; ++i)
                {
                    var irect = GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if ((SelectionMode == SelectionMode.One && SelectedIndex == i)
                        || (SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i))
                        || (SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
                                irect, i,
                                DrawItemState.Selected, ForeColor,
                                BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
                                irect, i,
                                DrawItemState.Default, ForeColor,
                                BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }
    }
}
