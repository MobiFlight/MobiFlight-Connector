using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight
{
    public class MidiInputDefinition
    {
        /// <summary>
        /// Friendly label for the input. Required.
        /// </summary>
        public string Label;

        /// <summary>
        /// Friendly label ids for the input, replacing the % in the label. Required.
        /// </summary>
        public string[] LabelIds;

        /// <summary>
        /// Associated layer for the input. Optional.
        /// </summary>
        public string Layer;

        /// <summary>
        /// The input's type. Supported values: Button, EndlessKnob, LimitedKnob, Fader, Pitch. Required.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MidiBoardDeviceType InputType;

        /// <summary>
        /// The midi message type. Supported values: Note, CC, Pitch. Required.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MidiMessageType MessageType;

        /// <summary>
        /// The midi message channel. Possible value range from 1 to 16. Required.
        /// </summary>
        public byte MessageChannel;

        /// <summary>
        /// The midi message ids. Possible value range from 0 to 127. Required.
        /// </summary>
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
