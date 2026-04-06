using System.Collections.Generic;
using System.Xml.Serialization;

namespace MobiFlight.Base
{
    [XmlRoot("MobiflightConnector")]
    public class FsuipcPresetConfig
    {
        [XmlElement("config")]
        public List<OutputConfigFileXmlElement> outputConfigs;
    }
}
