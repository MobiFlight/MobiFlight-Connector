using MobiFlight.OutputConfig;
using Newtonsoft.Json.Linq;
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
    public class Substring : ModifierBase
    {
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("en");
        public int Start = 0;
        public int End = 7;

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
            return
                obj != null && obj is Substring &&
                this.Active == (obj as Substring).Active &&
                this.Start == (obj as Substring).Start &&
                this.End == (obj as Substring).End;
        }

        public override ConnectorValue Apply(ConnectorValue value, List<ConfigRefValue> configRefs)
        {
            ConnectorValue result = value.Clone() as ConnectorValue;

            if (!Active) return result;

            switch (value.type)
            {
                case FSUIPCOffsetType.Float:
                case FSUIPCOffsetType.Integer:
                    string tmpValue = Apply(value.Float64.ToString());
                    result.String = tmpValue;
                    break;

                case FSUIPCOffsetType.String:
                    result.String = Apply(value.String);
                    break;
            }

            return result;
        }

        protected string Apply(string value)
        {
            if (Start > value.Length) return "";

            int length = (End - Start) + 1;
            
            if (End > value.Length)
            {
                var endIndex = value.Length - 1;
                length = (endIndex - Start) + 1;
            }

            length = Math.Min(length, value.Length);

            if (length < 0) length = 0;

            return value.Substring(Start, length);
        }

        public override string ToSummaryLabel()
        {
            return $"Substring: from {Start} to {End}";
        }
    }
}
