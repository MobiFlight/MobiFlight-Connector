using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class Interpolation : IXmlSerializable, ICloneable
    {
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("en");

        SortedDictionary<float, float> Values = new SortedDictionary<float, float>();
        public int Count { get { return Values.Count; } }
        public void Add (float x, float y)
        {
            if (Values.ContainsKey(x))
                throw new XvalueAlreadyExistsException();
            Values.Add(x, y); 
        }

        public SortedDictionary<float, float> GetValues()
        {
            return Values;
        }

        public void Clear()
        {
            Values.Clear();
        }

        public object Clone()
        {
            Interpolation Clone = new Interpolation();
            foreach(float Key in Values.Keys)
            {
                Clone.Add(Key, Values[Key]);
            }
            return Clone;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool atPosition = false;
            // read precondition settings if present
            if (reader.ReadToDescendant("value"))
            {
                // load a list
                do
                {
                    Values.Add(float.Parse(reader["x"], serializationCulture), 
                               float.Parse(reader["y"], serializationCulture));
                    reader.ReadToNextSibling("value");
                } while (reader.LocalName == "value");
            }
        }

        public float Value(float x)
        {
            float first = Values.Keys.First();
            if (x <= first) return Values[first];

            float second = Values.Keys.Last();
            if (x >= second) return Values[second];

            if (Values.Count > 2)
            {
                for (int i = 1; i != Values.Count; ++i)
                {
                    float currentKey = Values.ElementAt(i).Key;
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

        public void WriteXml(XmlWriter writer)
        {
            if (Count == 0) return;

            writer.WriteStartElement("interpolation");
            foreach(float x in Values.Keys)
            {
                writer.WriteStartElement("value");
                writer.WriteAttributeString("x", x.ToString(serializationCulture));
                writer.WriteAttributeString("y", Values[x].ToString(serializationCulture));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        protected float interpolate(float value, float x1, float y1, float x2, float y2)
        {
            if (x1 == x2) return y1;
            // this can not throw a division by zero exception
            return y1 + ((y2 - y1) / (x2 - x1)) * (value - x1);
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
