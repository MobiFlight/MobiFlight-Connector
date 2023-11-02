using MobiFlight.Config;
using System;

namespace MobiFlight
{
    public class JoystickDevice : IBaseDevice
    {
        public DeviceType Type { get; set; }
        public String Name { get; set; }
        public String Label { get; set; }

        public JoystickDeviceType JoystickDeviceType { get; set; }

        public ListItem<IBaseDevice> ToListItem()
        {
            return new ListItem<IBaseDevice>() { Label = Label, Value = this };
        }
    }

    public class JoystickOutputDevice : JoystickDevice
    {
        public byte Byte = 0;
        public byte Bit = 0;
        public byte State = 0;
        public JoystickOutputDevice()
        {
            Type = DeviceType.LedModule;
        }
    }
}
