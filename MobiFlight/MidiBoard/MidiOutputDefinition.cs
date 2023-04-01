using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight
{
    public class MidiOutputDefinition
    {
        public string Label;
    
        public string[] LabelIds;

        public string Layer;

        [JsonConverter(typeof(StringEnumConverter))]
        public MidiMessageType MessageType;

        public byte MessageChannel;

        public byte[] MessageIds;

        public byte ValueOn;

        public byte? ValueBlinkOn;

        public byte ValueOff;

        public string RelatedInputLabel;

        public string[] RelatedIds;

        public string GetLabelWithIndex(int index)
        {
            return Label.Replace("%", LabelIds[index]);
        }
    }
}
