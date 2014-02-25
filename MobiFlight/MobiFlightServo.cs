using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;

namespace MobiFlight
{
    public class MobiFlightServo : IConnectedDevice
    {
        private String _name = "Servo";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        private String _type = "SERVO";
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public CmdMessenger CmdMessenger { get; set; }
        public int ServoNumber { get; set; }
        
        public MobiFlightServo()
        {
        }

        public void MoveToPosition(int value)
        {
            int mappedValue = value;
            
            var command = new SendCommand((int)MobiFlightModule.Command.SetServo);
            command.AddArgument(ServoNumber);
            command.AddArgument(mappedValue);
            
            // Send command
            CmdMessenger.SendCommand(command);
        }
    }
}
