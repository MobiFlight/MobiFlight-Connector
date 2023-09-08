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
        public string BlinkValue = "0";
        public List<int> OnOffSequence = new List<int>();
        public int OffDurationInMs = 500;
        public long FirstExecutionTime = 0;

        public override void ReadXml(XmlReader reader)
        {
            if (reader["active"] != null)
                Active = bool.Parse(reader["active"]);
            // read precondition settings if present
            if (reader["blinkValue"] != null)
                BlinkValue = reader["blinkValue"] as String;
            
            if (reader["onOffSequence"] != null)
            {
                OnOffSequence.Clear();
                foreach(string s in reader["onOffSequence"].Split(','))
                {
                    OnOffSequence.Add(int.Parse(s));
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("blink");
                writer.WriteAttributeString("active", Active.ToString());
                writer.WriteAttributeString("blinkValue", BlinkValue);
                writer.WriteAttributeString("onOffSequence", string.Join(",", OnOffSequence));
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            var Clone = new Blink();
            Clone.Active = Active;
            Clone.BlinkValue = BlinkValue;
            Clone.OnOffSequence = OnOffSequence.ToArray().ToList();
            return Clone;
        }

        public override bool Equals(object obj)
        {
            return
                obj != null && obj is Blink &&
                this.Active == (obj as Blink).Active &&
                this.BlinkValue == (obj as Blink).BlinkValue &&
                this.OnOffSequence == (obj as Blink).OnOffSequence;
        }

        public override ConnectorValue Apply(ConnectorValue value, List<ConfigRefValue> configRefs)
        {
            ConnectorValue result = value;
            if (FirstExecutionTime == 0) FirstExecutionTime = DateTime.Now.Ticks;

            var Now = DateTime.Now.Ticks - FirstExecutionTime;
            if (Now == 0) Now = 1;
            Now /= 10000;
            Now %= (OnOffSequence.Sum() > 0 ? OnOffSequence.Sum() : 1000);

            bool IsOn = true;

            foreach(var time in OnOffSequence)
            {
                if (Now > time) {
                    Now -= time;
                    IsOn = !IsOn;
                    continue;
                }

                if (IsOn)
                {
                    continue;
                }

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

        public override string ToSummaryLabel()
        {
            return $"Blink value: \"{BlinkValue}\", ON-OFF-Sequence: {String.Join((", "), OnOffSequence.Select(s => s+" ms"))}";
        }
    }
}
