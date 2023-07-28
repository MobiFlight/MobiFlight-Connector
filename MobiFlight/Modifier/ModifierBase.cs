using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Modifier
{
    public abstract class ModifierBase : IXmlSerializable, ICloneable
    {
        public bool Active { get; set; }
        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public abstract void ReadXml(XmlReader reader);

        public abstract void WriteXml(XmlWriter writer);

        public abstract object Clone();

        public abstract string ToSummaryLabel();

        public abstract ConnectorValue Apply(ConnectorValue value, List<ConfigRefValue> configRefs);
    }
}
