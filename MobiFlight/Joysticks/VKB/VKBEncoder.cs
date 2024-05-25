using System.Collections.Generic;
using static MobiFlight.Joysticks.VKB.VKBEncoder;

namespace MobiFlight.Joysticks.VKB
{
    internal class VKBEncoder
    {
        public enum EncoderAction
        {
            DEC = 0,
            INC = 5
        }
        private bool firstStart = false; // Ensures initial values get pulled from the device when encoder object is created from definition file.
        public JoystickDevice DeviceInc = null; // Virtual button for increment events.
        public JoystickDevice DeviceDec = null; // Virtual button for decrement events.
        private ushort value = 0; // Stores current state of encoder
        public VKBEncoder(byte index, ushort? initialValue = null)
        {
            // Constructor if virtual button devices are not created otherwise.
            if (initialValue == null) firstStart = true;
            else
            {
                value = initialValue?? 0;
                firstStart = false;
            }
            CreateDevices(index);
        }
        public VKBEncoder(JoystickDevice inc, JoystickDevice dec, ushort? initialValue = null)
        {
            // Constructor if virtual button devices have already been created from definition.
            DeviceInc = inc;
            DeviceDec = dec;
            if (initialValue == null)
            {
                firstStart = true;
            }
            else
            {
                value = initialValue ?? 0;
                firstStart = false;
            }
        }
        private void CreateDevices(int index)
        {
            // Virtual buttons use a button ID range that is out of the reach of DirectInput.
            // For ease of readability, the numbering scheme is 1IID, where II is a two-digit encoder ID and D is a direction (0 for decrement, 5 for increment).
            DeviceInc = new JoystickDevice { Name = $"Button {1000 + 10 * index + (int)EncoderAction.INC}", Label = $"Encoder {index+1} INC", Type = DeviceType.Button, JoystickDeviceType = JoystickDeviceType.Button };
            DeviceDec = new JoystickDevice { Name = $"Button {1000 + 10 * index + (int)EncoderAction.DEC}", Label = $"Encoder {index+1} DEC", Type = DeviceType.Button, JoystickDeviceType = JoystickDeviceType.Button };
        }
        public IEnumerable<InputEventArgs> Update(ushort newPosition)
        {
            List<InputEventArgs> events = new List<InputEventArgs>();
            if (firstStart)
            {
                value = newPosition;
                firstStart= false;
                return events;
            }
            short deltaCount = (short)((newPosition - value) & 0xFFFF);
            if (deltaCount > 0)
            {
                while (value != newPosition) // Send one press for each step moved and update value accordingly until the new position is reached.
                {
                    value++;
                    events.Add(new InputEventArgs { DeviceId = DeviceInc.Name, DeviceLabel = DeviceInc.Label, Type = DeviceType.Button, Value = (int)MobiFlightButton.InputEvent.PRESS });
                }
            }
            else if (deltaCount < 0)
            {
                while (value != newPosition)
                {
                    value--;
                    events.Add(new InputEventArgs { DeviceId = DeviceDec.Name, DeviceLabel = DeviceDec.Label, Type = DeviceType.Button, Value = (int)MobiFlightButton.InputEvent.PRESS });
                }
            }
            return events;
        }
    }
}
