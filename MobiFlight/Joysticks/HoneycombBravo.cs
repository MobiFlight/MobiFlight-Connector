using HidSharp;
using System.Collections.Generic;

namespace MobiFlight.Joysticks
{
    internal class HoneycombBravo : LabeledJoystick
    {
        HidStream Stream { get; set; }
        HidDevice Device { get; set; }

        public HoneycombBravo(SharpDX.DirectInput.Joystick joystick, HidDefinition definition) : base(joystick, definition) {
        }

        public void Connect()
        {
            if (Device == null)
            {
                Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: Definition.VendorId, productID: Definition.ProductId);
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
