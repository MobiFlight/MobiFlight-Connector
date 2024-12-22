using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;

namespace MobiFlight
{
    public class MobiFlightOutput : IConnectedDevice
    {
        public const string TYPE = "Output";

        private String _name = "Output";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        private DeviceType _type = DeviceType.Output;
        public DeviceType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public CmdMessenger CmdMessenger { get; set; }
        public int Pin { get; set; }

        public int OutputNumber { get; set; }

        public bool sendDeviceID { get; set; }
        public MobiFlightOutput() { }

        public void Set(int value)
        {
            var command = new SendCommand((int)MobiFlightModule.Command.SetPin);
            int output;

            if (sendDeviceID)
                output = OutputNumber;
            else
                output = Pin;

            Log.Instance.log($"Command: SetPin <{(int)MobiFlightModule.Command.SetPin},{output},{value};>.", LogSeverity.Debug);
            command.AddArgument(output);
            command.AddArgument(value);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        public void Stop()
        {
            Set(0);
        }

    }
}
