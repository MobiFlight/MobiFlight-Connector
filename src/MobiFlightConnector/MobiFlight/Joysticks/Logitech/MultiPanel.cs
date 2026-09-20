using HidSharp;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Native raw HID controller for the Logitech Multi Panel.
    /// </summary>
    internal sealed class MultiPanel : Joystick
    {
        private readonly HidReportReceiver Receiver = new();
        private readonly Lock ConnectionLock = new();
        private readonly Lock OutputLock = new();
        private string CachedSerialNumber;
        private bool Disconnected;
        private bool OpenFailureLogged;

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

        public MultiPanelReport UsbReport { get; private set; }

        public MultiPanel(JoystickDefinition definition) : base(null, definition)
        {
            UsbReport = new MultiPanelReport();
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
                    Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, "MultiPanel-HID-Reader");
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
        /// Converts an absolute multi-panel input report into MobiFlight button state.
        /// </summary>
        private void OnReportReceived(HidReport inputReport)
        {
            if (inputReport.ReportId != 0)
            {
                Log.Instance.log($"Ignoring {Name} input report with unexpected report ID {inputReport.ReportId:X2}.", LogSeverity.Debug);
                return;
            }

            var newState = MultiPanelReport.Parse(inputReport.Payload).ToJoystickState();
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

                var data = MultiPanelReport.FromOutputDeviceState(Lights);
                try
                {
                    Stream.SetFeature(data, 0, data.Length);
                    RequiresOutputUpdate = false;
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"{Name} Output write failed, disconnecting {Serial}: {ex.Message}", LogSeverity.Error);
                    Disconnect();
                }
            }
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