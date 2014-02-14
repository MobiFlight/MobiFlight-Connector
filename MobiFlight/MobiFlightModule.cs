using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using CommandMessenger;
using CommandMessenger.TransportLayer;
using FSUIPC;

namespace MobiFlight
{
    public class ButtonArgs : EventArgs    
    {
        public string ButtonId { get; set; }
        public int Value { get; set; }
    }

    public class MobiFlightModule : IModule, IOutputModule
    {
        public delegate void ButtonEventHandler(object sender, ButtonArgs e);
        /// <summary>
        /// Gets raised whenever a button is pressed
        /// </summary>
        public event ButtonEventHandler OnButtonPressed;

        delegate void AddLogCallback(string text);
        SerialPort _serialPort;

        String _comPort = "COM3";
        public String Port { get { return _comPort; } }
        public String Name { get; set; }
        public String Type { get; set; }
        public String Serial { get; set; }
        public String Version { get; set; }

        public bool RunLoop { get; set; }
        private SerialTransport _transportLayer;
        //private SerialPortManager _transportLayer;
        private CmdMessenger _cmdMessenger;
        private bool _ledState;
        private int _count;
        private string _log = "";

        List<MobiFlightLedModule> ledModules = new List<MobiFlightLedModule>();
        List<MobiFlightStepper> stepperModules = new List<MobiFlightStepper>();
        List<MobiFlightServo> servoModules = new List<MobiFlightServo>();
        Dictionary<String, int> buttonValues = new Dictionary<String, int>();


        public bool Connected { get; set; }

        /// <summary>
        /// the look up table with the last set values
        /// </summary>
        Dictionary<string, string> lastValue = new Dictionary<string, string>();

        // This is the list of recognized commands. These can be commands that can either be sent or received. 
        // In order to receive, attach a callback function to these events
        public enum Command
        {            
            InitModule,    // 0
            SetModule,     // 1
            SetPin,        // 2
            SetStepper,    // 3
            SetServo,      // 4
            Status,        // 5
            EncoderChange, // 6
            ButtonChange,  // 7
            StepperChange, // 8
            GetInfo,       // 9
            Info,          // 10
            SetConfig,     // 11
            GetConfig,     // 12
        };
        
        public MobiFlightModule(MobiFlightModuleConfig config)
        {
            Name = "Default";
            UpdateConfig(config);
        }

        public void UpdateConfig (MobiFlightModuleConfig config) 
        {
            _comPort = config.ComPort;            
        }

        public void Connect()
        {
            if (this.Connected) return;

            // Create Serial Port object
            _transportLayer = new SerialTransport
            //_transportLayer = new SerialPortManager
            {
                CurrentSerialSettings = { PortName = _comPort, BaudRate = 115200, DtrEnable = true } // object initializer
            };

            _cmdMessenger = new CmdMessenger(_transportLayer);

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Start listening            
            _cmdMessenger.StartListening();

            // add Led Modules
            ledModules.Clear();
            ledModules.Add(new MobiFlightLedModule() { CmdMessenger = _cmdMessenger, ModuleNumber = 0 });

            stepperModules.Clear();
            stepperModules.Add(new MobiFlightStepper28BYJ() { CmdMessenger = _cmdMessenger, StepperNumber = 0 });
            this.Connected = true;
        }

        public void Disconnect()
        {
            this.Connected = false;
            _cmdMessenger.StopListening();
            _cmdMessenger.Dispose();
            _transportLayer.Dispose();
        }
        
        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Status, OnStatus);
            _cmdMessenger.Attach((int)Command.Info, OnInfo);
            _cmdMessenger.Attach((int)Command.EncoderChange, OnEncoderChange);
            _cmdMessenger.Attach((int)Command.ButtonChange, OnButtonChange);
        }

        /// Executes when an unknown command has been received.
        void OnUnknownCommand(ReceivedCommand arguments)
        {
            //addLog("Command without attached callback received\n");
            return;
        }

        // Callback function that prints the Arduino status to the console
        void OnStatus(ReceivedCommand arguments)
        {
            _log = arguments.ReadStringArg();
            //addLog("Arduino status: ");
            //addLog(arguments.ReadStringArg() + "\n");
        }

        // Callback function that prints the Arduino status to the console
        void OnInfo(ReceivedCommand arguments)
        {
            String port = arguments.ReadStringArg();
            String type = arguments.ReadStringArg();
            String name = arguments.ReadStringArg();
            //addLog("Arduino status: ");
            //addLog(arguments.ReadStringArg() + "\n");
        }

        // Callback function that prints the Arduino status to the console
        void OnEncoderChange(ReceivedCommand arguments)
        {
            String enc = arguments.ReadStringArg();
            String pos = arguments.ReadStringArg();
            int value;
            if (!int.TryParse(pos, out value)) return;

            OnButtonPressed(this, new ButtonArgs() { ButtonId = enc, Value = value});
            //addLog("Enc: " + enc + ":" + pos);
        }

        // Callback function that prints the Arduino status to the console
        void OnButtonChange(ReceivedCommand arguments)
        {
            String button = arguments.ReadStringArg();
            String state = arguments.ReadStringArg();
            //addLog("Button: " + button + ":" + state);
            OnButtonPressed(this, new ButtonArgs() { ButtonId = button, Value = int.Parse(state) });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group">the virtual port on the board or extension</param>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPin(string group, int pin, int value)
        {
            // if value has not changed since the last time, then we continue to next item to prevent 
            // unnecessary communication with Arcaze USB
            String key = group + "_" + pin.ToString();

            if (lastValue.ContainsKey(key) &&
                lastValue[key] == value.ToString()) return false;

            lastValue[key] = value.ToString();
            
            var command = new SendCommand((int)MobiFlightModule.Command.SetPin);
            command.AddArgument(pin);
            command.AddArgument(value);
            // Send command
            _cmdMessenger.SendCommand(command);

            return true;
        }

        public bool SetDisplay(int module, int pos, string value)
        {
            String key = "LED_" + module;            

            if (lastValue.ContainsKey(key) &&
                lastValue[key] == value) return false;

            lastValue[key] = value;
            ledModules[module].Display(value);
            return true;
        }

        public bool SetServo(int servoAddress, int value)
        {
            String key = "SERVO_" + servoAddress;

            int iLastValue;
            if (lastValue.ContainsKey(key))
            {
                if (lastValue[key] == value.ToString()) return false;
                iLastValue = int.Parse(lastValue[key]);
            }
            else
            {
                iLastValue = value;
            }

            servoModules[servoAddress].MoveToPosition(value);
            lastValue[key] = value.ToString();
            return true;
        }

        public bool SetStepper(int stepper, int value)
        {
            String key = "STEPPER_" + stepper;

            int iLastValue;
            if (lastValue.ContainsKey(key))
            {
                if (lastValue[key] == value.ToString()) return false;
                iLastValue = int.Parse(lastValue[key]);
            }
            else
            {
                iLastValue = value;
            }

            stepperModules[stepper].MoveToPosition(value, (value - iLastValue) > 0);
            lastValue[key] = value.ToString();
            return true;
        }

        public IModuleInfo GetInfo()
        {
            MobiFlightModuleInfo devInfo = new MobiFlightModuleInfo() { Name = "Unknown", Type = MobiFlightModuleInfo.TYPE_UNKNOWN, Port = _comPort };

            var command = new SendCommand((int)MobiFlightModule.Command.GetInfo,(int)MobiFlightModule.Command.Info, 1000);
            var InfoCommand = _cmdMessenger.SendCommand(command);
            InfoCommand = _cmdMessenger.SendCommand(command);
            if (InfoCommand.Ok)
            {
                devInfo.Type = InfoCommand.ReadStringArg();
                devInfo.Name = InfoCommand.ReadStringArg();
                devInfo.Serial = InfoCommand.ReadStringArg();
            }

            Type = devInfo.Type;
            Name = devInfo.Name;
            Serial = devInfo.Serial;
            
            return devInfo;
        }

        public List<IConnectedDevice> GetConnectedDevices()
        {
            List<IConnectedDevice> result = new List<IConnectedDevice>();
            foreach (MobiFlightLedModule ledModule in ledModules)
            {
                result.Add(ledModule);
            }

            foreach (MobiFlightStepper stepper in stepperModules)
            {
                result.Add(stepper);
            }

            return result;
        }
    }
}
