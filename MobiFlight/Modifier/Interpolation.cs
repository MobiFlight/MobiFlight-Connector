using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Modifier
{
    public class Interpolation : ModifierBase
    {
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("en");

        SortedDictionary<double, double> Values = new SortedDictionary<double, double>();
        public int Count { get { return Values.Count; } }
        public double Max { get { return max; } }
        public double Min { get { return min; } }
        
        protected double max = double.MinValue;
        protected double min = double.MaxValue;

        public Interpolation()
        {
            Active = false;
        }
        
        public void Add (double x, double y)
        {
            if (Values.ContainsKey(x))
                throw new XvalueAlreadyExistsException();
            if (y > Max) max = y;
            if (y < Min) min = y;

            Values.Add(x, y); 
        }

        public SortedDictionary<double, double> GetValues()
        {
            return Values;
        }

        public void Clear()
        {
            Values.Clear();
        }

        override public object Clone()
        {
            Interpolation Clone = new Interpolation();
            // clone the get/set properties
            Clone.Active = Active;

            // clone the Vlaues
            foreach (double Key in Values.Keys)
            {
                Clone.Add(Key, Values[Key]);
            }
            return Clone;
        }

        override public void ReadXml(XmlReader reader)
        {
            bool atPosition = false;
            if (reader["active"] != null && reader["active"] != "")
            {
                Active = Boolean.Parse(reader["active"]);
            }
            // read precondition settings if present
            if (reader.ReadToDescendant("value"))
            {
                Values.Clear();
                // load a list
                do
                {
                    Values.Add(double.Parse(reader["x"], serializationCulture), 
                               double.Parse(reader["y"], serializationCulture));
                    reader.ReadToNextSibling("value");
                } while (reader.LocalName == "value");
            }

            if (reader.LocalName == "interpolation")
                reader.Read(); // this closes the interpolation node
        }

        public override ConnectorValue Apply(ConnectorValue connectorValue, List<ConfigRefValue> configRefs)
        {
            ConnectorValue result = connectorValue.Clone() as ConnectorValue;

            if (Count == 0) return result;

            switch (connectorValue.type)
            {
                case FSUIPCOffsetType.Float:
                case FSUIPCOffsetType.Integer:
                    result.Float64 = Math.Round(Value(connectorValue.Float64));
                    break;

                case FSUIPCOffsetType.String:
                    // we can't apply interpolation to a string
                    break;
            } 
                
            return result;
        }

        public string Apply(string strValue)
        {
            string result = strValue;
            if (Count > 0)
            {
                result = Math.Round(Value(float.Parse(strValue)), 0).ToString();
            }

            return result;
        }

        public double Value(double x)
        {
            double first = Values.Keys.First();
            if (x <= first) return Values[first];

            double second = Values.Keys.Last();
            if (x >= second) return Values[second];

            if (Values.Count > 2)
            {
                for (int i = 1; i != Values.Count; ++i)
                {
                    double currentKey = Values.ElementAt(i).Key;
                    if (currentKey <= x && currentKey > first)
                    {
                        if (currentKey == x) return Values.ElementAt(i).Value;
                        first = currentKey;
                        continue;
                    }

                    if (currentKey >= x && currentKey < second)
                    {
                        second = currentKey;
                        if (currentKey == x) return Values.ElementAt(i).Value;
                        break;
                    }
                }
            }

            return interpolate(x, first, Values[first], second, Values[second]);
        }

        override public void WriteXml(XmlWriter writer)
        {
            if (Count == 0) return;
            
            writer.WriteStartElement("interpolation");

            writer.WriteAttributeString("active", Active.ToString());
            foreach (double x in Values.Keys)
            {
                writer.WriteStartElement("value");
                writer.WriteAttributeString("x", x.ToString(serializationCulture));
                writer.WriteAttributeString("y", Values[x].ToString(serializationCulture));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        protected double interpolate(double value, double x1, double y1, double x2, double y2)
        {
            if (x1 == x2) return y1;
            // this can not throw a division by zero exception
            return y1 + ((y2 - y1) / (x2 - x1)) * (value - x1);
        }

        public override bool Equals(object obj)
        {
            bool entriesAreSame = (Values.Count == (obj as Interpolation).Count);
            if (entriesAreSame)
            {
                foreach (double x in Values.Keys)
                {
                    entriesAreSame = entriesAreSame && ((obj as Interpolation).Values.ContainsKey(x) && Values[x] == (obj as Interpolation).Values[x]);
                }
            }

            return
                obj != null && obj is Interpolation && 
                Max == (obj as Interpolation).Max &&
                Min == (obj as Interpolation).Min &&
                Count == (obj as Interpolation).Count &&
                Active == (obj as Interpolation).Active &&
                entriesAreSame;
        }
    }

    [Serializable]
    public class XvalueAlreadyExistsException : Exception
    {
        public XvalueAlreadyExistsException()
        {
        }

        public XvalueAlreadyExistsException(string message) : base(message)
        {
        }

        public XvalueAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XvalueAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
