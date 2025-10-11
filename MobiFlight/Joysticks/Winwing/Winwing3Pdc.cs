using MobiFlight.Config;
using MobiFlightWwFcu;
using System;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Winwing
{
    internal class Winwing3Pdc : Joystick
    {
        private WinwingDisplayControl DisplayControl;

        private List<IBaseDevice> LcdDevices = new List<IBaseDevice>();

        public Winwing3Pdc(SharpDX.DirectInput.Joystick joystick, JoystickDefinition def, int productId, WebSocketServer server) : base(joystick, def)
        {
            Log.Instance.log($"WinWing3Pdc - New WinWing3Pdc ProductId={productId.ToString("X")}", LogSeverity.Debug);
            DisplayControl = new WinwingDisplayControl(productId, server);
            var displayNames = DisplayControl.GetDisplayNames();

            DisplayControl.ErrorMessageCreated += DisplayControl_ErrorMessageCreated;

            // Initialize LCD for brightness setting and current value cache
            foreach (string displayName in displayNames)
            {
                LcdDevices.Add(new LcdDisplay() { Name = displayName }); // Col and Lines values don't matter   
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
            return new List<DeviceType>() { DeviceType.LcdDisplay };
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
