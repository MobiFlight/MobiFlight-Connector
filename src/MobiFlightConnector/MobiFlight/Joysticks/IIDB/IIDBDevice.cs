using System;
using System.Linq;
using System.Text;
using System.IO;
using HidSharp;

namespace MobiFlight.Joysticks.IIDB
{
    internal class IIDBDevice : Joystick
    {
        public const int IIDB_VENDOR_ID = 0x99DB;
        private static readonly object _globalUsbLock = new object();

        // HID Report Constants
        // The device expects a 33-byte report (1 byte for Report ID + 32 bytes for payload)
        private const int REPORT_LENGTH = 33;
        // Report ID 3 is used for sending commands like brightness control to the device
        private const int REPORT_ID_COMMAND = 3;

        private byte _currentBrightness = 0;
        private byte _lastSentBrightness = 255;

        public IIDBDevice(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition)
            : base(joystick, definition)
        {
        }

        public override void SetOutputDeviceState(string name, byte state)
        {
            if (name == "Backlight")
            {
                _currentBrightness = state;
            }

            base.SetOutputDeviceState(name, state);
        }

        public override void UpdateOutputDeviceStates()
        {
            if (_currentBrightness != _lastSentBrightness)
            {
                SendData(_currentBrightness);
                _lastSentBrightness = _currentBrightness;
            }
        }

        public override void Stop()
        {
            _currentBrightness = 0;
            SendData(0);
            _lastSentBrightness = 0;
            base.Stop();
        }

        public override void Shutdown()
        {
            if (_lastSentBrightness != 0)
            {
                SendData(0);
            }
            base.Shutdown();
        }

        private void SendData(byte brightness)
        {
            lock (_globalUsbLock)
            {
                try
                {
                    var device = DeviceList.Local?.GetHidDevices(Definition.VendorId, Definition.ProductId).FirstOrDefault();
                    if (device == null) return;

                    var options = new OpenConfiguration();
                    options.SetOption(OpenOption.Exclusive, false);

                    using (var stream = device.Open(options))
                    {
                        stream.WriteTimeout = 50;

                        byte[] reportData = new byte[REPORT_LENGTH];
                        reportData[0] = REPORT_ID_COMMAND;

                        int val = Math.Max(0, Math.Min((int)brightness, 100));
                        string command = $"SetB:{val:D3}";
                        byte[] commandBytes = Encoding.ASCII.GetBytes(command);

                        // Copy the command string to the report, leaving space for the Report ID at index 0
                        Array.Copy(commandBytes, 0, reportData, 1, Math.Min(commandBytes.Length, REPORT_LENGTH - 1));

                        stream.Write(reportData);
                    }
                }
                catch (IOException ex)
                {
                    Log.Instance.log($"IIDB: I/O Error during SendData (Device might be disconnected): {ex.Message}", LogSeverity.Debug);
                    _lastSentBrightness = 255; 
                    base.OnDeviceRemoved();  
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"IIDB: Unexpected USB error: {ex.Message}", LogSeverity.Error);
                    _lastSentBrightness = 255; 
                }
            }
        }

        protected override void SendData(byte[] data) { }
    }
}