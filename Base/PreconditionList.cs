using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Base
{
    public class PreconditionList : IXmlSerializable, ICloneable, IEnumerable
    {
        List<Precondition> Preconditions = new List<Precondition>();
        public int Count { get { return Preconditions.Count; } }
        public bool ExecuteOnFalse = false;
        public String FalseCaseValue = "";

        public object Clone()
        {
            PreconditionList c = new PreconditionList();
            foreach (Precondition p in Preconditions)
            {
                c.Preconditions.Add(p.Clone() as Precondition);
            }
            return c;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Preconditions).GetEnumerator();
        }


        public void Add(Precondition p)
        {
            Preconditions.Add(p);
        }

        public void Remove (Precondition p)
        {
            Preconditions.Remove(p);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // read precondition settings if present
            if (reader.LocalName == "precondition")
            {
                // do so
                Precondition tmp = new Precondition();
                tmp.ReadXml(reader);
                Preconditions.Add(tmp);
            }

            if (reader.LocalName == "preconditions")
            {
                bool atPosition = false;
                // read precondition settings if present
                if (reader.ReadToDescendant("precondition"))
                {
                    // load a list
                    do
                    {
                        Precondition tmp = new Precondition();
                        tmp.ReadXml(reader);
                        Preconditions.Add(tmp);
                        reader.ReadStartElement();
                    } while (reader.LocalName == "precondition");

                    // read to the end of preconditions-node
                    reader.ReadEndElement();
                }
                else
                {
                    reader.ReadStartElement();
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("preconditions");
            foreach (Precondition p in Preconditions)
            {
                p.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}
