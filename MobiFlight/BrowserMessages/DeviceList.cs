using System.Collections.Generic;

namespace MobiFlight.BrowserMessages
{
    public class DeviceList
    {
        public List<Frontend.IDeviceItem> Devices { get; set; } = new List<Frontend.IDeviceItem>();
    }
}
