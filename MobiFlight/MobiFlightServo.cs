using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;

namespace MobiFlight
{
    public class MobiFlightServo : IConnectedDevice
    {
        public const string TYPE = "Servo";
        public const int OutputLower = 0;
        public const int OutputUpper = 180;

        private String _name = "Servo";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        private DeviceType _type = DeviceType.Servo;
        public DeviceType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public CmdMessenger CmdMessenger { get; set; }
        public int ServoNumber { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxRotationPercent { get; set; }
        
        public MobiFlightServo()
        {
            Min = 0;
            Max = 180;
            MaxRotationPercent = 100;
        }

        private int map(int value)
        {
            int outputUpper = (int)Math.Round((float) OutputUpper * MaxRotationPercent / 100);
            float relVal = (value - Min) / (float)(Max - Min);
            return (int)Math.Round((relVal * (outputUpper - OutputLower)) + Min, 0);
        }

        public void MoveToPosition(int value)
        {
            int mappedValue = map(value);
            
            var command = new SendCommand((int)MobiFlightModule.Command.SetServo);
            command.AddArgument(ServoNumber);
            command.AddArgument(mappedValue);
            Log.Instance.log($"Command: SetServo <{(int)MobiFlightModule.Command.SetServo},{ServoNumber},{mappedValue};>.", LogSeverity.Debug);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        public void Stop()
        {
            MoveToPosition(0);
        }
    }
}
