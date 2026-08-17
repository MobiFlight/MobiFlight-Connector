using HidSharp;
using System;
using System.Linq;

namespace MobiFlight.Joysticks.FreeJoy
{
    /// <summary>
    /// Adds external LED control for FreeJoy boards (https://github.com/FreeJoy-Team/FreeJoyWiki).
    /// </summary>
    /// <remarks>
    /// Buttons and axes keep using the regular DirectInput path in the base class: FreeJoy is
    /// user-flashed firmware, so there is no fixed input report layout to parse.
    /// <para>
    /// Only the LEDs need a device of their own. They are addressed on a second, vendor-specific
    /// HID interface with an output report, so neither the <c>GetHidDeviceOrNull</c> lookup nor
    /// the <c>SetFeature</c> write in the base class can reach them.
    /// </para>
    /// <para>
    /// Only LEDs flagged "External" in the FreeJoy Configurator follow this report, the rest keep
    /// following the board's own button logic.
    /// </para>
    /// </remarks>
    internal class FreeJoyController : Joystick
    {
        public const int FREEJOY_VENDOR_ID = 0x0483;
        public const int FREEJOY_PRODUCT_ID = 0x5757;

        /// <summary>
        /// Usage page of the vendor-specific interface carrying the LED reports.
        /// </summary>
        private const uint LED_USAGE_PAGE = 0xFF00;

        /// <summary>
        /// Set once the "interface not found" warning has been logged, so a board that never
        /// exposes the LED interface does not flood the log on every poll cycle while an output
        /// update is pending.
        /// </summary>
        private bool LedInterfaceMissingLogged;

        public FreeJoyController(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
        }

        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);

            // Opening the LED interface here keeps the failure visible in the log at startup
            // instead of on the first output event.
            ConnectLedInterface();
        }

        /// <summary>
        /// Opens the LED interface, unless it is already open. Called again before every write so
        /// a board that shows up late, or comes back after a reconnect, is picked up.
        /// </summary>
        /// <returns>True if the interface is ready to be written to.</returns>
        private bool ConnectLedInterface()
        {
            if (Definition?.Outputs == null || Definition.Outputs.Count == 0) return false;

            if (Device == null)
            {
                Device = FindLedInterface(Definition.VendorId, Definition.ProductId);
                if (Device == null)
                {
                    if (!LedInterfaceMissingLogged)
                    {
                        Log.Instance.log($"No FreeJoy LED interface with usage page 0x{LED_USAGE_PAGE:X4} found for VID:{Definition.VendorId:X4} PID:{Definition.ProductId:X4}, external LED control unavailable.", LogSeverity.Warn);
                        LedInterfaceMissingLogged = true;
                    }
                    return false;
                }
                LedInterfaceMissingLogged = false;
            }

            if (Stream == null)
            {
                try
                {
                    Stream = Device.Open();
                }
                catch (Exception ex)
                {
                    // Connect runs on the joystick scan's thread - an open failure
                    // (e.g. the device is held exclusively by another tool) must not
                    // abort the scan.
                    Log.Instance.log($"Failed to open the FreeJoy LED interface: {ex.Message}", LogSeverity.Error);
                    Device = null;
                    return false;
                }

                Log.Instance.log($"Connected to the FreeJoy LED interface of {Name}.", LogSeverity.Debug);
            }

            return true;
        }

        /// <summary>
        /// The board exposes its joystick interface and its LED interface behind the same
        /// vendor and product id, so they can only be told apart by their usage page.
        /// </summary>
        private static HidDevice FindLedInterface(int vendorId, int productId)
        {
            return DeviceList.Local
                             .GetHidDevices(vendorID: vendorId, productID: productId)
                             .FirstOrDefault(HasLedUsagePage);
        }

        private static bool HasLedUsagePage(HidDevice device)
        {
            try
            {
                return device.GetReportDescriptor()
                             .DeviceItems
                             .SelectMany(item => item.Usages.GetAllValues())
                             .Any(usage => (usage >> 16) == LED_USAGE_PAGE);
            }
            catch (Exception)
            {
                // A device that will not hand out its report descriptor is not the one
                // we are looking for, and must not abort the search for the others.
                return false;
            }
        }

        public override void UpdateOutputDeviceStates()
        {
            if (!RequiresOutputUpdate) return;

            SendData(FreeJoyReport.FromOutputDeviceState(Lights));
        }

        protected override void SendData(byte[] data)
        {
            if (data == null) return;
            if (!ConnectLedInterface()) return;

            try
            {
                // HidSharp expects the report ID at index 0. The payload is padded out to the
                // length the board declares because Windows rejects an output report of any
                // other size.
                var report = new byte[Device.GetMaxOutputReportLength()];
                report[0] = FreeJoyReport.ReportId;
                Array.Copy(data, 0, report, 1, Math.Min(data.Length, report.Length - 1));

                Stream.Write(report, 0, report.Length);
                RequiresOutputUpdate = false;
            }
            catch (Exception ex)
            {
                // Catch-all to prevent unhandled exceptions from the write from crashing the
                // application, in line with how the other HID controllers handle device removal.
                Log.Instance.log($"Exception during write to the FreeJoy LED interface ({ex.GetType().Name}): {ex.Message}", LogSeverity.Error);
                CloseLedInterface();
                OnDeviceRemoved();
            }
        }

        public override void Shutdown()
        {
            CloseLedInterface();

            base.Shutdown();
        }

        private void CloseLedInterface()
        {
            Stream?.Close();
            Stream = null;
            Device = null;
            LedInterfaceMissingLogged = false;
        }
    }
}
