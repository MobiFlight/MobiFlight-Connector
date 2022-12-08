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

        public HoneycombBravo(SharpDX.DirectInput.Joystick joystick) : base(joystick) {
            Labels = new Dictionary<string, string>()
            {
                // Mode
                { "Button 17", "Mode - IAS" },
                { "Button 18", "Mode - CRS" },
                { "Button 19", "Mode - HDG" },
                { "Button 20", "Mode - VS" },
                { "Button 21", "Mode - ALT" },
                // AP Buttons
                { "Button 1", "AP - HDG" },
                { "Button 2", "AP - NAV" },
                { "Button 3", "AP - APR" },
                { "Button 4", "AP - REV" },
                { "Button 5", "AP - ALT" },
                { "Button 6", "AP - VS" },
                { "Button 7", "AP - IAS" },
                { "Button 8", "AP - Autopilot" },
                // Generic Encoder
                { "Button 13", "Encoder - Right" },
                { "Button 14", "Encoder - Left" },
                // Gear
                { "Button 31", "Gear - Up" },
                { "Button 32", "Gear - Down" },
                // Flaps
                { "Button 16", "Flaps - Up" },
                { "Button 15", "Flaps - Down" },
                // Trim
                { "Button 22", "Trim - Nose Down" },
                { "Button 23", "Trim - Nose Up" },
                // Levers
                { "Button 24", "Lever 1 - Detent" },
                { "Button 25", "Lever 2 - Detent" },
                { "Button 26", "Lever 3 - Detent" },
                { "Button 27", "Lever 4 - Detent" },
                { "Button 28", "Lever 5 - Detent" },
                { "Button 33", "Lever 6 - Detent" },
                //
                { "Button 29", "Lever 1 - Button" },
                { "Button 9", "Lever 2 - Button" },
                { "Button 10", "Lever 3 - Button" },
                { "Button 11", "Lever 4 - Button" },
                { "Button 12", "Lever 5 - Button" },
                //
                { "Button 30", "Lever 3 - Reverser" },
                { "Button 48", "Lever 4 - Reverser" },

                //
                { "Button 34", "Switch 1 - Up" },
                { "Button 35", "Switch 1 - Down" },
                { "Button 36", "Switch 2 - Up" },
                { "Button 37", "Switch 2 - Down" },
                { "Button 38", "Switch 3 - Up" },
                { "Button 39", "Switch 3 - Down" },
                { "Button 40", "Switch 4 - Up" },
                { "Button 41", "Switch 4 - Down" },
                { "Button 42", "Switch 5 - Up" },
                { "Button 43", "Switch 5 - Down" },
                { "Button 44", "Switch 6 - Up" },
                { "Button 45", "Switch 6 - Down" },
                { "Button 46", "Switch 7 - Up" },
                { "Button 47", "Switch 7 - Down" },
                // Axis
                { "Y Axis", "Axis - Lever 1" },
                { "X Axis", "Axis - Lever 2" },
                { "Z Rotation", "Axis - Lever 3" },
                { "Y Rotation", "Axis - Lever 4" },
                { "X Rotation", "Axis - Lever 5" },
                { "Z Axis", "Axis - Lever 6" },
            };
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
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - HDG", Name = "AP.hdg", Byte = 1, Bit = 0 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - NAV", Name = "AP.nav", Byte = 1, Bit = 1 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - APR", Name = "AP.apr", Byte = 1, Bit = 2 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - REV", Name = "AP.rev", Byte = 1, Bit = 3 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - ALT", Name = "AP.alt", Byte = 1, Bit = 4 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - VS", Name = "AP.vs", Byte = 1, Bit = 5 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - IAS", Name = "AP.ias", Byte = 1, Bit = 6 });
            Lights.Add(new JoystickOutputDevice() { Label = "AP Mode - On/Off", Name = "AP.autopilot", Byte = 1, Bit = 7 });
            // -- Byte 2
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Left Green", Name = "Gear.LeftGreen", Byte = 2, Bit = 0 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Left Red", Name = "Gear.LeftRed", Byte = 2, Bit = 1 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Center Green", Name = "Gear.CenterGreen", Byte = 2, Bit = 2 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Center Red", Name = "Gear.CenterRed", Byte = 2, Bit = 3 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Right Green", Name = "Gear.RightGreen", Byte = 2, Bit = 4 });
            Lights.Add(new JoystickOutputDevice() { Label = "Gear - Right Red", Name = "Gear.RightRed", Byte = 2, Bit = 5 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Master Warning", Name = "Light.MasterWarn", Byte = 2, Bit = 6 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Engine Fire", Name = "Light.EngineFire", Byte = 2, Bit = 7 });
            // -- Byte 3
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Low Oil Pressure", Name = "Light.LowOil", Byte = 3, Bit = 0 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Low Fuel Pressure", Name = "Light.LowFuel", Byte = 3, Bit = 1 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Anti Ice", Name = "Light.Antiice", Byte = 3, Bit = 2 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Starter Engaged", Name = "Light.Starter", Byte = 3, Bit = 3 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - APU", Name = "Light.APU", Byte = 3, Bit = 4 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Master Caution", Name = "Light.MasterCaution", Byte = 3, Bit = 5 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Vacuum", Name = "Light.Vacuum", Byte = 3, Bit = 6 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Low Hyd Pressure", Name = "Light.LowHydPressURE", Byte = 3, Bit = 7 });
            // -- Byte 4
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Aux Fuel Pump", Name = "Lights.AuxFuelPump", Byte = 4, Bit = 0 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Parking Brake", Name = "Lights.ParkingBrake", Byte = 4, Bit = 1 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Low Volts", Name = "Lights.LowVolts", Byte = 4, Bit = 2 });
            Lights.Add(new JoystickOutputDevice() { Label = "Lights - Door", Name = "Lights.door", Byte = 4, Bit = 3 });
        }
    }
}
