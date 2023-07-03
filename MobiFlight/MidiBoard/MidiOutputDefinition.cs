using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight
{
    public class MidiOutputDefinition
    {
        /// <summary>
        /// Friendly label for the output. Required.
        /// </summary>
        public string Label;

        /// <summary>
        /// Friendly label ids for the output, replacing the % in the label. Required.
        /// </summary>
        public string[] LabelIds;

        /// <summary>
        /// Associated layer for the output. Optional.
        /// </summary>
        public string Layer;

        /// <summary>
        /// The midi message type. Supported values: Note, CC. Required.
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

        /// <summary>
        /// Midi message value for turning on the LED. Required.
        /// </summary>
        public byte ValueOn;

        /// <summary>
        /// Midi message value for putting LED to blink mode. Optional.
        /// </summary>
        public byte? ValueBlinkOn;

        /// <summary>
        /// Midi message value for turning off the LED. Required.
        /// </summary>
        public byte ValueOff;

        /// <summary>
        /// Label of related input. When related input is triggered, output is auto refreshed. Optional.
        /// </summary>
        public string RelatedInputLabel;

        /// <summary>
        /// Label ids of related input, replacing the % in the related input label. Optional.
        /// </summary>
        public string[] RelatedIds;

        public string GetLabelWithIndex(int index)
        {
            return Label.Replace("%", LabelIds[index]);
        }
    }
}
