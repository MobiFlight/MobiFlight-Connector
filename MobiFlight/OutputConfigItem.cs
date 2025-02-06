using MobiFlight.Base;
using MobiFlight.Config;
using MobiFlight.InputConfig;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
using MobiFlight.xplane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class OutputConfigItem : ConfigItem, IFsuipcConfigItem, IXmlSerializable, ICloneable, IConfigRefConfigItem
    {
        // we initialize a cultureInfo object 
        // which is used for serialization
        // independently from current cultureInfo
        // @see: https://forge.simple-solutions.de/issues/275
        public System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");
        
        // this implements the FSUIPC Config Item Interface
        // It would be nicer to have an aggregation of FSUIPC.FSUIPCConfigItem instead
		public SourceType           SourceType                  { get; set; }
		public FsuipcOffset         FSUIPC                      { get; set; }
		public SimConnectValue      SimConnectValue             { get; set; }
		public MobiFlightVariable   MobiFlightVariable          { get; set; }

        public XplaneDataRef        XplaneDataRef               { get; set; }
		
        public ConnectorValue       TestValue                   { get; set; }
        public OutputConfig.Output     Pin                         { get; set; }
		public OutputConfig.LedModule   LedModule               { get; set; }
		public OutputConfig.LcdDisplay  LcdDisplay              { get; set; }
		public List<string>         BcdPins                     { get; set; }
        public OutputConfig.Servo   Servo { get; set; }
        public OutputConfig.Stepper Stepper { get; set; }
        public OutputConfig.ShiftRegister ShiftRegister         { get; set; }
        public OutputConfig.CustomDevice CustomDevice           { get; set; } = new OutputConfig.CustomDevice();
        public string       DisplayTrigger                      { get; set; }
        
        public InputConfig.ButtonInputConfig ButtonInputConfig { get; set; }

        public InputConfig.AnalogInputConfig AnalogInputConfig { get; set; }

        public string DeviceType { get; set; }

        public OutputConfigItem()
        {
            SourceType = SourceType.SIMCONNECT;
            FSUIPC = new FsuipcOffset();
            SimConnectValue = new SimConnectValue();
            MobiFlightVariable = new MobiFlightVariable();
            XplaneDataRef = new XplaneDataRef();
            TestValue = new ConnectorValue() { type = FSUIPCOffsetType.Float, Float64 = 1 };
            Modifiers = new ModifierList();
            Pin = new OutputConfig.Output();
            LedModule = new OutputConfig.LedModule();
            LcdDisplay = new OutputConfig.LcdDisplay();
            Servo = new OutputConfig.Servo();
            Stepper = new OutputConfig.Stepper() { CompassMode = false };
            BcdPins = new List<string>() { "A01", "A02", "A03", "A04", "A05" };
            ShiftRegister = new OutputConfig.ShiftRegister();
            Preconditions = new PreconditionList();
            ConfigRefs = new ConfigRefList();
            ButtonInputConfig = null;
            AnalogInputConfig = null;
        }

        public override bool Equals(object obj)
        { 
            var areEqual = base.Equals(obj);
            if (!areEqual) return false;

            return (
                obj != null && obj is OutputConfigItem &&
                this.DeviceType == (obj as OutputConfigItem).DeviceType &&
                this.SourceType == (obj as OutputConfigItem).SourceType &&
                this.FSUIPC.Equals((obj as OutputConfigItem).FSUIPC) &&
                this.SimConnectValue.Equals((obj as OutputConfigItem).SimConnectValue) &&
                this.XplaneDataRef.Equals((obj as OutputConfigItem).XplaneDataRef) &&
                this.MobiFlightVariable.Equals((obj as OutputConfigItem).MobiFlightVariable) &&
                //===
                this.TestValue.Equals((obj as OutputConfigItem).TestValue) &&
                //===
                this.Pin.Equals((obj as OutputConfigItem).Pin) &&
                //===
                this.LedModule.Equals((obj as OutputConfigItem).LedModule) &&
                //===
                this.LcdDisplay.Equals((obj as OutputConfigItem).LcdDisplay) &&
                //===
                this.Stepper.Equals((obj as OutputConfigItem).Stepper) &&
                //==
                this.Servo.Equals((obj as OutputConfigItem).Servo) &&
                //===
                // TODO: I will ignore this, because it is a deprecated feature
                // this.BcdPins.Equals((obj as OutputConfigItem).BcdPins) &&
                //===
                this.ShiftRegister.Equals((obj as OutputConfigItem).ShiftRegister) &&
                //===
                this.CustomDevice.Equals((obj as OutputConfigItem).CustomDevice) &&
                //===
                ((this.ButtonInputConfig == null && (obj as OutputConfigItem).ButtonInputConfig == null) || (
                this.ButtonInputConfig != null && this.ButtonInputConfig.Equals((obj as OutputConfigItem).ButtonInputConfig))) &&
                //===
                ((this.AnalogInputConfig==null&&(obj as OutputConfigItem).AnalogInputConfig == null) || (
                this.AnalogInputConfig != null && this.AnalogInputConfig.Equals((obj as OutputConfigItem).AnalogInputConfig)))
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
                    SourceType = SourceType.SIMCONNECT;
                    this.SimConnectValue.ReadXml(reader);
                } else if (reader["type"] == "Variable")
                {
                    SourceType = SourceType.VARIABLE;
                    this.MobiFlightVariable.ReadXml(reader);
                } else if (reader["type"] == "XplaneDataRef")
                {
                    SourceType = SourceType.XPLANE;
                    this.XplaneDataRef.ReadXml(reader);
                }
                else
                {
                    SourceType = SourceType.FSUIPC;
                    this.FSUIPC.ReadXml(reader);

                    if(FSUIPC.OffsetType == FSUIPCOffsetType.String)
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
                DisplayTrigger = reader["trigger"];

                if (DeviceType == MobiFlightOutput.TYPE)
                {
                    Pin.ReadXml(reader);
                }
                else if (DeviceType == MobiFlightLedModule.TYPE)
                {
                    LedModule.XmlRead(reader);
                }
                else if (DeviceType == ArcazeBcd4056.TYPE)
                {
                    // ignore empty values
                    if (reader["bcdPins"] != null && reader["bcdPins"] != "")
                    {
                        BcdPins = reader["bcdPins"].Split(',').ToList();
                    }
                }
                else if (DeviceType == MobiFlightServo.TYPE)
                {
                    Servo.ReadXml(reader);
                }
                else if (DeviceType == MobiFlightStepper.TYPE)
                {
                    Stepper.ReadXml(reader);
                }
                else if (DeviceType == OutputConfig.LcdDisplay.DeprecatedType)
                {
                    if (LcdDisplay == null) LcdDisplay = new OutputConfig.LcdDisplay();
                    LcdDisplay.ReadXml(reader);
                }
                else if (DeviceType == MobiFlightShiftRegister.TYPE)
                {
                    ShiftRegister.ReadXml(reader);
                }
                else if (DeviceType == MobiFlightCustomDevice.TYPE)
                {
                    CustomDevice.ReadXml(reader);
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
                if (SourceType == SourceType.FSUIPC)
                    this.FSUIPC.WriteXml(writer);
                else if (SourceType == SourceType.VARIABLE)
                    this.MobiFlightVariable.WriteXml(writer);
                else if (SourceType == SourceType.XPLANE)
                    this.XplaneDataRef.WriteXml(writer);
                else
                    this.SimConnectValue.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("test");
                writer.WriteAttributeString("type", TestValue.type.ToString());
                writer.WriteAttributeString("value", TestValue.ToString());
            writer.WriteEndElement();

            Modifiers.WriteXml(writer);

            writer.WriteStartElement("display");
                writer.WriteAttributeString("type", DeviceType);
                writer.WriteAttributeString("serial", ModuleSerial);

                if ( DisplayTrigger != null)
                    writer.WriteAttributeString("trigger", DisplayTrigger);

            if (DeviceType == ArcazeLedDigit.TYPE)
            {
                LedModule.WriteXml(writer);

            }
            else if (DeviceType == ArcazeBcd4056.TYPE)
            {
                writer.WriteAttributeString("bcdPins", String.Join(",", BcdPins));
            }
            else if (DeviceType == MobiFlightServo.TYPE)
            {
                Servo.WriteXml(writer);
            }
            else if (DeviceType == MobiFlightStepper.TYPE)
            {
                Stepper.WriteXml(writer);
            }
            else if (DeviceType == MobiFlightLcdDisplay.TYPE)
            {
                if (LcdDisplay == null) LcdDisplay = new OutputConfig.LcdDisplay();
                LcdDisplay.WriteXml(writer);
            }
            else if (DeviceType == MobiFlightShiftRegister.TYPE)
            {
                ShiftRegister.WriteXml(writer);
            }
            else if (DeviceType == MobiFlightCustomDevice.TYPE)
            {
                CustomDevice.WriteXml(writer);
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
            else
            {
                Pin.WriteXml(writer);
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
            this.SourceType = config.SourceType;
            this.FSUIPC = config.FSUIPC.Clone() as FsuipcOffset;
            this.SimConnectValue = config.SimConnectValue.Clone() as SimConnectValue;
            this.MobiFlightVariable = config.MobiFlightVariable.Clone() as MobiFlightVariable;
            this.XplaneDataRef = config.XplaneDataRef.Clone() as XplaneDataRef;

            this.DeviceType = config.DeviceType;
            this.ModuleSerial = config.ModuleSerial;

            this.LedModule = config.LedModule.Clone() as OutputConfig.LedModule;

            this.Pin = config.Pin.Clone() as OutputConfig.Output;

            this.BcdPins = new List<string>(config.BcdPins);

            this.DisplayTrigger = config.DisplayTrigger;
            this.Servo = config.Servo.Clone() as OutputConfig.Servo;
            this.Stepper = config.Stepper.Clone() as OutputConfig.Stepper;

            this.ShiftRegister = config.ShiftRegister.Clone() as OutputConfig.ShiftRegister;
            this.CustomDevice = config.CustomDevice.Clone() as OutputConfig.CustomDevice;

            this.LcdDisplay = config.LcdDisplay.Clone() as OutputConfig.LcdDisplay;
            this.Preconditions = Preconditions.Clone() as PreconditionList;

            this.ConfigRefs = config.ConfigRefs.Clone() as ConfigRefList;
            this.ButtonInputConfig = config.ButtonInputConfig?.Clone() as InputConfig.ButtonInputConfig;
            this.AnalogInputConfig = config.AnalogInputConfig?.Clone() as InputConfig.AnalogInputConfig;

            this.Modifiers = config.Modifiers.Clone() as ModifierList;
            this.TestValue = config.TestValue.Clone() as ConnectorValue;
        }

        public object Clone()
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
                    return Pin;
                case ArcazeLedDigit.TYPE:
                    return LedModule;
                case MobiFlightServo.TYPE:
                    return Servo;
                case MobiFlightStepper.TYPE:
                    return Stepper;
                case MobiFlightShiftRegister.TYPE:
                    return ShiftRegister;
                case MobiFlightLcdDisplay.TYPE:
                    return LcdDisplay;
                case MobiFlightCustomDevice.TYPE:
                    return CustomDevice;
                default:
                    return null;
            }
        }
    }

    public enum SourceType
    {
        FSUIPC,
        SIMCONNECT,
        VARIABLE,
        XPLANE
    }
}
