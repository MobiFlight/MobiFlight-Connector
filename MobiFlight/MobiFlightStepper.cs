using CommandMessenger;
using MobiFlight.Config;
using System;

namespace MobiFlight
{
    class MobiFlightStepper : IConnectedDevice
    {
        public const string TYPE = "Stepper";

        protected String _name = "Stepper";

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private DeviceType _type = DeviceType.Stepper;
        public DeviceType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public CmdMessenger CmdMessenger { get; set; }
        public int StepperNumber { get; set; }
        public int InputRevolutionSteps { get; set; }
        public int OutputRevolutionSteps { get; set; }
        public StepperProfilePreset Profile { get; set; }
        protected Int16 _acceleration;
        public Int16 Acceleration
        {
            get { return _acceleration; }
            set
            {
                if (_acceleration != value)
                {
                    _acceleration = value;
                    _updateSpeedAndAcceleration();
                }
            }
        }

        protected Int16 _speed;
        public Int16 Speed
        {
            get { return _speed; }
            set { 
                if (_speed != value) {
                    _speed = value;
                    _updateSpeedAndAcceleration();
                } 
            }
        }

        public bool HasAutoZero { get; set; }
        public bool CompassMode { get; set; }

        protected DateTime lastCall;
        protected int lastValue;
        protected int outputValue;
        protected bool moveCalled = false;
        protected int zeroPoint = 0;

        public MobiFlightStepper()
        {
            HasAutoZero = false;
            lastValue = 0;
            outputValue = 0;
            StepperNumber = 0;
            InputRevolutionSteps = 1000;
            CompassMode = false;
        }

        public void MoveToPosition(int value, bool direction)
        {
            int mappedValue = 0;
            if ((double)InputRevolutionSteps != 0)
            {
                mappedValue = Convert.ToInt32(Math.Ceiling((value / (double)InputRevolutionSteps) * OutputRevolutionSteps));
            }

            int delta = mappedValue - lastValue;

            lastValue = mappedValue;

            if (delta == 0) return;

            if (CompassMode && Math.Abs(delta) > (OutputRevolutionSteps / 2))
            {

                if (delta < 0)
                    outputValue += (OutputRevolutionSteps + delta);
                else
                    outputValue -= (OutputRevolutionSteps - delta);
            }
            else
            {
                outputValue += delta;
            }

            var command = new SendCommand((int)MobiFlightModule.Command.SetStepper);
            command.AddArgument(this.StepperNumber);
            command.AddArgument(outputValue);
            Log.Instance.log($"Command: SetStepper <{(int)MobiFlightModule.Command.SetStepper},{StepperNumber},{outputValue};>.", LogSeverity.Debug);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        public int Position()
        {
            return lastValue;
        }

        public void Init()
        {
            var command = new SendCommand((int)MobiFlightModule.Command.ResetStepper);
            command.AddArgument(this.StepperNumber);

            Log.Instance.log($"Command: ResetStepper <{(int)MobiFlightModule.Command.SetZeroStepper},{StepperNumber};>.", LogSeverity.Debug);

            // Send command
            CmdMessenger.SendCommand(command);
        }

        internal void Reset()
        {
            var command = new SendCommand((int)MobiFlightModule.Command.SetZeroStepper);
            command.AddArgument(this.StepperNumber);

            Log.Instance.log($"Command: SetZeroStepper <{(int)MobiFlightModule.Command.SetZeroStepper},{StepperNumber};>.", LogSeverity.Debug);

            // Send command
            CmdMessenger.SendCommand(command);

            // We have set the new zero position
            // so this has to be updated internally too.
            lastValue = 0;
            outputValue = 0;
        }

        public void Stop()
        {
            MoveToPosition(0, true);
        }

        private void _updateSpeedAndAcceleration()
        {
            var command = new SendCommand((int)MobiFlightModule.Command.SetStepperSpeedAccel);
            command.AddArgument(this.StepperNumber);
            command.AddArgument(this.Speed);
            command.AddArgument(this.Acceleration);

            Log.Instance.log($"Command: SetStepperSpeedAccel <{(int)MobiFlightModule.Command.SetStepperSpeedAccel},{StepperNumber},{Speed},{Acceleration};>.", LogSeverity.Debug);

            // Send command
            CmdMessenger.SendCommand(command);
        }
    }
}
