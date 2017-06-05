using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;
using MobiFlight.OutputConfig;

namespace MobiFlight
{
    public class MobiFlightLcdDisplay : IConnectedDevice
    {
        public const string TYPE = "Lcd Display";

        public CmdMessenger CmdMessenger { get; set; }
        public int Address  { get; set; }
        public int Cols     { get; set; }
        public int Lines    { get; set; }

        private String _name = "Lcd Display";
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        
        private DeviceType _type = DeviceType.LcdDisplay;
        public DeviceType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        
        protected bool _initialized = false;

        public MobiFlightLcdDisplay()
        {
            Cols = 16;
            Lines = 2;
        }

        protected void Initialize()
        {
            if (_initialized) return;

            // Create command
            /*
            var command = new SendCommand((int)MobiFlightModule.Command.InitModule);
            command.AddArgument(this.ModuleNumber);
            command.AddArgument(this.Brightness);

            // Send command
            CmdMessenger.SendCommand(command);
            */

            _initialized = true;
        }

        public void Display(string address, String value)
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetLcdDisplayI2C);

            command.AddArgument(this.Address);
            command.AddArgument(value);

            Log.Instance.log("Command: SetModule <" + (int)MobiFlightModule.Command.SetLcdDisplayI2C + "," +
                                                      this.Address + "," +
                                                      value +
                                                      ";>", LogSeverity.Info);

            // Send command
            CmdMessenger.SendCommand(command);
        }

        public string Apply(LcdDisplay lcdConfig, string value, List<Tuple<String, String>> replacements)
        {
            String result = "";
            replacements.Add(new Tuple<string, string>("$", value));

            for (int line = 0; line != Lines; line++)
            {
                if (line < lcdConfig.Lines.Count)
                {
                    String cLine = lcdConfig.Lines[line];
                    foreach (Tuple<String, String> rep in replacements)
                    {
                        cLine = _ApplyReplacement(cLine, rep.Item1[0], rep.Item2);
                    }
                    result += cLine + new string(' ', Cols - (cLine.Length));
                } else
                {
                    result += new string(' ', Cols);
                }
                
            }
            return result;
        }

        internal string _ApplyReplacement (String line, char replace, String value)
        {
            String result = "";
            Char[] lineArray = line.ToArray();
            int pos = value.Length - 1;
            for (int j = (lineArray.Count() - 1); j >= 0; j--)
            {
                if (lineArray[j] == replace)
                {
                    lineArray[j] = value[pos];
                    pos--;

                    if (pos < 0) break;
                }
            }
            result += String.Join("", lineArray);

            return result;
        }
    }
}
