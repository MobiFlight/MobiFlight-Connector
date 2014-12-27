using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig 
{
    public enum EncoderInputDirectionType
    {
        LEFT,
        RIGHT
    }

    public enum EncoderInputEventType
    {
        NORMAL,
        FAST,
        FASTEST
    }

    public class EncoderInputConfig : IXmlSerializable, ICloneable
    {
        [XmlElement]
        public EncoderInputDirectionType direction = EncoderInputDirectionType.LEFT;

        [XmlElement]
        public EncoderInputEventType type = EncoderInputEventType.NORMAL;

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
