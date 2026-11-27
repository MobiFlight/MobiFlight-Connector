using HidSharp;
using MobiFlight.Firmware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Native raw HID controller for the Logitech Radio Panel.
    /// Aircraft-specific mapping stays in the MobiFlight configs. Each row shows the
    /// active/standby values of the mode selected on its rotary, and the encoders and
    /// ACT/STBY buttons are additionally reported as mode-specific inputs.
    /// </summary>
    internal sealed class RadioPanel : Joystick
    {
        private const int UpperSelectorFirstButton = 0;
        private const int LowerSelectorFirstButton = 7;

        // Mode buttons first: input detection in the config wizard takes the first press,
        // and the mode button is the one a user wants in almost every case.
        private static readonly int[] ButtonEventOrder = Enumerable
            .Range(RadioPanelModeInputs.FirstButton, RadioPanelModeInputs.ButtonCount)
            .Concat(Enumerable.Range(0, RadioPanelReport.ButtonCount))
            .ToArray();

        private readonly HidReportReceiver Receiver = new HidReportReceiver();
        private readonly object ConnectionLock = new object();
        private readonly object OutputLock = new object();
        private string CachedSerialNumber;
        private bool Disconnected;
        private bool OpenFailureLogged;

        // Guarded by OutputLock: the selected modes and cached values are read together when the LCDs are redrawn.
        private readonly RadioPanelDisplayState DisplayState = new RadioPanelDisplayState();

        // Only used on the HID reader thread.
        private readonly RadioPanelModeInputs ModeInputs = new RadioPanelModeInputs();
        private RadioPanelMode UpperMode = RadioPanelMode.None;
        private RadioPanelMode LowerMode = RadioPanelMode.None;

        public override string Name => Definition?.InstanceName ?? "Logitech Radio Panel";

        public override string Serial
        {
            get
            {
                if (CachedSerialNumber == null && Device != null)
                {
                    CachedSerialNumber = GetDeviceSerialNumber() ?? string.Empty;
                }

                return !string.IsNullOrEmpty(CachedSerialNumber)
                    ? $"{SerialPrefix}{CachedSerialNumber}"
                    : $"{SerialPrefix}{Name.ToUpper().Replace(" ", "-")}-06A3-0D05";
            }
        }

        public RadioPanel(JoystickDefinition definition) : base(null, definition)
        {
        }

        public override void Connect(IntPtr handle)
        {
            if (Buttons.Count == 0)
            {
                EnumerateDevices();
                EnumerateOutputDevices();
            }

            ConnectHid();
        }

        private bool ConnectHid()
        {
            lock (ConnectionLock)
            {
                if (Disconnected) return false;

                if (Device == null)
                {
                    Device = DeviceList.Local.GetHidDeviceOrNull(
                        vendorID: Definition.VendorId,
                        productID: Definition.ProductId);
                    if (Device == null)
                    {
                        Log.Instance.log($"No {Name} found with VID:{Definition.VendorId:X4} and PID:{Definition.ProductId:X4}", LogSeverity.Info);
                        return false;
                    }
                }

                if (Stream == null)
                {
                    try
                    {
                        Stream = Device.Open();
                        OpenFailureLogged = false;
                    }
                    catch (Exception ex)
                    {
                        if (!OpenFailureLogged)
                        {
                            OpenFailureLogged = true;
                            Log.Instance.log($"Failed to open {Name} VID:{Device.VendorID:X4} PID:{Device.ProductID:X4} Path:{Device.DevicePath}: {ex.Message}", LogSeverity.Error);
                        }
                        return false;
                    }
                }

                if (!Receiver.IsRunning)
                {
                    Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, "RadioPanel-HID-Reader");
                }
            }

            Log.Instance.log($"{Name} detected: VID:{Device.VendorID:X4} PID:{Device.ProductID:X4} Serial:{Serial} Path:{Device.DevicePath} MaxInputReportLength:{Device.GetMaxInputReportLength()} MaxFeatureReportLength:{Device.GetMaxFeatureReportLength()}", LogSeverity.Debug);
            return true;
        }

        protected override void EnumerateDevices()
        {
            Buttons.Clear();
            Definition?.Inputs?.ForEach(input =>
            {
                if (input.Type != JoystickDeviceType.Button) return;

                Buttons.Add(new JoystickDevice
                {
                    Name = input.Name,
                    Label = input.Label,
                    Type = DeviceType.Button,
                    JoystickDeviceType = JoystickDeviceType.Button
                });
            });
        }

        protected override void EnumerateOutputDevices()
        {
            Lights.Clear();
            Definition?.Outputs?.ForEach(device =>
            {
                if (device.Type != DeviceType.LcdDisplay.ToString()) return;

                Lights.Add(new JoystickOutputDisplay
                {
                    Name = device.Id,
                    Label = device.Label,
                    Type = DeviceType.LcdDisplay,
                    Cols = device.Cols,
                    Lines = device.Lines,
                    Byte = device.Byte
                });
            });
        }

        /// <summary>
        /// Converts a raw Radio Panel HID report into MobiFlight button state, adds the
        /// mode-specific buttons and follows the selector of each row to pick what the LCDs show.
        /// </summary>
        private void OnReportReceived(HidReport inputReport)
        {
            if (inputReport.ReportId != 0)
            {
                Log.Instance.log($"{Name}: ignoring HID report with unexpected report ID {inputReport.ReportId:X2}.", LogSeverity.Debug);
                return;
            }

            JoystickState newState;
            try
            {
                newState = RadioPanelReport.Parse(inputReport.Payload).ToJoystickState();
            }
            catch (ArgumentException ex)
            {
                Log.Instance.log($"{Name}: ignoring malformed HID report: {ex.Message}", LogSeverity.Debug);
                return;
            }

            // No bit set is a transitional read while the rotary moves, keep the previous mode.
            UpperMode = GetMode(newState, UpperSelectorFirstButton) ?? UpperMode;
            LowerMode = GetMode(newState, LowerSelectorFirstButton) ?? LowerMode;

            ModeInputs.Apply(newState, UpperMode, LowerMode);
            // Skip mode buttons a custom definition without them does not declare.
            UpdateButtons(newState, ButtonEventOrder.Where(index => index < Buttons.Count));

            lock (OutputLock)
            {
                if (DisplayState.SetModes(UpperMode, LowerMode))
                {
                    RequiresOutputUpdate = true;
                }
            }

            State = newState;
        }

        /// <summary>
        /// Each row has a 7-position rotary selector reported as mutually exclusive buttons.
        /// </summary>
        private static RadioPanelMode? GetMode(JoystickState state, int firstButton)
        {
            for (var position = 0; position < RadioPanelModes.Count; position++)
            {
                if (state.Buttons[firstButton + position]) return (RadioPanelMode)position;
            }

            return null;
        }

        /// <summary>
        /// Receives a value from a MobiFlight output config (address matches the JSON
        /// output Id, e.g. "LCD COM1 ACTIVE" or "LCD XPDR STBY") and caches it for the next refresh.
        /// </summary>
        protected override void OnSetLcdDisplay(string address, string value)
        {
            if (!RadioPanelDisplayState.TryParseAddress(address, out var mode, out var slot))
            {
                Log.Instance.log($"{Name} received LCD value for unknown address '{address}'", LogSeverity.Warn);
                return;
            }

            lock (OutputLock)
            {
                if (DisplayState.SetValue(mode, slot, value))
                {
                    RequiresOutputUpdate = true;
                }
            }
        }

        /// <summary>
        /// Called when MobiFlight stops running the configs. Forgets the cached values, so a display
        /// that no config writes to anymore (e.g. after moving a config to another display) blanks
        /// instead of showing its last value until MobiFlight restarts.
        /// </summary>
        protected override void OnStop()
        {
            lock (OutputLock)
            {
                DisplayState.ClearValues();
                RequiresOutputUpdate = true;
            }

            UpdateOutputDeviceStates();
        }

        private void OnReadError(Exception exception)
        {
            Log.Instance.log($"{Name} read failed, disconnecting {Serial}: {exception.Message}", LogSeverity.Error);
            Disconnect();
        }

        public override void Update()
        {
            if (Disconnected) return;

            if (Stream == null || !Receiver.IsRunning)
            {
                ConnectHid();
            }

            UpdateOutputDeviceStates();
        }

        public override void UpdateOutputDeviceStates()
        {
            if (!RequiresOutputUpdate || Disconnected) return;

            lock (OutputLock)
            {
                if (!RequiresOutputUpdate || Disconnected) return;
                if (Stream == null && !ConnectHid()) return;

                var featureReport = DisplayState.ToFeatureReport();

                try
                {
                    Stream.SetFeature(featureReport, 0, featureReport.Length);
                    RequiresOutputUpdate = false;
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"{Name} output write failed, disconnecting {Serial}: {ex.Message}", LogSeverity.Error);
                    Disconnect();
                }
            }
        }

        public override void Shutdown()
        {
            lock (ConnectionLock)
            {
                Disconnected = true;
            }

            Receiver.Stop();
            lock (ConnectionLock)
            {
                Stream?.Close();
                Stream = null;
            }

            base.Shutdown();
        }

        private void Disconnect()
        {
            lock (ConnectionLock)
            {
                if (Disconnected) return;
                Disconnected = true;
            }

            Receiver.Stop();
            lock (ConnectionLock)
            {
                Stream?.Close();
                Stream = null;
            }

            OnDeviceRemoved();
        }

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

        public override IEnumerable<DeviceType> GetConnectedOutputDeviceTypes()
        {
            return new List<DeviceType> { DeviceType.LcdDisplay };
        }
    }
}
