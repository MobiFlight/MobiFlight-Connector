#define COMMAND_MESSENGER_3_6
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using CommandMessenger;
using CommandMessenger.TransportLayer;
using FSUIPC;
using System.Text.RegularExpressions;
#if COMMAND_MESSENGER_3_6
using CommandMessenger.Serialport;
//using CommandMessenger.Transport.Serial;
#endif
using System.Threading;

namespace MobiFlight
{
    public class InputEventArgs : EventArgs    
    {
        public string Serial { get; set; }
        public string DeviceId { get; set; }
        public DeviceType Type { get; set; }
        public int Value { get; set; }
    }

    public class FirmwareFeature
    {
        public const string GenerateSerial = "1.3.0";
        public const string SetName        = "1.6.0";
    }

    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    public enum DeviceType
    {
        NotSet,              // 0 
        Button,              // 1
        EncoderSingleDetent, // 2 (retained for backwards compatibility, use Encoder for new configs)
        Output,              // 3
        LedModule,           // 4
        StepperDeprecated,   // 5
        Servo,               // 6
        LcdDisplay,          // 7
        Encoder,             // 8
        Stepper,             // 9
        ShiftRegister,       // 10
        AnalogInput          // 11
    }

    public class MobiFlightModule : IModule, IOutputModule
    {
        public enum Command
        {
            InitModule,             // 0
            SetModule,              // 1
            SetPin,                 // 2
            SetStepper,             // 3
            SetServo,               // 4
            Status,                 // 5
            EncoderChange,          // 6
            ButtonChange,           // 7
            StepperChange,          // 8
            GetInfo,                // 9
            Info,                   // 10
            SetConfig,              // 11
            GetConfig,              // 12
            ResetConfig,            // 13
            SaveConfig,             // 14
            ConfigSaved,            // 15
            ActivateConfig,         // 16
            ConfigActivated,        // 17
            SetPowerSavingMode,     // 18
            SetName,                // 19
            GenNewSerial,           // 20
            ResetStepper,           // 21
            SetZeroStepper,         // 22
            Retrigger,              // 23
            ResetBoard,             // 24
            SetLcdDisplayI2C,       // 25
            SetModuleBrightness,    // 26,
            SetShiftRegisterPins,   // 27
            AnalogChange            // 28           
        };

        public delegate void InputDeviceEventHandler(object sender, InputEventArgs e);
        /// <summary>
        /// Gets raised whenever a button is pressed
        /// </summary>
        public event InputDeviceEventHandler OnInputDeviceAction;

        delegate void AddLogCallback(string text);
        SerialPort _serialPort;
        protected Config.Config _config = null;

        /// <summary>
        /// max length of device name
        /// </summary>
        /// <see href="https://bitbucket.org/mobiflight/mobiflightfc/issues/195/length-of-device-names-occure-in-missing"/>
        public const byte MaxDeviceNameLength = 16;

        /// <summary>
        /// characters that are not allowed in device names to prevent any conflict when parsing EEPROM text
        /// </summary>
        public static List<string> ReservedChars = new List<string>
		{
			@":",
            @".",
			@";",
			@",",
			@"#",
			@"/",
			@"|"
		};
        
        String _comPort = "COM3";
        public String Port { get { return _comPort; } }
        public String Name { get; set; }
        public String Type { get; set; }
        public String ArduinoType { get { 
                if (Type == MobiFlightModuleInfo.TYPE_ARDUINO_MICRO || Type == MobiFlightModuleInfo.TYPE_MICRO) return MobiFlightModuleInfo.TYPE_ARDUINO_MICRO;
                if (Type == MobiFlightModuleInfo.TYPE_ARDUINO_MEGA || Type == MobiFlightModuleInfo.TYPE_MEGA) return MobiFlightModuleInfo.TYPE_ARDUINO_MEGA;
                if (Type == MobiFlightModuleInfo.TYPE_ARDUINO_UNO || Type == MobiFlightModuleInfo.TYPE_UNO) return MobiFlightModuleInfo.TYPE_ARDUINO_UNO;
                return MobiFlightModuleInfo.TYPE_UNKNOWN;
                } }
        public String Serial { get; set; }
        public String Version { get; set; }
        public Config.Config Config { 
            get {                    
                    if (_config==null) {
                        if (!Connected) return null;
                        
                        var command = new SendCommand((int)MobiFlightModule.Command.GetConfig, (int)MobiFlightModule.Command.Info, CommandTimeout);
                        var InfoCommand = _cmdMessenger.SendCommand(command);

                        // Sometimes first attempt times out.
                        if (!InfoCommand.Ok)
                        {
                            Log.Instance.log("MobiflightModule.Config: Timeout. !InfoCommand.Ok. Retrying...", LogSeverity.Debug);
                            InfoCommand = _cmdMessenger.SendCommand(command);
                        }
                        
                        if (Type == MobiFlightModuleInfo.TYPE_UNO || Type == MobiFlightModuleInfo.TYPE_ARDUINO_UNO)
                        {
                            if (!InfoCommand.Ok)
                            InfoCommand = _cmdMessenger.SendCommand(command);
                            if (!InfoCommand.Ok)
                            InfoCommand = _cmdMessenger.SendCommand(command);
                        }

                        if (InfoCommand.Ok)
                        {
                            _config = new Config.Config(InfoCommand.ReadStringArg());
                        }
                        else
                        {
                            Log.Instance.log("MobiflightModule.Config: !InfoCommand.Ok. Init with empty config.", LogSeverity.Debug);
                            _config = new Config.Config();
                        }
                            
                    }
                    return _config;
                }

            set
            {
                _config = value;
            }
        }

        public const int CommandTimeout = 2000;
        
        const int KeepAliveIntervalInMinutes = 5; // 5 Minutes
        DateTime lastUpdate = new DateTime();

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
        Dictionary<String, MobiFlightLcdDisplay> lcdDisplays = new Dictionary<string, MobiFlightLcdDisplay>();
        Dictionary<String, MobiFlightButton> buttons = new Dictionary<string, MobiFlightButton>();
        Dictionary<String, MobiFlightEncoder> encoders = new Dictionary<string, MobiFlightEncoder>();
        Dictionary<String, MobiFlightAnalogInput> analogInputs = new Dictionary<string, MobiFlightAnalogInput>();
        Dictionary<String, MobiFlightShiftRegister> shiftRegisters = new Dictionary<string, MobiFlightShiftRegister>();

        Dictionary<String, int> buttonValues = new Dictionary<String, int>();

        private bool connected;
        public bool Connected {
            get { return connected; }
        }

        /// <summary>
        /// the look up table with the last set values
        /// </summary>
        Dictionary<string, string> lastValue = new Dictionary<string, string>();
        public int MaxMessageSize
        {
            get;
            set;
        }
        public int EepromSize
        {
            get;
            set;
        }

        

        
        
        public MobiFlightModule(MobiFlightModuleConfig config)
        {
            Name = "Default";
            Version = "n/a"; // this is simply unknown, in case of an unflashed Arduino
            Serial = "n/a"; // this is simply unknown, in case of an unflashed Arduino
            this.MaxMessageSize = MobiFlightModuleInfo.MESSAGE_MAX_SIZE_MICRO;
            this.EepromSize = MobiFlightModuleInfo.EEPROM_SIZE_MICRO;
            UpdateConfig(config);
        }

        public void UpdateConfig (MobiFlightModuleConfig config) 
        {
            Type = config.Type;
            _comPort = config.ComPort;            
        }

        public void Connect()
        {
            if (this.Connected)
            {
                Log.Instance.log("MobiflightModule.connect: Already connected to " + this.Name + " at " + _comPort + " of Type " + Type, LogSeverity.Warn);
                return;
            }

            // Create Serial Port object
            bool dtrEnable = (Type == MobiFlightModuleInfo.TYPE_ARDUINO_MICRO || Type == MobiFlightModuleInfo.TYPE_MICRO);
            int baudRate = 115200;
            //baudRate = 57600;
            _transportLayer = new SerialTransport
            //_transportLayer = new SerialPortManager
            {
                //CurrentSerialSettings = { PortName = _comPort, BaudRate = 115200, DtrEnable = dtrEnable } // object initializer
                CurrentSerialSettings = { PortName = _comPort, BaudRate = baudRate, DtrEnable = true } // object initializer
            };

            _cmdMessenger = new CmdMessenger(_transportLayer)
#if COMMAND_MESSENGER_3_6
            {
                BoardType = BoardType.Bit16 // Set if it is communicating with a 16- or 32-bit Arduino board
            }
#endif
            ;

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Start listening    
            var status = _cmdMessenger.Connect();
            Log.Instance.log("MobiflightModule.connect: Connected to " + this.Name + " at " + _comPort + " of Type " + Type + " (DTR=>" + _transportLayer.CurrentSerialSettings.DtrEnable + ")", LogSeverity.Info);
            //this.Connected = status;
            this.connected = true;
            
            // this sleep helps during initialization
            // without this line modules did not connect properly
            System.Threading.Thread.Sleep(1250);

            // workaround ahead!!!
            if (Type == MobiFlightModuleInfo.TYPE_UNO || Type == MobiFlightModuleInfo.TYPE_ARDUINO_UNO)
                System.Threading.Thread.Sleep(500);

            //if (!this.Connected) return;
            //ResetBoard();
            LoadConfig();
        }

        public void ResetBoard()
        {
            var command = new SendCommand((int)MobiFlightModule.Command.ResetBoard);
            var InfoCommand = _cmdMessenger.SendCommand(command);
        }

        public void LoadConfig()
        {           
            ledModules.Clear();
            stepperModules.Clear();
            servoModules.Clear();
            outputs.Clear();
            lcdDisplays.Clear();
            buttons.Clear();
            encoders.Clear();
            analogInputs.Clear();
            shiftRegisters.Clear();

            foreach (Config.BaseDevice device in Config.Items)
            {
                if (device == null) continue; // Can happen during development if trying with an older firmware, which prevents you from starting.

                String deviceName = device.Name;
                switch(device.Type) {
                    case DeviceType.LedModule:
                        int submodules = 1;
                        int.TryParse( (device as Config.LedModule).NumModules, out submodules);
                        int brightness = 15;
                        device.Name = GenerateUniqueDeviceName(ledModules.Keys.ToArray(), device.Name);
                        ledModules.Add(device.Name, new MobiFlightLedModule() { CmdMessenger = _cmdMessenger, Name = device.Name, ModuleNumber = ledModules.Count, SubModules = submodules, Brightness = (device as Config.LedModule).Brightness });
                        break;

                    case DeviceType.Stepper:
                        device.Name = GenerateUniqueDeviceName(stepperModules.Keys.ToArray(), device.Name);
                        stepperModules.Add(device.Name, new MobiFlightStepper28BYJ() { CmdMessenger = _cmdMessenger, Name = device.Name, StepperNumber = stepperModules.Count, HasAutoZero = (device as Config.Stepper).BtnPin != "0" });
                        break;

                    case DeviceType.Servo:
                        device.Name = GenerateUniqueDeviceName(servoModules.Keys.ToArray(), device.Name);
                        servoModules.Add(device.Name, new MobiFlightServo() { CmdMessenger = _cmdMessenger, Name = device.Name, ServoNumber = servoModules.Count });
                        break;

                    case DeviceType.Output:
                        device.Name = GenerateUniqueDeviceName(outputs.Keys.ToArray(), device.Name);
                        outputs.Add(device.Name, new MobiFlightOutput() { CmdMessenger = _cmdMessenger, Name = device.Name, Pin = Int16.Parse((device as Config.Output).Pin) });
                        break;

                    case DeviceType.LcdDisplay:
                        device.Name = GenerateUniqueDeviceName(outputs.Keys.ToArray(), device.Name);
                        lcdDisplays.Add(device.Name, new MobiFlightLcdDisplay() { CmdMessenger = _cmdMessenger, Name = device.Name, Address = lcdDisplays.Count, Cols = (device as Config.LcdDisplay).Cols, Lines = (device as Config.LcdDisplay).Lines });
                        break;
                    case DeviceType.Button:
                        device.Name = GenerateUniqueDeviceName(outputs.Keys.ToArray(), device.Name);
                        buttons.Add(device.Name, new MobiFlightButton() { Name = device.Name });
                        break;
                    case DeviceType.Encoder:
                        device.Name = GenerateUniqueDeviceName(outputs.Keys.ToArray(), device.Name);
                        encoders.Add(device.Name, new MobiFlightEncoder() { Name = device.Name });
                        break;
                    case DeviceType.AnalogInput:
                        device.Name = GenerateUniqueDeviceName(outputs.Keys.ToArray(), device.Name);
                        analogInputs.Add(device.Name, new MobiFlightAnalogInput() { Name = device.Name });
                        break;

                    case DeviceType.ShiftRegister:
                        device.Name = GenerateUniqueDeviceName(outputs.Keys.ToArray(), device.Name);
                        int.TryParse((device as Config.ShiftRegister).NumModules, out submodules);
                        shiftRegisters.Add(device.Name, new MobiFlightShiftRegister() { CmdMessenger = _cmdMessenger, Name = device.Name, NumberOfShifters = submodules, ModuleNumber = shiftRegisters.Count});
                        break;
                }                
            }
        }

        public static string GenerateUniqueDeviceName(String[] Keys, String Name)
        {
            String result = Name;
            
            bool renameNecessary = false;
            int renameIndex = 1;
            string renamePat = Name + @"\s\d";
            Regex renameRegex = new Regex(renamePat);

            foreach (String key in Keys)
            {
                Match m = renameRegex.Match(key);
                if (m.Success) renameIndex++;

                if (key == Name)
                {
                    // duplicated name found ... :(
                    // add an index and let user know
                    renameNecessary = true;
                }
            }
            if (renameNecessary)
            {
                result = Name + " " + renameIndex;
            }

            return result;
        }

        public static bool IsValidDeviceName(string Name)
        {
            bool result;
            if (Name.Length > MaxDeviceNameLength)
            {
                result = false;
            }
            else
            {
                System.Collections.Generic.List<string> EscapedReservedChars = MobiFlightModule.ReservedChars;
                for (int i = 0; i != EscapedReservedChars.Count; i++)
                {
                    EscapedReservedChars[i] = Regex.Escape(EscapedReservedChars[i].Replace(@"\", ""));
                }
                result = !Regex.IsMatch(Name, string.Join("|", EscapedReservedChars.ToArray()));
            }
            return result;
        }

        public void Disconnect()
        {
            if (!this.Connected)
            {
                Log.Instance.log("MobiflightModule.disconnect: Already Disconnected " + this.Name + " at " + _comPort, LogSeverity.Info);
                return;
            }

            this.connected = false;
            
#if COMMAND_MESSENGER_3_6    
            _cmdMessenger.Disconnect();
#else
            _cmdMessenger.StopListening();
#endif
            _cmdMessenger.Dispose();
            _transportLayer.Dispose();

            _config = null;

            Log.Instance.log("MobiflightModule.disconnect: Disconnected " + this.Name + " at " + _comPort, LogSeverity.Info);
        }

        public String InitUploadAndReturnUploadPort()
        {
            String result = _comPort;

            List<String> connectedPorts = SerialPort.GetPortNames().ToList();

            Disconnect();
            if (Type == MobiFlightModuleInfo.TYPE_ARDUINO_MICRO || Type == MobiFlightModuleInfo.TYPE_MICRO)
            {
                SerialTransport tmpSerial = new SerialTransport() {
                    CurrentSerialSettings = { PortName = _comPort, BaudRate = 1200, DtrEnable = true } // object initializer
                };
#if COMMAND_MESSENGER_3_6    
                tmpSerial.Connect();
                tmpSerial.Disconnect();
#else
                tmpSerial.StartListening();
                tmpSerial.StopListening();
#endif
                tmpSerial.Dispose();
                Thread.Sleep(1000);
                List<String> connectedPorts2 = SerialPort.GetPortNames().ToList();
                connectedPorts2.Add(_comPort);
                if (connectedPorts2.Except(connectedPorts).Count() > 0)
                {
                    result = connectedPorts2.Except(connectedPorts).Last();
                }
            };
            return result;
        }
        
        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Status, OnStatus);
            _cmdMessenger.Attach((int)Command.Info, OnInfo);
            _cmdMessenger.Attach((int)Command.EncoderChange, OnEncoderChange);
            _cmdMessenger.Attach((int)Command.ButtonChange, OnButtonChange);
            _cmdMessenger.Attach((int)Command.AnalogChange, OnAnalogChange);

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
            
            if (OnInputDeviceAction != null)
                OnInputDeviceAction(this, new InputEventArgs() { Serial = this.Serial, DeviceId = enc, Type = DeviceType.Encoder, Value = value});
            //addLog("Enc: " + enc + ":" + pos);
        }

        // Callback function that prints the Arduino status to the console
        void OnButtonChange(ReceivedCommand arguments)
        {
            String button = arguments.ReadStringArg();
            String state = arguments.ReadStringArg();
            //addLog("Button: " + button + ":" + state);
            if (OnInputDeviceAction != null)
                OnInputDeviceAction(this, new InputEventArgs() { Serial = this.Serial, DeviceId = button, Type = DeviceType.Button, Value = int.Parse(state) });
        }

         // Callback function that prints the Arduino status to the console
        void OnAnalogChange(ReceivedCommand arguments)
        {
            String name = arguments.ReadStringArg();
            String value = arguments.ReadStringArg();
            //addLog("Button: " + button + ":" + state);
            if (OnInputDeviceAction != null)
                OnInputDeviceAction(this, new InputEventArgs() { Serial = this.Serial, DeviceId = name, Type = DeviceType.AnalogInput, Value = int.Parse(value) });
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

            if (!KeepAliveNeeded() && lastValue.ContainsKey(key) &&
                lastValue[key] == value.ToString()) return false;

            lastValue[key] = value.ToString();

            if (!outputs.ContainsKey(pin)) return false;

            outputs[pin].Set(value);

            return true;
        }

        public bool SetDisplay(string name, int module, byte points, byte mask, string value)
        {
            String key = "LED_" + name + "_" + module + "_" + mask;
            String cachedValue = value + "_" + points;

            if (!KeepAliveNeeded() && lastValue.ContainsKey(key) &&
                lastValue[key] == cachedValue) return false;

            lastValue[key] = cachedValue;
            ledModules[name].Display(module, value, points, mask);
            return true;
        }

        public bool SetDisplayBrightness(string name, int module, string value)
        {
            String key = "LEDBrightness_" + name + "_" + module;
            String cachedValue = value;

            if (!KeepAliveNeeded() && lastValue.ContainsKey(key) &&
                lastValue[key] == cachedValue) return false;

            lastValue[key] = cachedValue;
            ledModules[name].SetBrightness(module, value);
            return true;
        }



        public bool SetServo(string servoAddress, int value, int min, int max, byte maxRotationPercent)
        {
            String key = "SERVO_" + servoAddress;

            int iLastValue;
            if (lastValue.ContainsKey(key))
            {
                if (!KeepAliveNeeded() && lastValue[key] == value.ToString()) return false;
                iLastValue = int.Parse(lastValue[key]);
            }
            else
            {
                iLastValue = value;
            }

            servoModules[servoAddress].Min = min;
            servoModules[servoAddress].Max = max;
            servoModules[servoAddress].MaxRotationPercent = maxRotationPercent;
            servoModules[servoAddress].MoveToPosition(value);
            lastValue[key] = value.ToString();
            return true;
        }

        public bool SetStepper(string stepper, int value, int inputRevolutionSteps = -1)
        {
            String key = "STEPPER_" + stepper;

            int iLastValue;
            if (lastValue.ContainsKey(key))
            {
                if (!KeepAliveNeeded() && lastValue[key] == value.ToString()) return false;
                iLastValue = int.Parse(lastValue[key]);
            }
            else
            {
                iLastValue = value;
            }

            if (inputRevolutionSteps > -1)
            {
                stepperModules[stepper].InputRevolutionSteps = inputRevolutionSteps;
            }
            
            stepperModules[stepper].MoveToPosition(value, (value - iLastValue) > 0);
            lastValue[key] = value.ToString();
            return true;
        }

        public bool ResetStepper(string stepper)
        {
            String key = "STEPPER_" + stepper;
            stepperModules[stepper].Reset();
            lastValue[key] = "0";
            return true;
        }

        public bool SetLcdDisplay(string address, string value)
        {
            String key = "LCD_" + address;
            String cachedValue = value;

            if (!KeepAliveNeeded() && lastValue.ContainsKey(key) &&
                lastValue[key] == cachedValue) return false;

            lastValue[key] = cachedValue;

            lcdDisplays[address].Display(address, value);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepper"></param>
        /// <throws>ArgumentOutOfRangeException</throws>
        /// <returns></returns>
        internal MobiFlightStepper GetStepper(string stepper)
        {
            if (!stepperModules.ContainsKey(stepper))
            {
                throw new ArgumentOutOfRangeException();
            }

            return stepperModules[stepper];
        }

        internal bool setShiftRegisterOutput(string moduleID, string outputPin, string value)
        {
            String key = "ShiftReg_" + moduleID + outputPin;
            String cachedValue = value;

            if (!KeepAliveNeeded() && lastValue.ContainsKey(key) &&
                lastValue[key] == cachedValue) return false;

            lastValue[key] = cachedValue;

            shiftRegisters[moduleID].Display(outputPin, value);
            return true;
        }       

        public bool Retrigger ()
        {
            bool isOk = true;
            var command = new SendCommand((int)MobiFlightModule.Command.Retrigger, (int)MobiFlightModule.Command.Status);
            command.AddArgument(Name);
            ReceivedCommand StatusCommand = _cmdMessenger.SendCommand(command);

            isOk = StatusCommand.Ok;
            return isOk;
        }

        public IModuleInfo GetInfo()
        {
            MobiFlightModuleInfo devInfo = new MobiFlightModuleInfo() {
                Name = "Unknown",
                Type = Type,
                Port = _comPort
            };

            var command = new SendCommand((int)MobiFlightModule.Command.GetInfo, (int)MobiFlightModule.Command.Info, CommandTimeout);
            var InfoCommand = _cmdMessenger.SendCommand(command);

            if (InfoCommand.Ok)
            {
                devInfo.Type = InfoCommand.ReadStringArg();
                devInfo.Name = InfoCommand.ReadStringArg();                
                devInfo.Serial = InfoCommand.ReadStringArg();
                String v = InfoCommand.ReadStringArg();
                if (v.IndexOf(":") == -1)
                    devInfo.Version = v;
                else
                {
                    devInfo.Version = "1.0.0";
                }

                Type = devInfo.Type;
                Name = devInfo.Name;
                Version = devInfo.Version;
                Serial = devInfo.Serial;

                MaxMessageSize = MobiFlightModuleInfo.MESSAGE_MAX_SIZE_MICRO;
                EepromSize = MobiFlightModuleInfo.EEPROM_SIZE_MICRO;

                if (ArduinoType == MobiFlightModuleInfo.TYPE_ARDUINO_UNO)
                {
                    MaxMessageSize = MobiFlightModuleInfo.MESSAGE_MAX_SIZE_UNO;
                    EepromSize = MobiFlightModuleInfo.EEPROM_SIZE_UNO;
                } else if (ArduinoType == MobiFlightModuleInfo.TYPE_ARDUINO_MEGA)
                {
                    MaxMessageSize = MobiFlightModuleInfo.MESSAGE_MAX_SIZE_MEGA;
                    EepromSize = MobiFlightModuleInfo.EEPROM_SIZE_MEGA;
                }
            }
            Log.Instance.log("MobiFlightModule.GetInfo: " + Type + ", " + Name + "," + Version + ", " + Serial, LogSeverity.Debug);
            return devInfo;
        }

        public bool SaveName()
        {
            bool isOk = true;
            var command = new SendCommand((int)MobiFlightModule.Command.SetName, (int)MobiFlightModule.Command.Status, CommandTimeout);
            command.AddArgument(Name);
            ReceivedCommand StatusCommand = _cmdMessenger.SendCommand(command);

            isOk = StatusCommand.Ok;

            return isOk;
        }

        public bool SaveConfig()
        {
            bool isOk = SaveName();
            var command = new SendCommand((int)MobiFlightModule.Command.ResetConfig, (int)MobiFlightModule.Command.Status, CommandTimeout);
            Log.Instance.log("Reset config: " + (int)MobiFlightModule.Command.ResetConfig, LogSeverity.Debug);
            _cmdMessenger.SendCommand(command);

            foreach (string MessagePart in this.Config.ToInternal(this.MaxMessageSize))
            {
                Log.Instance.log("Uploading config (Part): " + MessagePart, LogSeverity.Debug);
                command = new SendCommand((int)MobiFlightModule.Command.SetConfig, (int)MobiFlightModule.Command.Status, CommandTimeout);
                command.AddArgument(MessagePart);
                ReceivedCommand StatusCommand = _cmdMessenger.SendCommand(command);
                if (!StatusCommand.Ok)
                {
                    isOk = false;
                    break;
                }
            }
            if (!isOk)
            {
                Log.Instance.log("Error on Uploading.", LogSeverity.Error);
            }
            
            if (isOk)
            {
                command = new SendCommand((int)MobiFlightModule.Command.SaveConfig, (int)MobiFlightModule.Command.ConfigSaved, CommandTimeout);
                Log.Instance.log("Save config: " + (int)MobiFlightModule.Command.SaveConfig, LogSeverity.Debug);
                ReceivedCommand StatusCommand = _cmdMessenger.SendCommand(command);

                if (StatusCommand.Ok)
                {
                    command = new SendCommand((int)MobiFlightModule.Command.ActivateConfig, (int)MobiFlightModule.Command.ConfigActivated, CommandTimeout);
                    StatusCommand = _cmdMessenger.SendCommand(command);
                    isOk = StatusCommand.Ok;
                }
                else
                {
                    if (!isOk)
                    {
                        Log.Instance.log("Error on Saving.", LogSeverity.Error);
                    }
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

            foreach (MobiFlightLcdDisplay lcdDisplay in lcdDisplays.Values)
            {
                result.Add(lcdDisplay);
            }

            foreach (MobiFlightShiftRegister shiftRegister in shiftRegisters.Values)
            {
                result.Add(shiftRegister);
            }

            return result;
        }

        public Dictionary<String, int> GetConnectedDevicesStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();
            result[MobiFlightOutput.TYPE] = outputs.Count;
            result[MobiFlightLedModule.TYPE] = ledModules.Count;
            result[MobiFlightStepper.TYPE] = stepperModules.Count;
            result[MobiFlightServo.TYPE] = servoModules.Count;
            result[MobiFlightLcdDisplay.TYPE] = lcdDisplays.Count;
            result[MobiFlightShiftRegister.TYPE] = shiftRegisters.Count;
            result[MobiFlightButton.TYPE] = buttons.Count;
            result[MobiFlightEncoder.TYPE] = encoders.Count;
            result[MobiFlightAnalogInput.TYPE] = analogInputs.Count;

            return result;
        }

        public List<IConnectedDevice> GetConnectedDevices(String name)
        {
            List<IConnectedDevice> result = new List<IConnectedDevice>();

            foreach (MobiFlightOutput output in outputs.Values)
            {
                if (output.Name == name)
                    result.Add(output);
            }

            foreach (MobiFlightLedModule ledModule in ledModules.Values)
            {
                if (ledModule.Name == name)
                    result.Add(ledModule);
            }

            foreach (MobiFlightStepper stepper in stepperModules.Values)
            {
                if (stepper.Name == name)
                    result.Add(stepper);
            }

            foreach (MobiFlightServo servo in servoModules.Values)
            {
                if (servo.Name == name)
                    result.Add(servo);
            }

            foreach (MobiFlightLcdDisplay lcdDisplay in lcdDisplays.Values)
            {
                if (lcdDisplay.Name == name)
                    result.Add(lcdDisplay);
            }

            foreach (MobiFlightShiftRegister shiftRegister in shiftRegisters.Values)
            {
                if (shiftRegister.Name == name)
                    result.Add(shiftRegister);
            }

            return result;
        }

        public List<MobiFlightPin> getPwmPins()
        {
            switch (this.Type)
            {
                case MobiFlightModuleInfo.TYPE_MICRO:
                    return MobiFlightModuleInfo.MICRO_PINS.FindAll(x=>x.isPWM==true);

                case MobiFlightModuleInfo.TYPE_UNO:
                    return MobiFlightModuleInfo.UNO_PINS.FindAll(x => x.isPWM == true);

                default:
                    return MobiFlightModuleInfo.MEGA_PINS.FindAll(x => x.isPWM == true); ;
            }
        }
        
        public IEnumerable<DeviceType> GetConnectedOutputDeviceTypes()
        {
            List<DeviceType> result = new List<DeviceType>();
            if (outputs.Count > 0) result.Add(DeviceType.Output);
            if (ledModules.Count > 0) result.Add(DeviceType.LedModule);
            if (stepperModules.Count > 0) result.Add(DeviceType.Stepper);
            if (servoModules.Count > 0) result.Add(DeviceType.Servo);
            if (lcdDisplays.Count > 0) result.Add(DeviceType.LcdDisplay);
            if (shiftRegisters.Count > 0) result.Add(DeviceType.ShiftRegister);

            return result;
        }
        
        public IEnumerable<DeviceType> GetConnectedInputDeviceTypes()
        {
            bool _hasButtons = false;
            bool _hasEncoder = false;
            bool _hasAnalog = false;

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

                    case DeviceType.AnalogInput:
                        _hasAnalog = true;
                        break;

                }
            }            
            if (_hasButtons) result.Add(DeviceType.Button);
            if (_hasEncoder) result.Add(DeviceType.Encoder);
            if (_hasAnalog) result.Add(DeviceType.AnalogInput);

            return result;
        }

        public IEnumerable<Config.BaseDevice> GetConnectedInputDevices()
        {
            List<Config.BaseDevice> result = new List<Config.BaseDevice>();

            foreach (Config.BaseDevice dev in Config.Items)
            {
                switch (dev.Type)
                {
                    case DeviceType.Button:
                    case DeviceType.Encoder:
                    case DeviceType.AnalogInput:
                        result.Add(dev);
                        break;
                }
            }
            return result;
        }

        internal MobiFlightModuleInfo ToMobiFlightModuleInfo()
        {
            return new MobiFlightModuleInfo()
            { 
                    Serial  = Serial,
                    Name    = Name, 
                    Type    = Type, 
                    Port    = Port,
                    Version = Version
            };
            
        }

        protected bool KeepAliveNeeded() {
            if (lastUpdate.AddMinutes(KeepAliveIntervalInMinutes) < DateTime.UtcNow)
            {
                lastUpdate = DateTime.UtcNow;
                Log.Instance.log("Preventing entering EnergySaving Mode: KeepAlive!", LogSeverity.Info);
                return true;
            }

            return false;
        }

        public bool GenerateNewSerial()
        {
            System.Version minVersion = new System.Version("1.3.0");
            System.Version currentVersion = new System.Version(Version != "n/a" ? Version : "0.0.0");
            if (currentVersion.CompareTo(minVersion) < 0)
            {
                throw new FirmwareVersionTooLowException(minVersion, currentVersion);
            }
            SendCommand command = new SendCommand((int)MobiFlightModule.Command.GenNewSerial, (int)MobiFlightModule.Command.Status, CommandTimeout);
            ReceivedCommand StatusCommand = this._cmdMessenger.SendCommand(command);
            return StatusCommand.Ok;
        }

        public bool HasFirmwareFeature(String FirmwareFeature)
        {
            System.Version minVersion = new System.Version(FirmwareFeature);
            System.Version currentVersion = new System.Version(Version != "n/a" ? Version : "0.0.0");

            return (currentVersion.CompareTo(minVersion) >= 0);
        }

        public void Stop()
        {
            foreach (MobiFlightOutput output in outputs.Values)
            {
                SetPin("base", output.Name, 0);
            }
            
            foreach (MobiFlightLedModule module in ledModules.Values)
            {
                for(int i = 0; i!=module.SubModules;i++)
                    SetDisplay(module.Name, i, 0, 0xff, "        ");
            }
            
            foreach (MobiFlightStepper stepper in stepperModules.Values)
            {
                SetStepper(stepper.Name, 0);
            }

            lastValue.Clear();
        }

        public List<MobiFlightPin> GetFreePins()
        {
            return GetPins(true);
        }

        public List<MobiFlightPin> GetPins(bool FreeOnly = false)
        {
            List<MobiFlightPin> Pins = MobiFlightModuleInfo.MEGA_PINS;
                 
            if (Type == MobiFlightModuleInfo.TYPE_MICRO || Type == MobiFlightModuleInfo.TYPE_ARDUINO_MICRO)
            {
                Pins = MobiFlightModuleInfo.MICRO_PINS;
            }
            else if (Type == MobiFlightModuleInfo.TYPE_UNO || Type == MobiFlightModuleInfo.TYPE_ARDUINO_UNO)
            {
                Pins = MobiFlightModuleInfo.UNO_PINS;
            }

            List<MobiFlightPin> ResultPins = new List<MobiFlightPin>();
            ResultPins.AddRange(Pins.Select(x => new MobiFlightPin(x)));

            List<byte> usedPins = new List<byte>();

            foreach (Config.BaseDevice device in Config.Items)
            {
                String deviceName = device.Name;
                switch (device.Type)
                {
                    case DeviceType.LedModule:
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.LedModule).ClkPin));
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.LedModule).ClsPin));
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.LedModule).DinPin));
                        break;

                    case DeviceType.Stepper:
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Stepper).Pin1));
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Stepper).Pin2));
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Stepper).Pin3));
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Stepper).Pin4));

                        // We don't have to set the default 0 pin (for none auto zero)
                        if ((device as MobiFlight.Config.Stepper).BtnPin != "0")
                            usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Stepper).BtnPin));
                        break;

                    case DeviceType.Servo:
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Servo).DataPin));
                        break;

                    case DeviceType.Button:
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Button).Pin));
                        break;

                    case DeviceType.Encoder:
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Encoder).PinLeft));
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Encoder).PinRight));
                        break;

                    case DeviceType.LcdDisplay:
                        // Statically add correct I2C pins
                        foreach (MobiFlightPin pin in Pins.FindAll(x=>x.isI2C==true))
                        {
                            if (usedPins.Contains(Convert.ToByte(pin.Pin))) continue;
                            
                            usedPins.Add(Convert.ToByte(pin.Pin));
                        }
                        break;

                    case DeviceType.Output:
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.Output).Pin));
                        break;
            
                    case DeviceType.AnalogInput:
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.AnalogInput).Pin));
                        break;


                    case DeviceType.ShiftRegister:
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.ShiftRegister).ClockPin));
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.ShiftRegister).LatchPin));
                        usedPins.Add(Convert.ToByte((device as MobiFlight.Config.ShiftRegister).DataPin));
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            foreach (byte i in usedPins)
            {
                if (i != 0)
                {
                    ResultPins.Find(item => item.Pin == i).Used = true;
                }                
            }

            if (FreeOnly)
                ResultPins = ResultPins.FindAll(x => x.Used == false);

            return ResultPins;
        }
    }
}
