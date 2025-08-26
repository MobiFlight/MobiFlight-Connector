using System;
using System.Linq;

namespace MobiFlight.Joysticks.FliteSim
{
    
    internal class FliteSimFfb : Joystick
    {
        private readonly FliteSimProtocol _protocol; // Changed from UdpInterface to FliteSimProtocol
        private float[] _lastReceivedState = new float[13]; // Updated from 10 to 13
        private readonly object _stateLock = new object();
        
        // Field for storing flight data for UDP transmission
        private float[] _flightData = new float[30];
        private readonly object _flightDataLock = new object();

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
            // Use the JSON-based device lookup instead of hardcoded mapping
            var device = GetDeviceForChannel(channel);
            if (device == null) return;

            if (device.Type == DeviceType.AnalogInput)
            {
                HandleAxisChange(channel, newValue);
            }
            else if (device.Type == DeviceType.Button)
            {
                HandleButtonChange(channel, newValue);
            }
        }

        private void HandleAxisChange(int channel, float newValue)
        {
            var device = GetDeviceForChannel(channel);
            if (device == null) return;

            TriggerButtonPressed(this, new InputEventArgs()
            {
                Name = Name,
                DeviceId = device.Name,
                DeviceLabel = device.Label,
                Serial = Serial,
                Type = device.Type,
                Value = (int)(newValue * 32767) // Scale to joystick range
            });
        }

        private void HandleButtonChange(int channel, float newValue)
        {
            var device = GetDeviceForChannel(channel);
            if (device == null) return;

            TriggerButtonPressed(this, new InputEventArgs()
            {
                Name = Name,
                DeviceId = device.Name,
                DeviceLabel = device.Label,
                Serial = Serial,
                Type = DeviceType.Button,
                Value = newValue > 0.5f ? 1 : 0
            });
        }

        private float GetValueForDevice(JoystickDevice device, float newValue)
        {
            return (device.Type == DeviceType.AnalogInput ? newValue * 32000 : newValue);
        }

        private JoystickDevice GetDeviceForChannel(int channel)
        {
            // find it based on information in the definition
            var deviceDefinition = Definition?.Inputs.Find(d => d.Id == channel);
            var device = Axes.Find(a => a.Name == deviceDefinition?.Name) ?? Buttons.Find(b=>b.Name==deviceDefinition?.Name);
            return device;
        }

        public override void SetOutputDeviceState(string name, float value)
        {
            // Handle flight data outputs for UDP transmission
            // Map output device names to flight data array indices
            // This will be called by ConfigItemExecutor for each flight parameter
            
            // For now, find the output device and store the value
            var outputDevice = Lights.FirstOrDefault(l => l.Label == name);
            if (outputDevice != null)
            {
                lock (_flightDataLock)
                {
                    // Map the output device to the appropriate flight data index
                    // This is a simplified mapping - in practice you'd map based on the device definition
                    var index = outputDevice.Byte; // Use byte as index for now
                    if (index < _flightData.Length)
                    {
                        _flightData[index] = value;
                        RequiresOutputUpdate = true;
                    }
                }
            }
        }

        public override void UpdateOutputDeviceStates()
        {
            // Override to send flight data via UDP instead of HID
            if (!RequiresOutputUpdate) return;
            
            lock (_flightDataLock)
            {
                // Send the current flight data array via UDP
                _protocol.SendFlightData(_flightData);
                RequiresOutputUpdate = false;
            }
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
            // Use the JSON definition instead of hardware enumeration since this is a virtual device
            if (Definition?.Inputs != null)
            {
                foreach (var input in Definition.Inputs)
                {
                    if (input.Type == JoystickDeviceType.Axis)
                    {
                        Axes.Add(new JoystickDevice 
                        { 
                            Name = input.Name, 
                            Label = input.Label, 
                            Type = DeviceType.AnalogInput, 
                            JoystickDeviceType = JoystickDeviceType.Axis 
                        });
                    }
                    else if (input.Type == JoystickDeviceType.Button)
                    {
                        Buttons.Add(new JoystickDevice 
                        { 
                            Name = input.Name, 
                            Label = input.Label, 
                            Type = DeviceType.Button, 
                            JoystickDeviceType = JoystickDeviceType.Button 
                        });
                    }
                }
            }
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
