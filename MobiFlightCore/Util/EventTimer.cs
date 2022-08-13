using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace MobiFlight
{
    public class EventTimer : Timer
    {
        public event EventHandler Stopped;
        public event EventHandler Started;
        public new bool Enabled
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
