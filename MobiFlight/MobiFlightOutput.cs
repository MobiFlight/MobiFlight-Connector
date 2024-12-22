using System;
using System.Collections.Generic;
using System.Globalization;
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

        public MobiFlightOutput() {
            OutputNumber = 0;
        }

        public void Set(int value)
        {
            var command = new SendCommand((int)MobiFlightModule.Command.SetPin);
            command.AddArgument(value);

            //if (HasFirmwareFeature(FirmwareFeature.Output_DeviceID))
            {
                command.AddArgument(OutputNumber);
                Log.Instance.log($"Command: SetPin <{(int)MobiFlightModule.Command.SetPin},{OutputNumber},{value};>.", LogSeverity.Debug);
            }
        /*
            else
            {
                command.AddArgument(Pin);
                Log.Instance.log($"Command: SetPin <{(int)MobiFlightModule.Command.SetPin},{Pin},{value};>.", LogSeverity.Debug);
            }
         */
            // Send command
            CmdMessenger.SendCommand(command);
        }

        public void Stop()
        {
            Set(0);
        }

    }
}
