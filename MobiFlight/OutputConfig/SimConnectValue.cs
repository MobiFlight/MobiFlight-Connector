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

        public void ReadXml(XmlReader reader)
        {
            if (reader["Value"] != null && reader["Value"] != "")
            {
                Value = reader["Value"];
                VarType = (SimConnectVarType) Enum.Parse(typeof(SimConnectVarType), reader["VarType"]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", "SimConnect");
            writer.WriteAttributeString("VarType", VarType.ToString());
            writer.WriteAttributeString("Value", Value);
        }

        public override bool Equals(object obj)
        {
            return (obj != null) && (obj is SimConnectValue) &&
                this.Value == (obj as SimConnectValue).Value &&
                this.VarType == (obj as SimConnectValue).VarType;
        }

        public object Clone()
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
