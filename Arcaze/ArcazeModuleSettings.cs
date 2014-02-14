using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SimpleSolutions.Usb;

namespace ArcazeUSB
{
    public class ArcazeModuleSettings : IXmlSerializable
    {
        public string serial { get; set; }
        public ArcazeCommand.ExtModuleType type { get; set; }
        public byte numModules { get; set; }
        public byte globalBrightness { get; set; }

        public ArcazeModuleSettings()
        {
            globalBrightness = Properties.Settings.Default.LedIntensity;
            numModules = 1;
            //type = ArcazeCommand.ExtModuleType.DisplayDriver;
            type = ArcazeCommand.ExtModuleType.LedDriver3;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            serial = reader["serial"];
            type = stringToExtModuleType(reader["type"]);
            numModules = byte.Parse(reader["numModules"]);
            globalBrightness = byte.Parse(reader["brightness"]);
            reader.Read();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("serial", serial);
            writer.WriteAttributeString("type", type.ToString());
            writer.WriteAttributeString("numModules", numModules.ToString());
            writer.WriteAttributeString("brightness", globalBrightness.ToString());
        }

        public ArcazeCommand.ExtModuleType stringToExtModuleType(string value)
        {            
            if (ArcazeCommand.ExtModuleType.DisplayDriver.ToString() == value) {
                return ArcazeCommand.ExtModuleType.DisplayDriver;
            }

            if (ArcazeCommand.ExtModuleType.LedDriver2.ToString() == value)
            {
                return ArcazeCommand.ExtModuleType.LedDriver2;
            }

            if (ArcazeCommand.ExtModuleType.LedDriver3.ToString() == value)
            {
                return ArcazeCommand.ExtModuleType.LedDriver3;
            }

            return ArcazeCommand.ExtModuleType.InternalIo;
        }

        public static string ExtractSerial(String s)
        {
            if (!s.Contains("/")) return "";

            return s.Split('/')[1].Trim();
        }
    }
}
