using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight 
{
    class MobiFlightButton : IConnectedDevice
    {
        public const string TYPE = "Button";
        public enum InputEvent
        {
            PRESS = 0,
            RELEASE = 1,
            REPEAT = 2,
            LONG_RELEASE = 3
        }

        public string Name { get; set; }

        public DeviceType Type { get { return DeviceType.Button; } }

        public static string InputEventIdToString(int enumId) {
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
                case (int)InputEvent.LONG_RELEASE:
                    eventAction = InputEvent.LONG_RELEASE.ToString();
                    break;
            }

            return eventAction;
        }
        public void Stop()
        {
            // do nothing
            return;
        }
    }
}
