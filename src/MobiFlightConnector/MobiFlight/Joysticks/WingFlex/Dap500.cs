using HidSharp;
using HidSharp.Reports.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MobiFlight.Joysticks.WingFlex
{
    public class DapConfig
    {
        public bool AutoBackLightEnabled { get; set; }
        public bool LightSensorEnabled { get; set; }
        public int AutoStandByTimeout { get; set; }

        public byte[] ToData
        {
            get
            {
                return new byte[] {
                    4, // ReportID=4  
                    (byte)((LightSensorEnabled ? 2 : 0) | (AutoBackLightEnabled ? 1 : 0)), // Bit 0: AutoBackLightEnabled, Bit 1: LightSensorEnabled 
                    0, // Reserved 
                    (byte)(AutoStandByTimeout >> 8), // Higher 8 bits of AutoStandByTimeout 
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
        protected HidDeviceInputReceiver inputReceiver;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public Dap500(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
        }

        /// <summary>
        /// This creates a connection to the HID device using the Device.Net library.
        /// </summary>
        /// <returns></returns>
        protected bool Connect()
        {
            var VendorId = Definition.VendorId;
            var ProductId = Definition.ProductId;

            // Prevent reentry and parallel execution by multiple threads
            lock (this)
            {
                if (Device == null)
                {
                    Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
                    if (Device == null) return false;
                }

                var reportDescriptor = Device.GetReportDescriptor();

                if (Stream == null)
                {
                    OpenConfiguration config = new OpenConfiguration();
                    config.SetOption(OpenOption.Exclusive, true);
                    Stream = Device.Open(config);
                    Stream.ReadTimeout = Timeout.Infinite;
                }

                if (inputReceiver == null)
                {
                    inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
                    inputReceiver.Received += InputReceiver_Received;
                    inputReceiver.Start(Stream);
                }
            }

            if (Stream != null)
            {
                RequiresOutputUpdate = true;
                SendData(new DapConfig() { AutoBackLightEnabled = false, LightSensorEnabled = true, AutoStandByTimeout = 3600 }.ToData);
            }

            return true;
        }

        private void InputReceiver_Received(object sender, System.EventArgs e)
        {
            var inputReceiver = sender as HidDeviceInputReceiver;
            byte[] inputReportBuffer = new byte[8];

            while (inputReceiver.TryRead(inputReportBuffer, 0, out _))
            {
                //
                ProcessInputReportBuffer(inputReportBuffer);
            }
        }

        /// <summary>
        /// Update is called by the base class
        /// It is currently needed to ensure that the hid device is correctly initialized.
        /// </summary>
        public override async void Update()
        {
            // Octavi is not a DirectInput device
            // so we have to connect it here.
            if (Stream == null || inputReceiver == null)
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
        /// Enumerates and initializes the output devices associated with the current instance.
        /// </summary>
        /// <remarks>This method identifies output devices of type <see cref="DeviceType.LcdDisplay"/> and
        /// adds them to the collection of lights as <see cref="JoystickOutputDisplay"/> instances. The method relies on
        /// the `this.Definition` <see cref="JoystickDefinition"/> property to retrieve device information.</remarks>
        protected override void EnumerateOutputDevices()
        {
            base.EnumerateOutputDevices();
        }

        /// <summary>
        /// Retrieves a collection of distinct output device types that are currently connected.
        /// </summary>
        /// <remarks>
        /// The method examines the outputs defined in the Defintiion property (<see cref="JoystickDefinition"/>) and
        /// determines the corresponding device types. If an output does not specify a type, it is categorized as <see
        /// cref="DeviceType.Output"/>. Duplicate device types are excluded from the result.
        /// </remarks>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DeviceType"/> representing the distinct types of connected
        /// output devices. The collection will be empty if no outputs are defined or connected.</returns>
        public override IEnumerable<DeviceType> GetConnectedOutputDeviceTypes()
        {
            return base.GetConnectedOutputDeviceTypes();
        }

        /// <summary>
        /// Cleans up any specific resources, e.g. thread and device connection.
        /// </summary>
        public override void Shutdown()
        {
            RequiresOutputUpdate = true;
            SendData(new DapConfig() { AutoBackLightEnabled = false, LightSensorEnabled = false, AutoStandByTimeout = 5 }.ToData);
            base.Shutdown();
        }

        /// <summary>
        /// Resets all outputs to a "stop" state
        /// </summary>
        public override void Stop()
        {
            base.Stop();
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

        public void SetConfig(DapConfig config)
        {

        }
    }
}