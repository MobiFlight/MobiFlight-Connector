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
		public SourceType   SourceType                  { get; set; }
		public FsuipcOffset FSUIPC                      { get; set; }
		public SimConnectValue SimConnectValue              { get; set; }
		public MobiFlightVariable MobiFlightVariable          { get; set; }
		public Transformation Transform                   { get; set; }
		public string Value                       { get; set; }	
		public OutputConfig.Comparison Comparison                  { get; set; }
		public string       DisplayType                 { get; set; }
		public string       DisplaySerial               { get; set; }
		public OutputConfig.Pin Pin                         { get; set; }
		public OutputConfig.LedModule LedModule                   { get; set; }
		public OutputConfig.LcdDisplay LcdDisplay                  { get; set; }
		public List<string> BcdPins                     { get; set; }
        public OutputConfig.Servo Servo { get; set; }
        public OutputConfig.Stepper Stepper { get; set; }
        public Interpolation Interpolation              { get; set; }
        public string       ShiftRegister               { get; set; }
        public string       RegisterOutputPin           { get; set; }
        public string       DisplayTrigger              { get; set; }
        public PreconditionList   Preconditions       { get; set; }
        public ConfigRefList      ConfigRefs          { get; set; }        

        public OutputConfigItem()
        {
            SourceType = SourceType.FSUIPC;
            FSUIPC = new FsuipcOffset();
            SimConnectValue = new SimConnectValue();
            MobiFlightVariable = new MobiFlightVariable();
            Transform = new Transformation();
            Comparison = new OutputConfig.Comparison();
            Pin = new OutputConfig.Pin();
            LedModule = new OutputConfig.LedModule();
            LcdDisplay = new OutputConfig.LcdDisplay();
            Servo = new OutputConfig.Servo();
            Stepper = new OutputConfig.Stepper() { CompassMode = false };
            BcdPins = new List<string>() { "A01", "A02", "A03", "A04", "A05" };
            Interpolation = new Interpolation();
            Preconditions = new PreconditionList();
            ConfigRefs = new ConfigRefList();
        }

        public override bool Equals(object obj)
        { 
            return (
                obj != null && obj is OutputConfigItem &&
                this.SourceType == (obj as OutputConfigItem).SourceType &&
                this.FSUIPC.Equals((obj as OutputConfigItem).FSUIPC) &&
                this.SimConnectValue.Equals((obj as OutputConfigItem).SimConnectValue) &&
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
                //===
                // TODO: I will ignore this, because it is a deprecated feature
                // this.BcdPins.Equals((obj as OutputConfigItem).BcdPins) &&
                //===
                this.Interpolation.Equals((obj as OutputConfigItem).Interpolation) &&
                //===
                this.Preconditions.Equals((obj as OutputConfigItem).Preconditions) &&
                //===
                this.ConfigRefs.Equals((obj as OutputConfigItem).ConfigRefs)
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
            }

            if (reader.ReadToNextSibling("comparison"))
            {
                Comparison.ReadXml(reader);
            }

            if (reader.ReadToNextSibling("display"))
            {
                DisplayType = reader["type"];
                // preserve backward compatibility
                if (DisplayType == ArcazeLedDigit.OLDTYPE) DisplayType = ArcazeLedDigit.TYPE;

                DisplaySerial = reader["serial"];
                DisplayTrigger = reader["trigger"];

                if (DisplayType == MobiFlightOutput.TYPE || DisplayType == "Pin")
                {
                    Pin.ReadXml(reader);
                }
                else if (DisplayType == MobiFlightLedModule.TYPE) {

                    LedModule.XmlRead(reader);
                }
                else if (DisplayType == ArcazeBcd4056.TYPE) { 
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
                    
                    // don't read to the end tag all the way
                    reader.Read();
                }
                else if (DisplayType == MobiFlightShiftRegister.TYPE)
                {
                    if (reader["registerOutputPin"] != null && reader["registerOutputPin"] != "")
                    {
                        RegisterOutputPin = reader["registerOutputPin"];
                    }

                    if (reader["shiftRegister"] != null && reader["shiftRegister"] != "")
                    {
                        ShiftRegister = reader["shiftRegister"];
                    }
                }
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

            // forward
            if (reader.LocalName == "display")
                reader.ReadStartElement();

            // Actually interpolation is in he wrong spot. :(
            // it should not be nested
            // it should be outside of the display node
            if (reader.LocalName == "interpolation")
            {
                Interpolation.ReadXml(reader);
                if (reader.LocalName != "preconditions")
                    reader.ReadEndElement(); // this closes the display node
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
                        reader.ReadStartElement();
                    } while (reader.LocalName == "configref");

                    // read to the end of configref-node
                    reader.ReadEndElement();
                }
                else
                {
                    reader.ReadStartElement();
                }
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("source");
                if(SourceType==SourceType.FSUIPC)
                    this.FSUIPC.WriteXml(writer);
                else if (SourceType == SourceType.VARIABLE)
                    this.MobiFlightVariable.WriteXml(writer);
                else
                    this.SimConnectValue.WriteXml(writer);
            writer.WriteEndElement();

            
            Comparison.WriteXml(writer);

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
                    writer.WriteAttributeString("bcdPins", String.Join(",",BcdPins));
                }
                else if (DisplayType == DeviceType.Servo.ToString("F"))
                {
                    Servo.WriteXml(writer);
                }
                else if (DisplayType == DeviceType.Stepper.ToString("F"))
                {
                    Stepper.WriteXml(writer);
                }
                else if (DisplayType == OutputConfig.LcdDisplay.Type)
                {
                    if (LcdDisplay == null) LcdDisplay = new OutputConfig.LcdDisplay();
                    LcdDisplay.WriteXml(writer);
                }
                else if (DisplayType == MobiFlightShiftRegister.TYPE)
                {
                    writer.WriteAttributeString("shiftRegister", ShiftRegister);
                    writer.WriteAttributeString("registerOutputPin", RegisterOutputPin);
                }
                else
                {
                    Pin.WriteXml(writer);
                }
                                
            writer.WriteEndElement(); // end of display

            Interpolation.WriteXml(writer);

            Preconditions.WriteXml(writer);

            Transform.WriteXml(writer);

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


            clone.Transform                 = this.Transform.Clone() as Transformation;
            clone.Comparison                = this.Comparison.Clone() as OutputConfig.Comparison;

            clone.DisplayType               = this.DisplayType;
            clone.DisplaySerial             = this.DisplaySerial;

            clone.LedModule                 = this.LedModule.Clone() as OutputConfig.LedModule;

            clone.Pin                       = this.Pin.Clone() as OutputConfig.Pin;
            
            clone.BcdPins                   = new List<string>(this.BcdPins);

            clone.DisplayTrigger            = this.DisplayTrigger;
            clone.Servo                     = Servo.Clone() as OutputConfig.Servo;
            clone.Stepper                   = Stepper.Clone() as OutputConfig.Stepper;

            clone.ShiftRegister             = this.ShiftRegister;
            clone.RegisterOutputPin         = this.RegisterOutputPin;

            clone.LcdDisplay                = this.LcdDisplay.Clone() as OutputConfig.LcdDisplay;
            clone.Preconditions             = Preconditions.Clone() as PreconditionList;

            clone.Interpolation             = this.Interpolation.Clone() as Interpolation;
            clone.ConfigRefs                = ConfigRefs.Clone() as ConfigRefList;

            return clone;
        }
    }

    public enum SourceType
    {
        FSUIPC,
        SIMCONNECT,
        VARIABLE
    }
}
