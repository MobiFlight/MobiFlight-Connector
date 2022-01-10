﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MobiFlight;
using MobiFlight.InputConfig;
using MobiFlight.Base;
using MobiFlight.Config;

namespace MobiFlight
{
    public class InputConfigItem : IBaseConfigItem, IXmlSerializable, ICloneable, IConfigRefConfigItem
    {
        // we initialize a cultureInfo object 
        // which is used for serialization
        // independently from current cultureInfo
        // @see: https://forge.simple-solutions.de/issues/275
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");
        public const String TYPE_NOTSET = "";
        public const String TYPE_BUTTON = "Button";
        public const String TYPE_ENCODER = "Encoder";
        public const String TYPE_INPUT_SHIFT_REGISTER = "InputShiftRegister";
        public const String TYPE_DIG_INPUT_MUX = "DigInputMux";
        public const String TYPE_ANALOG = "Analog";

        public string ModuleSerial { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public ButtonInputConfig button { get; set; }
        public EncoderInputConfig encoder { get; set; }
        public InputShiftRegisterConfig inputShiftRegister { get; set; }
        public DigInputMuxConfig digInputMux { get; set; }
        public AnalogInputConfig analog { get; set; }
        public PreconditionList Preconditions { get; set; }
        public ConfigRefList ConfigRefs { get; set; }

        public InputConfigItem()
        {
            Preconditions = new PreconditionList();
            Type = TYPE_NOTSET;

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
            return result;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            ModuleSerial = reader["serial"];
            Name = reader["name"];
            if (reader["type"] != null && reader["type"] != "")
            {
                Type = reader["type"];
            }

            reader.Read(); // this should be the button or encoder
            if (reader.LocalName == "button")
            {
                button = new ButtonInputConfig();
                button.ReadXml(reader);
                if (reader.NodeType != XmlNodeType.EndElement) reader.Read(); // this should be the corresponding "end" node
                reader.Read(); // advance to the next node
            }

            if (reader.LocalName == "encoder")
            {
                encoder = new EncoderInputConfig();
                encoder.ReadXml(reader);
                if (reader.NodeType != XmlNodeType.EndElement) reader.Read(); // this should be the corresponding "end" node
                reader.Read(); // advance to the next node
            }

            if (reader.LocalName == "inputShiftRegister")
            {
                inputShiftRegister = new InputShiftRegisterConfig();
                inputShiftRegister.ReadXml(reader);
                if (reader.NodeType != XmlNodeType.EndElement) reader.Read(); // this should be the corresponding "end" node
                reader.Read(); // advance to the next node
            }

            if (reader.LocalName == "digInputMux")
            {
                digInputMux = new DigInputMuxConfig();
                digInputMux.ReadXml(reader);
                if (reader.NodeType != XmlNodeType.EndElement) reader.Read(); // this should be the corresponding "end" node
                reader.Read(); // advance to the next node
            }

            if (reader.LocalName == "analog")
            {
                analog = new AnalogInputConfig();
                analog.ReadXml(reader);
                if (reader.NodeType != XmlNodeType.EndElement) reader.Read(); // this should be the corresponding "end" node
                reader.Read(); // advance to the next node
            }

            // this is fallback, because type was not set in the past
            if (Type == TYPE_NOTSET)
            {
                if (button != null)
                    Type = TYPE_BUTTON;
                if (encoder != null)
                    Type = TYPE_ENCODER;
            }

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
                        reader.Read();
                    } while (reader.LocalName == "precondition");
                }
                if (reader.NodeType != XmlNodeType.EndElement) reader.Read(); // this should be the corresponding "end" node
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
                        reader.Read();
                    } while (reader.LocalName == "configref");
                }

                reader.Read(); // advance to the next
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("serial", this.ModuleSerial);
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("type", this.Type);

            if (button != null)
            {
                writer.WriteStartElement("button");
                button.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (encoder != null)
            {
                writer.WriteStartElement("encoder");
                encoder.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (inputShiftRegister != null)
            {
                writer.WriteStartElement("inputShiftRegister");
                inputShiftRegister.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (digInputMux != null)
            {
                writer.WriteStartElement("digInputMux");
                digInputMux.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (analog != null)
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

        public object Clone()
        {
            InputConfigItem clone = new InputConfigItem();
            clone.ModuleSerial = ModuleSerial;
            clone.Name = Name;
            clone.Type = Type;

            if (button != null)
                clone.button = (ButtonInputConfig)this.button.Clone();

            if (encoder != null)
                clone.encoder = (EncoderInputConfig)this.encoder.Clone();

            if (inputShiftRegister != null)
                clone.inputShiftRegister = (InputShiftRegisterConfig)this.inputShiftRegister.Clone();

            if (digInputMux != null)
                clone.digInputMux = (DigInputMuxConfig)this.digInputMux.Clone();

            if (analog != null)
                clone.analog = (AnalogInputConfig)this.analog.Clone();

            foreach (Precondition p in Preconditions)
            {
                clone.Preconditions.Add(p.Clone() as Precondition);
            }

            foreach (ConfigRef configRef in ConfigRefs)
            {
                clone.ConfigRefs.Add(configRef.Clone() as ConfigRef);
            }

            return clone;
        }

        internal void execute(
            FSUIPC.Fsuipc2Cache fsuipcCache,
            SimConnectMSFS.SimConnectCache simConnectCache,
            MobiFlightCache moduleCache,
            InputEventArgs e,
            List<ConfigRefValue> configRefs)
        {
            switch (Type)
            {
                case TYPE_BUTTON:
                    if (button != null)
                        button.execute(fsuipcCache, simConnectCache, moduleCache, e, configRefs);
                    break;
                case TYPE_ENCODER:
                    if (encoder != null)
                        encoder.execute(fsuipcCache, simConnectCache, moduleCache, e, configRefs);
                    break;

                case TYPE_INPUT_SHIFT_REGISTER:
                    if (inputShiftRegister != null)
                        inputShiftRegister.execute(fsuipcCache, simConnectCache, moduleCache, e, configRefs);
                    break;

                case TYPE_DIG_INPUT_MUX:
                    if (digInputMux != null)
                        digInputMux.execute(fsuipcCache, simConnectCache, moduleCache, e, configRefs);
                    break;

                case TYPE_ANALOG:
                    if (analog != null)
                        analog.execute(fsuipcCache, simConnectCache, moduleCache, e, configRefs);
                    break;
            }
        }

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            if (Type == TYPE_BUTTON)
            {
                // explicit test is needed 
                // in some older version we didn't save the node correctly
                if (button != null)
                    result = button?.GetStatistics();

            } else if (Type == TYPE_ENCODER)
            {
                // explicit test is needed 
                // in some older version we didn't save the node correctly
                if (encoder != null)
                    result = encoder.GetStatistics();
            }
            else if (Type == TYPE_ANALOG)
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
            bool areSame = (obj != null && obj is InputConfigItem) &&
                            ModuleSerial == (obj as InputConfigItem).ModuleSerial &&
                            Name == (obj as InputConfigItem).Name &&
                            Type == (obj as InputConfigItem).Type;

            if (areSame) {
                areSame = areSame && ((button == null && (obj as InputConfigItem).button == null) || (button != null && button.Equals((obj as InputConfigItem).button)));
                areSame = areSame && ((encoder == null && (obj as InputConfigItem).encoder == null) || (encoder != null && encoder.Equals((obj as InputConfigItem).encoder)));
                areSame = areSame && ((analog == null && (obj as InputConfigItem).analog == null) || (analog != null && analog.Equals((obj as InputConfigItem).analog)));
                areSame = areSame && ((inputShiftRegister == null && (obj as InputConfigItem).inputShiftRegister == null) || (inputShiftRegister != null && inputShiftRegister.Equals((obj as InputConfigItem).inputShiftRegister)));
                areSame = areSame && ((digInputMux == null && (obj as InputConfigItem).digInputMux == null) || (digInputMux != null && digInputMux.Equals((obj as InputConfigItem).digInputMux)));

                areSame = areSame && 
                            Preconditions.Equals((obj as InputConfigItem).Preconditions) &&
                            ConfigRefs.Equals((obj as InputConfigItem).ConfigRefs);
            }

            return areSame;
        }
    }
}
