using CommandMessenger;
using MobiFlight.Base;

namespace MobiFlight
{
    public class MobiFlightInputShiftRegister : DeviceConfig, IConnectedDevice
    {
        public enum InputEvent
        {
            PRESS,
            RELEASE,
            REPEAT,     // For future uses, like Buttons
        }

        public const string TYPE = "InputShiftRegister";
        public const string LABEL_PREFIX = "Input";

        public CmdMessenger CmdMessenger { get; set; }

        public int NumberOfShifters { get; set; }

        public int ModuleNumber { get; set; }

        public override string Name { get => "InputShiftRegister"; }

        protected bool _initialized = false;

        public MobiFlightInputShiftRegister()
        {
            Name = "InputShiftRegister";
            NumberOfShifters = 1; // Default to 1 shifter
            ModuleNumber = 0; // Default module number
        }

        public MobiFlightInputShiftRegister(MobiFlightInputShiftRegister copyFrom)
        {
            Name = copyFrom.Name;
            NumberOfShifters = copyFrom.NumberOfShifters;
            ModuleNumber = copyFrom.ModuleNumber;
        }

        public override object Clone()
        {
            return new MobiFlightInputShiftRegister(this);
        }

        private DeviceType _type = DeviceType.InputShiftRegister;

        public static string InputEventIdToString(int enumId)
        {
            string eventAction = "n/a";
            switch (enumId)
            {
                case (int)InputEvent.PRESS:
                    eventAction = InputEvent.PRESS.ToString();
                    break;

                case (int)InputEvent.RELEASE:
                    eventAction = InputEvent.RELEASE.ToString();
                    break;

                case (int)InputEvent.REPEAT:
                    eventAction = InputEvent.REPEAT.ToString();
                    break;
            }

            return eventAction;
        }

        public DeviceType TypeDeprecated
        {
            get { return _type; }
            set { _type = value; }
        }

        protected void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }

        public void Stop()
        {
            // do nothing
            return;
        }
    }
}