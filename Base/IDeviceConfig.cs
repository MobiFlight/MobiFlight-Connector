using Device.Net;
using MobiFlight.Base.Serialization.Json;
using Newtonsoft.Json;
using System.Xml;

namespace MobiFlight.Base
{
    [JsonConverter(typeof(DeviceConfigConverter))]
    public interface IDeviceConfig
    {
        string Type { get; }
        string OldType { get; }
        string Name { get; }

        object Clone();

        void ReadXml(XmlReader reader);
        void WriteXml(XmlWriter writer);
    }

    public abstract class DeviceConfig : IDeviceConfig
    {
        protected string _type = "NOTSET";
        public virtual string Type { get { return GetType().Name.ToString(); } }

        public virtual string OldType { get { return _type; } }
        public virtual string Name { get; set; }

        abstract public object Clone();

        public virtual void ReadXml(XmlReader reader) {
            // This name is only present with input devices
            // and it is in the wrong place.
            Name = reader["name"];
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            // This name is only present with input devices
            // and it is in the wrong place.
            if (!string.IsNullOrEmpty(Name))
            {
                writer.WriteAttributeString("name", Name);
            }
        }
    }
}
