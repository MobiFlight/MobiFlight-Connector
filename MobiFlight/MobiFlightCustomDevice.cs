using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.Design;
using CommandMessenger;
using MobiFlight.Base;
using MobiFlight.CustomDevices;
using Newtonsoft.Json.Linq;
using SharpDX.DirectInput;

namespace MobiFlight
{
    public class MobiFlightCustomDevice : IConnectedDevice
    {
        public const string TYPE = "CustomDevice";

        public CmdMessenger CmdMessenger { get; set; }


        private String _name = "Custom Device";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        private DeviceType _type = DeviceType.CustomDevice;
        public DeviceType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int DeviceNumber { get; set; }
        public CustomDevices.CustomDevice CustomDevice { get; set; }

        protected bool _initialized = false;

        public MobiFlightCustomDevice()
        {
            
        }

        protected void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }

        public void Display(string MessageType, string value)
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetCustomDevice);

            command.AddArgument(DeviceNumber);
            command.AddArgument(MessageType);            
            command.AddArgument(value);

            Log.Instance.log($"Command: SetCustomDevice <{(int)MobiFlightModule.Command.SetCustomDevice},{DeviceNumber},{MessageType},{value};>.", LogSeverity.Debug);

            // Send command
            System.Threading.Thread.Sleep(1);
            CmdMessenger.SendCommand(command);
        }

        // Blank the display when stopped
        public void Stop()
        {
        }
    }
}
