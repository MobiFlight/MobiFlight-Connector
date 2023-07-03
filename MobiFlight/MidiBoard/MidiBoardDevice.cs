using System;
using System.Collections.Generic;

namespace MobiFlight
{
    public class MidiBoardDevice
    {
        public String Name;
        public String Label;
        public String Layer;
        public DeviceType Type;
        public List<MidiBoardOutputDevice> RelatedOutputDevices = new List<MidiBoardOutputDevice>();

        // Used to find MidiBoardDevice for corresponding config
        public override string ToString()
        {
            return Name;
        }
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
