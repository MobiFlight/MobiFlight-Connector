using MobiFlight.Base;
using System;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class Output : DeviceConfig
    {
        public override string Name { get { return Pin; } }

        public string Pin { get; set; }
        public byte Brightness { get; set; }
        public bool PwmMode { get; set; }

        public Output ()
        {
            Pin = ""; // not initialized anymore
            Brightness = byte.MaxValue;
            PwmMode = false;
        }

        public override bool Equals(Object obj)
        {
            return
                (obj != null) && (obj is Output) &&
                (this.Pin            == (obj as Output).Pin) &&
                (this.Brightness  == (obj as Output).Brightness) &&
                (this.PwmMode         == (obj as Output).PwmMode);
        }

        public void ReadXml(XmlReader reader)
        {

            if (reader["pin"] != null && reader["pin"] != "")
            {
                Pin = reader["pin"];
            }
            if (reader["pinBrightness"] != null && reader["pinBrightness"] != "")
            {
                Brightness = byte.Parse(reader["pinBrightness"]);
            }
            if (reader["pinPwm"] != null && reader["pinPwm"] != "")
            {
                PwmMode = bool.Parse(reader["pinPwm"]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("pin", Pin);
            writer.WriteAttributeString("pinBrightness", Brightness.ToString());

            // only write the info if enabled (not many pins can actually set this)
            if (PwmMode)
                writer.WriteAttributeString("pinPwm", PwmMode.ToString());
        }

        public override object Clone()
        {
            Output clone = new Output();
            clone.Pin = Pin;
            clone.Brightness = Brightness;
            clone.PwmMode = PwmMode;
            return clone;
        }
    }
}
