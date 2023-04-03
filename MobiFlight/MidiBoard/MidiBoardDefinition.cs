using System.Collections.Generic;

namespace MobiFlight
{
    public class MidiBoardDefinition
    {
        /// <summary>
        /// Instance name for the device. Required. This is used to match the definition with a connected device.
        /// </summary>
        public string InstanceName;

        /// <summary>
        /// Name of the midi output port, if different from input name. Optional.
        /// </summary>
        public string DifferingOutputName;

        /// <summary>
        /// Set the encoder neutral position. For example 0 or 64 Optional.
        /// </summary>
        public int EncoderNeutralPosition = 0;

        /// <summary>
        /// When layer property is used, initial active layer on startup. Optional.
        /// </summary>
        public string InitialLayer;

        /// <summary>
        /// List of inputs supported by the device. Required.
        /// </summary>
        public List<MidiInputDefinition> Inputs;

        /// <summary>
        /// List of LED outputs supported by the device. Optional.
        /// </summary>
        public List<MidiOutputDefinition> Outputs;

        private Dictionary<string, string> inputNameToLabelDictionary;

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
                for (int i = 0; i < inputDef.MessageIds.Length; i++)
                {
                    string name = inputDef.GetNameWithIndex(i);
                    string label = inputDef.GetLabelWithIndex(i);
                    inputNameToLabelDictionary[name] = label;
                }
            }
        }
    }
}
