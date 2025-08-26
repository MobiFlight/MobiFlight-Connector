using System;

namespace MobiFlight.Joysticks.FliteSim
{
    
    internal class FliteSimFfb : Joystick
    {
        private readonly FliteSimProtocol _protocol; // Changed from UdpInterface to FliteSimProtocol
        private float[] _lastReceivedState = new float[13]; // Updated from 10 to 13
        private readonly object _stateLock = new object();
        
        public override string Name => "FliteSim FFB (UDP)";
        public override string Serial => "JS-UDP-FFB";

        public FliteSimFfb(UdpSettings settings, JoystickDefinition definition)
            : base(null, definition) // No DIJoystick, no HID definition
        {
            _protocol = new FliteSimProtocol(settings);
            _protocol.ControlDataReceived += OnUdpDataReceived;
            _protocol.HandshakeCompleted += OnHandshakeCompleted;
            _protocol.ConnectionLost += OnConnectionLost;
        }

        public override void Connect(IntPtr handle)
        {
            EnumerateDevices();
            EnumerateOutputDevices();
            _protocol.StartListening();
            
            // Initiate handshake with FFB device
            _protocol.InitiateHandshake();
        }

        private void OnHandshakeCompleted()
        {
            Log.Instance.log("FliteSimFfb: Handshake completed successfully", LogSeverity.Info);
        }

        private void OnConnectionLost()
        {
            Log.Instance.log("FliteSimFfb: Connection lost", LogSeverity.Warn);
        }

        private void OnUdpDataReceived(float[] data)
        {
            if (data.Length != 13) return; // Expect 13-float control data (updated from 10)

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

                case 12: // Autopilot disconnect button (updated from 9 to 12)
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
            // This method is called by the base class to send output data
            // For now, we'll implement flight data sending here
            // In the future, this could be expanded to handle different data types
        }

        /// <summary>
        /// Send flight simulation data to FFB device (30 floats)
        /// </summary>
        /// <param name="flightData">30-element float array with flight simulation data</param>
        public void SendFlightData(float[] flightData)
        {
            _protocol.SendFlightData(flightData);
        }

        protected void TriggerButtonPress(int i, MobiFlightButton.InputEvent inputEvent)
        {
            if (i < Buttons.Count)
            {
                TriggerButtonPressed(this, new InputEventArgs()
                {
                    Name = Name,
                    DeviceId = Buttons[i].Name,
                    DeviceLabel = Buttons[i].Label,
                    Serial = Serial, // Use our own serial instead of DIJoystick
                    Type = DeviceType.Button,
                    Value = (int)inputEvent
                });
            }
        }

        public override void Update()
        {
            // Check for handshake timeouts
            _protocol.CheckHandshakeTimeout();
        }

        protected override void EnumerateDevices()
        {
            // Define axes for the control channels
            Axes.Add(new JoystickDevice { Name = "Axis X", Label = "Pitch", Type = DeviceType.AnalogInput, JoystickDeviceType = JoystickDeviceType.Axis });
            Axes.Add(new JoystickDevice { Name = "Axis Y", Label = "Roll", Type = DeviceType.AnalogInput, JoystickDeviceType = JoystickDeviceType.Axis });
            Axes.Add(new JoystickDevice { Name = "Axis Z", Label = "Yaw", Type = DeviceType.AnalogInput, JoystickDeviceType = JoystickDeviceType.Axis });
            
            // Define buttons for discrete controls
            Buttons.Add(new JoystickDevice { Name = "Button 1", Label = "Autopilot Disconnect", Type = DeviceType.Button, JoystickDeviceType = JoystickDeviceType.Button });
        }

        public override void Stop()
        {
            // Send neutral settings if needed
        }

        public override void Shutdown()
        {
            _protocol?.Dispose(); // This will send quit message and stop listening
        }
    }
}
