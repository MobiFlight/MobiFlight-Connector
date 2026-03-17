using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class FsuipcBCD
    {
        public int Value { get; set; }
        public int asBCD { get { return Int16.Parse(this.Value.ToString("X4")); } }
    }
}
