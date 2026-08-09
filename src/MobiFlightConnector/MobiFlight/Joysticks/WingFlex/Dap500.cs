using HidSharp;
using System;

namespace MobiFlight.Joysticks.WingFlex
{
    public class DapConfig
    {
        public static readonly byte ReportId = 4;
        public bool AutoBackLightEnabled { get; set; } = false;
        public bool LightSensorEnabled { get; set; } = false;
        public ushort AutoStandByTimeout { get; set; } = 5;

        public byte[] ToData
        {
            get
            {
                return new byte[] {
                    ReportId, // ReportID=4  
                    (byte)((LightSensorEnabled ? 2 : 0) | (AutoBackLightEnabled ? 1 : 0)), // Bit 0: AutoBackLightEnabled, Bit 1: LightSensorEnabled 
                    0, // Reserved 
                    (byte)((AutoStandByTimeout >> 8) & 0xFF), // Higher 8 bits of AutoStandByTimeout 
                    (byte)(AutoStandByTimeout & 0xFF) // Lower 8 bits of AutoStandByTimeout
                };
            }
        }
    }

    internal class Dap500 : Joystick
    {
        /// <summary>
        /// The report implementation for usb report.
        /// </summary>
        private readonly Dap500Report UsbReport = new Dap500Report();
        private readonly HidReportReceiver Receiver = new HidReportReceiver();

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public Dap500(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
        }

        /// <summary>
        /// This creates a connection to the HID device using the HidSharp library.
        /// </summary>
        /// <returns></returns>
        protected bool Connect()
        {
            // Prevent reentry and parallel execution by multiple threads
            lock (this)
            {
                if (Device == null)
                {
                    Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: Definition.VendorId, productID: Definition.ProductId);
                    if (Device == null) return false;
                }

                if (Stream == null)
                {
                    OpenConfiguration config = new OpenConfiguration();
                    config.SetOption(OpenOption.Exclusive, true);
                    Stream = Device.Open(config);
                }

                if (!Receiver.IsReceiving)
                {
                    Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, "Dap500-HID-Reader");
                }
            }

            if (Stream != null)
            {
                InitConfig();
            }

            return true;
        }

        /// <summary>
        /// Initializes the device configuration by sending a DapConfig with specific settings to the device.
        /// We do this once on connecting to the device.
        /// </summary>
        private void InitConfig()
        {
            RequiresOutputUpdate = true;
            SendData(new DapConfig() { AutoBackLightEnabled = false, LightSensorEnabled = true, AutoStandByTimeout = 3600 }.ToData);
        }

        /// <summary>
        /// Restores the default configuration of the device by sending a default DapConfig to the device.
        /// </summary>
        private void RestoreConfig()
        {
            RequiresOutputUpdate = true;
            SendData(new DapConfig() { }.ToData);
        }

        private void OnReportReceived(byte[] inputReport)
        {
            ProcessInputReportBuffer(inputReport);
        }

        private void OnReadError(Exception exception)
        {
            Log.Instance.log($"Dap500 read failed, closing connection: {exception.Message}", LogSeverity.Error);

            // Drop the dead connection so the next Update() can reconnect.
            lock (this)
            {
                Stream?.Close();
                Stream = null;
                Device = null;
            }
        }

        /// <summary>
        /// Update is called by the base class
        /// It is currently needed to ensure that the hid device is correctly initialized.
        /// </summary>
        public override void Update()
        {
            // The Dap500 input is not read via DirectInput
            // so we have to connect it here.
            if (Stream == null || !Receiver.IsReceiving)
            {
                Connect();
            }
        }

        /// <summary>
        /// This processes the input report buffer, triggers button events and stores the state
        /// 
        /// </summary>
        /// <remarks>
        /// This could be done in the base class.
        /// </remarks>
        /// <param name="inputReportBuffer"></param>
        protected void ProcessInputReportBuffer(byte[] inputReportBuffer)
        {
            var newState = UsbReport.Parse(inputReportBuffer).ToJoystickState();

            UpdateButtons(newState);
            UpdateAxis(newState);
            // Finally store the new state as last state
            State = newState;
        }

        /// <summary>
        /// Updates the state of the output device by sending the current output data.
        /// </summary>
        /// <remarks>This method retrieves the output device state and sends it to the device.  
        /// 
        /// It has to be called regularly from an external caller.
        /// 
        /// If the output data is unavailable, the method exits without performing any action.  
        /// If the device is removed during the operation, an <see cref="System.IO.IOException"/> is caught 
        /// inside SendData, and the `OnDeviceRemoved` method is invoked.</remarks>
        public override void UpdateOutputDeviceStates()
        {
            // Feature ReportID=2 for the LEDs
            var data = new byte[] { 2, 0, 0, 0, 0 };

            foreach (var light in Lights)
            {
                data[light.Byte] |= (byte)(light.State << light.Bit);
            }

            try
            {
                SendData(data);
            }
            catch (System.IO.IOException)
            {
                // this happens when the device is removed.
                OnDeviceRemoved();
            }
        }

        /// <summary>
        /// Enumerates and categorizes joystick devices based on their type.
        /// </summary>
        /// <remarks>This method processes the joystick device definitions and categorizes them into 
        /// analog inputs, buttons, or POV controls. Devices are added to their respective  collections based on their
        /// type.</remarks>
        protected override void EnumerateDevices()
        {
            Definition.Inputs.ForEach(d =>
            {
                var device = new JoystickDevice() { Name = d.Name, Label = d.Label, JoystickDeviceType = d.Type };
                switch (d.Type)
                {
                    case JoystickDeviceType.Axis:
                        device.Type = DeviceType.AnalogInput;
                        Axes.Add(device);
                        break;
                    case JoystickDeviceType.Button:
                        device.Type = DeviceType.Button;
                        Buttons.Add(device);
                        break;
                    case JoystickDeviceType.POV:
                        device.Type = DeviceType.Button;
                        POV.Add(device);
                        break;
                }
            });
        }

        /// <summary>
        /// Cleans up any specific resources, e.g. thread and device connection.
        /// </summary>
        public override void Shutdown()
        {
            RestoreConfig();
            Receiver.Stop();

            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }

            base.Shutdown();
        }

        protected override void SendData(byte[] data)
        {
            // Don't try and send data if no outputs are defined.
            if (Definition?.Outputs == null || Definition?.Outputs.Count == 0)
            {
                return;
            }

            if (!RequiresOutputUpdate) return;
            if (Stream == null)
            {
                Connect();
            }

            try
            {
                Stream.SetFeature(data, 0, 5);
            }
            catch (Exception e)
            {
                Log.Instance.log($"Error sending data to device: {e.Message}", LogSeverity.Error);
            }

            RequiresOutputUpdate = false;
        }
    }
}