using MobiFlight.Config;
using MobiFlight.Frontend;

namespace MobiFlight.BrowserMessages
{
    public class DeviceUploadRequest
    {
        public MobiFlightModuleDeviceAdapter Device { get; set; }
    }

    public class DeviceElementEditRequest
    {
        public MobiFlightModuleDeviceAdapter Device { get; set; }
        public DeviceElement Element { get; set; }
    }

    public class DeviceElementEditResponse
    {
        public DeviceElement Element { get; set; }
        public string[] Pins { get; set; }

        static public DeviceElementEditResponse Create(BaseDevice baseDevice)
        {
            switch (baseDevice.Type)
            {
                case DeviceType.Button:
                    return CreateButtonResponse(baseDevice);
            }

            return null;
        }

        private static DeviceElementEditResponse CreateButtonResponse(BaseDevice baseDevice)
        {
            var button = baseDevice as Button;
            return new DeviceElementEditResponse
            {
                Element = new DeviceElement
                {
                    Id = button.Name,
                    Name = button.Name,
                    Type = button.Type.ToString()
                },
                Pins = new string[] { button.Pin }
            };
        }
    }
}
