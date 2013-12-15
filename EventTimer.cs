using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArcazeUSB
{
    public class EventTimer : Timer
    {
        public event EventHandler Stopped;
        public event EventHandler Started;
        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                if (value)
                {
                    Started(this, new EventArgs());
                }
                else
                {
                    Stopped(this, new EventArgs());
                } //if
            }
        } //prop
    };
}
