using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;

namespace MobiFlight
{
    class MobiFlightStepper : IConnectedDevice
    {
        protected String _name = "Stepper";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        private String _type = "STEPPER";
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public CmdMessenger CmdMessenger { get; set; }
        public int StepperNumber { get; set; }
        public int InputRevolutionSteps { get; set; }
        public int OutputRevolutionSteps { get; set; }
        protected DateTime lastCall;
        protected int lastValue;
        protected bool moveCalled = false;
        protected int zeroPoint = 0;
        
        public MobiFlightStepper()
        {
            StepperNumber = 0;
            InputRevolutionSteps = 60;
        }

        private int map(int value, int inputLower, int inputUpper, int outputLower, int outputUpper)
        {
            float relVal = (value - inputLower) / (float)(inputUpper - inputLower);
            return (int) Math.Round((relVal * (outputUpper - outputLower)) + inputLower, 0);
        }

        public void MoveToPosition(int value, bool direction)
        {
            int mappedValue = Convert.ToInt32( Math.Ceiling ((value / (double) InputRevolutionSteps) * OutputRevolutionSteps));
            int currentSpeed = 100;
            
            if (!moveCalled) {
                zeroPoint = mappedValue;
                lastValue = mappedValue;                
                lastCall = DateTime.Now;
                moveCalled = true;
            }

            int delta = mappedValue - lastValue;            
            lastValue = mappedValue;

            if (delta == 0) return;
            var command = new SendCommand((int)MobiFlightModule.Command.SetStepper);
            command.AddArgument(this.StepperNumber);
            command.AddArgument(mappedValue + delta - zeroPoint);
            
            // Send command
            CmdMessenger.SendCommand(command);
        }
    }
}
