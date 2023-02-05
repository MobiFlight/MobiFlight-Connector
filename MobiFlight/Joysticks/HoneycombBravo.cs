using HidSharp;
using System.Collections.Generic;

namespace MobiFlight.Joysticks
{
    internal class HoneycombBravo : LabeledJoystick
    {
        int VendorId = 0x294B;
        int ProductId = 0x1901;
        HidStream Stream { get; set; }
        HidDevice Device { get; set; }

        readonly HidDefinition Definition;

        public HoneycombBravo(SharpDX.DirectInput.Joystick joystick, HidDefinition definition) : base(joystick, definition) {
        }

        public void Connect()
        {
            if (Device == null)
            {
                Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
                if (Device == null) return;
            }

            Stream = Device.Open();
        }

        protected override void SendData(byte[] data)
        {
            if (!RequiresOutputUpdate) return;
            if (Stream == null)
            {
                Connect();
            };
            Stream.SetFeature(data);
            base.SendData(data);
        }
    }
}
