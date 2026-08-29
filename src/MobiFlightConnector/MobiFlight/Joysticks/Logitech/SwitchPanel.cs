using HidSharp;
using System;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Native raw HID controller for the Logitech Switch Panel.
    /// </summary>
    internal sealed class SwitchPanel : Joystick
    {
        private readonly HidReportReceiver Receiver = new HidReportReceiver();
        private readonly object ConnectionLock = new object();
        private readonly object OutputLock = new object();
        private string CachedSerialNumber;
        private bool Disconnected;
        private bool OpenFailureLogged;

        public override string Name => Definition?.InstanceName ?? "Logitech Switch Panel";

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
                    : $"{Name.ToUpper().Replace(" ", "-")}-1234-ABCD-12345678";
            }
        }

        public SwitchPanel(JoystickDefinition definition) : base(null, definition)
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

        /// <summary>
        /// Finds the HidSharp device matching the definition's VID/PID and starts its
        /// dedicated blocking report reader.
        /// </summary>
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

        /// <summary>
        /// Converts an absolute switch-panel input report into MobiFlight button state.
        /// </summary>
        private void OnReportReceived(HidReport inputReport)
        {
            if (inputReport.ReportId != 0)
            {
                Log.Instance.log($"Ignoring {Name} input report with unexpected report ID {inputReport.ReportId:X2}.", LogSeverity.Debug);
                return;
            }

            var newState = SwitchPanelReport.Parse(inputReport.Payload).ToJoystickState();
            UpdateButtons(newState);
            State = newState;
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

                var ledState = new SwitchPanelLedState();
                foreach (var light in Lights)
                {
                    ledState.SetChannel(light.Bit, light.State != 0);
                }

                var featureReport = ledState.ToFeatureReport();
                try
                {
                    Stream.SetFeature(featureReport, 0, featureReport.Length);
                    RequiresOutputUpdate = false;
                    Log.Instance.log($"{Name} LED output: 0x{ledState.Value:X2}", LogSeverity.Debug);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"{Name} LED write failed, disconnecting {Serial}: {ex.Message}", LogSeverity.Error);
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
    }
}
