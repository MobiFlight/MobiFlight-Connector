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

    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    public enum DeviceType
    {
        NotSet,      // 0 
        Button,      // 1
        Encoder,     // 2
        Output,      // 3
        LedModule,   // 4
        Stepper,     // 5
        Servo,       // 6
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
        protected MobiFlight.Config.Config _config = null;

        String _comPort = "COM3";
        public String Port { get { return _comPort; } }
        public String Name { get; set; }
        public String Type { get; set; }
        public String Serial { get; set; }
        public String Version { get; set; }
        public MobiFlight.Config.Config Config { 
            get {
                    if (!Connected) return null;
                    if (_config==null) {
                        var command = new SendCommand((int)MobiFlightModule.Command.GetConfig, (int)MobiFlightModule.Command.Info, 2000);
                        var InfoCommand = _cmdMessenger.SendCommand(command);
                        InfoCommand = _cmdMessenger.SendCommand(command);
                        if (InfoCommand.Ok)
                        {
                            _config = new Config.Config(InfoCommand.ReadStringArg());
                        }
                        else
                            _config = new Config.Config();
                    }
                    return _config;
                }

            set
            {
                _config = value;
            }
        }

        public bool RunLoop { get; set; }
        private SerialTransport _transportLayer;
        //private SerialPortManager _transportLayer;
        private CmdMessenger _cmdMessenger;
        private bool _ledState;
        private int _count;
        private string _log = "";

        Dictionary<String, MobiFlightLedModule> ledModules = new Dictionary<string, MobiFlightLedModule>();
        Dictionary<String, MobiFlightStepper> stepperModules = new Dictionary<string, MobiFlightStepper>();
        Dictionary<String, MobiFlightServo> servoModules = new Dictionary<string, MobiFlightServo>();
        Dictionary<String, MobiFlightOutput> outputs = new Dictionary<string,MobiFlightOutput>();

        Dictionary<String, int> buttonValues = new Dictionary<String, int>();

        public bool Connected { get; set; }

        /// <summary>
        /// the look up table with the last set values
        /// </summary>
        Dictionary<string, string> lastValue = new Dictionary<string, string>();

        

        public enum Command
        {            
            InitModule,     // 0
            SetModule,      // 1
            SetPin,         // 2
            SetStepper,     // 3
            SetServo,       // 4
            Status,         // 5
            EncoderChange,  // 6
            ButtonChange,   // 7
            StepperChange,  // 8
            GetInfo,        // 9
            Info,           // 10
            SetConfig,      // 11
            GetConfig,      // 12
            ResetConfig,    // 13
            SaveConfig,     // 14
            ConfigSaved,    // 15
            ActivateConfig, // 16
            ConfigActivated // 17
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
            
            this.Connected = true;

            LoadConfig();
        }

        public void LoadConfig()
        {           
            ledModules.Clear();
            stepperModules.Clear();
            servoModules.Clear();
            outputs.Clear();
                        
            foreach (Config.BaseDevice device in Config.Items)
            {
                switch(device.Type) {
                    case DeviceType.LedModule:
                        ledModules.Add(device.Name, new MobiFlightLedModule() { CmdMessenger = _cmdMessenger, Name = device.Name, ModuleNumber = ledModules.Count });
                        break;

                    case DeviceType.Stepper:
                        stepperModules.Add(device.Name, new MobiFlightStepper28BYJ() { CmdMessenger = _cmdMessenger, Name = device.Name, StepperNumber = stepperModules.Count });
                        break;
                    
                    case DeviceType.Servo:
                        servoModules.Add(device.Name, new MobiFlightServo() { CmdMessenger = _cmdMessenger, Name = device.Name, ServoNumber = servoModules.Count });
                        break;

                    case DeviceType.Output:
                        outputs.Add(device.Name, new MobiFlightOutput() { CmdMessenger = _cmdMessenger, Name = device.Name, Pin = Int16.Parse((device as Config.Output).Pin) });
                        break;
                }                
            }
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
        /// <param name="port">the virtual port on the board or extension</param>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPin(string port, string pin, int value)
        {
            // if value has not changed since the last time, then we continue to next item to prevent 
            // unnecessary communication with Arcaze USB
            String key = port+pin;

            if (lastValue.ContainsKey(key) &&
                lastValue[key] == value.ToString()) return false;

            lastValue[key] = value.ToString();

            if (!outputs.ContainsKey(pin)) return false;

            outputs[pin].Set(value);

            return true;
        }

        public bool SetDisplay(string name, int module, byte points, byte mask, string value)
        {
            String key = "LED_" + name + "_" + module + "_" + points + "_" + mask;            

            if (lastValue.ContainsKey(key) &&
                lastValue[key] == value) return false;

            lastValue[key] = value;
            ledModules[name].Display(module, value, points, mask);
            return true;
        }

        public bool SetServo(string servoAddress, int value, int min, int max)
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

            servoModules[servoAddress].Min = min;
            servoModules[servoAddress].Max = max;
            servoModules[servoAddress].MoveToPosition(value);
            lastValue[key] = value.ToString();
            return true;
        }

        public bool SetStepper(string stepper, int value)
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

            var command = new SendCommand((int)MobiFlightModule.Command.GetInfo, (int)MobiFlightModule.Command.Info, 1000);
            var InfoCommand = _cmdMessenger.SendCommand(command);
            InfoCommand = _cmdMessenger.SendCommand(command);
            if (InfoCommand.Ok)
            {
                devInfo.Type = InfoCommand.ReadStringArg();
                devInfo.Name = InfoCommand.ReadStringArg();
                devInfo.Serial = InfoCommand.ReadStringArg();                

                Type = devInfo.Type;
                Name = devInfo.Name;
                Serial = devInfo.Serial;
            }

            return devInfo;
        }

        public bool SaveConfig()
        {
            bool isOk = true;
            var command = new SendCommand((int)MobiFlightModule.Command.ResetConfig, (int)MobiFlightModule.Command.Status, 1000);
            _cmdMessenger.SendCommand(command);

            //foreach (MobiFlight.Config.BaseDevice dev in Config.Items)
            //{
            command = new SendCommand((int)MobiFlightModule.Command.SetConfig, (int)MobiFlightModule.Command.Status, 1000);
            command.AddArgument(Config.ToInternal());
            var StatusCommand = _cmdMessenger.SendCommand(command);
            if (!StatusCommand.Ok)
            {
                isOk = false;
                //break;
            }
            //}

            if (isOk)
            {
                command = new SendCommand((int)MobiFlightModule.Command.SaveConfig, (int)MobiFlightModule.Command.ConfigSaved, 1000);
                StatusCommand = _cmdMessenger.SendCommand(command);

                if (StatusCommand.Ok)
                {
                    command = new SendCommand((int)MobiFlightModule.Command.ActivateConfig, (int)MobiFlightModule.Command.ConfigActivated, 1000);
                    StatusCommand = _cmdMessenger.SendCommand(command);
                    isOk = StatusCommand.Ok;
                }
            }

            return isOk;
        }

        public List<IConnectedDevice> GetConnectedDevices()
        {
            List<IConnectedDevice> result = new List<IConnectedDevice>();

            foreach (MobiFlightOutput output in outputs.Values)
            {
                result.Add(output);
            }

            foreach (MobiFlightLedModule ledModule in ledModules.Values)
            {
                result.Add(ledModule);
            }

            foreach (MobiFlightStepper stepper in stepperModules.Values)
            {
                result.Add(stepper);
            }

            foreach (MobiFlightServo servo in servoModules.Values)
            {
                result.Add(servo);
            }

            return result;
        }

        public IEnumerable<DeviceType> GetConnectedOutputDeviceTypes()
        {
            List<DeviceType> result = new List<DeviceType>();
            if (outputs.Count > 0) result.Add(DeviceType.Output);
            if (ledModules.Count > 0) result.Add(DeviceType.LedModule);
            if (stepperModules.Count > 0) result.Add(DeviceType.Stepper);
            if (servoModules.Count > 0) result.Add(DeviceType.Servo);

            return result;
        }

        public IEnumerable<DeviceType> GetConnectedInputDeviceTypes()
        {
            bool _hasButtons = false;
            bool _hasEncoder = false;

            List<DeviceType> result = new List<DeviceType>();
            
            foreach (Config.BaseDevice dev in Config.Items)
            {
                switch (dev.Type)
                {
                    case DeviceType.Button:
                        _hasButtons = true;
                        break;
                    case DeviceType.Encoder:
                        _hasEncoder = true;
                        break;
                }
            }            
            if (_hasButtons) result.Add(DeviceType.Button);
            if (_hasEncoder) result.Add(DeviceType.Encoder);
            
            return result;
        }
    }
}
