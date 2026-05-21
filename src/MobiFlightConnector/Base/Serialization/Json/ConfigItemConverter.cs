using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MobiFlight.Base.Serialization.Json
{
    public class ConfigItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IConfigItem).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var typeName = $"MobiFlight.{(string)jsonObject["Type"]}";

            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new NotSupportedException($"Unknown type: {typeName}");
            }

            // Migration: If ModuleSerial exists but Controller doesn't, convert ModuleSerial to Controller
            if (jsonObject["ModuleSerial"] != null && jsonObject["Controller"] == null)
            {
                var moduleSerial = (string)jsonObject["ModuleSerial"];
                var controller = SerialNumber.CreateController(moduleSerial);

                // Add Controller property to JSON
                if (controller != null)
                    jsonObject["Controller"] = JObject.FromObject(controller);

                // Remove ModuleSerial from JSON (it will be ignored during deserialization anyway)
                jsonObject.Remove("ModuleSerial");
            }

            // Migration: If DeviceName and DeviceType exist but Device doesn't, convert them to Device
            var deviceName = (string)jsonObject["DeviceName"];
            var deviceType = (string)jsonObject["DeviceType"];
            if (deviceName != null && deviceType != null && jsonObject["Device"] == null)
            {
                var subIndex = 0;
                if (deviceType == InputConfigItem.TYPE_INPUT_MULTIPLEXER)
                {
                    var multiplexerPin = jsonObject["inputMultiplexer"]?["DataPin"];
                    if (multiplexerPin != null) {
                        int.TryParse(multiplexerPin.ToString(), out subIndex);
                    }
                }

                if (deviceType == InputConfigItem.TYPE_INPUT_SHIFT_REGISTER)
                {
                    var pin = jsonObject["inputShiftRegister"]?["ExtPin"];
                    if (pin != null) {
                        int.TryParse(pin.ToString(), out subIndex);
                    }
                }

                var device = InputConfigItem.CreateInputDevice(deviceType, deviceName, subIndex);
                if (device != null)
                    jsonObject["Device"] = JObject.FromObject(device);

                jsonObject.Remove("DeviceName");
                jsonObject.Remove("DeviceType");
            }

            var configItem = Activator.CreateInstance(type) as IConfigItem;
            serializer.Populate(jsonObject.CreateReader(), configItem);
            return configItem;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Use default serialization
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}