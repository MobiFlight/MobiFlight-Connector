using MobiFlight.Base;
using MobiFlight.InputConfig;
using Newtonsoft.Json;

namespace MobiFlight
{
    public class MobiFlightButton : DeviceConfig, IConnectedDevice
    {
        public const string TYPE = "Button";
        public ButtonInputConfig Config { get; set; } = new ButtonInputConfig();

        public enum InputEvent
        {
            PRESS = 0,
            RELEASE = 1,
            LONG_RELEASE = 2,
            HOLD = 3
        }

        public MobiFlightButton()
        {
            Name = "Button";
            _type = TYPE;
        }

        public MobiFlightButton(MobiFlightButton copyFrom)
        {
            Name = copyFrom.Name;
            Config = copyFrom.Config.Clone() as ButtonInputConfig;
        }

        public override object Clone()
        {
            return new MobiFlightButton(this);
        }

        [JsonIgnore]
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
