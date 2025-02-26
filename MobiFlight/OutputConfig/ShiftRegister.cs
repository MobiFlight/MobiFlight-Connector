using MobiFlight.Base;
using System;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class ShiftRegister : DeviceConfig
    {
        public override string Name { get { return Address; } }
        public String Pin { get; set; }
        public String Address { get; set; }
        public byte Brightness { get; set; } = byte.MaxValue;
        public bool PWM { get; set; } = false;

        public ShiftRegister()
        {
            Address = "";
            Pin = "";
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is ShiftRegister)) return false;
            var other = (ShiftRegister)obj;

            return
                Pin == other.Pin &&
                Address == other.Address &&
                Brightness == other.Brightness &&
                PWM == other.PWM
                ;
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

            // this is only for backward compatibility
            // with the old XML structure
            // this has been copied over from the Output class
            if (reader["pinBrightness"] != null && reader["pinBrightness"] != "")
            {
                Brightness = byte.Parse(reader["pinBrightness"]);
            }
            if (reader["pinPwm"] != null && reader["pinPwm"] != "")
            {
                PWM = bool.Parse(reader["pinPwm"]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("shiftRegister", Address);
            writer.WriteAttributeString("registerOutputPin", Pin);

            // this is only for backward compatibility
            // with the old XML structure
            // this has been copied over from the Output class
            writer.WriteAttributeString("pinBrightness", Brightness.ToString());
            if (PWM)
                writer.WriteAttributeString("pinPwm", PWM.ToString());
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

        public override object Clone()
        {
            ShiftRegister clone = new ShiftRegister();
            clone.Pin = Pin;
            clone.Address = Address;
            clone.Brightness = Brightness;
            clone.PWM = PWM;
            return clone;
        }
    }
}
