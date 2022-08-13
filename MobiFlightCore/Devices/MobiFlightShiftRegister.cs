using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;

namespace MobiFlight
{
    public class MobiFlightShiftRegister : IConnectedDevice
    {
        public const string TYPE = "ShiftRegister";
        public const string LABEL_PREFIX = "Output";

        public CmdMessenger CmdMessenger { get; set; }

        public int NumberOfShifters { get; set; }
        
        public int ModuleNumber { get; set; }

        private String _name = "ShiftRegister";

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected bool _initialized = false;

        public MobiFlightShiftRegister()
        {
        }

        private DeviceType _type = DeviceType.ShiftRegister;

        public DeviceType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        protected void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }

        public void Display(String outputPins, String value)
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetShiftRegisterPins);

            // Let's strip the static label
            String pinsOnly = outputPins.Replace(LABEL_PREFIX + " ", "");

            // clamp and reverse the string
            if (value.Length > 8) value = value.Substring(0, 8);

            command.AddArgument(this.ModuleNumber);
            command.AddArgument(pinsOnly);
            command.AddArgument(value);

            Log.Instance.log("Command: SetShiftRegisterPin <" + (int)MobiFlightModule.Command.SetShiftRegisterPins + "," +
                                                      this.ModuleNumber + "," +
                                                      pinsOnly + "," +
                                                      value + ";>", LogSeverity.Debug);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        public void Stop()
        {
            List<String> pins = new List<string>();
            for (int i = 0; i != NumberOfShifters*8; i++)
                pins.Add(i.ToString());

            String pinString = string.Join("|", pins);
            Display(pinString, "0");
        }
    }
}
