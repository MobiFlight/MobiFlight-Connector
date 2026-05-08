using System;
using System.Linq;
using System.Text;
using System.IO;
using HidSharp;

namespace MobiFlight.Joysticks.IIDB
{
    internal class IIDBDevices : Joystick
    {
        public const int IIDB_VENDOR_ID = 0x99DB;
        private static readonly object _globalUsbLock = new object();

        private byte _currentBrightness = 0;
        private byte _lastSentBrightness = 255;

        public IIDBDevices(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition)
            : base(joystick, definition)
        {
        }

        public override void SetOutputDeviceState(string name, byte state)
        {
            if (name == "Backlight")
            {
                _currentBrightness = state;
            }

            try
            {
                base.SetOutputDeviceState(name, state);
            }
            catch { }
        }

        public override void UpdateOutputDeviceStates()
        {
            if (_currentBrightness != _lastSentBrightness)
            {
                SendData(_currentBrightness);
                _lastSentBrightness = _currentBrightness;
            }
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

                        byte[] reportData = new byte[33];
                        reportData[0] = 3;

                        int val = Math.Max(0, Math.Min((int)brightness, 100));
                        string command = $"SetB:{val:D3}";
                        byte[] commandBytes = Encoding.ASCII.GetBytes(command);
                        Array.Copy(commandBytes, 0, reportData, 1, Math.Min(commandBytes.Length, 32));

                        stream.Write(reportData);
                    }
                }
                catch (IOException)
                {
                    _lastSentBrightness = 255;
                }
                catch (Exception) { }
            }
        }

        protected override void SendData(byte[] data) { }
    }
}