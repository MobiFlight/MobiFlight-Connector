using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;

namespace MobiFlight
{
    public class MobiFlightLedModule : IConnectedDevice
    {
        public const string TYPE = "Display Module";

        public CmdMessenger CmdMessenger { get; set; }
        public int ModuleNumber { get; set; }
        public int Brightness { get; set; }
        public int SubModules { get; set; }

        private String _name = "Led Module";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        
        private DeviceType _type = DeviceType.LedModule;
        public DeviceType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        
        protected bool _initialized = false;

        public MobiFlightLedModule()
        {
            Brightness = 15;
        }

        protected void Initialize()
        {
            if (_initialized) return;

            // Create command
            /*
            var command = new SendCommand((int)MobiFlightModule.Command.InitModule);
            command.AddArgument(this.ModuleNumber);
            command.AddArgument(this.Brightness);

            // Send command
            CmdMessenger.SendCommand(command);
            */

            _initialized = true;
        }

        public void Display( int subModule, String value, byte points, byte mask )
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetModule);

            // clamp and reverse the string
            if (value.Length > 8) value = value.Substring(0, 8);
            //while (value.Length < 8) value += " ";
            
            //value = new string(value.ToCharArray().Reverse().ToArray());
            
            command.AddArgument(this.ModuleNumber);
            command.AddArgument(subModule);            
            command.AddArgument(value);
            command.AddArgument(points);
            command.AddArgument(mask);

            Log.Instance.log("Command: SetModule <" + (int)MobiFlightModule.Command.SetModule + "," +
                                                      this.ModuleNumber + "," +
                                                      subModule + "," +
                                                      value + "," +
                                                      mask + ";>", LogSeverity.Debug);

            // Send command
            CmdMessenger.SendCommand(command);
        }
    }
}
