using MobiFlight.Firmware;
using HidSharp;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Native raw HID controller for the Logitech Multi Panel.
    /// </summary>
    internal sealed class MultiPanel : Joystick
    {
        private readonly HidReportReceiver Receiver = new HidReportReceiver();
        private readonly object ConnectionLock = new object();
        private readonly object OutputLock = new object();
        private string CachedSerialNumber;
        private bool Disconnected;
        private bool OpenFailureLogged;

        private const int FeatureReportLength = 13;
        private readonly MultiPanelLedState LedState = new MultiPanelLedState();
        private readonly MultiPanelDisplayState DisplayState = new MultiPanelDisplayState();

        // Indices into JoystickState.Buttons matching the JSON's SEL ALT..SEL CRS inputs (Ids 0-4).
        private const int SelectorButtonAlt = 0;
        private const int SelectorButtonVs = 1;
        private const int SelectorButtonIas = 2;
        private const int SelectorButtonHdg = 3;
        private const int SelectorButtonCrs = 4;

        // Guarded by OutputLock: both the selector and the cached values must be
        // read together and consistently by UpdateOutputDeviceStates().
        private MultiPanelSelector CurrentSelector = MultiPanelSelector.Alt;
        private int AltValue, VsValue, IasValue, HdgValue, CrsValue;

        public override string Name => Definition?.InstanceName ?? "Logitech Multi Panel";

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
                    : $"{SerialPrefix}{Name.ToUpper().Replace(" ", "-")}-1234-ABCD-12345678";
            }
        }

        public MultiPanel(JoystickDefinition definition) : base(null, definition)
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
                        Log.Instance.log($"No {Name} found with VID:{Definition.VendorId:X4} " + $"and PID:{Definition.ProductId:X4}", LogSeverity.Info);
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
                            Log.Instance.log($"Failed to open {Name} " + $"VID:{Device.VendorID:X4} " + $"PID:{Device.ProductID:X4} " + $"Path:{Device.DevicePath}: {ex.Message}", LogSeverity.Error);
                        }
                        return false;
                    }
                }

                if (!Receiver.IsRunning)
                {
                    Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, "SwitchPanel-HID-Reader");
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
                if (device.Type == DeviceType.LcdDisplay.ToString())
                {
                    Lights.Add(new JoystickOutputDisplay
                    {
                        Name = device.Id,
                        Label = device.Label,
                        Type = DeviceType.LcdDisplay,
                        Cols = device.Cols,
                        Lines = device.Lines,
                        Byte = device.Byte
                    });
                }
                else
                {
                    Lights.Add(new JoystickOutputDevice
                    {
                        Name = device.Id,
                        Label = device.Label,
                        Byte = device.Byte,
                        Bit = device.Bit
                    });
                }
            });
        }

        /// <summary>
        /// Converts a raw Multi Panel HID report into MobiFlight button state, and
        /// updates the active selector position from the SEL ALT..SEL CRS buttons.
        /// </summary>
        private void OnReportReceived(HidReport inputReport)
        {
            if (inputReport.ReportId != 0)
            {
                Log.Instance.log($"{Name}: ignoring HID report with unexpected report ID " + $"{inputReport.ReportId:X2}.", LogSeverity.Debug);
                return;
            }

            var newState = MultiPanelReport.Parse(inputReport.Payload).ToJoystickState();
            UpdateButtons(newState);
            UpdateSelectorFromState(newState);
            State = newState;
        }

        /// <summary>
        /// The selector is a 5-position rotary switch reported as 5 mutually
        /// exclusive buttons (SEL ALT..SEL CRS). When it moves to a new position,
        /// the display must be recomputed even though no LCD *value* changed -
        /// e.g. flipping from ALT to HDG has to redraw immediately using the
        /// already-cached HdgValue, not wait for MobiFlight to resend it.
        /// </summary>
        private void UpdateSelectorFromState(JoystickState newState)
        {
            MultiPanelSelector? selector = null;
            if (newState.Buttons[SelectorButtonAlt]) selector = MultiPanelSelector.Alt;
            else if (newState.Buttons[SelectorButtonVs]) selector = MultiPanelSelector.Vs;
            else if (newState.Buttons[SelectorButtonIas]) selector = MultiPanelSelector.Ias;
            else if (newState.Buttons[SelectorButtonHdg]) selector = MultiPanelSelector.Hdg;
            else if (newState.Buttons[SelectorButtonCrs]) selector = MultiPanelSelector.Crs;

            // No bit set is treated as "no change" rather than "no selector" -
            // the device always has exactly one position active, so an all-zero
            // read here is a transitional/glitch read, not a real state.
            if (selector == null) return;

            lock (OutputLock)
            {
                if (CurrentSelector == selector.Value) return;

                CurrentSelector = selector.Value;
                RequiresOutputUpdate = true;
            }
        }

        /// <summary>
        /// Receives a numeric LCD value from a MobiFlight output config (address
        /// matches the JSON output Id, e.g. "LCD ALT") and caches it for the next
        /// display refresh.
        /// </summary>
        public override void SetLcdDisplay(string address, string value)
        {
            if (!int.TryParse(value, out int intValue))
            {
                Log.Instance.log($"{Name} received non-numeric LCD value '{value}' for '{address}', ignoring.", LogSeverity.Warn);
                return;
            }

            lock (OutputLock)
            {
                switch (address)
                {
                    case "LCD ALT": AltValue = intValue; break;
                    case "LCD VS": VsValue = intValue; break;
                    case "LCD IAS": IasValue = intValue; break;
                    case "LCD HDG": HdgValue = intValue; break;
                    case "LCD CRS": CrsValue = intValue; break;
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

                foreach (var light in Lights)
                {
                    // LcdDisplay entries share this collection with button LEDs
                    // (see EnumerateOutputDevices) but don't represent an LED
                    // channel - skip them here or SetChannel corrupts byte 11
                    // using whatever Bit/State happens to sit on a display item.
                    if (light.Type == DeviceType.LcdDisplay) continue;

                    LedState.SetChannel(light.Bit, light.State != 0);
                }

                DisplayState.SetDisplay(CurrentSelector, AltValue, VsValue, IasValue, HdgValue, CrsValue);

                var featureReport = new byte[FeatureReportLength];
                featureReport[0] = 0; // report ID
                DisplayState.WriteInto(featureReport);
                LedState.WriteInto(featureReport);

                try
                {
                    Stream.SetFeature(featureReport, 0, featureReport.Length);
                    RequiresOutputUpdate = false;
                    Log.Instance.log($"{Name} output: {BitConverter.ToString(featureReport)} LED=0x{LedState.Value:X2}", LogSeverity.Info);
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
            return new List<DeviceType>
            {
                DeviceType.Output,
                DeviceType.LcdDisplay
            };
        }
    }
}