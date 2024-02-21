using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    using Newtonsoft.Json;
    using System;

    public class HexStringToNumberConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int) || objectType == typeof(long);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string valueString = reader.Value.ToString();
                // If the string starts with 0x then it is hex and should be converted. Otherwise assume it is decimal and convert normally.
                if (valueString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    valueString = valueString.Substring(2);
                    return Convert.ToInt32(valueString, 16);
                }
                else
                {
                    return Convert.ToInt32(valueString);
                }
            }
            // This handles backwards compatibility for old files where it was stored as an integer.
            else if (reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToInt32(reader.Value.ToString());
            }

            throw new JsonSerializationException("Unexpected token type: " + reader.TokenType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is int || value is long)
            {
                string hexString = $"0x{Convert.ToString(Convert.ToInt64(value), 16)}";
                writer.WriteValue(hexString);
            }
            else
            {
                throw new JsonSerializationException("Unexpected value type: " + value.GetType());
            }
        }
    }

    public class JoystickInfo
    {
        /// <summary>
        /// The device's icon filename. Required.
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// The device's picture filename. Required.
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// The device's manufacturer. Required.
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// the device's website. Required.
        /// </summary>
        public string Website { get; set; }
    }

    public class JoystickDefinition : IMigrateable
    {
        public JoystickInfo Info { get; set; }
        /// <summary>
        /// Instance name for the device. This is used to match the definition with a connected device.
        /// </summary>
        public string InstanceName;

        /// <summary>
        /// List of inputs supported by the device.
        /// </summary>
        public List<JoystickInput> Inputs;

        /// <summary>
        /// List of options supported by the device.
        /// </summary>
        public List<JoystickOutput> Outputs;

        /// <summary>
        /// The device's USB ProductId. Required if Outputs are provided.
        /// </summary>
        [JsonConverter(typeof(HexStringToNumberConverter))]
        public int ProductId;

        /// <summary>
        /// The device's USB VendorId. Required if Outputs are provided.
        /// </summary>
        [JsonConverter(typeof(HexStringToNumberConverter))]
        public int VendorId;

        /// <summary>
        /// Finds a JoystickInput given an input name. This will eventually get replaced with a method that
        /// looks up by Id instead.
        /// </summary>
        /// <param name="name">The name of the input to look up</param>
        /// <returns>The first JoystickInput that matches the specified name or null if none found.</returns>
        public JoystickInput FindInputByName(string name)
        {
            return Inputs.Find(input => input.Name == name);
        }

        /// <summary>
        /// Migrates values from a prior version of the JSON schema to the newest version.
        /// </summary>
        public void Migrate() { }
    }
}
