using MobiFlight.Base;

namespace MobiFlight 
{
    public class MobiFlightAnalogInput : DeviceConfig, IConnectedDevice
    {
        public const string TYPE = "AnalogInput";
        public const string TYPE_OLD = "Analog"; // Deprecated type for backward compatibility

        public enum InputEvent
        {
            CHANGE
        }

        public DeviceType TypeDeprecated { get { return DeviceType.AnalogInput; } }

        public MobiFlightAnalogInput()
        {
            Name = "AnalogInput";
        }

        public MobiFlightAnalogInput(MobiFlightAnalogInput copyFrom)
        {
            Name = copyFrom.Name;
        }

        public override object Clone()
        {
            return new MobiFlightAnalogInput(this);
        }

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
