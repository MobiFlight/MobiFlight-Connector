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
        public string Value { get; set; }
        public SimConnectVarType VarType { get; set; }

        internal void ReadXml(XmlReader reader)
        {
            if (reader["Value"] != null && reader["Value"] != "")
            {
                Value = reader["Value"];
                VarType = (SimConnectVarType) Enum.Parse(typeof(SimConnectVarType), reader["VarType"]);
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", "SimConnect");
            writer.WriteAttributeString("VarType", VarType.ToString());
            writer.WriteAttributeString("Value", Value);
        }

        internal object Clone()
        {
            return new SimConnectValue() { 
                Value = this.Value,
                VarType = this.VarType
            };
        }
    }

    public enum SimConnectVarType
    {
        LVAR,
        AVAR,
        CODE
    }
}
