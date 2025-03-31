using System;
using System.Xml;
using ProSimSDK;

namespace MobiFlight.ProSim
{
    public class ProSimDataRef
    {
        public String Path { get; set; } = "";
        

        public object Clone()
        {
            ProSimDataRef clone = new ProSimDataRef();
            clone.Path = Path;
            return clone;
        }

        public void ReadXml(XmlReader reader)
        {
            Path = reader["dataRefName"];
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", "ProSimDataRef");
            writer.WriteAttributeString("dataRefName", Path);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is ProSimDataRef &&
                this.Path == (obj as ProSimDataRef).Path;
        }
    }
} 