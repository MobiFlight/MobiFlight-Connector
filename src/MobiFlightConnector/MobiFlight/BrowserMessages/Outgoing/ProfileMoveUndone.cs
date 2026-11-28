using System;
using System.Collections.Generic;
using System.Text;

namespace MobiFlight.BrowserMessages.Outgoing
{
    internal class ProfileMoveUndone
    {
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
    }
}
