using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{
    // Since input shift registers are really just a bunch of buttons, deriving from ButtonInputConfig
    // saves copying over a ton of code for reading/writing XML and executing actions and ensures its
    // fundamental capabilities stay in sync with buttons.
    public class InputShiftRegisterConfig : ButtonInputConfig
    {
        public int ExtPin;

        public new object Clone()
        {
            InputShiftRegisterConfig clone = new InputShiftRegisterConfig();
            if (onPress != null) clone.onPress = (InputAction)onPress.Clone();
            if (onRelease != null) clone.onRelease = (InputAction)onRelease.Clone();
            clone.ExtPin = ExtPin;
            return clone;
        }

        public new void ReadXml(System.Xml.XmlReader reader)
        {
            ExtPin = Convert.ToInt32(reader.GetAttribute(ExtPin));
            base.ReadXml(reader);
        }

        public new void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("ExtPin", ExtPin.ToString());
            base.WriteXml(writer);
        }

        public override bool Equals(object obj)
        {
            // Input shift registers configurations are equal when their ExtPin values are the same and all of
            // the button configuration from the base class matches.
            return (obj is InputShiftRegisterConfig) && ((obj as InputShiftRegisterConfig).ExtPin == ExtPin) && base.Equals(obj);
        }

        public new Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            result["Input.InputShiftRegister"] = 1;

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