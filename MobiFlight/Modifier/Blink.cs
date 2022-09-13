using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Modifier
{
    public class Blink : ModifierBase
    {
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("en");
        public string BlinkValue = "";
        public int FrequencyInHz = 1;

        public override void ReadXml(XmlReader reader)
        {
            if (reader["active"] != null)
                Active = bool.Parse(reader["active"]);
            // read precondition settings if present
            if (reader["blinkValue"] != null)
                BlinkValue = reader["blinkValue"] as String;
            
            if (reader["FrequencyInHz"] != null)
                FrequencyInHz = int.Parse(reader["FrequencyInHz"]);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("blink");
                writer.WriteAttributeString("active", Active.ToString());
                writer.WriteAttributeString("blinkValue", BlinkValue);
                writer.WriteAttributeString("FrequencyInHz", FrequencyInHz.ToString());
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            var Clone = new Blink();
            Clone.Active = Active;
            Clone.BlinkValue = BlinkValue;
            Clone.FrequencyInHz = FrequencyInHz;
            return Clone;
        }

        public override bool Equals(object obj)
        {
            return
                obj != null && obj is Blink &&
                this.Active == (obj as Blink).Active &&
                this.BlinkValue == (obj as Blink).BlinkValue &&
                this.FrequencyInHz == (obj as Blink).FrequencyInHz;
        }

        public override ConnectorValue Apply(ConnectorValue value, List<ConfigRefValue> configRefs)
        {
            ConnectorValue result = value;
            var Now = DateTime.Now.Millisecond;

            if (Now%(1000 / (FrequencyInHz)) > 1000 / (FrequencyInHz * 2))
            {
                switch (value.type)
                {
                    case FSUIPCOffsetType.Float:
                    case FSUIPCOffsetType.Integer:
                        string tmpValue = BlinkValue;
                        if (Double.TryParse(tmpValue, out value.Float64))
                        {
                            result.Float64 = value.Float64;
                        }
                        else
                        {
                            // Expression has now made this a string
                            result.type = FSUIPCOffsetType.String;
                            result.String = tmpValue;
                        }
                        break;

                    case FSUIPCOffsetType.String:
                        result.String = BlinkValue;
                        break;
                }
            }

            return result;
        }
    }
}
