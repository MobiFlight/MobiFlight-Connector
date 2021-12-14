using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.OutputConfig
{
    public class ShiftRegister
    {
        public String Pin { get; set; }
        public String Address { get; set; }

        public ShiftRegister()
        {
            Address = "";
            Pin = "";
        }

        public override bool Equals(Object obj)
        {
            return
                (obj != null) && (obj is ShiftRegister) &&
                (this.Pin == (obj as ShiftRegister).Pin) &&
                (this.Address == (obj as ShiftRegister).Address);
        }

        public void ReadXml(XmlReader reader)
        {

            if (reader["registerOutputPin"] != null && reader["registerOutputPin"] != "")
            {
                Pin = reader["registerOutputPin"];
            }

            if (reader["shiftRegister"] != null && reader["shiftRegister"] != "")
            {
                Address = reader["shiftRegister"];
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("shiftRegister", Address);
            writer.WriteAttributeString("registerOutputPin", Pin);
        }

        public object Clone()
        {
            ShiftRegister clone = new ShiftRegister();
            clone.Pin = Pin;
            clone.Address = Address;
            return clone;
        }
    }
}
