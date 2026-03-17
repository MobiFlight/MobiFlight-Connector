using Newtonsoft.Json;
using System.Collections.Generic;

namespace MobiFlight
{
    public class MidiBoardDefinition : IMigrateable
    {
        /// <summary>
        /// Instance name for the device. Required. This is used to match the definition with a connected device.
        /// </summary>
        public string InstanceName;

        /// <summary>
        /// Name of the midi output port, if different from input name. Optional.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DifferingOutputName;

        /// <summary>
        /// Set the encoder neutral position. For example 0 or 64 Optional.
        /// </summary>
        public int EncoderNeutralPosition = 0;

        /// <summary>
        /// When layer property is used, initial active layer on startup. Optional.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InitialLayer;

        /// <summary>
        /// List of inputs supported by the device. Required.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MidiInputDefinition> Inputs;

        /// <summary>
        /// List of LED outputs supported by the device. Optional.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] 
        public List<MidiOutputDefinition> Outputs;

        private Dictionary<string, string> inputNameToLabelDictionary;

        [JsonIgnore]
        public Dictionary<string, string> InputNameToLabelDictionary
        {
            get
            {
                if (inputNameToLabelDictionary == null) CreateInputNameToLabelDictionary();
                return inputNameToLabelDictionary;
            }
        }

        private void CreateInputNameToLabelDictionary()
        {
            inputNameToLabelDictionary = new Dictionary<string, string>();
            foreach (var inputDef in Inputs)
            {
                for (int i = 0; i < inputDef.MessageIds.Count; i++)
                {
                    string name = inputDef.GetNameWithIndex(i);
                    string label = inputDef.GetLabelWithIndex(i);
                    inputNameToLabelDictionary[name] = label;
                }
            }
        }

        public string MapDeviceNameToLabel(string deviceName)
        {
            if (InputNameToLabelDictionary.TryGetValue(deviceName, out string label))
            {
                return label;
            }

            return deviceName; // Return the device name if no label is found
        }

        // Nothing to migrate currently but the method implementation is required
        // when using JsonBoardObject to load definitions from JSON.
        public void Migrate() { }
    }
}
