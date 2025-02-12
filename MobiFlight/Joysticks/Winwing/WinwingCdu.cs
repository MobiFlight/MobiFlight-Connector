using MobiFlight.Config;
using System;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingCdu : Joystick
    {
        private JoystickDefinition Definition;
        private WinwingDisplayControl DisplayControl;

        private List<IBaseDevice> LcdDevices = new List<IBaseDevice>();
        private List<ListItem<IBaseDevice>> LedDevices = new List<ListItem<IBaseDevice>>();

        public WinwingCdu(SharpDX.DirectInput.Joystick joystick, JoystickDefinition def, int productId, WebSocketServer server) : base(joystick, def)
        {
            Definition = def;
            DisplayControl = new WinwingDisplayControl(productId, server);
            var displayNames = DisplayControl.GetDisplayNames();
            var ledNames = DisplayControl.GetLedNames();

            DisplayControl.ErrorMessageCreated += DisplayControl_ErrorMessageCreated;

            // Initialize LCD and LED device lists and current value cache
            foreach (string displayName in displayNames)
            {
                LcdDevices.Add(new LcdDisplay() { Name = displayName }); // Col and Lines values don't matter   
            }
            foreach (string ledName in ledNames)
            {
                LedDevices.Add(new JoystickOutputDevice() { Label = ledName, Name = ledName }.ToListItem()); // Byte and Bit values don't matter           
            }
        }

        private void DisplayControl_ErrorMessageCreated(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                Log.Instance.log(e, LogSeverity.Error);
            }
        }


        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            DisplayControl.Connect();
        }


        public override IEnumerable<DeviceType> GetConnectedOutputDeviceTypes()
        {
            // Output for the led indicators, LcdDisplay to control brightness
            return new List<DeviceType>() { DeviceType.Output, DeviceType.LcdDisplay };
        }

        public override void SetLcdDisplay(string address, string value)
        {
            // Check for value change is done in display control
            DisplayControl.SetDisplay(address, value);
        }

        public override void SetOutputDeviceState(string name, byte state)
        {
            // Check for value change is done in display control
            DisplayControl.SetLed(name, state);
        }

        public override List<IBaseDevice> GetAvailableLcdDevices()
        {
            return LcdDevices;
        }

        public override List<ListItem<IBaseDevice>> GetAvailableOutputDevicesAsListItems()
        {
            return LedDevices;
        }

        public override void UpdateOutputDeviceStates()
        {
            // do nothing, update is event based not polled
        }

        protected override void SendData(byte[] data)
        {
            // do nothing, data is directly send in SetOutputDeviceState
        }

        public override void Shutdown()
        {
            DisplayControl.Shutdown();
        }
    }
}
