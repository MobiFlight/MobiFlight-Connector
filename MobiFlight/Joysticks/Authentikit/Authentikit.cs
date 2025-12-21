using Device.Net;
using Hid.Net;
using Hid.Net.Windows;
using SharpDX.DirectInput;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Joysticks.AuthentiKit
{
    internal class AuthentiKit : Joystick
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
        /// The report implementation.
        /// </summary>
        private readonly AuthentiKitReport report = new AuthentiKitReport();

        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Definition file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName.Trim() ?? "AuthentiKit"; }
        }

        /// <summary>
        /// Provides Serial including prefix.
        /// Serial information is provided through DirectInput GUID.
        /// </summary>
        public override string Serial
        {
            get { return $"{Joystick.SerialPrefix}{DIJoystick.Information.InstanceGuid}"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="joystick">The DirectInput joystick instance.</param>
        /// <param name="definition">Joystick definition file.</param>
        public AuthentiKit(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
        }

        /// <summary>
        /// This creates a connection to the HID device using the Device.Net library.
        /// </summary>
        /// <returns>True if connection was successful, false otherwise.</returns>
        protected async Task<bool> Connect()
        {
            var VendorId = Definition.VendorId;
            var ProductId = Definition.ProductId;

            var hidFactory = new FilterDeviceDefinition(vendorId: (uint)VendorId, productId: (uint)ProductId).CreateWindowsHidDeviceFactory(writeBufferSize: 1);
            var deviceDefinitions = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).ToList();

            if (deviceDefinitions.Count == 0)
            {
                Log.Instance.log($"no AuthentiKit found with VID:{VendorId.ToString("X4")} and PID:{ProductId.ToString("X4")}", LogSeverity.Info);
                return false;
            }

            Device = (IHidDevice)await hidFactory.GetDeviceAsync(deviceDefinitions.First()).ConfigureAwait(false);

            try
            {
                await Device.InitializeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Failed to open AuthentiKit device: {ex.Message}", LogSeverity.Error);
                return false;
            }

            DoReadHidReports = true;

            readThread = new Thread(ReadHidReportsLoop)
            {
                IsBackground = true,
                Name = "Authentikit-HID-Reader"
            };
            readThread.Start();

            return true;
        }

        /// <summary>
        /// Continuously reads HID reports from the device in a background thread.
        /// Processes incoming reports and handles disconnection gracefully.
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
                    // Exception when disconnecting while mobiflight is running.
                    Shutdown();
                    break;
                }
            }
        }

        /// <summary>
        /// Update is called by the base class.
        /// It ensures that the HID device is correctly initialized.
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
        /// This processes the input report buffer, triggers button events and stores the state.
        /// </summary>
        /// <param name="reportId">The HID report ID</param>
        /// <param name="inputReportBuffer">The report data buffer</param>
        protected void ProcessInputReportBuffer(byte reportId, byte[] inputReportBuffer)
        {
            var newState = report.Parse(inputReportBuffer).ToJoystickState(Axes);
            UpdateButtons(newState);
            UpdateAxis(newState);
            // Finally store the new state as last state
            State = newState;
        }

        /// <summary>
        /// Cleans up HID device resources and stops the background reading thread.
        /// </summary>
        public override void Shutdown()
        {
            DoReadHidReports = false;
            readThread?.Join(1000);
            Device?.Dispose();

            base.Shutdown();
        }

        /// <summary>
        /// We are applying some hysteresis to avoid noise triggering events.
        /// </summary>
        /// <param name="newState">The new joystick state to compare against.</param>
        protected override void UpdateAxis(JoystickState newState)
        {
            for (int CurrentAxis = 0; CurrentAxis != Axes.Count; CurrentAxis++)
            {

                int oldValue = 0;
                if (StateExists())
                {
                    oldValue = GetValueForAxisFromState(CurrentAxis, State);
                }

                int newValue = GetValueForAxisFromState(CurrentAxis, newState);

                if (StateExists() && !ExceedsThreshold(oldValue, newValue)) continue;

                TriggerButtonPressed(this, new InputEventArgs()
                {
                    Name = Name,
                    DeviceId = Axes[CurrentAxis].Name,
                    DeviceLabel = Axes[CurrentAxis].Label,
                    Serial = Serial,
                    Type = DeviceType.AnalogInput,
                    Value = newValue
                });
            }
        }

        /// <summary>
        /// Tests if the change in axis value exceeds the defined threshold.
        /// </summary>
        /// <param name="oldValue">The old joystick value</param>
        /// <param name="newValue">The new joystick value to compare.</param>
        /// <returns>True if the change exceeds the threshold; otherwise, false.</returns>
        private static bool ExceedsThreshold(int oldValue, int newValue)
        {
            return Math.Abs(oldValue - newValue) >= 2 << 4;
        }
    }
}