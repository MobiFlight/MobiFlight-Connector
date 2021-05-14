using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class SimConnectValue
    {
        public string LVar { get; set; }

        internal void ReadXml(XmlReader reader)
        {
            if (reader["LVar"] != null && reader["LVar"] != "")
            {
                LVar = reader["LVar"];
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", "SimConnect");
            writer.WriteAttributeString("LVar", LVar);
        }

        internal object Clone()
        {
            return new SimConnectValue() { LVar = this.LVar };
        }
    }
}
