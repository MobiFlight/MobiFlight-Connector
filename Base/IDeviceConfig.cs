namespace MobiFlight.Base
{
    public interface IDeviceConfig
    {
        string Type { get; }
        string Name { get; }

        object Clone();
    }

    public abstract class DeviceConfig : IDeviceConfig
    {
        public virtual string Type { get { return GetType().Name.ToString(); } }
        public virtual string Name { get; set; }

        abstract public object Clone();
    }
}
