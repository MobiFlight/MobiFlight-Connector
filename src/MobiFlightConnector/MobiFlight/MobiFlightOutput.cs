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
        public DeviceType TypeDeprecated
        {
            get { return _type; }
            set { _type = value; }
        }

        public CmdMessenger CmdMessenger { get; set; }
        public int Pin { get; set; }

        public virtual void Set(int value)
        {
            var command = new SendCommand((int)MobiFlightModule.Command.SetPin);

            Log.Instance.log($"Command: SetPin <{(int)MobiFlightModule.Command.SetPin},{Pin},{value};>.", LogSeverity.Debug);

            command.AddArgument(Pin);
            command.AddArgument(value);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        public void Stop()
        {
            Set(0);
        }
    }

    public class MobiFlightOutputV3 : MobiFlightOutput
    {
        public int DeviceIndex { get; set; }

        public override void Set(int value)
        {
            var command = new SendCommand((int)MobiFlightModule.Command.SetPin);

            Log.Instance.log($"Command: SetPin <{(int)MobiFlightModule.Command.SetPin},{DeviceIndex},{value};>.", LogSeverity.Debug);

            command.AddArgument(DeviceIndex);
            command.AddArgument(value);
            // Send command
            CmdMessenger.SendCommand(command);
        }
    }
}
