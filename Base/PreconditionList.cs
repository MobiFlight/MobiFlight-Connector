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
    public class PreconditionList : IXmlSerializable, ICloneable, IEnumerable, ICollection<Precondition>
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
            c.ExecuteOnFalse = ExecuteOnFalse;
            c.FalseCaseValue = FalseCaseValue;
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
        public void Clear()
        {
            Preconditions.Clear();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.LocalName == "preconditions")
            {
                reader.Read();
            }

            if (reader.LocalName == "precondition")
            {
                // load a list
                do
                {
                    Precondition tmp = new Precondition();
                    tmp.ReadXml(reader);
                    Preconditions.Add(tmp);
                } while (reader.LocalName == "precondition");
            }

            // read to the end of preconditions-node
            if (reader.LocalName=="preconditions")
                reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("preconditions");

            if (ExecuteOnFalse) writer.WriteAttributeString("executeOnFalse", ExecuteOnFalse.ToString());
            if (FalseCaseValue!="") writer.WriteAttributeString("falseCaseValue", FalseCaseValue);

            foreach (Precondition p in Preconditions)
            {
                p.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        public override bool Equals(object obj)
        {
            bool areEqual = 
                obj != null && 
                obj is PreconditionList && 
                (Count == (obj as PreconditionList).Count) &&
                ExecuteOnFalse == (obj as PreconditionList).ExecuteOnFalse &&
                FalseCaseValue == (obj as PreconditionList).FalseCaseValue
                ;
            if (areEqual)
            {
                for (int i=0; i!=Count; i++)
                {
                    areEqual = areEqual && (Preconditions[i].Equals((obj as PreconditionList).Preconditions[i]));
                }
            }

            return areEqual;
        }

        public bool Contains(Precondition item)
        {
            return Preconditions.Contains(item);
        }

        public void CopyTo(Precondition[] array, int arrayIndex)
        {
            Preconditions.CopyTo(array, arrayIndex);
        }

        bool ICollection<Precondition>.Remove(Precondition item)
        {
            return Preconditions.Remove(item);
        }

        public bool IsReadOnly => false;

        IEnumerator<Precondition> IEnumerable<Precondition>.GetEnumerator()
        {
            return Preconditions.GetEnumerator();
        }
    }
}
