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

        public override string ToString()
        {
            // The list of selected pins is stored as a string in this format: Output 1|Output 2|Output 10
            // That's too wordy to display in UI so strip out the "Output " and "|", then separate
            // the pin assignments with a comma. The resulting display string is: ShiftRegister:1,2,10.
            //
            // The UI forces at least one pin assignment so there is no case where the resulting string
            // would be ShiftRegister: (with no pins listed and a trailing colon).
            return $"{Address}:{Pin.Replace("Output ", ",").Replace("|", "").TrimStart(',')}";
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
