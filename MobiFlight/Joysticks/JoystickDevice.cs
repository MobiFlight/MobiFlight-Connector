using MobiFlight.Config;
using MobiFlight.CustomDevices;
using System;
using System.Collections.Generic;

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
            JoystickDeviceType = JoystickDeviceType.Light;
            Type = DeviceType.Output;
        }
    }

    public class JoystickStringOutputDevice : JoystickOutputDevice, ICustomDevice
    {
        public string StringState = null;

        public JoystickStringOutputDevice()
        {
            Type = DeviceType.CustomDevice;
            JoystickDeviceType = JoystickDeviceType.String;
        }

        public JoystickStringOutputDevice(JoystickOutputDevice device)
        {
            Type = DeviceType.CustomDevice;
            Name = device.Name;
            Label = device.Label;
            Byte = device.Byte;
            JoystickDeviceType = JoystickDeviceType.String;
        }

        public void Display(int MessageType, string value)
        {
            StringState = value;
        }

        public List<MessageType> MessageTypes
        {
            get
            {
                var list = new List<MessageType>();
                list.Add(new MessageType() { Id=1, Description = "You can display up to 5 characters.", Label="Set LCD" });
                return list;
            }
        }

        public void Stop()
        {
            // Do nothing
        }
    }
}
