using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;

namespace MobiFlight
{
    public class MobiFlightLedModule : IConnectedDevice
    {
        public CmdMessenger CmdMessenger { get; set; }
        public int ModuleNumber { get; set; }
        public int Brightness { get; set; }

        private String _name = "Led Module";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        
        private String _type = "LEDMODULE";
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }
        
        protected bool _initialized = false;

        public MobiFlightLedModule()
        {
            Brightness = 10;
        }

        protected void Initialize()
        {
            if (_initialized) return;

            // Create command
            var command = new SendCommand((int)MobiFlightModule.Command.InitModule);
            command.AddArgument(this.ModuleNumber);
            command.AddArgument(this.Brightness);

            // Send command
            CmdMessenger.SendCommand(command);

            _initialized = true;
        }

        public void Display( String value )
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetModule);

            // clamp and reverse the string
            if (value.Length > 8) value = value.Substring(0, 8);
            while (value.Length < 8) value += " ";
            
            value = new string(value.ToCharArray().Reverse().ToArray());
            
            command.AddArgument(this.ModuleNumber);
            command.AddArgument(value);

            // Send command
            CmdMessenger.SendCommand(command);
        }
    }
}
