using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlaneConnector;

namespace MobiFlight.xplane
{
    public class XplaneDataRef
    {
        private DataRefElement dataRefElement = new DataRefElement();
        public String Path
        {
            get { return dataRefElement.DataRef; }
            set
            {
                if (dataRefElement.DataRef == value) return;
                dataRefElement.DataRef = value;
            }
        }

        public object Clone()
        {
            XplaneDataRef clone = new XplaneDataRef();
            clone.dataRefElement = new DataRefElement();
            clone.dataRefElement.DataRef = dataRefElement.DataRef;

            return clone;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {

            Path = reader["path"];
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", "XplaneDataRef");
            writer.WriteAttributeString("path", Path);
        }

        public override bool Equals(object obj)
        {
            return
                this.Path == (obj as XplaneDataRef).Path;
        }
    }
}
