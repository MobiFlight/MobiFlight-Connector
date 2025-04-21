using MobiFlight.Base;

namespace MobiFlight
{
    public class ConfigRefValue
    {

        public ConfigRefValue() { }
        public ConfigRefValue(ConfigRef c, string value)
        {
            this.Value = value;
            this.ConfigRef = c.Clone() as ConfigRef;
        }

        public string Value { get; set; }
        public ConfigRef ConfigRef { get; set; }

    }
}
