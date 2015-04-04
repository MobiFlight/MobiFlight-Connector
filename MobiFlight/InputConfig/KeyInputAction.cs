using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.InputConfig
{
    public class KeyInputAction : InputAction, ICloneable
    {
        public System.Windows.Forms.Keys Key;
        public bool Control;
        public bool Alt;
        public bool Shift;

        override public object Clone()
        {
            KeyInputAction clone = new KeyInputAction();
            clone.Key = Key;
            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            String value = reader["key"];
            if (value.Contains("Ctrl+")) { value = value.Replace("Ctrl+", ""); Control = true; }
            if (value.Contains("Shift+")) { value = value.Replace("Shift+", ""); Shift = true; }
            if (value.Contains("Alt+")) { value = value.Replace("Alt+", ""); Alt = true; }

            KeysConverter kc = new KeysConverter();
            Key = (Keys)kc.ConvertFrom(value);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            String value = "";
            if (Control) value+="Ctrl+";
            if (Shift) value += "Shift+";
            if (Alt) value += "Alt+";
            value += Key.ToString();
            writer.WriteAttributeString("type", "KeyInputAction");
            writer.WriteAttributeString("key", value);
        }

        public override void execute(MobiFlight.Fsuipc2Cache fsuipcCache)
        {
            KeyboardInput.SendKeyAsInput(Key, Control, Alt, Shift);
        }
    }
}
