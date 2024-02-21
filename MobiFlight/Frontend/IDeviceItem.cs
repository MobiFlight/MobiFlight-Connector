using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Configuration;

namespace MobiFlight.Frontend
{
    public interface IDeviceElement
    {
        DeviceType Type { get; set; }
        string Id { get; set; }
        string Name { get; set; }
    }

    public interface IDeviceItem
    {
        string Type { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        Dictionary<string, string> MetaData { get; set; }

        IDeviceElement[] Elements { get; set; }

    }

    public class DeviceElement : IDeviceElement
    {
        public DeviceType Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
