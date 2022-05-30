using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

// from InputMultiplexerConfig

namespace MobiFlight.InputConfig
{
    // Since digital inputs on a multiplexer are really just a bunch of buttons, 
    // deriving from ButtonInputConfig saves copying over a ton of code 
    // for reading/writing XML and executing actions and ensures
    // its fundamental capabilities stay in sync with buttons.
    public class InputMultiplexerConfig : ButtonInputConfig
    {
        public int DataPin;

        public new object Clone()
        {
            InputMultiplexerConfig clone = new InputMultiplexerConfig();
            if (onPress != null) clone.onPress = (InputAction)onPress.Clone();
            if (onRelease != null) clone.onRelease = (InputAction)onRelease.Clone();
            clone.DataPin = DataPin;
            return clone;
        }

        public new void ReadXml(System.Xml.XmlReader reader)
        {
            DataPin = Convert.ToInt32(reader.GetAttribute(DataPin));
            base.ReadXml(reader);
        }

        public new void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("DataPin", DataPin.ToString());
            base.WriteXml(writer);
        }

        public override bool Equals(object obj)
        {
            // Digital input multiplexer configurations are equal when their DataPin is the same
            // and all of the button configuration from the base class matches.
            return (obj is InputMultiplexerConfig) && ((obj as InputMultiplexerConfig).DataPin == DataPin) && base.Equals(obj);
        }

        public new Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            result["Input.InputMultiplexer"] = 1;

            if (onPress != null)
            {
                result["Input.OnPress"] = 1;
                result["Input." + onPress.GetType().Name] = 1;
            }

            if (onRelease != null)
            {
                result["Input.OnPress"] = 1;
                result["Input." + onRelease.GetType().Name] = 1;
            }

            return result;
        }
    }
}