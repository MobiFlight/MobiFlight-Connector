using System;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Outgoing
{
    internal class Notification
    {
        public string Event { get; set; }
        public Guid? Id { get; set; }
        public Dictionary<string, string> Context { get; set; }
    }
}
