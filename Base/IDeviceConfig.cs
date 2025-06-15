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
        string Name { get; set; }

        object Clone();

        void ReadXml(XmlReader reader);
        void WriteXml(XmlWriter writer);
    }

    public abstract class DeviceConfig : IDeviceConfig
    {
        // part of temporary refactoring
        // was originally in InputConfigItem
        public const string TYPE_INPUT_NOTSET = "-";
        public const string TYPE_OUTPUT_NOTSET = "";

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

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is DeviceConfig)) return false;
            DeviceConfig other = obj as DeviceConfig;
            return this.Type == other.Type &&
                   this.Name == other.Name;
        }
    }
}
