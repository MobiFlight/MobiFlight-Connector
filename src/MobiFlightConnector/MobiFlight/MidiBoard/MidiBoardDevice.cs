using MobiFlight.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MobiFlight
{
    public class MidiBoardDevice : DeviceReference
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Layer;

        public List<MidiBoardOutputDevice> RelatedOutputDevices = new List<MidiBoardOutputDevice>();
    }

    public class MidiBoardOutputDevice : MidiBoardDevice
    {        
        public MidiMessageType MessageType;
        public byte Channel;
        public byte Id;
        public byte ValueOn;
        public byte ValueOff;
        public byte State = 0;
        public bool IsActive = false;
        public MidiBoardOutputDevice()
        {
            Type = DeviceType.LedModule;
        }
    }
}