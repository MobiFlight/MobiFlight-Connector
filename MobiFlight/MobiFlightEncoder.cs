using MobiFlight.Base;

namespace MobiFlight
{
    public class MobiFlightEncoder : DeviceConfig, IConnectedDevice
    {
        public const string TYPE = "Encoder";
        public enum InputEvent
        {
            LEFT,
            LEFT_FAST,
            RIGHT,
            RIGHT_FAST
        }

        public MobiFlightEncoder()
        {
            Name = "Encoder";
        }

        public MobiFlightEncoder(MobiFlightEncoder copyFrom)
        {
            Name = copyFrom.Name;
        }

        public override object Clone()
        {
            return new MobiFlightEncoder(this);
        }

        public DeviceType TypeDeprecated { get { return DeviceType.Encoder; } }

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
