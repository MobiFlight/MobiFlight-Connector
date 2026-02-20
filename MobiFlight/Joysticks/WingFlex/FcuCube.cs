using Device.Net;
using Hid.Net;
using Hid.Net.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class FcuCube : Joystick
    {
        /// <summary>
        /// Used for reading HID reports in a background thread.
        /// </summary>
        bool DoReadHidReports = false;

        /// <summary>
        /// The thread that reads HID reports.
        /// </summary>
        private Thread readThread;

        /// <summary>
        /// The specific HID device instance.
        /// This is using the Device.Net library for HID communication.
        /// It provides improved performance compared to HidSharp
        /// </summary>
        IHidDevice Device { get; set; }

        /// <summary>
        /// The FCU Cube needs to store the output state of those devices
        /// that are explicityly set.
        /// </summary>
        private List<JoystickOutputDevice> OutputState = new List<JoystickOutputDevice>();

        /// <summary>
        /// The report implementation for the FCU Cube.
        /// </summary>
        private readonly FcuCubeReport FcuCubeReport = new FcuCubeReport();

        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Defintion file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName ?? "FcuCube"; }
        }

        /// <summary>
        /// Provides Serial including prefix.
        /// Serial information is provided through Device.Net
        /// </summary>
        public override string Serial
        {
            get { return $"{Joystick.SerialPrefix}{Device?.ConnectedDeviceDefinition?.SerialNumber}" ?? "FCU-CUBE-1234-ABCD-12345678"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public FcuCube(JoystickDefinition definition) : base(null, definition)
        {
        }

        /// <summary>
        /// This creates a connection to the HID device using the Device.Net library.
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> Connect()
        {
            var VendorId = Definition.VendorId;
            var ProductId = Definition.ProductId;

            var hidFactory = new FilterDeviceDefinition(vendorId: (uint)VendorId, productId: (uint)ProductId).CreateWindowsHidDeviceFactory();
            var deviceDefinitions = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).ToList();

            if (deviceDefinitions.Count == 0)
            {
                Log.Instance.log($"no FcuCube found with VID:{VendorId.ToString("X4")} and PID:{ProductId.ToString("X4")}", LogSeverity.Info);
                return false;
            }

            Device = (IHidDevice)await hidFactory.GetDeviceAsync(deviceDefinitions.First()).ConfigureAwait(false);
            await Device.InitializeAsync().ConfigureAwait(false);

            DoReadHidReports = true;

            readThread = new Thread(ReadHidReportsLoop)
            {
                IsBackground = true,
                Name = "FcuCube-HID-Reader"
            };
            readThread.Start();

            return true;
        }

        /// <summary>
        /// Method called by read thread
        /// </summary>
        private void ReadHidReportsLoop()
        {
            while (DoReadHidReports)
            {
                try
                {
                    var HidReport = Device.ReadReportAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    var data = HidReport.TransferResult.Data;
                    ProcessInputReportBuffer(HidReport.ReportId, data);
                }
                catch
                {
                    // Exception when disconnecting fcu while mobiflight is running.
                    Shutdown();
                    break;
                }
            }
        }

        /// <summary>
        /// Update is called by the base class
        /// It is currently needed to ensure that the hid device is correctly initialized.
        /// </summary>
        public override void Update()
        {
            if (Device == null || !Device.IsInitialized)
            {
                var connected = Connect().GetAwaiter().GetResult();
                if (!connected) return;
            }
        }

        /// <summary>
        /// This processes the input report buffer, triggers button events and stores the state
        /// 
        /// </summary>
        /// <remarks>
        /// This could be done in the base class.
        /// </remarks>
        /// <param name="reportId"></param>
        /// <param name="inputReportBuffer"></param>
        protected void ProcessInputReportBuffer(byte reportId, byte[] inputReportBuffer) {
            var newState = FcuCubeReport.Parse(inputReportBuffer).ToJoystickState();
            UpdateButtons(newState);
            UpdateAxis(newState);
            // Finally store the new state as last state
            State = newState;
        }

        /// <summary>
        /// Sends out the data to the device as correct output report.
        /// </summary>
        /// <param name="data"></param>
        protected async override void SendData(byte[] data)
        {
            if (Device == null || !Device.IsInitialized || data == null) return;

            try
            {
                await Device.WriteReportAsync(data, 0).ConfigureAwait(false);
                RequiresOutputUpdate = false;
            }
            catch (Exception ex)
            {
                // Catch-all to prevent unhandled exceptions from WriteReportAsync from crashing the application.
                // This aligns with the pattern used in ReadHidReportsLoop where all exceptions are caught
                // to handle device removal and unexpected disconnect scenarios gracefully.
                Log.Instance.log($"Exception during write to FcuCube ({ex.GetType().Name}): {ex.Message}", LogSeverity.Error);
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
            var data = FcuCubeReport?.FromOutputDeviceState(OutputState);

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
            Definition?.Outputs?.FindAll(d => d.Type==DeviceType.LcdDisplay.ToString()).ForEach(device =>
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
            DoReadHidReports = false;
            readThread?.Join(1000);
            Device?.Dispose();

            base.Shutdown();
        }

        /// <summary>
        /// Resets all outputs to a "stop" state
        /// </summary>
        public override void Stop()
        {
            // Reset all outputs to initial state
            var data = FcuCubeReport?.FromOutputDeviceState(Lights);
            SendData(data);

            // then clear all output states
            OutputState.Clear();

            base.Stop();
        }
    }
}
