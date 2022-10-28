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
        public KeyboardInputInterface Keyboard;
        public new const String Label = "MobiFlight - Keyboard Input";
        public const String TYPE = "KeyInputAction";

        override public object Clone()
        {
            KeyInputAction clone = new KeyInputAction();
            clone.Key = Key;
            clone.Alt = Alt;
            clone.Shift = Shift;
            clone.Control = Control;

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
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("key", value);
        }

        public override void execute(
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            if (Keyboard != null)
                Keyboard.SendKeyAsInput(Key, Control, Alt, Shift);
            else
                KeyboardInput.SendKeyAsInput(Key, Control, Alt, Shift);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is KeyInputAction &&
                Key == (obj as KeyInputAction).Key &&
                Alt == (obj as KeyInputAction).Alt &&
                Shift == (obj as KeyInputAction).Shift &&
                Control == (obj as KeyInputAction).Control;
        }
    }
}
