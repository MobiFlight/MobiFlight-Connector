using MobiFlight.Frontend;

namespace MobiFlight.BrowserMessages
{
    public class DeviceFileOpenRequest
    {
        public bool IgnoreTypeMismatch { get; set; }
        public MobiFlightModuleDeviceAdapter Device { get; set; }
    }
}
