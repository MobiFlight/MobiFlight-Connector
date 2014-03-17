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
        
        public MobiFlightServo()
        {
            Min = 0;
            Max = 180;
        }

        private int map(int value)
        {
            int outputLower = 0;
            int outputUpper = 180;
            float relVal = (value - Min) / (float)(Max - Min);
            return (int)Math.Round((relVal * (outputUpper - outputLower)) + Min, 0);
        }

        public void MoveToPosition(int value)
        {
            int mappedValue = map(value);
            
            var command = new SendCommand((int)MobiFlightModule.Command.SetServo);
            command.AddArgument(ServoNumber);
            command.AddArgument(mappedValue);
            
            // Send command
            CmdMessenger.SendCommand(command);
        }
    }
}
