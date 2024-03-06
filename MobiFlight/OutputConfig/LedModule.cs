using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class LedModule
    {
        // the display stuff
        public string Address { get; set; }
        public byte Connector { get; set; }
        public byte ModuleSize { get; set; }
        public bool Padding { get; set; }
        public string PaddingChar { get; set; }
        public List<string> Digits { get; set; }
        public List<string> DecimalPoints { get; set; }
        public bool ReverseDigits { get; set; }
        public string BrightnessReference { get; set; }

        public LedModule()
        {
            Connector = 1;
            Address = "0";
            Padding = false;
            ReverseDigits = false;
            BrightnessReference = string.Empty;
            PaddingChar = "0";
            ModuleSize = 8;
            Digits = new List<string>();
            DecimalPoints = new List<string>();
        }

        public object Clone()
        {
            LedModule clone = new LedModule();
            clone.Connector = Connector;
            clone.Address = Address;
            clone.Padding = Padding;
            clone.ReverseDigits = ReverseDigits;
            clone.BrightnessReference = BrightnessReference;
            clone.PaddingChar = PaddingChar;
            clone.ModuleSize = ModuleSize;
            clone.Digits = new List<string>(Digits.ToArray());
            clone.DecimalPoints = new List<string>(DecimalPoints.ToArray());
            return clone;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !(obj is LedModule)) return false;

            bool digitsAreEqual = true && Digits?.Count == (obj as LedModule)?.Digits.Count;
            if (digitsAreEqual)
                for (int i = 0; i != Digits.Count; i++)
                {
                    digitsAreEqual = digitsAreEqual && (Digits[i] == (obj as LedModule).Digits[i]);
                }

            bool pointsAreEqual = true && DecimalPoints.Count == (obj as LedModule)?.DecimalPoints.Count;
            if (pointsAreEqual)
                for (int i = 0; i != DecimalPoints.Count; i++)
                {
                    pointsAreEqual = pointsAreEqual && (DecimalPoints[i] == (obj as LedModule).DecimalPoints[i]);
                }

            return
                (obj != null) && (obj is LedModule) &&
                (this.Connector == (obj as LedModule).Connector) &&
                (this.Address == (obj as LedModule).Address) &&
                (this.Padding == (obj as LedModule).Padding) &&
                (this.ReverseDigits == (obj as LedModule).ReverseDigits) &&
                (this.BrightnessReference == (obj as LedModule).BrightnessReference) &&
                (this.PaddingChar == (obj as LedModule).PaddingChar) &&
                (this.ModuleSize == (obj as LedModule).ModuleSize) &&
                (digitsAreEqual) &&
                (pointsAreEqual);
        }

        internal void XmlRead(XmlReader reader)
        {
            if (reader["ledAddress"] != null && reader["ledAddress"] != "")
            {
                Address = reader["ledAddress"];
            }

            if (reader["ledConnector"] != null && reader["ledConnector"] != "")
            {
                Connector = byte.Parse(reader["ledConnector"]);
            }

            if (reader["ledModuleSize"] != null && reader["ledModuleSize"] != "")
            {
                ModuleSize = byte.Parse(reader["ledModuleSize"]);
            }

            if (reader["ledPadding"] != null && reader["ledPadding"] != "")
            {
                Padding = Boolean.Parse(reader["ledPadding"]);
            }

            if (reader["ledReverseDigits"] != null && reader["ledReverseDigits"] != "")
            {
                ReverseDigits = Boolean.Parse(reader["ledReverseDigits"]);
            }
            if (reader["ledBrightnessRef"] != null && reader["ledBrightnessRef"] != "")
            {
                BrightnessReference = reader["ledBrightnessRef"];
            }

            if (reader["ledPaddingChar"] != null && reader["ledPaddingChar"] != "")
            {
                PaddingChar = reader["ledPaddingChar"];
            }

            // ignore empty values
            if (reader["ledDigits"] != null && reader["ledDigits"] != "")
            {
                Digits = reader["ledDigits"].Split(',').ToList();
            }

            // ignore empty values
            if (reader["ledDecimalPoints"] != null && reader["ledDecimalPoints"] != "")
            {
                DecimalPoints = reader["ledDecimalPoints"].Split(',').ToList();
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ledAddress", Address);
            writer.WriteAttributeString("ledConnector", Connector.ToString());
            writer.WriteAttributeString("ledModuleSize", ModuleSize.ToString());
            writer.WriteAttributeString("ledPadding", Padding.ToString());
            if (ReverseDigits)
                writer.WriteAttributeString("ledReverseDigits", ReverseDigits.ToString());
            if (!string.IsNullOrEmpty(BrightnessReference))
                writer.WriteAttributeString("ledBrightnessRef", BrightnessReference.ToString());

            writer.WriteAttributeString("ledPaddingChar", PaddingChar);

            if (Digits.Count > 0)
            {
                writer.WriteAttributeString("ledDigits", String.Join(",", Digits));
            }

            if (DecimalPoints.Count > 0)
            {
                writer.WriteAttributeString("ledDecimalPoints", String.Join(",", DecimalPoints));
            }
        }
    }
}
