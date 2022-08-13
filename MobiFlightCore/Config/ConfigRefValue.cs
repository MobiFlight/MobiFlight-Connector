using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class ConfigRefValue
    {

        public ConfigRefValue() { }
        public ConfigRefValue(ConfigRef c, string value)
        {
            this.Value = value;
            this.ConfigRef = c.Clone() as ConfigRef;
        }

        public string Value { get; set; }
        public ConfigRef ConfigRef { get; set; }

    }
}
