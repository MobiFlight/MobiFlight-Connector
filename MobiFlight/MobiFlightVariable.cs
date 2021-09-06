using System;
using System.Xml;

namespace MobiFlight
{
    public class MobiFlightVariable
    {
        public const string TYPE_NUMBER = "number";
        public const string TYPE_STRING = "string";

        public string TYPE = TYPE_NUMBER;
        public string Name = "MyVar";
        public double Number;
        public string Text = "";
        public string Expression = "$";

        public object Clone()
        {
            MobiFlightVariable clone = new MobiFlightVariable();
            clone.TYPE = TYPE;
            clone.Name = Name;
            clone.Number = Number;
            clone.Text = Text;
            clone.Expression = Expression;

            return clone;
        }


        public void ReadXml(System.Xml.XmlReader reader)
        {
            TYPE = reader["varType"];
            Name =  reader["varName"];
            Expression = reader["varExpression"];
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", "Variable");
            writer.WriteAttributeString("varType", TYPE);
            writer.WriteAttributeString("varName", Name);
            writer.WriteAttributeString("varExpression", Expression);
        }

        public override bool Equals(object obj)
        {
            return
                this.TYPE == (obj as MobiFlightVariable).TYPE &&
                this.Name == (obj as MobiFlightVariable).Name &&
                this.Number == (obj as MobiFlightVariable).Number &&
                this.Text == (obj as MobiFlightVariable).Text &&
                this.Expression == (obj as MobiFlightVariable).Expression;
        }
    }
}