using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SimpleSolutions.Usb;

namespace MobiFlight
{
    public class ArcazeModuleSettings : IXmlSerializable
    {
        string _serial;
        public string serial
        {
            get { return _serial; }
            set
            {
                if (_serial == value) return;
                _serial = value;
                _hasChanged = true;
            }
        }
        ArcazeCommand.ExtModuleType _type;
        public ArcazeCommand.ExtModuleType type
        {
            get { return _type; }
            set
            {
                if (_type == value) return;
                _type = value;
                _hasChanged = true;
            }
        }

        byte _numModules;
        public byte numModules
        {
            get { return _numModules; }
            set
            {
                if (_numModules == value) return;
                _numModules = value;
                _hasChanged = true;
            }
        }
        byte _globalBrightness;
        public byte globalBrightness
        {
            get { return _globalBrightness; }
            set
            {
                if (_globalBrightness == value) return;
                _globalBrightness = value;
                _hasChanged = true;
            }
        }
        private bool _hasChanged = false;
        public bool HasChanged { get {return _hasChanged;} }

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
            _hasChanged = false;
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
    }
}
