using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    class MobiFlightEncoder : IConnectedDevice
    {
        public const string TYPE = "Encoder";
        public enum InputEvent
        {
            LEFT,
            LEFT_FAST,
            RIGHT,
            RIGHT_FAST
        }

        public string Name { get; set; }

        public DeviceType Type { get { return DeviceType.Encoder; } }

        public static string InputEventIdToString(int enumId)
        {
            string eventAction = "n/a";

            switch (enumId)
            {
                case (int)InputEvent.LEFT:
                    eventAction = InputEvent.LEFT.ToString();
                    break;

                case (int)InputEvent.LEFT_FAST:
                    eventAction = InputEvent.LEFT_FAST.ToString();
                    break;

                case (int)InputEvent.RIGHT:
                    eventAction = InputEvent.RIGHT.ToString();
                    break;

                case (int)InputEvent.RIGHT_FAST:
                    eventAction = InputEvent.RIGHT_FAST.ToString();
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
