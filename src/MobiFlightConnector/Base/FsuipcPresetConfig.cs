using System.Collections.Generic;
using System.Xml.Serialization;

namespace MobiFlight.Base
{
    [XmlRoot("MobiflightConnector")]
    internal class FsuipcPresetConfig
    {
        [XmlElement("config")]
        public List<OutputConfigFileXmlElement> outputConfigs;
    }
}
