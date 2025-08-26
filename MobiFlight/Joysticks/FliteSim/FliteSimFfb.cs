using System;

namespace MobiFlight.Joysticks.FliteSim
{
    
    internal class FliteSimFfb : Joystick
    {
        // readonly int VendorId = 0x04D8;
        // readonly int ProductId = 0xE6D6;
        private readonly UdpInterface _udp;
        private float[] _lastReceivedState = new float[10]; // Store last received UDP state
        private readonly object _stateLock = new object();
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
            if (data.Length != 10) return; // Expect 10-float control data

            lock (_stateLock)
            {
                // Check for changes and raise events
                for (int i = 0; i < Math.Min(data.Length, _lastReceivedState.Length); i++)
                {
                    if (Math.Abs(data[i] - _lastReceivedState[i]) > 0.001f) // Float comparison with tolerance
                    {
                        // Map UDP channels to joystick axes/buttons
                        HandleChannelChange(i, data[i], _lastReceivedState[i]);
                    }
                }

                // Update stored state
                Array.Copy(data, _lastReceivedState, Math.Min(data.Length, _lastReceivedState.Length));
            }
        }

        private void HandleChannelChange(int channel, float newValue, float oldValue)
        {
            switch (channel)
            {
                case 3: // Pitch axis (yoke_pitch_ratio)
                    if (Axes.Count > 0)
                    {
                        TriggerButtonPressed(this, new InputEventArgs()
                        {
                            Name = Name,
                            DeviceId = Axes[0].Name, // "Axis X"
                            DeviceLabel = Axes[0].Label, // "Pitch"
                            Serial = Serial,
                            Type = DeviceType.AnalogInput,
                            Value = (int)(newValue * 32767) // Scale to joystick range
                        });
                    }
                    break;

                case 4: // Roll axis (yoke_roll_ratio)
                    if (Axes.Count > 1)
                    {
                        TriggerButtonPressed(this, new InputEventArgs()
                        {
                            Name = Name,
                            DeviceId = Axes[1].Name,
                            DeviceLabel = Axes[1].Label,
                            Serial = Serial,
                            Type = DeviceType.AnalogInput,
                            Value = (int)(newValue * 32767)
                        });
                    }
                    break;

                case 5: // Yaw axis (yoke_heading_ratio)
                    if (Axes.Count > 2)
                    {
                        TriggerButtonPressed(this, new InputEventArgs()
                        {
                            Name = Name,
                            DeviceId = Axes[2].Name,
                            DeviceLabel = Axes[2].Label,
                            Serial = Serial,
                            Type = DeviceType.AnalogInput,
                            Value = (int)(newValue * 32767)
                        });
                    }
                    break;

                case 9: // Autopilot disconnect button
                    if (Buttons.Count > 0)
                    {
                        TriggerButtonPressed(this, new InputEventArgs()
                        {
                            Name = Name,
                            DeviceId = Buttons[0].Name,
                            DeviceLabel = Buttons[0].Label,
                            Serial = Serial,
                            Type = DeviceType.Button,
                            Value = newValue > 0.5f ? 1 : 0 // Convert float to button state
                        });
                    }
                    break;
            }
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
