using HidSharp;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Native raw HID controller for the Logitech/Saitek PZ55 Switch Panel.
    /// </summary>
    internal sealed class Pz55SwitchPanel : Joystick
    {
        public const int VendorId = 0x06A3;
        public const int ProductId = 0x0D67;

        private static readonly (int VendorId, int ProductId)[] SupportedDevices =
        {
            (VendorId, ProductId)
        };

        private readonly HidReportReceiver Receiver = new HidReportReceiver();
        private readonly object ConnectionLock = new object();
        private readonly object OutputLock = new object();
        private readonly string StableSerial;
        private bool Disconnected;
        private bool OpenFailureLogged;

        public override string Name => Definition?.InstanceName ?? "Logitech/Saitek Switch Panel PZ55";
        public override string Serial => StableSerial;

        public Pz55SwitchPanel(HidDevice device, JoystickDefinition definition) : base(null, definition)
        {
            Device = device ?? throw new ArgumentNullException(nameof(device));
            StableSerial = BuildStableSerial(device);
        }

        /// <summary>
        /// Returns whether a USB identity is handled by this controller implementation.
        /// </summary>
        public static bool IsSupported(int vendorId, int productId)
        {
            return SupportedDevices.Any(device => device.VendorId == vendorId && device.ProductId == productId);
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
        /// Opens the exact HidSharp device selected during enumeration and starts its
        /// dedicated blocking report reader.
        /// </summary>
        private bool ConnectHid()
        {
            lock (ConnectionLock)
            {
                if (Disconnected) return false;

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
                            Log.Instance.log($"Failed to open PZ55 VID:{Device.VendorID:X4} PID:{Device.ProductID:X4} Path:{Device.DevicePath}: {ex.Message}", LogSeverity.Error);
                        }
                        return false;
                    }
                }

                if (!Receiver.IsRunning)
                {
                    Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, "PZ55-HID-Reader");
                }
            }

            Log.Instance.log($"PZ55 detected: VID:{Device.VendorID:X4} PID:{Device.ProductID:X4} Product:{SafeProductName()} Serial:{Serial} Path:{Device.DevicePath} MaxInputReportLength:{Device.GetMaxInputReportLength()} MaxFeatureReportLength:{Device.GetMaxFeatureReportLength()}", LogSeverity.Debug);
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
        /// Converts an absolute PZ55 input report into MobiFlight button state.
        /// </summary>
        private void OnReportReceived(HidReport inputReport)
        {
            if (inputReport.ReportId != 0)
            {
                Log.Instance.log($"Ignoring PZ55 input report with unexpected report ID {inputReport.ReportId:X2}.", LogSeverity.Debug);
                return;
            }

            Log.Instance.log($"PZ55 input report: {BitConverter.ToString(inputReport.Buffer)}", LogSeverity.Debug);
            var newState = Pz55Report.Parse(inputReport.Payload).ToJoystickState();
            UpdateButtons(newState);
            State = newState;
        }

        private void OnReadError(Exception exception)
        {
            Log.Instance.log($"PZ55 read failed, disconnecting {Serial}: {exception.Message}", LogSeverity.Error);
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

                var ledState = new Pz55LedState();
                foreach (var light in Lights)
                {
                    ledState.SetChannel(light.Bit, light.State != 0);
                }

                var featureReport = ledState.ToFeatureReport();
                try
                {
                    Stream.SetFeature(featureReport, 0, featureReport.Length);
                    RequiresOutputUpdate = false;
                    Log.Instance.log($"PZ55 LED output: 0x{ledState.Value:X2}", LogSeverity.Debug);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"PZ55 LED write failed, disconnecting {Serial}: {ex.Message}", LogSeverity.Error);
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

        private string SafeProductName()
        {
            try
            {
                return Device.GetProductName();
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Uses the USB serial when available, otherwise hashes the HID path so multiple
        /// identical panels can retain distinct controller identities.
        /// </summary>
        private static string BuildStableSerial(HidDevice device)
        {
            try
            {
                var hardwareSerial = device.GetSerialNumber();
                if (!string.IsNullOrWhiteSpace(hardwareSerial))
                {
                    return $"{SerialPrefix}PZ55-{hardwareSerial.Trim()}";
                }
            }
            catch
            {
                // Most PZ55 panels do not expose a USB serial number.
            }

            var pathBytes = Encoding.UTF8.GetBytes(device.DevicePath ?? $"{device.VendorID:X4}:{device.ProductID:X4}");
            var pathHash = SHA256.HashData(pathBytes);
            return $"{SerialPrefix}PZ55-{Convert.ToHexString(pathHash, 0, 8)}";
        }
    }
}
