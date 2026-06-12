using MobiFlight.Base;

namespace MobiFlight
{
    public class JoystickDevice : DeviceReference
    {
        public JoystickDeviceType JoystickDeviceType { get; set; }
    }

    public class JoystickOutputDevice : JoystickDevice
    {
        public byte Byte = 0;
        public byte Bit = 0;
        public byte State = 0;
        public JoystickOutputDevice()
        {
            Type = DeviceType.Output;
        }
    }

    public class JoystickOutputDisplay : JoystickOutputDevice
    {
        public byte Cols = 0;
        public byte Lines = 0;
        public string Text = "";
        public JoystickOutputDisplay()
        {
            Type = DeviceType.LcdDisplay;
        }
    }
}