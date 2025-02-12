using MobiFlight.Base;
using System;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class Output : DeviceConfig
    {
        public override string Name { get { return DisplayPin; } }

        public string DisplayPin { get; set; }
        public byte DisplayPinBrightness { get; set; }
        public bool DisplayPinPWM { get; set; }

        public Output ()
        {
            DisplayPin = ""; // not initialized anymore
            DisplayPinBrightness = byte.MaxValue;
            DisplayPinPWM = false;
        }

        public override bool Equals(Object obj)
        {
            return
                (obj != null) && (obj is Output) &&
                (this.DisplayPin            == (obj as Output).DisplayPin) &&
                (this.DisplayPinBrightness  == (obj as Output).DisplayPinBrightness) &&
                (this.DisplayPinPWM         == (obj as Output).DisplayPinPWM);
        }

        public void ReadXml(XmlReader reader)
        {

            if (reader["pin"] != null && reader["pin"] != "")
            {
                DisplayPin = reader["pin"];
            }
            if (reader["pinBrightness"] != null && reader["pinBrightness"] != "")
            {
                DisplayPinBrightness = byte.Parse(reader["pinBrightness"]);
            }
            if (reader["pinPwm"] != null && reader["pinPwm"] != "")
            {
                DisplayPinPWM = bool.Parse(reader["pinPwm"]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("pin", DisplayPin);
            writer.WriteAttributeString("pinBrightness", DisplayPinBrightness.ToString());

            // only write the info if enabled (not many pins can actually set this)
            if (DisplayPinPWM)
                writer.WriteAttributeString("pinPwm", DisplayPinPWM.ToString());
        }

        public override object Clone()
        {
            Output clone = new Output();
            clone.DisplayPin = DisplayPin;
            clone.DisplayPinBrightness = DisplayPinBrightness;
            clone.DisplayPinPWM = DisplayPinPWM;
            return clone;
        }
    }
}
