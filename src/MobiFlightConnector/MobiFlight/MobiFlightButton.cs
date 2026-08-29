namespace MobiFlight
{
    class MobiFlightButton : IConnectedDevice
    {
        public const string TYPE = "Button";
        public enum InputEvent
        {
            PRESS = 0,
            RELEASE = 1,
            LONG_RELEASE = 2,
            HOLD = 3,
            // Fired instead of HOLD for every repeat tick after the first HOLD, while the button
            // is still held down
            REPEAT = 4
        }

        public string Name { get; set; }

        public DeviceType TypeDeprecated { get { return DeviceType.Button; } }

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

                case (int)InputEvent.LONG_RELEASE:
                    eventAction = InputEvent.LONG_RELEASE.ToString();
                    break;

                case (int)InputEvent.HOLD:
                    eventAction = InputEvent.HOLD.ToString();
                    break;

                case (int)InputEvent.REPEAT:
                    eventAction = InputEvent.REPEAT.ToString();
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