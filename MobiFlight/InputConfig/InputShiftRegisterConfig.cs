using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{
    public class InputShiftRegisterConfig : ButtonInputConfig
    {
        public int pin;

        public new object Clone()
        {
            InputShiftRegisterConfig clone = new InputShiftRegisterConfig();
            if (onPress != null) clone.onPress = (InputAction)onPress.Clone();
            if (onRelease != null) clone.onRelease = (InputAction)onRelease.Clone();
            clone.pin = pin;
            return clone;
        }

        public new void ReadXml(System.Xml.XmlReader reader)
        {
            pin = Convert.ToInt32(reader.GetAttribute(pin));
            base.ReadXml(reader);
        }

        public new void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("pin", pin.ToString());
            base.WriteXml(writer);
        }
    }
}
