using System.Collections.Generic;
using System.Xml.Serialization;

namespace MobiFlight.Base
{
    [XmlRoot("config")]
    public class ConfigFileXmlItem
    {

        [XmlAttribute("guid")]
        public string guid { get; set; }

        [XmlElement]
        public bool active { get; set; }

        [XmlElement]
        public string description { get; set; }
    }

    public class OutputConfigFileXmlElement : ConfigFileXmlItem
    {
        [XmlElement]
        public OutputConfigItem settings { get; set; }
    }

    public class InputConfigFileXmlElement : ConfigFileXmlItem
    {
        [XmlElement]
        public InputConfigItem settings { get; set; }
    }


    [XmlRoot("MobiflightConnector")]
    public class ConfigFileWrapperXML
    {
        [XmlArray("outputs")]
        [XmlArrayItem("config")]
        public List<OutputConfigFileXmlElement> outputConfigs;

        [XmlArray("inputs")]
        [XmlArrayItem("config")]
        public List<InputConfigFileXmlElement> inputConfigs;
    }
}
