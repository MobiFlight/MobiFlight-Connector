using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MobiFlight;
using MobiFlight.OutputConfig;
using MobiFlight.Base;
using MobiFlight.Config;
using MobiFlight.InputConfig;
using MobiFlight.xplane;
using MobiFlight.Modifier;

namespace MobiFlight
{
    public class OutputConfigItem : IBaseConfigItem, IFsuipcConfigItem, IXmlSerializable, ICloneable, IConfigRefConfigItem
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
		public string               Value                       { get; set; }	
        public ModifierList         Modifiers                   { get; set; }
		public string               DisplayType                 { get; set; }
		public string               DisplaySerial               { get; set; }
		public OutputConfig.Pin     Pin                         { get; set; }
		public OutputConfig.LedModule   LedModule               { get; set; }
		public OutputConfig.LcdDisplay  LcdDisplay              { get; set; }
		public List<string>         BcdPins                     { get; set; }
        public OutputConfig.Servo   Servo { get; set; }
        public OutputConfig.Stepper Stepper { get; set; }
        public OutputConfig.ShiftRegister ShiftRegister               { get; set; }
        public string       DisplayTrigger              { get; set; }
        public PreconditionList   Preconditions       { get; set; }
        public ConfigRefList      ConfigRefs          { get; set; }     
        
        public InputConfig.ButtonInputConfig ButtonInputConfig { get; set; }

        public InputConfig.AnalogInputConfig AnalogInputConfig { get; set; }

        // Legacy access to Transformation, Comparison and Interpolation
        public Modifier.Transformation Transform { 
            get { return Modifiers.Transformation; }
            set { Modifiers.Transformation = value; }
        }

        public Modifier.Comparison Comparison
        {
            get { return Modifiers.Comparison; }
            set { Modifiers.Comparison = value; }
        }

        public Modifier.Interpolation Interpolation
        {
            get { return Modifiers.Interpolation; }
            set { Modifiers.Interpolation = value; }
        }

        public OutputConfigItem()
        {
            SourceType = SourceType.SIMCONNECT;
            FSUIPC = new FsuipcOffset();
            SimConnectValue = new SimConnectValue();
            MobiFlightVariable = new MobiFlightVariable();
            XplaneDataRef = new XplaneDataRef();
            Modifiers = new ModifierList();
            Pin = new OutputConfig.Pin();
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
            return (
                obj != null && obj is OutputConfigItem &&
                this.DisplayType == (obj as OutputConfigItem).DisplayType &&
                this.DisplaySerial == (obj as OutputConfigItem).DisplaySerial &&
                this.SourceType == (obj as OutputConfigItem).SourceType &&
                this.FSUIPC.Equals((obj as OutputConfigItem).FSUIPC) &&
                this.SimConnectValue.Equals((obj as OutputConfigItem).SimConnectValue) &&
                this.XplaneDataRef.Equals((obj as OutputConfigItem).XplaneDataRef) &&
                this.MobiFlightVariable.Equals((obj as OutputConfigItem).MobiFlightVariable) &&
                this.Transform.Equals((obj as OutputConfigItem).Transform) &&
                //===
                this.Comparison.Equals((obj as OutputConfigItem).Comparison) &&
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
                this.Interpolation.Equals((obj as OutputConfigItem).Interpolation) &&
                //===
                this.Preconditions.Equals((obj as OutputConfigItem).Preconditions) &&
                //===
                this.ConfigRefs.Equals((obj as OutputConfigItem).ConfigRefs) &&
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
                if (reader["type"]=="SimConnect") {
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
                }

                // backward compatibility
                if (reader["multiplier"] != null)
                {
                    double multiplier = Double.Parse(reader["multiplier"], serializationCulture);
                    if (multiplier != 1.0)
                    {
                        Transform.Active = true;
                        // we have to replace the decimal in case "," is used (german style)
                        Transform.Expression = "$*" + multiplier.ToString().Replace(',', '.');
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
                Comparison.ReadXml(reader);
            }

            if (reader.ReadToNextSibling("display"))
            {
                DisplayType = reader["type"];

                // preserve backward compatibility
                if (DisplayType == "Pin") DisplayType = MobiFlightOutput.TYPE;
                if (DisplayType == ArcazeLedDigit.OLDTYPE) DisplayType = ArcazeLedDigit.TYPE;
                
                DisplaySerial = reader["serial"];
                DisplayTrigger = reader["trigger"];

                if (DisplayType == MobiFlightOutput.TYPE)
                {
                    Pin.ReadXml(reader);
                }
                else if (DisplayType == MobiFlightLedModule.TYPE)
                {
                    LedModule.XmlRead(reader);
                }
                else if (DisplayType == ArcazeBcd4056.TYPE)
                {
                    // ignore empty values
                    if (reader["bcdPins"] != null && reader["bcdPins"] != "")
                    {
                        BcdPins = reader["bcdPins"].Split(',').ToList();
                    }
                }
                else if (DisplayType == MobiFlightServo.TYPE)
                {
                    Servo.ReadXml(reader);
                }
                else if (DisplayType == MobiFlightStepper.TYPE)
                {
                    Stepper.ReadXml(reader);
                }
                else if (DisplayType == OutputConfig.LcdDisplay.Type)
                {
                    if (LcdDisplay == null) LcdDisplay = new OutputConfig.LcdDisplay();
                    LcdDisplay.ReadXml(reader);
                }
                else if (DisplayType == MobiFlightShiftRegister.TYPE)
                {
                    ShiftRegister.ReadXml(reader);
                }
                else if (DisplayType == "InputAction")
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
                    Interpolation.ReadXml(reader);
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
                Interpolation.ReadXml(reader);
                
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
                Transform.ReadXml(reader);
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

            Modifiers.WriteXml(writer);

            writer.WriteStartElement("display");
                writer.WriteAttributeString("type", DisplayType);
                writer.WriteAttributeString("serial", DisplaySerial);

                if ( DisplayTrigger != null)
                    writer.WriteAttributeString("trigger", DisplayTrigger);

            if (DisplayType == ArcazeLedDigit.TYPE)
            {
                LedModule.WriteXml(writer);

            }
            else if (DisplayType == ArcazeBcd4056.TYPE)
            {
                writer.WriteAttributeString("bcdPins", String.Join(",", BcdPins));
            }
            else if (DisplayType == MobiFlightServo.TYPE)
            {
                Servo.WriteXml(writer);
            }
            else if (DisplayType == MobiFlightStepper.TYPE)
            {
                Stepper.WriteXml(writer);
            }
            else if (DisplayType == MobiFlightLcdDisplay.TYPE)
            {
                if (LcdDisplay == null) LcdDisplay = new OutputConfig.LcdDisplay();
                LcdDisplay.WriteXml(writer);
            }
            else if (DisplayType == MobiFlightShiftRegister.TYPE)
            {
                ShiftRegister.WriteXml(writer);
            }
            else if (DisplayType == "InputAction")
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

        public object Clone()
        {
            OutputConfigItem clone = new OutputConfigItem();
            clone.SourceType                = this.SourceType;
            clone.FSUIPC                    = this.FSUIPC.Clone() as FsuipcOffset;
            clone.SimConnectValue           = this.SimConnectValue.Clone() as SimConnectValue;
            clone.MobiFlightVariable        = this.MobiFlightVariable.Clone() as MobiFlightVariable;
            clone.XplaneDataRef             = this.XplaneDataRef.Clone() as XplaneDataRef;


            clone.Transform                 = this.Transform.Clone() as Transformation;
            clone.Comparison                = this.Comparison.Clone() as Comparison;

            clone.DisplayType               = this.DisplayType;
            clone.DisplaySerial             = this.DisplaySerial;

            clone.LedModule                 = this.LedModule.Clone() as OutputConfig.LedModule;

            clone.Pin                       = this.Pin.Clone() as OutputConfig.Pin;
            
            clone.BcdPins                   = new List<string>(this.BcdPins);

            clone.DisplayTrigger            = this.DisplayTrigger;
            clone.Servo                     = Servo.Clone() as OutputConfig.Servo;
            clone.Stepper                   = Stepper.Clone() as OutputConfig.Stepper;

            clone.ShiftRegister             = ShiftRegister.Clone() as OutputConfig.ShiftRegister;

            clone.LcdDisplay                = this.LcdDisplay.Clone() as OutputConfig.LcdDisplay;
            clone.Preconditions             = Preconditions.Clone() as PreconditionList;

            clone.ConfigRefs                = ConfigRefs.Clone() as ConfigRefList;
            clone.ButtonInputConfig         = this.ButtonInputConfig?.Clone() as InputConfig.ButtonInputConfig;
            clone.AnalogInputConfig         = this.AnalogInputConfig?.Clone() as InputConfig.AnalogInputConfig;

            clone.Modifiers                 = this.Modifiers.Clone() as ModifierList;
            return clone;
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
