﻿namespace MobiFlight.BrowserMessages
{
    internal class Notification
    {
        public string Type { get; set; }

        public string Action { get; set; }
        public object Value { get; set; }
    }
}