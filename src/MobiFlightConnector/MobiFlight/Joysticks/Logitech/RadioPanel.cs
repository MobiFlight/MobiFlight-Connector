using HidSharp;
using MobiFlight.Firmware;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Native raw HID controller for the Logitech Radio Panel.
    /// Aircraft-specific mapping stays in the MobiFlight configs. The LCDs show
    /// COM1/COM2 values; the other selector positions are input-only.
    /// </summary>
    internal sealed class RadioPanel : Joystick
    {
        private const int UpperSelectorFirstButton = 0;
        private const int LowerSelectorFirstButton = 7;
        private const int SelectorPositionCount = 7;

        private readonly HidReportReceiver Receiver = new HidReportReceiver();
        private readonly object ConnectionLock = new object();
        private readonly object OutputLock = new object();
        private readonly RadioPanelDisplayState DisplayState = new RadioPanelDisplayState();
        private string CachedSerialNumber;
        private bool Disconnected;
        private bool OpenFailureLogged;

        // Guarded by OutputLock: bands and cached values are read together when the LCDs are redrawn.
        private RadioPanelBand UpperBand = RadioPanelBand.None;
        private RadioPanelBand LowerBand = RadioPanelBand.None;
        private string Com1Active, Com1Standby, Com2Active, Com2Standby;

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
        /// Converts a raw Radio Panel HID report into MobiFlight button state and
        /// follows the COM1/COM2 selector of each row to pick what the LCDs show.
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

            UpdateButtons(newState);
            UpdateBandsFromState(newState);
            State = newState;
        }

        /// <summary>
        /// Each row has a 7-position rotary selector reported as mutually exclusive buttons.
        /// Only COM1 and COM2 have an LCD source, any other position blanks the row.
        /// </summary>
        private void UpdateBandsFromState(JoystickState newState)
        {
            // No bit set is a transitional read while the rotary moves, keep the previous band.
            var upper = GetBand(newState, UpperSelectorFirstButton) ?? UpperBand;
            var lower = GetBand(newState, LowerSelectorFirstButton) ?? LowerBand;

            lock (OutputLock)
            {
                if (upper == UpperBand && lower == LowerBand) return;

                UpperBand = upper;
                LowerBand = lower;
                RequiresOutputUpdate = true;
            }
        }

        private static RadioPanelBand? GetBand(JoystickState state, int firstButton)
        {
            if (state.Buttons[firstButton]) return RadioPanelBand.Com1;
            if (state.Buttons[firstButton + 1]) return RadioPanelBand.Com2;

            for (var position = 2; position < SelectorPositionCount; position++)
            {
                if (state.Buttons[firstButton + position]) return RadioPanelBand.None;
            }

            return null;
        }

        /// <summary>
        /// Receives a frequency from a MobiFlight output config (address matches
        /// the JSON output Id, e.g. "LCD COM1 ACTIVE") and caches it for the next refresh.
        /// </summary>
        protected override void OnSetLcdDisplay(string address, string value)
        {
            lock (OutputLock)
            {
                switch (address)
                {
                    case "LCD COM1 ACTIVE":
                        if (Com1Active == value) return;
                        Com1Active = value;
                        break;
                    case "LCD COM1 STBY":
                        if (Com1Standby == value) return;
                        Com1Standby = value;
                        break;
                    case "LCD COM2 ACTIVE":
                        if (Com2Active == value) return;
                        Com2Active = value;
                        break;
                    case "LCD COM2 STBY":
                        if (Com2Standby == value) return;
                        Com2Standby = value;
                        break;
                    default:
                        Log.Instance.log($"{Name} received LCD value for unknown address '{address}'", LogSeverity.Warn);
                        return;
                }

                RequiresOutputUpdate = true;
            }
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

                DisplayState.SetValues(Com1Active, Com1Standby, Com2Active, Com2Standby, UpperBand, LowerBand);
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
