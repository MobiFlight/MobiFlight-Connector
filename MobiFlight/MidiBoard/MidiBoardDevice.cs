using MobiFlight.Config;
using System;
using System.Collections.Generic;

namespace MobiFlight
{
    public class MidiBoardDevice : IBaseDevice
    {
        public DeviceType Type { get; set; }
        public String Name { get; set; }
        public String Label { get; set; }
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
