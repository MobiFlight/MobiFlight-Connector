using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight
{
    public class JoystickInput
    {
        /// <summary>
        /// The input's unique identifier on the joystick.
        /// </summary>
        public int Id;

        /// <summary>
        /// The input's type. Supported values: Button, Axis.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public JoystickDeviceType Type;

        /// <summary>
        /// The legacy name for the input, for backwards compatibility with previous releases.
        /// </summary>
        public string Name
        {
            get
            {
                if (this.Type == JoystickDeviceType.Axis)
                {
                    try
                    {
                        var axisName = Joystick.GetAxisNameForUsage(Id);
                        return $"{Type} {axisName}";
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        // Log the issue for debugging/awareness, then fall through to default naming
                        Log.Instance.log($"JoystickInput: No axis mapping found for usage ID {Id}, using fallback naming", LogSeverity.Debug);
                        // Fall through to final return
                    }
                }

                return $"{Type} {Id}";
            }
        }

        /// <summary>
        /// Friendly label for the input.
        /// </summary>
        public string Label;
    }
}
