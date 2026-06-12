using MobiFlight.Base;
using MobiFlight.Firmware;
using MobiFlight.InputConfig;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using Button = MobiFlight.InputConfig.Button;
using Encoder = MobiFlight.InputConfig.Encoder;
using AnalogInput = MobiFlight.InputConfig.AnalogInput;

namespace MobiFlight
{
    public class InputConfigItem : ConfigItem, IXmlSerializable, ICloneable, IConfigRefConfigItem
    {
        // we initialize a cultureInfo object 
        // which is used for serialization
        // independently from current cultureInfo
        // @see: https://forge.simple-solutions.de/issues/275
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");

        public const String TYPE_NOTSET = "-";
        public const String TYPE_BUTTON = MobiFlightButton.TYPE;
        public const String TYPE_ENCODER = MobiFlightEncoder.TYPE;
        public const String DEPRECATED_TYPE_INPUT_SHIFT_REGISTER = MobiFlightInputShiftRegister.TYPE;
        public const String DEPRECATED_TYPE_INPUT_MULTIPLEXER = MobiFlightInputMultiplexer.TYPE;
        public const String TYPE_ANALOG = MobiFlightAnalogInput.TYPE;
        // only for backward compatibility during loading
        public const String TYPE_ANALOG_OLD = "Analog";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ButtonInputConfig button { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EncoderInputConfig encoder { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AnalogInputConfig analog { get; set; }

        public override IDeviceConfig Device { get; set; }

        public InputConfigItem()
        {
            Preconditions = new PreconditionList();
            ConfigRefs = new ConfigRefList();
        }

        public List<InputAction> GetInputActionsByType(System.Type type)
        {
            List<InputAction> result = new List<InputAction>();
            if (button != null)
            {
                result.AddRange(button.GetInputActionsByType(type));
            }

            if (encoder != null)
            {
                result.AddRange(encoder.GetInputActionsByType(type));
            }

            if (analog != null)
            {
                result.AddRange(analog.GetInputActionsByType(type));
            }
            return result;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            Controller = SerialNumber.CreateController(reader["serial"]);
            // This name is only present with input devices
            // and it is in the wrong place.
            var DeviceName = reader["name"];
            var DeviceType = TYPE_NOTSET;
            var ExtPin = -1;

            if (reader["type"] != null && reader["type"] != "")
            {
                DeviceType = reader["type"];
                if (DeviceType == TYPE_ANALOG_OLD) DeviceType = TYPE_ANALOG;
            }

            reader.Read(); // this should be the button or encoder

            if (reader.LocalName == "button")
            {
                button = new ButtonInputConfig();
                button.ReadXml(reader);
            }

            if (reader.LocalName == "encoder")
            {
                encoder = new EncoderInputConfig();
                encoder.ReadXml(reader);
            }

            if (reader.LocalName == "inputShiftRegister")
            {
                button = new ButtonInputConfig();
                // this is for backwards compatibility
                // the name of the attribute changed over time, but there were only one
                // so we will always try to read the first attribute, if it is present
                ExtPin = 0;
                var extPinAttribute = reader.GetAttribute(0);
                if (!string.IsNullOrEmpty(extPinAttribute) &&
                    int.TryParse(extPinAttribute, out var parsedExtPin))
                {
                    ExtPin = parsedExtPin;
                }
                button.ReadXml(reader);

            }

            if (reader.LocalName == "inputMultiplexer")
            {
                button = new ButtonInputConfig();
                // this is for backwards compatibility
                // the name of the attribute changed over time, but there were only one
                // so we will always try to read the first attribute, if it is present
                ExtPin = 0;
                var extPinAttribute = reader.GetAttribute(0);
                if (!string.IsNullOrEmpty(extPinAttribute) &&
                    int.TryParse(extPinAttribute, out var parsedExtPin))
                {
                    ExtPin = parsedExtPin;
                }
                button.ReadXml(reader);

            }

            if (reader.LocalName == "analog")
            {
                analog = new AnalogInputConfig();
                analog.ReadXml(reader);
            }

            // this is fallback, because type was not set in the past
            if (DeviceType == TYPE_NOTSET)
            {
                if (button != null)
                    DeviceType = TYPE_BUTTON;
                if (encoder != null)
                    DeviceType = TYPE_ENCODER;
            }

            // initialize the correct device config
            Device = CreateInputDevice(DeviceType, DeviceName, ExtPin);

            /*
            if (reader.LocalName != "preconditions")            
                reader.Read(); // this should be the preconditions tag
            if (reader.LocalName != "preconditions")
                reader.Read(); // this should be the preconditions tag
            */
            if (reader.LocalName == "preconditions")
            {
                bool atPosition = false;
                // read precondition settings if present
                if (reader.ReadToDescendant("precondition"))
                {
                    // load a list
                    do
                    {
                        Precondition tmp = new Precondition();
                        tmp.ReadXml(reader);
                        Preconditions.Add(tmp);
                    } while (reader.LocalName == "precondition");
                }
                if (reader.NodeType != XmlNodeType.EndElement)
                    reader.Read(); // this should be the corresponding "end" node

                if (reader.NodeType == XmlNodeType.EndElement)
                    reader.Read(); // move on to the next node
            }

            if (reader.LocalName == "configrefs")
            {
                bool atPosition = false;
                // read precondition settings if present
                if (reader.ReadToDescendant("configref"))
                {
                    // load a list
                    do
                    {
                        ConfigRef tmp = new ConfigRef();
                        tmp.ReadXml(reader);
                        ConfigRefs.Add(tmp);
                    } while (reader.LocalName == "configref");
                }

                reader.Read(); // advance to the next
            }
        }

        public static IDeviceConfig CreateInputDevice(string DeviceType, string DeviceName, int SubIndex = 0)
        {
            IDeviceConfig result = null;

            switch (DeviceType)
            {
                case TYPE_BUTTON:
                    result = new Button() { Name = DeviceName };
                    break;

                case TYPE_ENCODER:
                    result = new Encoder() { Name = DeviceName };
                    break;

                case TYPE_ANALOG:
                    result = new AnalogInput() { Name = DeviceName };
                    break;

                case DEPRECATED_TYPE_INPUT_SHIFT_REGISTER:
                    result = new Button() { Name = $"{DeviceName}:{SubIndex}" };
                    break;

                case DEPRECATED_TYPE_INPUT_MULTIPLEXER:
                    result = new Button() { Name = $"{DeviceName}:{SubIndex}" };
                    break;
            }

            return result;
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            WriteXml(writer, true);
        }

        public virtual void WriteXml(XmlWriter writer, bool writeInstanceData)
        {
            if (writeInstanceData)
            {
                writer.WriteAttributeString("msdata:InstanceType", $"MobiFlight.InputConfigItem, MFConnector, Version={Assembly.GetExecutingAssembly().GetName().Version}, Culture=neutral, PublicKeyToken=null");
                writer.WriteAttributeString("xmlns:msdata", "urn:schemas-microsoft-com:xml-msdata");
            }

            // the empty string is inconsistent with OutputConfigItem (NOT_SET),
            // but it is they way it was done in old config files
            var fullSerial = SerialNumber.BuildFullSerial(Controller) ?? "";

            writer.WriteAttributeString("serial", fullSerial);
            writer.WriteAttributeString("name", this.Device?.Name);
            writer.WriteAttributeString("type", this.Device?.Type ?? TYPE_NOTSET);

            if (this.Device?.Type == TYPE_BUTTON && button != null)
            {
                writer.WriteStartElement("button");
                button.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (this.Device?.Type == TYPE_ENCODER && encoder != null)
            {
                writer.WriteStartElement("encoder");
                encoder.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (this.Device?.Type == DEPRECATED_TYPE_INPUT_SHIFT_REGISTER && button != null)
            {
                writer.WriteStartElement("button");
                button.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (this.Device?.Type == DEPRECATED_TYPE_INPUT_MULTIPLEXER && button != null)
            {
                writer.WriteStartElement("button");
                button.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (this.Device?.Type == TYPE_ANALOG && analog != null)
            {
                writer.WriteStartElement("analog");
                analog.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("preconditions");
            foreach (Precondition p in Preconditions)
            {
                p.WriteXml(writer);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("configrefs");
            foreach (ConfigRef p in ConfigRefs)
            {
                p.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        public InputConfigItem(InputConfigItem config) : base(config)
        {
            this.button = (ButtonInputConfig)config.button?.Clone();
            this.encoder = (EncoderInputConfig)config.encoder?.Clone();
            this.analog = (AnalogInputConfig)config.analog?.Clone();
            this.Device = config.Device?.Clone() as IDeviceConfig;
        }

        public override object Clone()
        {
            return (object)new InputConfigItem(this);
        }

        public override Base.IConfigItem Duplicate()
        {
            return new InputConfigItem(this) { GUID = System.Guid.NewGuid().ToString() };
        }

        internal void execute(
            CacheCollection cacheCollection,
            InputEventArgs e,
            List<ConfigRefValue> configRefs)
        {
            switch (Device.Type)
            {
                case DEPRECATED_TYPE_INPUT_SHIFT_REGISTER:
                case DEPRECATED_TYPE_INPUT_MULTIPLEXER:
                case TYPE_BUTTON:
                    if (button != null)
                        button.execute(cacheCollection, e, configRefs);
                    break;
                case TYPE_ENCODER:
                    if (encoder != null)
                        encoder.execute(cacheCollection, e, configRefs);
                    break;

                case TYPE_ANALOG:
                    if (analog != null)
                        analog.execute(cacheCollection, e, configRefs);
                    break;
            }
        }
        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            if (Device.Type == TYPE_BUTTON)
            {
                // explicit test is needed 
                // in some older version we didn't save the node correctly
                if (button != null)
                    result = button?.GetStatistics();

            }
            else if (Device.Type == TYPE_ENCODER)
            {
                // explicit test is needed 
                // in some older version we didn't save the node correctly
                if (encoder != null)
                    result = encoder.GetStatistics();
            }
            else if (Device.Type == TYPE_ANALOG)
            {
                // explicit test is needed 
                // in some older version we didn't save the node correctly
                if (analog != null)
                    result = analog.GetStatistics();
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is InputConfigItem item)) return false;
            if (!base.Equals(obj)) return false;

            return Controller.AreEqual(item.Controller) &&
                    Device.AreEqual(item.Device) &&
                    button.AreEqual(item.button) &&
                    encoder.AreEqual(item.encoder) &&
                    analog.AreEqual(item.analog) &&
                    Preconditions.Equals(item.Preconditions) &&
                    ConfigRefs.Equals(item.ConfigRefs);
        }

        protected override IDeviceConfig GetDeviceConfig()
        {
            return Device;
        }
    }
}