using MobiFlight.Base;
using MobiFlight.Config;
using MobiFlight.InputConfig;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Newtonsoft.Json;

namespace MobiFlight
{
    public class InputConfigItem : ConfigItem, IXmlSerializable, ICloneable, IConfigRefConfigItem
    {
        public const string TYPE_NOTSET = "-";
        // we initialize a cultureInfo object 
        // which is used for serialization
        // independently from current cultureInfo
        // @see: https://forge.simple-solutions.de/issues/275
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");
                
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ButtonInputConfig button { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EncoderInputConfig encoder { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InputShiftRegisterConfig inputShiftRegister { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InputMultiplexerConfig inputMultiplexer { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AnalogInputConfig analog { get; set; }
        public string DeviceName { get; set; }

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

            if (inputShiftRegister != null)
            {
                result.AddRange(inputShiftRegister.GetInputActionsByType(type));
            }

            if (inputMultiplexer != null) {
                result.AddRange(inputMultiplexer.GetInputActionsByType(type));
            }
            return result;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            ModuleSerial = reader["serial"];
            
            if (reader["type"] != null && reader["type"] != "")
            {
                Device = InputDeviceConfigFactory.CreateFromType(reader["type"]);
                Device.ReadXml(reader);
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
                inputShiftRegister = new InputShiftRegisterConfig();
                inputShiftRegister.ReadXml(reader);
            }

            if (reader.LocalName == "inputMultiplexer")
            {
                inputMultiplexer = new InputMultiplexerConfig();
                inputMultiplexer.ReadXml(reader);
            }

            if (reader.LocalName == "analog")
            {
                analog = new AnalogInputConfig();
                analog.ReadXml(reader);
            }

            // this is fallback, because type was not set in the past
            if (Device == null)
            {
                if (button != null)
                {
                    Device = InputDeviceConfigFactory.CreateFromType(MobiFlightButton.TYPE);
                }

                if (encoder != null)
                {
                    Device = InputDeviceConfigFactory.CreateFromType(MobiFlightEncoder.TYPE);
                }
            }

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

            writer.WriteAttributeString("serial", this.ModuleSerial);

            if (this.Device != null) { 
                writer.WriteAttributeString("name", this.Device.Name);
                writer.WriteAttributeString("type", this.Device.OldType);
                if (this.Device.OldType == MobiFlightButton.TYPE && button != null)
                {
                    writer.WriteStartElement("button");
                    button.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (this.Device.OldType == MobiFlightEncoder.TYPE && encoder != null)
                {
                    writer.WriteStartElement("encoder");
                    encoder.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (this.Device.OldType == MobiFlightInputShiftRegister.TYPE && inputShiftRegister != null)
                {
                    writer.WriteStartElement("inputShiftRegister");
                    inputShiftRegister.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (this.Device.OldType == MobiFlightInputMultiplexer.TYPE && inputMultiplexer != null)
                {
                    writer.WriteStartElement("inputMultiplexer");
                    inputMultiplexer.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (this.Device.OldType == MobiFlightAnalogInput.TYPE && analog != null)
                {
                    writer.WriteStartElement("analog");
                    analog.WriteXml(writer);
                    writer.WriteEndElement();
                }
            } else
            {
                writer.WriteAttributeString("type", InputConfigItem.TYPE_NOTSET);
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
            this.inputShiftRegister = (InputShiftRegisterConfig)config.inputShiftRegister?.Clone();
            this.inputMultiplexer = (InputMultiplexerConfig)config.inputMultiplexer?.Clone();
            this.analog = (AnalogInputConfig)config.analog?.Clone();
            this.DeviceName = config.DeviceName?.Clone() as string;
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
            switch (Device?.OldType)
            {
                case MobiFlightButton.TYPE:
                    if (button != null)
                        button.execute(cacheCollection, e, configRefs);
                    break;
                case MobiFlightEncoder.TYPE:
                    if (encoder != null)
                        encoder.execute(cacheCollection, e, configRefs);
                    break;

                case MobiFlightInputShiftRegister.TYPE:
                    if (inputShiftRegister != null)
                        inputShiftRegister.execute(cacheCollection, e, configRefs);
                    break;

                case MobiFlightInputMultiplexer.TYPE:
                    if (inputMultiplexer != null)
                        inputMultiplexer.execute(cacheCollection, e, configRefs);
                    break;

                case MobiFlightAnalogInput.TYPE:
                    if (analog != null)
                        analog.execute(cacheCollection, e, configRefs);
                    break;
            }
        }

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            switch (Device?.OldType)
            {
                case MobiFlightButton.TYPE:
                    if (button != null)
                        result = button.GetStatistics();
                    break;
                case MobiFlightEncoder.TYPE:
                    if (encoder != null)
                        result = encoder.GetStatistics();
                    break;
                case MobiFlightInputShiftRegister.TYPE:
                    if (inputShiftRegister != null)
                        result = inputShiftRegister.GetStatistics();
                    break;
                case MobiFlightInputMultiplexer.TYPE:
                    if (inputMultiplexer != null)
                        result = inputMultiplexer.GetStatistics();
                    break;
                case MobiFlightAnalogInput.TYPE:
                    if (analog != null)
                        result = analog.GetStatistics();
                    break;
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is InputConfigItem item)) return false;
            if (!base.Equals(obj)) return false;

            return  Device.AreEqual(item.Device) &&
                    button.AreEqual(item.button) &&
                    encoder.AreEqual(item.encoder) &&
                    analog.AreEqual(item.analog) &&
                    inputShiftRegister.AreEqual(item.inputShiftRegister) &&
                    inputMultiplexer.AreEqual(item.inputMultiplexer) &&
                    Preconditions.Equals(item.Preconditions) &&
                    ConfigRefs.Equals(item.ConfigRefs);
        }

        protected override IDeviceConfig GetDeviceConfig()
        { 
            return _deviceConfig;
        }
    }
}
