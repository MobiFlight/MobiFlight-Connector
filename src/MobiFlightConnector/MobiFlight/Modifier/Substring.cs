using System.Collections.Generic;
using System.Xml;

namespace MobiFlight.Modifier
{
    public class Substring : ModifierBase
    {
        public int Start { get; set; } = 0;
        public int End { get; set; } = 7;

        public override void ReadXml(XmlReader reader)
        {
            if (reader["active"] != null)
                Active = bool.Parse(reader["active"]);
            
            if (reader["start"] != null)
                Start = int.Parse(reader["start"]);

            if (reader["end"] != null)
                End = int.Parse(reader["end"]);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("substring");
                writer.WriteAttributeString("active", Active.ToString());
                writer.WriteAttributeString("start", Start.ToString());
                writer.WriteAttributeString("end", End.ToString());
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            Substring Clone = new Substring();
            Clone.Active = Active;
            Clone.Start = Start;
            Clone.End = End;
            return Clone;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Substring)) return false;
            var other = obj as Substring;

            return
                this.Active == other.Active &&
                this.Start == other.Start &&
                this.End == other.End;
        }

        public override ConnectorValue Apply(ConnectorValue value, List<ConfigRefValue> configRefs)
        {
            ConnectorValue result = value;

            switch (value.type)
            {
                case FSUIPCOffsetType.Float:
                case FSUIPCOffsetType.Integer:
                    string tmpValue = Apply(value.Float64.ToString());
                    value.String = tmpValue;
                    break;

                case FSUIPCOffsetType.String:
                    value.String = Apply(value.String);
                    break;
            }

            return result;
        }

        protected string Apply(string value)
        {
            if (Start > value.Length) return "";

            int length = (End - Start);
            if (End > value.Length) length = value.Length - Start;

            return value.Substring(Start, length);
        }

        public override string ToSummaryLabel()
        {
            return $"Substring: from {Start} to {End}";
        }
    }
}
