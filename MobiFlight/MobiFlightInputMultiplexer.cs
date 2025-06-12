using CommandMessenger;
using MobiFlight.Base;

namespace MobiFlight
{
    public class MobiFlightInputMultiplexer : DeviceConfig, IConnectedDevice
    {
        public enum InputEvent
        {
            PRESS,
            RELEASE,
            REPEAT,     // For future uses, like Buttons
        }

        public const string TYPE = "InputMultiplexer";
        public const string LABEL_PREFIX = "Input";

        public CmdMessenger CmdMessenger { get; set; }


        private DeviceType _typeDeprecated = DeviceType.InputMultiplexer;
        public int ModuleNumber { get; set; }

        public override string Name { get { return "Multiplexer"; } }

        protected bool _initialized = false;

        public MobiFlightInputMultiplexer()
        {
            ModuleNumber = 0;
            _type = TYPE;
        }

        public MobiFlightInputMultiplexer(MobiFlightInputMultiplexer copyFrom)
        {
            ModuleNumber = copyFrom.ModuleNumber;
        }

        public override object Clone()
        {
            return new MobiFlightInputMultiplexer(this);
        }

        public void Stop() {}

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
            get { return _typeDeprecated; }
            set { _typeDeprecated = value; }
        }

        protected void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }
    }
}