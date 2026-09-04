using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class BaseCube : Joystick
    {
        /// <summary>
        /// Reads HID reports on a dedicated background thread.
        /// </summary>
        private readonly HidReportReceiver Receiver = new HidReportReceiver();

        /// <summary>
        /// The FCU Cube needs to store the output state of those devices
        /// that are explicitly set.
        /// </summary>
        private List<JoystickOutputDevice> OutputState = new List<JoystickOutputDevice>();

        /// <summary>
        /// The report implementation for the Cube.
        /// </summary>
        protected readonly ICubeReport CubeReport;

        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Definition file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName ?? "WFCube"; }
        }

        /// <summary>
        /// Backing field for <see cref="Serial"/>, fetched lazily once the device
        /// is connected. Empty string means the device reports no serial.
        /// </summary>
        private string CachedSerialNumber;

        /// <summary>
        /// Provides Serial including prefix.
        /// Serial information is provided through the HID device.
        /// </summary>
        public override string Serial
        {
            get
            {
                if (CachedSerialNumber == null && Device != null)
                {
                    CachedSerialNumber = GetDeviceSerialNumber() ?? string.Empty;
                }

                return
                    !string.IsNullOrEmpty(CachedSerialNumber) ?
                    $"{Joystick.SerialPrefix}{CachedSerialNumber}"
                    : $"{Name.ToUpper().Replace(" ", "-")}-1234-ABCD-12345678";
            }
        }

        /// <summary>
        /// Some devices refuse to report a serial number - treat that as "no serial"
        /// instead of failing.
        /// </summary>
        private string GetDeviceSerialNumber()
        {
            try
            {
                return Device?.GetSerialNumber();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public BaseCube(ICubeReport report, JoystickDefinition definition) : base(null, definition)
        {
            CubeReport = report;
        }

        /// <summary>
        /// This creates a connection to the HID device using the HidSharp library.
        /// </summary>
        /// <returns></returns>
        protected bool Connect()
        {
            var VendorId = Definition.VendorId;
            var ProductId = Definition.ProductId;

            if (Device == null)
            {
                Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
                if (Device == null)
                {
                    Log.Instance.log($"no {Name} found with VID:{VendorId.ToString("X4")} and PID:{ProductId.ToString("X4")}", LogSeverity.Info);
                    return false;
                }
            }

            if (Stream == null)
            {
                try
                {
                    Stream = Device.Open();
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Failed to open {Name} device: {ex.Message}", LogSeverity.Error);
                    return false;
                }
            }

            if (!Receiver.IsRunning)
            {
                Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, $"{Name}-HID-Reader");
            }

            Log.Instance.log($"Connected to {Name} with VID:{VendorId.ToString("X4")} and PID:{ProductId.ToString("X4")}", LogSeverity.Debug);
            return true;
        }

        private void OnReportReceived(HidReport inputReport)
        {
            ProcessInputReport(inputReport);
        }

        private void OnReadError(Exception exception)
        {
            Log.Instance.log($"Exception during read from {Name} ({exception.GetType().Name}): {exception.Message}", LogSeverity.Error);
            Log.Instance.log($"Stopping read thread and shutting down device {Name}.", LogSeverity.Error);
            // Exception when disconnecting fcu while mobiflight is running.
            Shutdown();
        }

        /// <summary>
        /// Update is called by the base class
        /// It is currently needed to ensure that the hid device is correctly initialized.
        /// </summary>
        public override void Update()
        {
            if (Stream == null || !Receiver.IsRunning)
            {
                var connected = Connect();
                if (!connected) return;
            }
        }

        /// <summary>
        /// This processes the input report, triggers button events and stores the state
        ///
        /// </summary>
        /// <remarks>
        /// This could be done in the base class.
        /// </remarks>
        /// <param name="inputReport">The received HID input report</param>
        protected void ProcessInputReport(HidReport inputReport)
        {
            // The report classes expect the payload without the leading report ID byte,
            // as documented in their protocol tables.
            var newState = CubeReport.Parse(inputReport.Payload).ToJoystickState();
            UpdateButtons(newState);
            UpdateAxis(newState);
            // Finally store the new state as last state
            State = newState;
        }

        /// <summary>
        /// Sends out the data to the device as correct output report.
        /// </summary>
        /// <param name="data"></param>
        protected override void SendData(byte[] data)
        {
            if (Stream == null || data == null) return;

            try
            {
                // The report classes produce the payload without the leading report ID
                // byte (0), which the HID stream expects at index 0.
                var report = new byte[data.Length + 1];
                Array.Copy(data, 0, report, 1, data.Length);
                Stream.Write(report, 0, report.Length);
                RequiresOutputUpdate = false;
            }
            catch (Exception ex)
            {
                // Catch-all to prevent unhandled exceptions from the write from crashing the application.
                // This aligns with the pattern used in OnReadError where all exceptions are caught
                // to handle device removal and unexpected disconnect scenarios gracefully.
                Log.Instance.log($"Exception during write to {Name} ({ex.GetType().Name}): {ex.Message}", LogSeverity.Error);
                OnDeviceRemoved();
            }
        }

        /// <summary>
        /// Sets the actual output device state for simple outputs
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        public override void SetOutputDeviceState(string name, byte state)
        {
            var light = Lights.FirstOrDefault(l => l.Label == name);
            if (light == null) return;

            var outputState = OutputState.FirstOrDefault(l => l.Label == name);
            if (outputState == null)
            {
                outputState = new JoystickOutputDevice() { Name = light.Name, Label = light.Label, Type = light.Type, Byte = light.Byte, Bit = light.Bit, State = state };
                OutputState.Add(outputState);
                RequiresOutputUpdate = true;
                return;
            }

            if (outputState.State == state) return;

            outputState.State = state;
            RequiresOutputUpdate = true;
            return;
        }

        /// <summary>
        /// Sets the actual output device state for lcd outputs
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        public override void SetLcdDisplay(string address, string value)
        {
            var display = Lights.Find(l => l.Name == address) as JoystickOutputDisplay;
            if (display == null) return;

            var outputState = OutputState.FirstOrDefault(l => l.Label == display.Label) as JoystickOutputDisplay;
            if (outputState == null)
            {
                outputState = new JoystickOutputDisplay() { Name = display.Name, Label = display.Label, Type = display.Type, Cols = display.Cols, Lines = display.Lines, Byte = display.Byte, Text = value };
                OutputState.Add(outputState);
                RequiresOutputUpdate = true;
                return;
            }

            if (outputState.Text == value) return;

            outputState.Text = value;
            RequiresOutputUpdate = true;
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
            // if (!RequiresOutputUpdate) return;
            var data = CubeReport?.FromOutputDeviceState(OutputState);

            if (data == null) return;

            SendData(data);
        }

        /// <summary>
        /// Enumerates and categorizes joystick devices based on their type.
        /// </summary>
        /// <remarks>This method processes the joystick device definitions and categorizes them into 
        /// analog inputs, buttons, or POV controls. Devices are added to their respective  collections based on their
        /// type.</remarks>
        protected override void EnumerateDevices()
        {
            Definition.Inputs.ForEach(d => {
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

            // LcdDisplays
            Definition?.Outputs?.FindAll(d => d.Type == DeviceType.LcdDisplay.ToString()).ForEach(device =>
            {
                Lights.Add(new JoystickOutputDisplay() { Name = device.Id, Label = device.Label, Type = DeviceType.LcdDisplay, Cols = device.Cols, Lines = device.Lines, Byte = device.Byte });
            });
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
            List<DeviceType> result = new List<DeviceType>();

            Definition?.Outputs?.ForEach(d => {
                if (d.Type == null && !result.Contains(DeviceType.Output))
                {
                    result.Add(DeviceType.Output);
                    return;
                }

                if (Enum.TryParse<DeviceType>(d.Type, out var deviceType) && !result.Contains(deviceType))
                {
                    result.Add(deviceType);
                }
            });

            return result;
        }

        /// <summary>
        /// Cleans up any specific resources, e.g. thread and device connection.
        /// </summary>
        public override void Shutdown()
        {
            Receiver.Stop();
            Stream?.Close();
            Stream = null;
            Device = null;

            base.Shutdown();
        }

        /// <summary>
        /// Resets all outputs to a "stop" state
        /// </summary>
        public override void Stop()
        {
            // Reset all outputs to initial state
            var data = CubeReport?.FromOutputDeviceState(Lights);
            SendData(data);

            // then clear all output states
            OutputState.Clear();

            base.Stop();
        }
    }
}