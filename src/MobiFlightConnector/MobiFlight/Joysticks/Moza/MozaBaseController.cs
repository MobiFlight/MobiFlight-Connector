using HidSharp;
using System;
namespace MobiFlight.Joysticks.Moza
{
    /// <summary>
    /// The MOZA FCD Display's base unit: buttons and status LEDs on a single USB HID
    /// interface. Discovered and driven the same way as <see cref="Logitech.SwitchPanel"/> -
    /// no DirectInput anywhere.
    /// </summary>
    internal class MozaBaseController : Joystick
    {
        private readonly HidReportReceiver Receiver = new HidReportReceiver();
        private readonly object ConnectionLock = new object();
        private string CachedSerialNumber;
        private bool Disconnected;
        private bool OpenFailureLogged;
        public override string Name => Definition?.InstanceName ?? "MOZA FCD Display";
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
        public MozaBaseController(JoystickDefinition definition) : base(null, definition)
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
        protected bool ConnectHid()
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
                    Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, "MozaBaseController-HID-Reader");
                }
            }
            Log.Instance.log($"{Name} detected: VID:{Device.VendorID:X4} PID:{Device.ProductID:X4} Serial:{Serial} Path:{Device.DevicePath} MaxInputReportLength:{Device.GetMaxInputReportLength()}", LogSeverity.Debug);
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
        /// Converts an incoming base-unit HID input report into MobiFlight button state.
        /// </summary>
        private void OnReportReceived(HidReport inputReport)
        {
            var newState = MozaButtonReport.Parse(inputReport.Payload).ToJoystickState();
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
        // Status LED wire mechanism is undocumented (plan risk R-LEDFORMAT). Logged once
        // per pending update rather than silently dropped.
        public override void UpdateOutputDeviceStates()
        {
            if (!RequiresOutputUpdate) return;
            RequiresOutputUpdate = false;
            Log.Instance.log($"{Name} - MOZA LED control not yet implemented.", LogSeverity.Debug);
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