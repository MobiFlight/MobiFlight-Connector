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
    public class Padding : ModifierBase
    {
        public enum PaddingDirection { Left, Right, Centered }
        public char Character = ' ';
        public int Length = 5;
        public PaddingDirection Direction = PaddingDirection.Left;

        public override void ReadXml(XmlReader reader)
        {
            if (reader["active"] != null)
                Active = bool.Parse(reader["active"]);
            // read precondition settings if present
            if (reader["direction"] != null)
            {
                PaddingDirection direction;
                if (Enum.TryParse<PaddingDirection>(reader["direction"] as String, out direction))
                {
                    Direction = direction;
                }
            }
            
            if (reader["length"] != null)
                Length = int.Parse(reader["length"]);

            if (reader["character"] != null)
                Character = (reader["character"] as String)[0];
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("padding");
                writer.WriteAttributeString("active", Active.ToString());
                writer.WriteAttributeString("direction", Direction.ToString());
                writer.WriteAttributeString("length", Length.ToString());
                writer.WriteAttributeString("character", Character.ToString());
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            Padding Clone = new Padding();
            Clone.Active = Active;
            Clone.Direction = Direction;
            Clone.Length = Length;
            Clone.Character = Character;
            return Clone;
        }

        public override bool Equals(object obj)
        {
            return
                obj != null && obj is Padding &&
                this.Active == (obj as Padding).Active &&
                this.Direction == (obj as Padding).Direction &&
                this.Length == (obj as Padding).Length &&
                this.Character == (obj as Padding).Character;
        }

        public override ConnectorValue Apply(ConnectorValue value, List<ConfigRefValue> configRefs)
        {
            ConnectorValue result = value;

            switch (value.type)
            {
                case FSUIPCOffsetType.Float:
                case FSUIPCOffsetType.Integer:
                    string tmpValue = Apply(value.Float64, configRefs);
                    // Expression has now made this a string
                    value.type = FSUIPCOffsetType.String;
                    value.String = tmpValue;
                    break;

                case FSUIPCOffsetType.String:
                    value.String = Apply(value.String);
                    break;
            }

            return result;
        }

        protected string Apply(Double value, List<ConfigRefValue> configRefs)
        {
            var result = value.ToString();
            foreach (ConfigRefValue configRef in configRefs)
            {
                result = result.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
            }

            return Apply(result);
        }
        protected string Apply(string value)
        {
            if (value.Length > Length)
            {
                value = value.Substring(0, Length);
            }

            switch (Direction)
            {
                case PaddingDirection.Left:
                    value = (value.PadLeft(Length, Character));
                    break;

                case PaddingDirection.Right:
                    value =  (value.PadRight(Length, Character));
                    break;
            }

            return value;
        }

        public override string ToSummaryLabel()
        {
            return $"Padding Char: \"{Character}\", Length: {Length}, Direction: {Direction.ToString()}";
        }
    }
}
