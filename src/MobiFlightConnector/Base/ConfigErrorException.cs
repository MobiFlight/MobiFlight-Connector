using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    class ConfigErrorException : Exception
    {
        private string p;

        public ConfigErrorException(string p, Exception e) : base(p, e) { }

        public ConfigErrorException() :  base()
        {
            // TODO: Complete member initialization
        }

        public ConfigErrorException(string p) : base(p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
    }
}
