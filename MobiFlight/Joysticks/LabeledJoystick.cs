using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks
{
    public class LabeledJoystick : Joystick
    {
        HidStream Stream { get; set; }
        HidDevice Device { get; set; }

        protected Dictionary<string, string> Labels = new Dictionary<string, string>();
        readonly public HidDefinition Definition;

        public LabeledJoystick(SharpDX.DirectInput.Joystick joystick, HidDefinition definition) : base(joystick) 
        {
            Definition = definition;
            Definition?.Inputs?.ForEach(input => Labels.Add(input.Name, input.Label));
        }

        private void Connect()
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
            // Don't try and send data if no outputs are defined.
            if (Definition.Outputs == null || Definition.Outputs.Count == 0)
            {
                return;
            }

            if (!RequiresOutputUpdate) return;
            if (Stream == null)
            {
                Connect();
            };
            Stream.SetFeature(data);
            base.SendData(data);
        }

        protected override List<JoystickDevice> GetButtonsSorted()
        {
            var buttons = Buttons.ToArray().ToList();
            buttons.Sort(SortByPositionInDictionary);

            return buttons;
        }

        protected override void EnumerateOutputDevices()
        {
            base.EnumerateOutputDevices();
            Definition?.Outputs?.ForEach(output => Lights.Add(new JoystickOutputDevice() { Label = output.Label, Name = output.Name, Byte = output.Byte, Bit = output.Bit }));
        }

        public override string MapDeviceNameToLabel(string name)
        {
            string result;
            if (Labels.ContainsKey(name))
                result = Labels[name];
            else
                result = base.MapDeviceNameToLabel(name);

            return result;
        }

        protected override List<JoystickDevice> GetAxisSorted()
        {
            var axes = Axes.ToArray().ToList();
            Axes.Sort(SortByPositionInDictionary);

            return axes;
        }

        public int GetIndexForKey(string key)
        {
            int result = Array.IndexOf(Labels.Keys.ToArray(), key);
            return result;
        }

        int SortByPositionInDictionary(JoystickDevice b1, JoystickDevice b2)
        {
            if (GetIndexForKey(b1.Name) == GetIndexForKey(b2.Name)) return 0;
            if (GetIndexForKey(b1.Name) > GetIndexForKey(b2.Name)) return 1;
            return -1;
        }
    }
}
