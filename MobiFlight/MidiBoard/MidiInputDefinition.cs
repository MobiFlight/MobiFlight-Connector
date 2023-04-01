using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight
{
    public class MidiInputDefinition
    {
        public string Label;

        public string[] LabelIds;

        public string Layer;

        [JsonConverter(typeof(StringEnumConverter))]
        public MidiBoardDeviceType InputType;

        [JsonConverter(typeof(StringEnumConverter))]
        public MidiMessageType MessageType;

        public byte MessageChannel;

        public byte[] MessageIds;

        public string GetLabelWithIndex(int index)
        {
            return Label.Replace("%", LabelIds[index]);
        }

        public string GetNameWithIndex(int index)
        {            
            return GetName(MessageType, MessageChannel, MessageIds[index]);
        }

        public static string GetName(MidiMessageType mType, byte Channel, byte Id)
        {
            if (mType == MidiMessageType.Pitch)
                return $"{mType} {Channel}";
            else
                return $"{mType} {Channel}_{Id}";
        }

    }
}
