using MobiFlight.Base;
using MobiFlight.Config;

namespace MobiFlight.BrowserMessages.Outgoing
{
    internal class ScanForInputResult
    {
        public Controller Controller { get; set; }
        public DeviceReference Device { get; set; }
    }
}