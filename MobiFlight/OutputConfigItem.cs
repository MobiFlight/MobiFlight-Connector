﻿using System;
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

        public SimConnectValue
                            SimConnectValue              { get; set; }

        public MobiFlightVariable
                            MobiFlightVariable          { get; set; }

        public Transformation
                            Transform                   { get; set; }
        public string       Value                       { get; set; }
        //
        public bool         ComparisonActive            { get; set; }
        public string       ComparisonOperand           { get; set; }
        public string       ComparisonValue             { get; set; }
        public string       ComparisonIfValue           { get; set; }
        public string       ComparisonElseValue         { get; set; }
        public string       DisplayType                 { get; set; }
        public string       DisplaySerial               { get; set; }
        public string       DisplayPin                  { get; set; }
        public byte         DisplayPinBrightness        { get; set; }
        public bool         DisplayPinPWM               { get; set; }
        // the display stuff
        public string       DisplayLedAddress           { get; set; }
        public byte         DisplayLedConnector         { get; set; }
        public byte         DisplayLedModuleSize        { get; set; }
        public bool         DisplayLedPadding           { get; set; }
        public string       DisplayLedPaddingChar       { get; set; }
        public List<string> DisplayLedDigits            { get; set; }
        public List<string> DisplayLedDecimalPoints     { get; set; }
        public bool         DisplayLedReverseDigits     { get; set; }
        public string       DisplayLedBrightnessReference { get; set; }


        // the lcd display stuff
        public OutputConfig.LcdDisplay LcdDisplay       { get; set; }

        // the bcd driver stuff
        public List<string> BcdPins                     { get; set; }
        // the servo stuff
        public string       ServoAddress                { get; set; }
        public string       ServoMin                    { get; set; }
        public string       ServoMax                    { get; set; }
        public string       ServoMaxRotationPercent     { get; set; }
        
        // the stepper stuff
        public string       StepperAddress              { get; set; }
        public string       StepperInputRev             { get; set; }
        public string       StepperOutputRev            { get; set; }
        public string       StepperTestValue            { get; set; }
        public bool         StepperCompassMode          { get; set; }

        // the interpolation settings
        public Interpolation Interpolation              { get; set; }

        // the shift register stuff 
        public string       ShiftRegister               { get; set; }
        public string       RegisterOutputPin           { get; set; }

        // deprecated?
        public string       DisplayTrigger              { get; set; }
                
        public PreconditionList   Preconditions       { get; set; }

        public List<ConfigRef>      ConfigRefs          { get; set; }        

        public OutputConfigItem()
        {
            SourceType = SourceType.FSUIPC;
            FSUIPC = new FsuipcOffset();
            SimConnectValue = new SimConnectValue();
            MobiFlightVariable = new MobiFlightVariable();

            Transform = new Transformation();

            ComparisonActive = false;
            ComparisonOperand = "";
            ComparisonValue = "";
            ComparisonIfValue = "";
            ComparisonElseValue = "";

            //DisplayPin = "A01"; // not initialized anymore
            DisplayPinBrightness = byte.MaxValue;
            DisplayPinPWM = false; 
            DisplayLedConnector = 1;
            DisplayLedAddress = "0";
            DisplayLedPadding = false;
            DisplayLedReverseDigits = false;
            DisplayLedBrightnessReference = string.Empty;
            DisplayLedPaddingChar = "0";
            DisplayLedModuleSize = 8;
            DisplayLedDigits = new List<string>();
            DisplayLedDecimalPoints = new List<string>();

            LcdDisplay = new OutputConfig.LcdDisplay();

            StepperCompassMode = false;
                
            BcdPins = new List<string>() { "A01", "A02", "A03", "A04", "A05" };

            Interpolation = new Interpolation();

            Preconditions = new PreconditionList();

            ConfigRefs = new List<ConfigRef>();
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
                ComparisonActive = Boolean.Parse(reader["active"]);
                ComparisonValue = reader["value"];
                ComparisonOperand = reader["operand"];
                ComparisonIfValue = reader["ifValue"];
                ComparisonElseValue = reader["elseValue"];
            }

            if (reader.ReadToNextSibling("display"))
            {
                DisplayType = reader["type"];
                // preserve backward compatibility
                if (DisplayType == ArcazeLedDigit.OLDTYPE) DisplayType = ArcazeLedDigit.TYPE;

                DisplayPin = reader["pin"];
                DisplaySerial = reader["serial"];
                DisplayTrigger = reader["trigger"];

                if (DisplayType == MobiFlightOutput.TYPE || DisplayType == "Pin")
                {
                    if (reader["pinBrightness"] != null && reader["pinBrightness"] != "")
                    {
                        DisplayPinBrightness = byte.Parse(reader["pinBrightness"]);
                    }
                    if (reader["pinPwm"] != null && reader["pinPwm"] != "")
                    {
                        DisplayPinPWM = bool.Parse(reader["pinPwm"]);
                    }
                }
                else if (DisplayType == MobiFlightLedModule.TYPE) {

                    if (reader["ledAddress"] != null && reader["ledAddress"] != "")
                    {
                        DisplayLedAddress = reader["ledAddress"];
                    }

                    if (reader["ledConnector"] != null && reader["ledConnector"] != "")
                    {
                        DisplayLedConnector = byte.Parse(reader["ledConnector"]);
                    }

                    if (reader["ledModuleSize"] != null && reader["ledModuleSize"] != "")
                    {
                        DisplayLedModuleSize = byte.Parse(reader["ledModuleSize"]);
                    }

                    if (reader["ledPadding"] != null && reader["ledPadding"] != "")
                    {
                        DisplayLedPadding = Boolean.Parse(reader["ledPadding"]);
                    }

                    if (reader["ledReverseDigits"] != null && reader["ledReverseDigits"] != "")
                    {
                        DisplayLedReverseDigits = Boolean.Parse(reader["ledReverseDigits"]);
                    }
                    if (reader["ledBrightnessRef"] != null && reader["ledBrightnessRef"] != "")
                    {
                        DisplayLedBrightnessReference = reader["ledBrightnessRef"];
                    }

                    if (reader["ledPaddingChar"] != null && reader["ledPaddingChar"] != "")
                    {
                        DisplayLedPaddingChar = reader["ledPaddingChar"];
                    }

                    // ignore empty values
                    if (reader["ledDigits"] != null && reader["ledDigits"] != "")
                    {
                        DisplayLedDigits = reader["ledDigits"].Split(',').ToList();
                    }

                    // ignore empty values
                    if (reader["ledDecimalPoints"] != null && reader["ledDecimalPoints"] != "")
                    {
                        DisplayLedDecimalPoints = reader["ledDecimalPoints"].Split(',').ToList();
                    }
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
                    // ignore empty values
                    if (reader["servoAddress"] != null && reader["servoAddress"] != "")
                    {
                        ServoAddress = reader["servoAddress"];
                    }
                    if (reader["servoMin"] != null && reader["servoMin"] != "")
                    {
                        ServoMin = reader["servoMin"];
                    }
                    if (reader["servoMax"] != null && reader["servoMax"] != "")
                    {
                        ServoMax = reader["servoMax"];
                    }

                    if (reader["servoMaxRotationPercent"] != null && reader["servoMaxRotationPercent"] != "")
                    {
                        ServoMaxRotationPercent = reader["servoMaxRotationPercent"];
                    }
                }
                else if (DisplayType == MobiFlightStepper.TYPE)
                {
                    // ignore empty values
                    if (reader["stepperAddress"] != null && reader["stepperAddress"] != "")
                    {
                        StepperAddress = reader["stepperAddress"];
                    }
                    if (reader["stepperInputRev"] != null && reader["stepperInputRev"] != "")
                    {
                        StepperInputRev = reader["stepperInputRev"];
                        StepperTestValue = reader["stepperInputRev"];
                    }
                    if (reader["stepperOutputRev"] != null && reader["stepperOutputRev"] != "")
                    {
                        StepperOutputRev = reader["stepperOutputRev"];
                    }
                    if (reader["stepperTestValue"] != null && reader["stepperTestValue"] != "")
                    {
                        StepperTestValue = reader["stepperTestValue"];
                    }

                    if (reader["stepperCompassMode"] != null && reader["stepperCompassMode"] != "")
                    {
                        StepperCompassMode = bool.Parse(reader["stepperCompassMode"]);
                    }
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

            writer.WriteStartElement("comparison");
                writer.WriteAttributeString("active", ComparisonActive.ToString());
                writer.WriteAttributeString("value", ComparisonValue);
                writer.WriteAttributeString("operand", ComparisonOperand);
                writer.WriteAttributeString("ifValue", ComparisonIfValue);
                writer.WriteAttributeString("elseValue", ComparisonElseValue);
            writer.WriteEndElement();

            writer.WriteStartElement("display");
                writer.WriteAttributeString("type", DisplayType);
                writer.WriteAttributeString("serial", DisplaySerial);

                if ( DisplayTrigger != null)
                    writer.WriteAttributeString("trigger", DisplayTrigger);

                if (DisplayType == ArcazeLedDigit.TYPE)
                {
                    writer.WriteAttributeString("ledAddress", DisplayLedAddress);
                    writer.WriteAttributeString("ledConnector", DisplayLedConnector.ToString());
                    writer.WriteAttributeString("ledModuleSize", DisplayLedModuleSize.ToString());
                    writer.WriteAttributeString("ledPadding", DisplayLedPadding.ToString());
                    if (DisplayLedReverseDigits)
                        writer.WriteAttributeString("ledReverseDigits", DisplayLedReverseDigits.ToString());
                    if (!string.IsNullOrEmpty(DisplayLedBrightnessReference))
                        writer.WriteAttributeString("ledBrightnessRef", DisplayLedBrightnessReference.ToString());

                    writer.WriteAttributeString("ledPaddingChar", DisplayLedPaddingChar);

                    if (DisplayLedDigits.Count > 0)
                    {
                        writer.WriteAttributeString("ledDigits", String.Join(",", DisplayLedDigits));
                    }

                    if (DisplayLedDecimalPoints.Count > 0)
                    {
                        writer.WriteAttributeString("ledDecimalPoints", String.Join(",", DisplayLedDecimalPoints));
                    }
                }
                else if (DisplayType == ArcazeBcd4056.TYPE)
                {
                    writer.WriteAttributeString("bcdPins", String.Join(",",BcdPins));
                }
                else if (DisplayType == DeviceType.Servo.ToString("F"))
                {
                    writer.WriteAttributeString("servoAddress", ServoAddress);
                    writer.WriteAttributeString("servoMin", ServoMin);
                    writer.WriteAttributeString("servoMax", ServoMax);
                    writer.WriteAttributeString("servoMaxRotationPercent", ServoMaxRotationPercent);
                }
                else if (DisplayType == DeviceType.Stepper.ToString("F"))
                {
                    writer.WriteAttributeString("stepperAddress", StepperAddress);
                    writer.WriteAttributeString("stepperInputRev", StepperInputRev);
                    writer.WriteAttributeString("stepperOutputRev", StepperOutputRev);
                    writer.WriteAttributeString("stepperTestValue", StepperTestValue);
                    writer.WriteAttributeString("stepperCompassMode", StepperCompassMode.ToString());
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
                    writer.WriteAttributeString("pin", DisplayPin);
                    writer.WriteAttributeString("pinBrightness", DisplayPinBrightness.ToString());

                    // only write the info if enabled (not many pins can actually set this)
                    if (DisplayPinPWM)
                        writer.WriteAttributeString("pinPwm", DisplayPinPWM.ToString());
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
            clone.ComparisonActive          = this.ComparisonActive;
            clone.ComparisonOperand         = this.ComparisonOperand;
            clone.ComparisonValue           = this.ComparisonValue;
            clone.ComparisonIfValue         = this.ComparisonIfValue;
            clone.ComparisonElseValue       = this.ComparisonElseValue;
            clone.DisplayType               = this.DisplayType;
            clone.DisplaySerial             = this.DisplaySerial;
            clone.DisplayLedReverseDigits   = this.DisplayLedReverseDigits;
            clone.DisplayPin                = this.DisplayPin;
            clone.DisplayPinBrightness      = this.DisplayPinBrightness;
            clone.DisplayPinPWM             = this.DisplayPinPWM;
            // the display stuff
            clone.DisplayLedAddress         = this.DisplayLedAddress;
            clone.DisplayLedConnector       = this.DisplayLedConnector;
            clone.DisplayLedModuleSize      = this.DisplayLedModuleSize;
            clone.DisplayLedPadding         = this.DisplayLedPadding;
            clone.DisplayLedPaddingChar     = this.DisplayLedPaddingChar;
            clone.DisplayLedDigits          = new List<string>(this.DisplayLedDigits); // we have to create an new object to clone, fix for https://forge.simple-solutions.de/issues/307
            clone.DisplayLedDecimalPoints   = new List<string>(this.DisplayLedDecimalPoints);
            clone.DisplayLedBrightnessReference = this.DisplayLedBrightnessReference;
            
            clone.BcdPins                   = new List<string>(this.BcdPins);

            clone.DisplayTrigger            = this.DisplayTrigger;

            clone.ServoAddress              = this.ServoAddress;
            clone.ServoMax                  = this.ServoMax;
            clone.ServoMin                  = this.ServoMin;
            clone.ServoMaxRotationPercent   = this.ServoMaxRotationPercent;

            clone.StepperAddress            = this.StepperAddress;
            clone.StepperInputRev           = this.StepperInputRev;
            clone.StepperOutputRev          = this.StepperOutputRev;
            clone.StepperTestValue          = this.StepperTestValue;
            clone.StepperCompassMode        = this.StepperCompassMode;

            clone.ShiftRegister = this.ShiftRegister;
            clone.RegisterOutputPin = this.RegisterOutputPin;

            clone.LcdDisplay                = this.LcdDisplay.Clone() as OutputConfig.LcdDisplay;

            foreach (Precondition p in Preconditions)
            {
                clone.Preconditions.Add(p.Clone() as Precondition);
            }

            clone.Interpolation = this.Interpolation.Clone() as Interpolation;

            foreach (ConfigRef configRef in ConfigRefs)
            {
                clone.ConfigRefs.Add(configRef.Clone() as ConfigRef);
            }

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
