using MobiFlight.Base;
using MobiFlight.InputConfig;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class OutputConfigItem : ConfigItem, IXmlSerializable, ICloneable, Config.IConfigRefConfigItem
    {
        // we initialize a cultureInfo object 
        // which is used for serialization
        // independently from current cultureInfo
        // @see: https://forge.simple-solutions.de/issues/275
        [JsonIgnore]
        public System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");

        public Source Source { get; set; }
        public ConnectorValue       TestValue                   { get; set; }

        public override IDeviceConfig Device { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InputConfig.ButtonInputConfig ButtonInputConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InputConfig.AnalogInputConfig AnalogInputConfig { get; set; }

        public string DeviceType { get; set; }
        public string DeviceName {  get { return Device?.Name;  } }
        public OutputConfigItem()
        {
            Source = new SimConnectSource();
            TestValue = new ConnectorValue() { type = FSUIPCOffsetType.Float, Float64 = 1 };
            Modifiers = new ModifierList();
            Device = null;
            Preconditions = new PreconditionList();
            ConfigRefs = new ConfigRefList();
            ButtonInputConfig = null;
            AnalogInputConfig = null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is OutputConfigItem item)) return false;
            if (!base.Equals(obj)) return false;

            return (
                DeviceName == item.DeviceName &&
                DeviceType == item.DeviceType &&
                Source.AreEqual(item.Source) &&
                TestValue.AreEqual(item.TestValue) &&
                Device.AreEqual(item.Device) &&
                ButtonInputConfig.AreEqual(item.ButtonInputConfig) &&
                AnalogInputConfig.AreEqual(item.AnalogInputConfig)
            );
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            if (reader.ReadToDescendant("source"))
            {
                // try to read it as FSUIPC Offset
                if (reader["type"] == "SimConnect") {
                    Source = new SimConnectSource();
                    (Source as SimConnectSource).SimConnectValue.ReadXml(reader);
                } else if (reader["type"] == "Variable")
                {
                    Source = new VariableSource();
                    (Source as VariableSource).MobiFlightVariable.ReadXml(reader);
                } else if (reader["type"] == "XplaneDataRef")
                {
                    Source = new XplaneSource();
                    (Source as XplaneSource).XplaneDataRef.ReadXml(reader);
                }
                else
                {
                    Source = new FsuipcSource();
                    (Source as FsuipcSource).FSUIPC.ReadXml(reader);
                    
                    if((Source as FsuipcSource).FSUIPC.OffsetType == FSUIPCOffsetType.String)
                    {
                        // this is a special case for backward compatibility
                        // https://github.com/MobiFlight/MobiFlight-Connector/issues/1348
                        Modifiers = new FsuipcStringModifierList();
                    }
                }

                // backward compatibility
                if (reader["multiplier"] != null)
                {
                    double multiplier = Double.Parse(reader["multiplier"], serializationCulture);
                    if (multiplier != 1.0)
                    {
                        Modifiers.Transformation.Active = true;
                        // we have to replace the decimal in case "," is used (german style)
                        Modifiers.Transformation.Expression = "$*" + multiplier.ToString().Replace(',', '.');
                    }
                }
                reader.Read();
            }

            if (reader.LocalName == "test")
            {
                if (reader["type"] != null)
                {
                    if (reader["type"] == FSUIPCOffsetType.String.ToString())
                    {
                        TestValue.type = FSUIPCOffsetType.String;
                        TestValue.String = reader["value"];
                    }
                    else
                    {
                        if (!Double.TryParse(reader["value"], out TestValue.Float64))
                        {
                            Log.Instance.log("Error reading config.", LogSeverity.Error);
                        };
                    }
                }
                reader.Read();
            }

            if (reader.LocalName == "modifiers")
            {
                Modifiers.ReadXml(reader);
            } else if (reader.LocalName == "comparison")
            {
                // backward compatibility when we have comparison
                // as a single node instead of modifiers
                Modifiers.Comparison.ReadXml(reader);
                reader.Read();
            }

            if (reader.LocalName == "display")
            {
                DeviceType = reader["type"];

                // preserve backward compatibility
                if (DeviceType == "Pin") DeviceType = MobiFlightOutput.TYPE;
                if (DeviceType == ArcazeLedDigit.OLDTYPE) DeviceType = ArcazeLedDigit.TYPE;

                ModuleSerial = reader["serial"];
                
                if (DeviceType == MobiFlightOutput.TYPE)
                {
                    Device = new OutputConfig.Output();
                    (Device as OutputConfig.Output).ReadXml(reader);
                }
                else if (DeviceType == MobiFlightLedModule.TYPE)
                {
                    Device = new OutputConfig.LedModule();
                    (Device as OutputConfig.LedModule).XmlRead(reader);
                }
                else if (DeviceType == MobiFlightServo.TYPE)
                {
                    Device = new OutputConfig.Servo();
                    (Device as Servo).ReadXml(reader);
                }
                else if (DeviceType == MobiFlightStepper.TYPE)
                {
                    Device = new OutputConfig.Stepper();
                    (Device as Stepper).ReadXml(reader);
                }
                else if (DeviceType == OutputConfig.LcdDisplay.DeprecatedType)
                {
                    Device = new OutputConfig.LcdDisplay();
                    (Device as LcdDisplay).ReadXml(reader);
                }
                else if (DeviceType == MobiFlightShiftRegister.TYPE)
                {
                    Device = new OutputConfig.ShiftRegister();
                    (Device as ShiftRegister).ReadXml(reader);
                }
                else if (DeviceType == MobiFlightCustomDevice.TYPE)
                {
                    Device = new OutputConfig.CustomDevice();
                    (Device as CustomDevice).ReadXml(reader);
                }
                else if (DeviceType == "InputAction")
                {
                    reader.Read();

                    if (reader.Name == "button")
                    {
                        ButtonInputConfig = new ButtonInputConfig();
                        ButtonInputConfig.ReadXml(reader);
                    }
                    else if (reader.Name == "analog")
                    {
                        AnalogInputConfig = new AnalogInputConfig();
                        AnalogInputConfig.ReadXml(reader);
                    }

                    // read to the end of the InputAction
                    reader.Read();
                }

                // Actually interpolation is in he wrong spot. :(
                // it should not be nested
                // it should be outside of the display node
                if (reader.LocalName == "interpolation")
                {
                    var interpolation = new Interpolation();
                    interpolation.ReadXml(reader);
                    Modifiers.Items.Add(interpolation);

                    if (reader.LocalName == "interpolation")
                        reader.Read();

                    if (reader.LocalName == "display")
                        reader.ReadEndElement(); // this closes the display node
                }

                if (reader.LocalName == "display" && reader.NodeType == XmlNodeType.Element)
                    reader.Read();

                // forward
                if (reader.LocalName == "display" && reader.NodeType == XmlNodeType.EndElement)
                    reader.ReadEndElement();
            }

            // Actually interpolation is in he wrong spot. :(
            // it should not be nested
            // it should be outside of the display node
            if (reader.LocalName == "interpolation")
            {
                var interpolation = new Interpolation();
                interpolation.ReadXml(reader);
                Modifiers.Items.Add(interpolation);

                if (reader.LocalName == "interpolation")
                    reader.Read();

                if (reader.LocalName == "display" && reader.NodeType == XmlNodeType.EndElement)
                    reader.Read(); // this closes the display node
            }

            // read precondition settings if present
            if (reader.LocalName == "precondition" || reader.LocalName == "preconditions")
            {
                Preconditions.ReadXml(reader);
            }

            if (reader.LocalName == "transformation")
            {
                // Transform.ReadXml(reader);
                var transform = new Transformation();
                transform.ReadXml(reader);
                Modifiers.Items.Insert(0, transform);
                reader.Read();
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
            }

            // read to the end of preconditions-node
            if (reader.LocalName == "configrefs" && reader.NodeType == XmlNodeType.EndElement)
                reader.ReadEndElement();

        }

        public virtual void WriteXml(XmlWriter writer)
        {
            WriteXml(writer, true);
        }

        public void WriteXml(XmlWriter writer, bool writeInstanceData)
        {
            if (writeInstanceData)
            {
                writer.WriteAttributeString("msdata:InstanceType", $"MobiFlight.OutputConfigItem, MFConnector, Version={Assembly.GetExecutingAssembly().GetName().Version}, Culture=neutral, PublicKeyToken=null");
                writer.WriteAttributeString("xmlns:msdata", "urn:schemas-microsoft-com:xml-msdata");
            }

            writer.WriteStartElement("source");
                if (Source is FsuipcSource)
                    (this.Source as FsuipcSource).FSUIPC.WriteXml(writer);
                else if (Source is VariableSource)
                    (this.Source as VariableSource).MobiFlightVariable.WriteXml(writer);
                else if (Source is XplaneSource)
                    (this.Source as XplaneSource).XplaneDataRef.WriteXml(writer);
                else
                    (this.Source as SimConnectSource).SimConnectValue.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("test");
                writer.WriteAttributeString("type", TestValue.type.ToString());
                writer.WriteAttributeString("value", TestValue.ToString());
            writer.WriteEndElement();

            Modifiers.WriteXml(writer);

            writer.WriteStartElement("display");
                writer.WriteAttributeString("type", DeviceType);
                writer.WriteAttributeString("serial", ModuleSerial);

            if (Device is LedModule)
            {
                (Device as LedModule).WriteXml(writer);
            }
            else if (DeviceType == MobiFlightServo.TYPE)
            {
                (Device as Servo).WriteXml(writer);
            }
            else if (DeviceType == MobiFlightStepper.TYPE)
            {
                (Device as Stepper).WriteXml(writer);
            }
            else if (DeviceType == MobiFlightLcdDisplay.TYPE)
            {
                (Device as LcdDisplay).WriteXml(writer);
            }
            else if (DeviceType == MobiFlightShiftRegister.TYPE)
            {
                (Device as ShiftRegister).WriteXml(writer);
            }
            else if (DeviceType == MobiFlightCustomDevice.TYPE)
            {
                (Device as CustomDevice).WriteXml(writer);
            }
            else if (DeviceType == "InputAction")
            {
                if (ButtonInputConfig != null)
                {
                    writer.WriteStartElement("button");
                    ButtonInputConfig.WriteXml(writer);
                    writer.WriteEndElement();
                }
                else if (AnalogInputConfig != null)
                {
                    writer.WriteStartElement("analog");
                    AnalogInputConfig.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
            else if (DeviceType == MobiFlightOutput.TYPE)
            {
                (Device as Output).WriteXml(writer);
            }
                                
            writer.WriteEndElement(); // end of display

            Preconditions.WriteXml(writer);

            writer.WriteStartElement("configrefs");
            foreach (ConfigRef p in ConfigRefs)
            {
                p.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        public OutputConfigItem(OutputConfigItem config) : base(config)
        {
            this.Source = config.Source.Clone() as Source;
            
            this.DeviceType = config.DeviceType;
            this.ModuleSerial = config.ModuleSerial;

            this.Device = config.Device?.Clone() as IDeviceConfig;

            this.Preconditions = Preconditions.Clone() as PreconditionList;

            this.ConfigRefs = config.ConfigRefs.Clone() as ConfigRefList;
            this.ButtonInputConfig = config.ButtonInputConfig?.Clone() as InputConfig.ButtonInputConfig;
            this.AnalogInputConfig = config.AnalogInputConfig?.Clone() as InputConfig.AnalogInputConfig;

            this.Modifiers = config.Modifiers.Clone() as ModifierList;
            this.TestValue = config.TestValue.Clone() as ConnectorValue;
        }

        public override object Clone()
        {
            OutputConfigItem clone = new OutputConfigItem(this);
            return clone;
        }

        public override Base.IConfigItem Duplicate()
        {
            return new OutputConfigItem(this) { GUID = System.Guid.NewGuid().ToString() };
        }

        protected override IDeviceConfig GetDeviceConfig()
        {
            switch (DeviceType)
            {
                case MobiFlightOutput.TYPE:
                case ArcazeLedDigit.TYPE:
                case MobiFlightServo.TYPE:
                case MobiFlightStepper.TYPE:
                case MobiFlightShiftRegister.TYPE:
                case MobiFlightLcdDisplay.TYPE:
                case MobiFlightCustomDevice.TYPE:
                    return Device;
                default:
                    return null;
            }
        }
    }
}
