using CommandMessenger;
using System;
using static MobiFlight.MobiFlightButton;

namespace MobiFlight
{
    public class MobiFlightInputMultiplexer : IConnectedDevice
    {
        public const string TYPE = "InputMultiplexer";
        public const string LABEL_PREFIX = "Input";

        public CmdMessenger CmdMessenger { get; set; }


        private DeviceType _type = DeviceType.InputMultiplexer;
        private String _name = "Multiplexer";
        public int ModuleNumber { get; set; }

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected bool _initialized = false;

        public MobiFlightInputMultiplexer()
        {
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


    }
}