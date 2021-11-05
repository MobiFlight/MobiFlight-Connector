using System;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class MobiFlightPin
    {
        [XmlAttribute]
        public byte Pin { get; set; }
        [XmlAttribute]
        public bool isAnalog = false;
        [XmlAttribute]
        public bool isPWM = false;
        [XmlAttribute]
        public bool isI2C = false;
        // This is internal state and shouldn't be serialized to/from the .board.xml files
        public bool Used = false;

        private string name = null;

        [XmlAttribute]
        public string Name
        {
            get { return name != null ? name : Pin.ToString(); }
            set { name = value; }
        }

        public MobiFlightPin()
        {
        }

        public MobiFlightPin(MobiFlightPin pin)
        {
            Pin = pin.Pin;
            isAnalog = pin.isAnalog;
            isPWM = pin.isPWM;
            isI2C = pin.isI2C;
            Used = pin.Used;
            Name = pin.Name;
        }

        public bool ShouldSerializeName()
        {
            return !string.IsNullOrEmpty(name);
        }

        public override String ToString()
        {
            return Pin.ToString();
        }
    }
}
