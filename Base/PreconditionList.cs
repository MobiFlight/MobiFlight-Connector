using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Base
{
    public class PreconditionList : IXmlSerializable, ICloneable, IEnumerable, ICollection<Precondition>
    {
        protected List<Precondition> Preconditions = new List<Precondition>();

        [JsonIgnore]
        public int Count { get { return Preconditions.Count; } }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ExecuteOnFalse { get; set; } = false;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FalseCaseValue { get; set; } = "";

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

        /// <summary>
        /// This is needed to implement IEnumerable interface
        /// </summary>
        /// <returns>IEnumerator for our Preconditions</returns>

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Preconditions).GetEnumerator();
        }

        /// <summary>
        /// This is needed to implement ICollection interface
        /// </summary>
        /// <returns>An enumerator for preconditions</returns>
        IEnumerator<Precondition> IEnumerable<Precondition>.GetEnumerator()
        {
            return Preconditions.GetEnumerator();
        }

        public void Add(Precondition p)
        {
            Preconditions.Add(p);
        }

        public bool Remove(Precondition p)
        {
            return Preconditions.Remove(p);
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
        
        public bool IsReadOnly => false;

        
    }
}
