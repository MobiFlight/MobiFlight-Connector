using HidSharp;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace MobiFlight.Joysticks.WingFlex
{
    public class GmpConfig
    {
        public static readonly byte ReportId = 4;
        public bool LightSensorEnabled { get; set; } = false;
        public ushort AutoStandByTimeout { get; set; } = 5;

        public byte[] ToData
        {
            get
            {
                return new byte[] {
                    ReportId, // ReportID=4  
                    (byte)((AutoStandByTimeout >> 8) & 0xFF), // Higher 8 bits of AutoStandByTimeout 
                    (byte)(AutoStandByTimeout & 0xFF), // Lower 8 bits of AutoStandByTimeout
                    (byte)((LightSensorEnabled ? 1 : 0)) // Bit 0: LightSensorEnabled 
                };
            }
        }
    }

    internal class Gmp : Joystick
    {
        /// <summary>
        /// The report implementation for usb report.
        /// </summary>
        private readonly GmpReport UsbReport = new GmpReport();
        private readonly HidReportReceiver Receiver = new HidReportReceiver();

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public Gmp(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
        }

        /// <summary>
        /// This creates a connection to the HID device using the HidSharp library.
        /// </summary>
        /// <returns></returns>
        protected bool Connect()
        {
            // Prevent reentry and parallel execution by multiple threads
            lock (this)
            {
                if (Device == null)
                {
                    Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: Definition.VendorId, productID: Definition.ProductId);
                    if (Device == null) return false;
                }

                if (Stream == null)
                {
                    OpenConfiguration config = new OpenConfiguration();
                    config.SetOption(OpenOption.Exclusive, true);
                    Stream = Device.Open(config);
                }

                if (!Receiver.IsRunning)
                {
                    Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, "Gmp-HID-Reader");
                }
            }

            if (Stream != null)
            {
                InitConfig();
            }

            return true;
        }

        /// <summary>
        /// Initializes the device configuration by sending a DapConfig with specific settings to the device.
        /// We do this once on connecting to the device.
        /// </summary>
        private void InitConfig()
        {
            RequiresOutputUpdate = true;
            SendData(new GmpConfig() { LightSensorEnabled = true, AutoStandByTimeout = 3600 }.ToData);
        }

        /// <summary>
        /// Restores the default configuration of the device by sending a default DapConfig to the device.
        /// </summary>
        private void RestoreConfig()
        {
            RequiresOutputUpdate = true;
            SendData(new GmpConfig() { }.ToData);
        }

        private void OnReportReceived(HidReport inputReport)
        {
            ProcessInputReport(inputReport);
        }

        private void OnReadError(Exception exception)
        {
            Log.Instance.log($"{Name} read failed, closing connection: {exception.Message}", LogSeverity.Error);

            // Drop the dead connection so the next Update() can reconnect.
            lock (this)
            {
                Stream?.Close();
                Stream = null;
                Device = null;
            }
        }

        /// <summary>
        /// Update is called by the base class
        /// It is currently needed to ensure that the hid device is correctly initialized.
        /// </summary>
        public override void Update()
        {
            // The input is not read via DirectInput
            // so we have to connect it here.
            if (Stream == null || !Receiver.IsRunning)
            {
                Connect();
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
            // The report parser works on the raw report including the report ID byte.
            var newState = UsbReport.Parse(inputReport.Buffer).ToJoystickState();

            UpdateButtons(newState);
            UpdateAxis(newState);
            // Finally store the new state as last state
            State = newState;
        }

        static public bool IsBrakePressureLcd(byte byteIndex)
        {
            return new byte[] { 22, 24, 26 }.Contains(byteIndex);
        }

        protected byte[] ProcessBrakePressureDisplay(string clampedText, byte[] data, JoystickOutputDisplay light)
        {
            if (!ushort.TryParse(clampedText, out ushort parsedValue))
            {
                parsedValue = 0;
            }

            data[light.Byte + 1] = (byte)(parsedValue >> 8 & 0xFF);
            data[light.Byte] = (byte)(parsedValue & 0xFF);
            return data;
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
            // Feature ReportID=2 for the LEDs
            var data = new byte[] { 2,
                0, 0, 255, 255, 255, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0
            };

            foreach (var light in Lights)
            {
                if (light is JoystickOutputDevice && light is not JoystickOutputDisplay)
                {
                    try
                    {
                        data[light.Byte] |= (byte)(light.State << light.Bit);
                    }
                    catch (Exception e)
                    {
                        Log.Instance.log(e.Message, LogSeverity.Debug);
                    }
                    continue;
                }

                if (light is JoystickOutputDisplay)
                {
                    var display = light as JoystickOutputDisplay;
                    if (display.Text != null)
                    {
                        var clampedText = display.Text[0..Math.Min(display.Text.Length, display.Cols)];

                        if (IsBrakePressureLcd(light.Byte))
                        {
                            data = ProcessBrakePressureDisplay(clampedText, data, display);
                            continue;
                        }

                        data = ProcessClockDisplay(clampedText, data, display);
                    }
                    continue;
                }
            }

            try
            {
                this.SendData(data);
            }
            catch (System.IO.IOException)
            {
                // this happens when the device is removed.
                OnDeviceRemoved();
            }
        }

        public static byte[] ProcessClockDisplay(string clampedText, byte[] data, JoystickOutputDisplay display)
        {
            var textBytes = clampedText.StringToGmpDisplayBytes();
            Array.Copy(textBytes, 0, data, display.Byte, textBytes.Length);
            return data;
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
        /// Cleans up any specific resources, e.g. thread and device connection.
        /// </summary>
        public override void Shutdown()
        {
            RestoreConfig();
            Receiver.Stop();

            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }

            base.Shutdown();
        }

        protected override void SendData(byte[] data)
        {
            // Don't try and send data if no outputs are defined.
            if (Definition?.Outputs == null || Definition?.Outputs.Count == 0)
            {
                return;
            }

            if (!RequiresOutputUpdate) return;
            if (Stream == null)
            {
                Connect();
            }

            try
            {
                Stream.SetFeature(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Log.Instance.log($"Error sending data to device: {e.Message}", LogSeverity.Error);
            }

            RequiresOutputUpdate = false;
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

            Definition?.Outputs?.ForEach(d =>
            {
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
    }

    internal static class GmpExtensions
    {
        /// <summary>
        /// Converts display text to byte array representation as defined in the GMP protocol.
        /// 0-9=digits 0-9, 0xA=dash, 0xF=blank
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] StringToGmpDisplayBytes(this string text)
        {
            var result = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c >= '0' && c <= '9')
                {
                    result[i] = (byte)(c - '0');
                }
                else if (c == '-')
                {
                    result[i] = 0xA;
                }
                else
                {
                    result[i] = 0xF; // blank
                }
            }
            return result;
        }
    }
}