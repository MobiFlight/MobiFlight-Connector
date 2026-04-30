namespace MobiFlight 
{
    class MobiFlightAnalogInput : IConnectedDevice
    {
        public const string TYPE = "AnalogInput";
        public enum InputEvent
        {
            CHANGE
        }

        public string Name { get; set; }

        public DeviceType TypeDeprecated { get { return DeviceType.AnalogInput; } }

        public static string InputEventIdToString(int enumId) {
            string eventAction = "n/a";
            switch (enumId)
            {
                case (int)InputEvent.CHANGE:
                    eventAction = InputEvent.CHANGE.ToString();
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