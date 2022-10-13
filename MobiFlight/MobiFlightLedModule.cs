using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using CommandMessenger;
using Newtonsoft.Json.Linq;
using SharpDX.DirectInput;

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
        }

        protected void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }

        public void Display(int subModule, String value, byte points, byte mask)
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetModule);

            // clamp and reverse the string
            if (value.Length > 8) value = value.Substring(0, 8);

            // cache hit
            if (_state[subModule] == null || !_state[subModule].DisplayRequiresUpdate(value, points, mask))
                return;

            command.AddArgument(this.ModuleNumber);
            command.AddArgument(subModule);            
            command.AddArgument(value);
            command.AddArgument(points);
            command.AddArgument(mask);

            Log.Instance.log("Command: SetModule <" + (int)MobiFlightModule.Command.SetModule + "," +
                                                      this.ModuleNumber + "," +
                                                      subModule + "," +
                                                      value + "," +
                                                      points + "," +
                                                      mask + ";>", LogSeverity.Debug);

            // Send command
            CmdMessenger.SendCommand(command);
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

            Log.Instance.log("Command: SetModuleBrightness <" + (int)MobiFlightModule.Command.SetModuleBrightness + "," +
                                                      this.ModuleNumber + "," +
                                                      subModule + "," +
                                                      value + ";>", LogSeverity.Debug);
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
            _state.Clear();
            for (int i = 0; i < SubModules; i++)
            {
                _state.Add(new LedModuleState());
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
