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
    public class Pin
    {
        public string DisplayPin { get; set; }
        public byte DisplayPinBrightness { get; set; }
        public bool DisplayPinPWM { get; set; }

        public Pin ()
        {
            DisplayPin = ""; // not initialized anymore
            DisplayPinBrightness = byte.MaxValue;
            DisplayPinPWM = false;
        }

        public override bool Equals(Object obj)
        {
            return
                (obj != null) && (obj is Pin) &&
                (this.DisplayPin            == (obj as Pin).DisplayPin) &&
                (this.DisplayPinBrightness  == (obj as Pin).DisplayPinBrightness) &&
                (this.DisplayPinPWM         == (obj as Pin).DisplayPinPWM);
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

        public object Clone()
        {
            Pin clone = new Pin();
            clone.DisplayPin = DisplayPin;
            clone.DisplayPinBrightness = DisplayPinBrightness;
            clone.DisplayPinPWM = DisplayPinPWM;
            return clone;
        }
    }
}
