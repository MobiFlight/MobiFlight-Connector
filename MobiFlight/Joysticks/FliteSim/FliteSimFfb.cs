using System;

namespace MobiFlight.Joysticks.FliteSim
{
    
    internal class FliteSimFfb : Joystick
    {
        // readonly int VendorId = 0x04D8;
        // readonly int ProductId = 0xE6D6;
        private readonly UdpInterface _udp;
        public override string Name => "FliteSim FFB (UDP)";
        public override string Serial => "JS-UDP-FFB";

        public FliteSimFfb(UdpSettings settings, JoystickDefinition definition)
            : base(null, definition) // No DIJoystick, no HID definition
        {
            _udp = new UdpInterface(settings);
            _udp.DataReceived += OnUdpDataReceived;
        }

        public override void Connect(IntPtr handle)
        {
            EnumerateDevices();
            EnumerateOutputDevices();
            _udp.StartListening();
        }

        private void OnUdpDataReceived(float[] data)
        {

        }

        protected override void SendData(byte[] data)
        {

        }

        protected void TriggerButtonPress(int i, MobiFlightButton.InputEvent inputEvent)
        {
            TriggerButtonPressed(this, new InputEventArgs()
            {
                Name = Name,
                DeviceId = Buttons[i].Name,
                DeviceLabel = Buttons[i].Label,
                Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString(),
                Type = DeviceType.Button,
                Value = (int)inputEvent
            });
        }

        public override void Update()
        {
        }

        protected override void EnumerateDevices()
        {
            // Manually define axes/buttons/POV as per protocol
            Axes.Add(new JoystickDevice { Name = "Axis X", Label = "Pitch", Type = DeviceType.AnalogInput, JoystickDeviceType = JoystickDeviceType.Axis });
            Axes.Add(new JoystickDevice { Name = "Axis Y", Label = "Roll", Type = DeviceType.AnalogInput, JoystickDeviceType = JoystickDeviceType.Axis });
            Axes.Add(new JoystickDevice { Name = "Axis Z", Label = "Yaw", Type = DeviceType.AnalogInput, JoystickDeviceType = JoystickDeviceType.Axis });
            // Add buttons as needed
        }

        public override void Stop()
        {
            // send some neutral settings to the joystick devices.
        }

        public override void Shutdown()
        {
            _udp.StopListening();
            _udp.Dispose();
        }
    }
}
