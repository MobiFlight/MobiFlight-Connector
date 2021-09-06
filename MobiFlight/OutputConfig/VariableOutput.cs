using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class VariableOutput
    {
        public MobiFlightVariable Variable { get; set; }

        public VariableOutput()
        {
            Variable = new MobiFlightVariable();
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader["varType"] != null && reader["varType"] != "" &&
                reader["varName"] != null && reader["varName"] != "")
            {
                Variable.TYPE = reader["varType"];
                Variable.Name = reader["varName"];
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", "VariableOutput");
            writer.WriteAttributeString("varType", Variable.TYPE);
            writer.WriteAttributeString("varName", Variable.Name);
        }

        public object Clone()
        {
            MobiFlightVariable clone = Variable.Clone() as MobiFlightVariable;

            return new VariableOutput() { 
                Variable = clone
            };
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is VariableOutput && 
                this.Variable.Equals((obj as VariableOutput).Variable);
        }
    }
}
