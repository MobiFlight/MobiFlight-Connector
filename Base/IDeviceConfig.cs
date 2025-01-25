namespace MobiFlight.Base
{
    public interface IDeviceConfig
    {
        string Type { get; }
        string Name { get; }
    }

    public abstract class DeviceConfig : IDeviceConfig
    {
        public virtual string Type { get { return GetType().ToString(); } }
        public virtual string Name { get; set; }
    }
}
