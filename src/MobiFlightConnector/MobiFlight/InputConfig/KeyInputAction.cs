using MobiFlight.Base.Serialization.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.InputConfig
{
    [JsonConverter(typeof(KeyInputActionConverter))]
    public class KeyInputAction : InputAction, ICloneable
    {
        public string Code { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }

        [JsonIgnore]
        public KeyboardInputInterface Keyboard;
        public new const String Label = "MobiFlight - Keyboard Input";
        public const String TYPE = "KeyInputAction";

        override public object Clone()
        {
            KeyInputAction clone = new KeyInputAction();
            clone.Code = Code;
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

            Code = value;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            String value = "";
            if (Control) value += "Ctrl+";
            if (Shift) value += "Shift+";
            if (Alt) value += "Alt+";
            value += Code;
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("key", value);
        }

        public override void execute(
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            var convertedKey = MapCodeToKeys(Code);

            if (convertedKey == Keys.None)
            {
                Log.Instance.log("KeyInputAction: Invalid key code: " + Code, LogSeverity.Error);
                return;
            }

            if (Keyboard != null)
                Keyboard.SendKeyAsInput(convertedKey, Control, Alt, Shift);
            else
                KeyboardInput.SendKeyAsInput(convertedKey, Control, Alt, Shift);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is KeyInputAction)) return false;
            var other = obj as KeyInputAction;

            return
                Code == other.Code &&
                Alt == other.Alt &&
                Shift == other.Shift &&
                Control == other.Control;
        }

        static public Keys MapCodeToKeys(string code)
        {
            var result = Keys.None;
            try
            {
                var kc = new KeysConverter();
                result = (Keys)kc.ConvertFromString(code);
            }
            catch
            {
                if (CodeToKeys.ContainsKey(code))
                {
                    result = CodeToKeys[code];
                }
            }

            return result;
        }

        static Dictionary<string, Keys> CodeToKeys = new Dictionary<string, Keys>()
        {
            {"Backspace", Keys.Back },
            {"Tab", Keys.Tab},
            {"Enter", Keys.Enter},
            {"Escape", Keys.Escape},
            {"Space", Keys.Space},

            {"ArrowLeft", Keys.Left},
            {"ArrowUp", Keys.Up},
            {"ArrowRight", Keys.Right},
            {"ArrowDown", Keys.Down},

            {"Insert", Keys.Insert},
            {"Delete", Keys.Delete},
            {"Home", Keys.Home},
            {"End", Keys.End},
            {"PageUp", Keys.PageUp},
            {"PageDown", Keys.PageDown},

            {"KeyA", Keys.A },
            {"KeyB", Keys.B },
            {"KeyC", Keys.C },
            {"KeyD", Keys.D },
            {"KeyE", Keys.E },
            {"KeyF", Keys.F },
            {"KeyG", Keys.G },
            {"KeyH", Keys.H },
            {"KeyI", Keys.I },
            {"KeyJ", Keys.J },
            {"KeyK", Keys.K },
            {"KeyL", Keys.L },
            {"KeyM", Keys.M },
            {"KeyN", Keys.N },
            {"KeyO", Keys.O },
            {"KeyP", Keys.P },
            {"KeyQ", Keys.Q },
            {"KeyR", Keys.R },
            {"KeyS", Keys.S },
            {"KeyT", Keys.T },
            {"KeyU", Keys.U },
            {"KeyV", Keys.V },
            {"KeyW", Keys.W },
            {"KeyX", Keys.X },
            {"KeyY", Keys.Y },
            {"KeyZ", Keys.Z },

            {"Digit0", Keys.D0 },
            {"Digit1", Keys.D1 },
            {"Digit2", Keys.D2 },
            {"Digit3", Keys.D3 },
            {"Digit4", Keys.D4 },
            {"Digit5", Keys.D5 },
            {"Digit6", Keys.D6 },
            {"Digit7", Keys.D7 },
            {"Digit8", Keys.D8 },
            {"Digit9", Keys.D9 },

            {"Numpad0", Keys.NumPad0},
            {"Numpad1", Keys.NumPad1},
            {"Numpad2", Keys.NumPad2},
            {"Numpad3", Keys.NumPad3},
            {"Numpad4", Keys.NumPad4},
            {"Numpad5", Keys.NumPad5},
            {"Numpad6", Keys.NumPad6},
            {"Numpad7", Keys.NumPad7},
            {"Numpad8", Keys.NumPad8},
            {"Numpad9", Keys.NumPad9},

            {"NumpadAdd", Keys.Add},
            {"NumpadSubtract", Keys.Subtract},
            {"NumpadMultiply", Keys.Multiply},
            {"NumpadDivide", Keys.Divide},
            {"NumpadDecimal", Keys.Decimal},
            {"NumpadEnter", Keys.Enter},
            {"IntlBackslash", Keys.Oem102},

            {"CapsLock", Keys.CapsLock},
            {"NumLock", Keys.NumLock},
            {"ScrollLock", Keys.Scroll},
            {"PrintScreen", Keys.PrintScreen},
            {"Pause", Keys.Pause},
            {"ContextMenu", Keys.Apps},

            {"F1", Keys.F1},
            {"F2", Keys.F2},
            {"F3", Keys.F3},
            {"F4", Keys.F4},
            {"F5", Keys.F5},
            {"F6", Keys.F6},
            {"F7", Keys.F7},
            {"F8", Keys.F8},
            {"F9", Keys.F9},
            {"F10", Keys.F10},
            {"F11", Keys.F11},
            {"F12", Keys.F12},

            {"MetaLeft", Keys.LWin},
            {"MetaRight", Keys.RWin},
            {"ShiftLeft", Keys.LShiftKey},
            {"ShiftRight", Keys.RShiftKey},
            {"ControlLeft", Keys.LControlKey},
            {"ControlRight", Keys.RControlKey},
            {"AltLeft", Keys.LMenu},
            {"AltRight", Keys.RMenu},

            {"Period", Keys.OemPeriod},
            {"Comma", Keys.Oemcomma},
            {"Minus", Keys.OemMinus},
            {"Equal", Keys.Oemplus},
            {"Semicolon", Keys.Oem1},
            {"Slash", Keys.Oem2},
            {"Backquote", Keys.Oem3},
            {"BracketLeft", Keys.Oem4},
            {"Backslash", Keys.Oem5},
            {"BracketRight", Keys.Oem6},
            {"Quote", Keys.Oem7},
        };
    }
}