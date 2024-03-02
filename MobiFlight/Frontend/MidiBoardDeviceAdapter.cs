using System.Collections.Generic;

namespace MobiFlight.Frontend
{
    internal class MidiBoardDeviceAdapter : IDeviceItem
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public IDeviceElement[] Elements { get; set; }

        public MobiFlightPin[] Pins { get; set; }

        public MidiBoardDeviceAdapter(MidiBoard device)
        {

            Type = "Midi";
            Id = device.Serial;
            Name = device.Name;
            MetaData = new Dictionary<string, string>(){
                { "HasMidiOutput", device.HasMidiOutput.ToString() },
            };
            //Elements = device.GetConnectedDevices().Select(d => new DeviceElement() { Id = d.Name, Name = d.Name, Type = d.Type }).ToArray();
        }
    }
}