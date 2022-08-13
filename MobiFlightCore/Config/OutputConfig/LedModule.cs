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
    public class LedModule
    {
        // the display stuff
        public string DisplayLedAddress { get; set; }
        public byte DisplayLedConnector { get; set; }
        public byte DisplayLedModuleSize { get; set; }
        public bool DisplayLedPadding { get; set; }
        public string DisplayLedPaddingChar { get; set; }
        public List<string> DisplayLedDigits { get; set; }
        public List<string> DisplayLedDecimalPoints { get; set; }
        public bool DisplayLedReverseDigits { get; set; }
        public string DisplayLedBrightnessReference { get; set; }

        public LedModule()
        {
            DisplayLedConnector = 1;
            DisplayLedAddress = "0";
            DisplayLedPadding = false;
            DisplayLedReverseDigits = false;
            DisplayLedBrightnessReference = string.Empty;
            DisplayLedPaddingChar = "0";
            DisplayLedModuleSize = 8;
            DisplayLedDigits = new List<string>();
            DisplayLedDecimalPoints = new List<string>();
        }

        public object Clone()
        {
            LedModule clone = new LedModule();
            clone.DisplayLedConnector = DisplayLedConnector;
            clone.DisplayLedAddress = DisplayLedAddress;
            clone.DisplayLedPadding = DisplayLedPadding;
            clone.DisplayLedReverseDigits = DisplayLedReverseDigits;
            clone.DisplayLedBrightnessReference = DisplayLedBrightnessReference;
            clone.DisplayLedPaddingChar = DisplayLedPaddingChar;
            clone.DisplayLedModuleSize = DisplayLedModuleSize;
            clone.DisplayLedDigits = new List<string> (DisplayLedDigits.ToArray());
            clone.DisplayLedDecimalPoints = new List<string>(DisplayLedDecimalPoints.ToArray());
            return clone;
        }

        public override bool Equals(Object obj)
        {
            bool digitsAreEqual = true && DisplayLedDigits.Count == (obj as LedModule).DisplayLedDigits.Count;
            if (digitsAreEqual)
                for (int i = 0; i != DisplayLedDigits.Count; i++)
                {
                    digitsAreEqual = digitsAreEqual && (DisplayLedDigits[i] == (obj as LedModule).DisplayLedDigits[i]);
                }

            bool pointsAreEqual = true && DisplayLedDecimalPoints.Count == (obj as LedModule).DisplayLedDecimalPoints.Count;
            if (pointsAreEqual)
                for (int i = 0; i != DisplayLedDecimalPoints.Count; i++)
                {
                    pointsAreEqual = pointsAreEqual && (DisplayLedDecimalPoints[i] == (obj as LedModule).DisplayLedDecimalPoints[i]);
                }

            return
                (obj != null) && (obj is LedModule) &&
                (this.DisplayLedConnector             == (obj as LedModule).DisplayLedConnector) &&
                (this.DisplayLedAddress               == (obj as LedModule).DisplayLedAddress) &&
                (this.DisplayLedPadding               == (obj as LedModule).DisplayLedPadding) &&
                (this.DisplayLedReverseDigits         == (obj as LedModule).DisplayLedReverseDigits) &&
                (this.DisplayLedBrightnessReference   == (obj as LedModule).DisplayLedBrightnessReference) &&
                (this.DisplayLedPaddingChar           == (obj as LedModule).DisplayLedPaddingChar) &&
                (this.DisplayLedModuleSize            == (obj as LedModule).DisplayLedModuleSize) &&
                (digitsAreEqual) &&
                (pointsAreEqual);
        }

        internal void XmlRead(XmlReader reader)
        {
            if (reader["ledAddress"] != null && reader["ledAddress"] != "")
            {
                DisplayLedAddress = reader["ledAddress"];
            }

            if (reader["ledConnector"] != null && reader["ledConnector"] != "")
            {
                DisplayLedConnector = byte.Parse(reader["ledConnector"]);
            }

            if (reader["ledModuleSize"] != null && reader["ledModuleSize"] != "")
            {
                DisplayLedModuleSize = byte.Parse(reader["ledModuleSize"]);
            }

            if (reader["ledPadding"] != null && reader["ledPadding"] != "")
            {
                DisplayLedPadding = Boolean.Parse(reader["ledPadding"]);
            }

            if (reader["ledReverseDigits"] != null && reader["ledReverseDigits"] != "")
            {
                DisplayLedReverseDigits = Boolean.Parse(reader["ledReverseDigits"]);
            }
            if (reader["ledBrightnessRef"] != null && reader["ledBrightnessRef"] != "")
            {
                DisplayLedBrightnessReference = reader["ledBrightnessRef"];
            }

            if (reader["ledPaddingChar"] != null && reader["ledPaddingChar"] != "")
            {
                DisplayLedPaddingChar = reader["ledPaddingChar"];
            }

            // ignore empty values
            if (reader["ledDigits"] != null && reader["ledDigits"] != "")
            {
                DisplayLedDigits = reader["ledDigits"].Split(',').ToList();
            }

            // ignore empty values
            if (reader["ledDecimalPoints"] != null && reader["ledDecimalPoints"] != "")
            {
                DisplayLedDecimalPoints = reader["ledDecimalPoints"].Split(',').ToList();
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ledAddress", DisplayLedAddress);
            writer.WriteAttributeString("ledConnector", DisplayLedConnector.ToString());
            writer.WriteAttributeString("ledModuleSize", DisplayLedModuleSize.ToString());
            writer.WriteAttributeString("ledPadding", DisplayLedPadding.ToString());
            if (DisplayLedReverseDigits)
                writer.WriteAttributeString("ledReverseDigits", DisplayLedReverseDigits.ToString());
            if (!string.IsNullOrEmpty(DisplayLedBrightnessReference))
                writer.WriteAttributeString("ledBrightnessRef", DisplayLedBrightnessReference.ToString());

            writer.WriteAttributeString("ledPaddingChar", DisplayLedPaddingChar);

            if (DisplayLedDigits.Count > 0)
            {
                writer.WriteAttributeString("ledDigits", String.Join(",", DisplayLedDigits));
            }

            if (DisplayLedDecimalPoints.Count > 0)
            {
                writer.WriteAttributeString("ledDecimalPoints", String.Join(",", DisplayLedDecimalPoints));
            }
        }
    }
}
