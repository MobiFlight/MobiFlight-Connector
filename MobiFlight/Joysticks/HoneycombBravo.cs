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

        HidDefinition Definition { get; set; }

        public HoneycombBravo(SharpDX.DirectInput.Joystick joystick, HidDefinition definition) : base(joystick) {
            Definition = definition;
            Labels = new Dictionary<string, string>();

            definition.Inputs.ForEach(input => Labels.Add(input.Name, input.Label));           
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

        protected override void EnumerateOutputDevices()
        {
            base.EnumerateOutputDevices();
            Definition.Outputs.ForEach(output => Lights.Add(new JoystickOutputDevice() { Label = output.Label, Name = output.Name, Byte = output.Byte, Bit = output.Bit }));
        }
    }
}
