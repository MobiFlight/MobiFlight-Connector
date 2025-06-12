using MobiFlight.Base.Serialization.Json;
using Newtonsoft.Json;

namespace MobiFlight.Base
{
    [JsonConverter(typeof(DeviceConfigConverter))]
    public interface IDeviceConfig
    {
        string Type { get; }
        string OldType { get; }
        string Name { get; }

        object Clone();
    }

    public abstract class DeviceConfig : IDeviceConfig
    {
        protected string _type = "NOTSET";
        public virtual string Type { get { return GetType().Name.ToString(); } }

        public virtual string OldType { get { return _type; } }
        public virtual string Name { get; set; }

        abstract public object Clone();
    }
}
