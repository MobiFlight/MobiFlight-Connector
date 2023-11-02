using CommandMessenger;
using MobiFlight.Base;
using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MobiFlight
{
    public class MobiFlightLedModule : IConnectedDevice
    {
        public const string TYPE = "Display Module";

        public CmdMessenger CmdMessenger { get; set; }
        public int ModuleNumber { get; set; }
        public int Brightness { get; set; }

        private int _subModules = 0;
        public int SubModules
        {
            get { return _subModules; }
            set {
                if (_subModules == value) return;
                _subModules = value;
                ClearState();
            }
        }
        public string ModelType { get; set; }

        List<LedModuleState> _state = new List<LedModuleState>();

        private String _name = "Led Module";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        
        private DeviceType _type = DeviceType.LedModule;
        public DeviceType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        
        protected bool _initialized = false;

        public MobiFlightLedModule()
        {
            Brightness = 15;
            SubModules = 1;
            ModelType = LedModule.MODEL_TYPE_MAX72xx;
        }

        protected void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }

        public void Display(int subModule, String value, byte points, byte mask, bool reverse = false)
        {
            try
            {

            
            if (!_initialized) Initialize();

            if (subModule > 1 && ModelType != LedModule.MODEL_TYPE_MAX72xx) return;

            var command = new SendCommand((int)MobiFlightModule.Command.SetModule);

            if (value.IndexOf(".") >=0 )
            {
                var prefix = CalculateCorrectLeadingSpaces(value);
                points = CalculateCorrectPoints(value, mask);
                value = ReplacePointsBySpaces(value);
                value = prefix + value.Replace(".", "");
            }

            // clamp and reverse the string
            var maxLength = Math.Min(mask.HammingWeight(), (byte) 8);
            if (value.Length > maxLength) value = value.Substring(0, maxLength);

            if (reverse)
            {
                value = new string(value.ToCharArray().Reverse().ToArray());
                points = points.Reverse();
                mask = mask.Reverse();
            }

            // cache hit
            lock(_state) { 
                if (_state[subModule] == null || !_state[subModule].DisplayRequiresUpdate(value, points, mask))
                    return;
            }

            command.AddArgument(this.ModuleNumber);
            command.AddArgument(subModule);            
            command.AddArgument(value);
            command.AddArgument(points);
            command.AddArgument(mask);

            Log.Instance.log($"Command: SetModule <{(int)MobiFlightModule.Command.SetModule},{this.ModuleNumber},{subModule},{value},{points},{mask};>.", LogSeverity.Debug);

            // Send command
            System.Threading.Thread.Sleep(1);
            CmdMessenger.SendCommand(command);
            }
            catch(Exception ex) {
                Log.Instance.log($"Exception {ex.Message}", LogSeverity.Debug);
            }
        }

        private string ReplacePointsBySpaces(string value)
        {
            var regex = new Regex(Regex.Escape(".."));
            while (value.IndexOf("..")>-1)
            {
                value = regex.Replace(value, " .", 1);
            }

            return value;
        }

        private string CalculateCorrectLeadingSpaces(string value)
        {
            var result = "";
            if (value.IndexOf('.') == 0)
                result = " ";

            return result;
        }

        static public byte CalculatePositionInString(string value, byte mask)
        {
            // we have to figure it, how many digits are in use
            var maskLength = Convert.ToString(mask, 2).Count(c => c == '1');

            // we have to find the n-th character
            // where "n" is the maskLength
            // but we have to treat decimal points correctly
            // because they don't occupy a digit
            // unless two are following each other
            // we start with the last character in the value string
            byte positionInValue = 0;

            // we set the last character to .
            // so that in case we have the first one being a "."
            // then we allocate a digit for it correctly
            var lastCharacter = '.';

            // out inital length is 0, because we start on the left
            var length = 0;

            for (byte i = 0; i != value.Length; i++)
            {
                // in case we see a "." but the last character
                // was something different,
                // then we don't have to account for this
                if (value[i] == '.' && lastCharacter != '.')
                {
                    // we store the last character
                    lastCharacter = value[i];
                    continue;
                }

                // in all other cases now we count this
                // character, length can be different from "i"
                length++;

                // we store the last character
                lastCharacter = value[i];

                // once we reach the length
                // we update the positionInValue
                if (length == maskLength)
                {
                    positionInValue = i;
                    if (i + 1 < value.Length && value[i + 1] == '.')
                        positionInValue += 1;
                    break;
                }
            }

            return positionInValue;
        }

        static public byte CalculateCorrectPoints(string value, byte mask)
        {
            byte points = 0;
            byte positionInValue = CalculatePositionInString(value, mask);

            // we go over all 8 potential digits
            for (byte digit=0; digit<8; digit++)
            {
                // if the digit is not active, go to the next
                if (((1 << digit) & mask) == 0)
                    continue;

                // stop when you ran out of value to display
                if (positionInValue < 0)
                    break;

                // when we have a decimal point at the current position
                if (value[positionInValue]=='.')
                {
                    // activate the point at the current digit
                    points |= (byte)(1 << digit);
                    
                    // then we stay on the digit one more time
                    // but only if the next character is not a 
                    // "." too
                    if (positionInValue-1 >=0 && value[positionInValue-1]!='.')
                        digit--;
                }

                // walk one character to the left
                positionInValue--;
            }
            return points;
        }

        public void SetBrightness(int subModule, String value)
        {
            if (!_initialized) Initialize();

            if (isCacheHit(subModule, value))
                return;

            var command = new SendCommand((int)MobiFlightModule.Command.SetModuleBrightness);

            // clamp and reverse the string
            if (value.Length > 8) value = value.Substring(0, 8);
            command.AddArgument(this.ModuleNumber);
            command.AddArgument(subModule);
            command.AddArgument(value);

            Log.Instance.log($"Command: SetModuleBrightness <{(int)MobiFlightModule.Command.SetModuleBrightness},{this.ModuleNumber},{subModule},{value};>.", LogSeverity.Debug);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        private bool isCacheHit(int subModule, string value)
        {
            return _state[subModule] == null || !_state[subModule].SetBrightnessRequiresUpdate(value);
        }

        // Blank the display when stopped
        public void Stop()
        {
            for (int i = 0; i != SubModules; i++)
            {
                Display(i, "        ", 0, 0xff);
            }

            ClearState();
        }

        public void ClearState()
        {
            // we can have cross-thread issues during startup
            lock (_state)
            {
                _state.Clear();
                for (int i = 0; i < SubModules; i++)
                {
                    _state.Add(new LedModuleState());
                }
            }
        }
    }

    public class LedModuleState
    {
        char[] Displays = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        byte Points = 0;
        String Brigthness = "";

        public bool DisplayRequiresUpdate(String value, byte points, byte mask)
        {
            bool DisplayUpdated = false;
            
            byte digit = 8;
            byte pos = 0;
            for (byte i = 0; i < 8; i++)
            {
                digit--;
                if (((1 << digit) & mask) == 0)
                    continue;

                if (pos == value.Length)
                    break;

                if (Displays[digit] != value[pos])
                {
                    Displays[digit] = value[pos];
                    DisplayUpdated = true;
                }
                pos++;
            }

            for (byte i = 0; i < 8; i++)
            {
                if (((1 << i) & mask) == 0)
                    continue;
                
                var cachedBit = (Points & (1 << i));
                var newBit = (points & (1 << i));

                if (cachedBit != newBit)
                {
                    if(cachedBit==0)
                        Points |= (byte)(1 << i);
                    else
                        Points &= (byte)~(1 << i);
                    DisplayUpdated = true;
                }
            }

            return DisplayUpdated;
        }

        public bool SetBrightnessRequiresUpdate(String value)
        {
            if (Brigthness == value)
                return false;

            Brigthness = value;
            return true;
        }

        internal void Reset()
        {
            Displays = new[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
            Points = 0;
            Brigthness = "";
        }
    }
}
