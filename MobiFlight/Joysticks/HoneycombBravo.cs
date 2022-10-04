using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidSharp;

namespace MobiFlight.Joysticks
{
    internal class HoneycombBravo : Joystick
    {
        int VendorId = 0x294B;
        int ProductId = 0x1901;
        HidStream Stream { get; set; }
        HidDevice Device { get; set; }
        public HoneycombBravo(SharpDX.DirectInput.Joystick joystick) : base(joystick)
        {

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
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - HDG",                Name = "AP.hdg",                Byte = 1, Bit = 0 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - NAV",                Name = "AP.nav",                Byte = 1, Bit = 1 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - APR",                Name = "AP.apr",                Byte = 1, Bit = 2 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - REV",                Name = "AP.rev",                Byte = 1, Bit = 3 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - ALT",                Name = "AP.alt",                Byte = 1, Bit = 4 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - VS",                 Name = "AP.vs",                 Byte = 1, Bit = 5 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - IAS",                Name = "AP.ias",                Byte = 1, Bit = 6 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - On/Off",             Name = "AP.autopilot",          Byte = 1, Bit = 7 });
            // -- Byte 2
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Left Green",            Name = "Gear.LeftGreen",        Byte = 2, Bit = 0 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Left Red",              Name = "Gear.LeftRed",          Byte = 2, Bit = 1 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Center Green",          Name = "Gear.CenterGreen",      Byte = 2, Bit = 2 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Center Red",            Name = "Gear.CenterRed",        Byte = 2, Bit = 3 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Right Green",           Name = "Gear.RightGreen",       Byte = 2, Bit = 4 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Right Red",             Name = "Gear.RightRed",         Byte = 2, Bit = 5 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Master Warning",      Name = "Light.MasterWarn",      Byte = 2, Bit = 6 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Engine Fire",         Name = "Light.EngineFire",      Byte = 2, Bit = 7 });
            // -- Byte 3
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Low Oil Pressure",    Name = "Light.LowOil",          Byte = 3, Bit = 0 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Low Fuel Pressure",   Name = "Light.LowFuel",         Byte = 3, Bit = 1 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Anti Ice",            Name = "Light.Antiice",         Byte = 3, Bit = 2 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Starter Engaged",     Name = "Light.Starter",         Byte = 3, Bit = 3 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - APU",                 Name = "Light.APU",             Byte = 3, Bit = 4 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Master Caution",      Name = "Light.MasterCaution",   Byte = 3, Bit = 5 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Vacuum",              Name = "Light.Vacuum",          Byte = 3, Bit = 6 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Low Hyd Pressure",    Name = "Light.LowHydPressURE",  Byte = 3, Bit = 7 });
            // -- Byte 4
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Aux Fuel Pump",       Name = "Lights.AuxFuelPump",    Byte = 4, Bit = 0 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Parking Brake",       Name = "Lights.ParkingBrake",   Byte = 4, Bit = 1 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Low Volts",           Name = "Lights.LowVolts",       Byte = 4, Bit = 2 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Door",                Name = "Lights.door",           Byte = 4, Bit = 3 });


        }
    }
}
