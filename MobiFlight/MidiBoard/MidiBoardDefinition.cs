using System.Collections.Generic;

namespace MobiFlight
{
    public class MidiBoardDefinition
    {
        // From deserialization of config
        public string InstanceName;

        // From deserialization of config
        public string DifferingOutputName;

        // From deserialization of config
        public int EncoderNeutral = 0;

        // From deserialization of config
        public string InitialLayer;

        // From deserialization of config
        public List<MidiInputDefinition> Inputs;

        // From deserialization of config
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
