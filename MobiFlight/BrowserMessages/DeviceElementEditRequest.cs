using MobiFlight.Frontend;

namespace MobiFlight.BrowserMessages
{
    public class DeviceUploadRequest
    {
        public MobiFlightModuleDeviceAdapter Device { get; set; }
    }

    public class DeviceElementCreateRequest
    {
        public MobiFlightModuleDeviceAdapter Device { get; set; }
        public string ElementType { get; set; }
    }

    public class DeviceElementCreateResponse
    {
        public MobiFlightModuleDeviceAdapter Device { get; set; }
        public IDeviceElement Element { get; set; }
    }
}
