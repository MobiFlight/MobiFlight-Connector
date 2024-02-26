using System.Collections.Generic;

namespace MobiFlight.Frontend
{
    public interface IDeviceElement
    {
        string Type { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        Dictionary<string, string> ConfigData { get; set; }
    }

    public interface IDeviceItem
    {
        string Type { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        Dictionary<string, string> MetaData { get; set; }

        IDeviceElement[] Elements { get; set; }

        MobiFlightPin[] Pins { get; set; }

    }

    public class DeviceElement : IDeviceElement
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        public Dictionary<string, string> ConfigData { get; set; }
    }
}
