using CommandMessenger;
using CommandMessenger.Transport.Serial;
using MobiFlight.Config;
using MobiFlight.UI.Panels.Settings.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace MobiFlight
{
    public class InputEventArgs : EventArgs
    {
        public string Serial { get; set; }
        public string DeviceId { get; set; }
        public string DeviceLabel { get; set; }
        public string Name { get; set; }
        public DeviceType Type { get; set; }
        public int? ExtPin { get; set; }
        public int Value { get; set; }

        public String StrValue { get; set; }

        public readonly DateTime Time = DateTime.Now;
    }

    public class FirmwareFeature
    {
        public const string GenerateSerial = "1.3.0";
        public const string SetName = "1.6.0";
        public const string LedModuleTypeTM1637 = "2.4.2";
        public const string CustomDevices = "2.4.2";
    }

    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    public enum DeviceType
    {
        NotSet,              // 0 
        Button,              // 1
        EncoderSingleDetent, // 2 (retained for backwards compatibility, use Encoder for new configs)
        Output,              // 3
        LedModuleDeprecated, // 4
        StepperDeprecatedV1, // 5
        Servo,               // 6
        LcdDisplay,          // 7
        Encoder,             // 8
        StepperDeprecatedV2, // 9
        ShiftRegister,       // 10
        AnalogInput,         // 11
        InputShiftRegister,  // 12
        MultiplexerDriver,   // 13  Not a proper device, but index required for update events
        InputMultiplexer, 	 // 14
        Stepper,             // 15
        LedModule,           // 16
        CustomDevice         // 17        
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
            SetModuleBrightness,    // 26
            SetShiftRegisterPins,   // 27
            AnalogChange,           // 28
            InputShiftRegisterChange, // 29
            InputMultiplexerChange, // 30
            SetStepperSpeedAccel,   // 31
            SetCustomDevice,        // 32
            DebugPrint =0xFF         // 255 for Debug Print from Firmware to log/terminal
        };

        public const string TYPE_COMPATIBLE = "Compatible";
        public const string TYPE_UNKNOWN = "Unknown";

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
        public string HardwareId { get; set; }

        public String Type
        {
            get
            {
                if (HasMfFirmware())
                {
                    return Board?.Info?.MobiFlightType ?? TYPE_UNKNOWN;
                }
                else
                {
                    var boards = BoardDefinitions.GetBoardsByHardwareId(HardwareId ?? "");
                    var type = Board?.Info.FriendlyName ?? TYPE_UNKNOWN;
                    if (boards.Count > 1)
                    {
                        type = TYPE_COMPATIBLE;
                    }
                    return type;
                }
            }
        }
        public String Serial { get; set; }
        public String Version { get; set; }
        public bool HasMfFirmware()
        {
            return !String.IsNullOrEmpty(Version);
        }

        public Config.Config Config
        {
            get
            {
                if (_config == null)
                {
                    if (!Connected) return null;

                    var command = new SendCommand((int)MobiFlightModule.Command.GetConfig, (int)MobiFlightModule.Command.Info, CommandTimeout);
                    var InfoCommand = _cmdMessenger.SendCommand(command);

                    // Sometimes first attempt times out.
                    if (!InfoCommand.Ok)
                    {
                        Log.Instance.log("Timeout. !InfoCommand.Ok. Retrying...", LogSeverity.Debug);
                        InfoCommand = _cmdMessenger.SendCommand(command);
                    }

                    // Some boards, like the Arduino Uno, require several attempts
                    // before the initial command succeeds.
                    if (Board.Connection.ExtraConnectionRetry)
                    {
                        if (!InfoCommand.Ok)
                            InfoCommand = _cmdMessenger.SendCommand(command);
                        if (!InfoCommand.Ok)
                            InfoCommand = _cmdMessenger.SendCommand(command);
                    }

                    if (InfoCommand.Ok)
                    {
                        // This is where the whole config in string form read from the Arduino is loaded
                        // to the internal objects (Config.Config.Items)
                        _config = new Config.Config(InfoCommand.ReadStringArg());
                    }
                    else
                    {
                        Log.Instance.log("!InfoCommand.Ok. Init with empty config.", LogSeverity.Debug);
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

        public const int CommandTimeout = 2500;
        public const int MessageSizeReductionValue = 10;

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
        Dictionary<String, MobiFlightOutput> outputs = new Dictionary<string, MobiFlightOutput>();
        Dictionary<String, MobiFlightLcdDisplay> lcdDisplays = new Dictionary<string, MobiFlightLcdDisplay>();
        Dictionary<String, MobiFlightButton> buttons = new Dictionary<string, MobiFlightButton>();
        Dictionary<String, MobiFlightEncoder> encoders = new Dictionary<string, MobiFlightEncoder>();
        Dictionary<String, MobiFlightAnalogInput> analogInputs = new Dictionary<string, MobiFlightAnalogInput>();
        Dictionary<String, MobiFlightShiftRegister> shiftRegisters = new Dictionary<string, MobiFlightShiftRegister>();
        Dictionary<String, MobiFlightInputShiftRegister> inputShiftRegisters = new Dictionary<string, MobiFlightInputShiftRegister>();
        Dictionary<String, MobiFlightInputMultiplexer> inputMultiplexers = new Dictionary<string, MobiFlightInputMultiplexer>();
        Dictionary<String, MobiFlightCustomDevice> customDevices = new Dictionary<string, MobiFlightCustomDevice>();

        Dictionary<String, int> buttonValues = new Dictionary<String, int>();

        private bool connected;
        public bool Connected
        {
            get { return connected; }
        }

        /// <summary>
        /// the look up table with the last set values
        /// </summary>
        Dictionary<string, string> lastValue = new Dictionary<string, string>();

        public Board Board { get; set; }

        public MobiFlightModule(String port, Board board)
        {
            Name = "Unknown";
            Version = null; // this is simply unknown, in case of an unflashed Arduino
            Serial = null; // this is simply unknown, in case of an unflashed Arduino
            _comPort = port;
            Board = board;
        }

        public MobiFlightModule(MobiFlightModuleInfo moduleInfo)
        {
            Name = moduleInfo.Name ?? "Unknown";
            Version = moduleInfo.Version;
            Serial = moduleInfo.Serial;
            _comPort = moduleInfo.Port;
            Board = moduleInfo.Board;
            HardwareId = moduleInfo.HardwareId;
        }

        public void Connect()
        {
            if (this.Connected)
            {
                Log.Instance.log($"Already connected to {Name} at {_comPort} of type {Board}.", LogSeverity.Debug);
                return;
            }

            // Create Serial Port object
            int baudRate = 115200;
            //baudRate = 57600;
            _transportLayer = new SerialTransport()
            //_transportLayer = new SerialPortManager
            {
                //CurrentSerialSettings = { PortName = _comPort, BaudRate = 115200, DtrEnable = dtrEnable } // object initializer
                CurrentSerialSettings = { PortName = _comPort, BaudRate = baudRate, DtrEnable = Board.Connection.DtrEnable } // object initializer
            };

            _cmdMessenger = new CmdMessenger(_transportLayer, BoardType.Bit16, ',', ';', '\\', Board.Connection.MessageSize);

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Start listening    
            var status = _cmdMessenger.Connect();
            Log.Instance.log($"MobiflightModule.connect: Connected to {Name} at {_comPort} of type {Board.Info.MobiFlightType} (DTR=>{_transportLayer.CurrentSerialSettings.DtrEnable}).", LogSeverity.Info);
            //this.Connected = status;
            this.connected = true;

            // this sleep helps during initialization
            // without this line modules did not connect properly
            if (Board.Connection.ConnectionDelay > 0)
            {
                System.Threading.Thread.Sleep(Board.Connection.ConnectionDelay);
            }

            // if (!this.Connected) return;
            // ResetBoard();
            // LoadConfig();
        }

        public void ResetBoard()
        {
            var command = new SendCommand((int)MobiFlightModule.Command.ResetBoard);
            var InfoCommand = _cmdMessenger.SendCommand(command);
        }

        public void LoadConfig()
        {
            // Rebuilds all objects from Config.Items.

            ledModules.Clear();
            stepperModules.Clear();
            servoModules.Clear();
            outputs.Clear();
            lcdDisplays.Clear();
            buttons.Clear();
            encoders.Clear();
            inputShiftRegisters.Clear();
            inputMultiplexers.Clear();
            analogInputs.Clear();
            shiftRegisters.Clear();
            customDevices.Clear();

            
            foreach (Config.BaseDevice device in Config.Items)
            {
                if (device == null) continue; // Can happen during development if trying with an older firmware, which prevents you from starting.
                
                try
                {
                    switch (device.Type)
                    {
                        case DeviceType.LedModule:
                            int ledSubmodules = 1;

                            if (!int.TryParse((device as Config.LedModule).NumModules, out ledSubmodules))
                            {
                                Log.Instance.log(
                                    $"Can't parse {Board.Info.FriendlyName} ({Port}) > [{(device as Config.LedModule).Name}]." +
                                    $"NumModules: {(device as Config.LedModule).NumModules}, " +
                                    $"using default {ledSubmodules}",
                                    LogSeverity.Error);
                                break;
                            }

                            device.Name = GenerateUniqueDeviceName(ledModules.Keys.ToArray(), device.Name);
                            var dev = device as Config.LedModule;
                            
                            ledModules.Add(device.Name, new MobiFlightLedModule()
                            {
                                CmdMessenger = _cmdMessenger,
                                Name = device.Name,
                                ModuleNumber = ledModules.Count,
                                ModelType = dev.ModelType,
                                SubModules = ledSubmodules,
                                Brightness = (device as Config.LedModule).Brightness
                            });
                            break;

                        case DeviceType.Stepper:
                            device.Name = GenerateUniqueDeviceName(stepperModules.Keys.ToArray(), device.Name);
                            var profile = MFStepperPanel.Profiles.Find(p => p.Value.id == (device as Config.Stepper).Profile).Value;

                            stepperModules.Add(device.Name, new MobiFlightStepper()
                            {
                                CmdMessenger = _cmdMessenger,
                                Name = device.Name,
                                StepperNumber = stepperModules.Count,
                                HasAutoZero = (device as Config.Stepper).BtnPin != "0",
                                Profile = profile
                            });
                            break;
                        case DeviceType.Servo:
                            device.Name = GenerateUniqueDeviceName(servoModules.Keys.ToArray(), device.Name);
                            servoModules.Add(device.Name, new MobiFlightServo() { CmdMessenger = _cmdMessenger, Name = device.Name, ServoNumber = servoModules.Count });
                            break;
                        case DeviceType.Output:
                            device.Name = GenerateUniqueDeviceName(outputs.Keys.ToArray(), device.Name);
                            Int16 pin;
                            if (!Int16.TryParse((device as Config.Output).Pin, out pin))
                            {
                                Log.Instance.log(
                                    $"Can't parse {Board.Info.FriendlyName} ({Port}) > [{(device as Config.Output).Name}]." +
                                    $"Pin: {(device as Config.Output).Pin}, skipping device.",
                                    LogSeverity.Error);
                                break;
                            }
                            outputs.Add(device.Name, new MobiFlightOutput()
                            {
                                CmdMessenger = _cmdMessenger,
                                Name = device.Name,
                                Pin = pin
                            });
                            break;
                        case DeviceType.LcdDisplay:
                            device.Name = GenerateUniqueDeviceName(lcdDisplays.Keys.ToArray(), device.Name);
                            lcdDisplays.Add(device.Name, new MobiFlightLcdDisplay() { CmdMessenger = _cmdMessenger, Name = device.Name, Address = lcdDisplays.Count, Cols = (device as Config.LcdDisplay).Cols, Lines = (device as Config.LcdDisplay).Lines });
                            break;
                        case DeviceType.Button:
                            device.Name = GenerateUniqueDeviceName(buttons.Keys.ToArray(), device.Name);
                            buttons.Add(device.Name, new MobiFlightButton() { Name = device.Name });
                            break;
                        case DeviceType.Encoder:
                            device.Name = GenerateUniqueDeviceName(encoders.Keys.ToArray(), device.Name);
                            encoders.Add(device.Name, new MobiFlightEncoder() { Name = device.Name });
                            break;
                        case DeviceType.AnalogInput:
                            device.Name = GenerateUniqueDeviceName(analogInputs.Keys.ToArray(), device.Name);
                            analogInputs.Add(device.Name, new MobiFlightAnalogInput() { Name = device.Name });
                            break;
                        case DeviceType.ShiftRegister:
                            device.Name = GenerateUniqueDeviceName(shiftRegisters.Keys.ToArray(), device.Name);
                            int submodules = 1;
                            if (!int.TryParse((device as Config.ShiftRegister).NumModules, out submodules))
                            {
                                Log.Instance.log(
                                    $"Can't parse {Board.Info.FriendlyName} ({Port}) > [{(device as Config.ShiftRegister).Name}]." +
                                    $"NumModules: {(device as Config.ShiftRegister).NumModules}, " +
                                    $"using default {submodules}",
                                    LogSeverity.Error);
                                break;
                            }
                            shiftRegisters.Add(device.Name, new MobiFlightShiftRegister() { CmdMessenger = _cmdMessenger, Name = device.Name, NumberOfShifters = submodules, ModuleNumber = shiftRegisters.Count });
                            break;

                        case DeviceType.InputShiftRegister:
                            device.Name = GenerateUniqueDeviceName(inputShiftRegisters.Keys.ToArray(), device.Name);
                            inputShiftRegisters.Add(device.Name, new MobiFlightInputShiftRegister() { Name = device.Name });
                            break;
                        case DeviceType.InputMultiplexer:
                            device.Name = GenerateUniqueDeviceName(inputMultiplexers.Keys.ToArray(), device.Name);
                            inputMultiplexers.Add(device.Name, new MobiFlightInputMultiplexer() { Name = device.Name });
                            break;
                        // DeviceType.MultiplexerDriver does not belong here (a "multiplexerDrivers" collection doesn't even exist)
                        // because all I/O devices here are only those meant to be linked to a user input event or output data,
                        // while MultiplexerDrivers are not addressable by the user (and shouldn't show in the UI).

                        case DeviceType.CustomDevice:
                            device.Name = GenerateUniqueDeviceName(customDevices.Keys.ToArray(), device.Name);
                            customDevices.Add(device.Name, new MobiFlightCustomDevice() {
                                    CmdMessenger = _cmdMessenger,
                                    DeviceNumber = customDevices.Count,
                                    Name = device.Name,
                                    CustomDevice = CustomDevices.CustomDeviceDefinitions.GetDeviceByType((device as Config.CustomDevice).CustomType)
                            });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Unable to load config for {Board.Info.FriendlyName} ({Port}) > {device.Name}: {ex.Message}", LogSeverity.Error);
                }
            } 
        }

        public static string GenerateUniqueDeviceName(String[] Keys, String Name)
        {
            String result = Name;
            int renameIndex = 1;

            while (Keys.Contains(result))
            {
                result = Name + " " + renameIndex;
                renameIndex++;
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
                Log.Instance.log($"Already disconnected {this.Name}:{_comPort}.", LogSeverity.Debug);
                return;
            }

            this.connected = false;

            _cmdMessenger.Disconnect();
            _cmdMessenger.Dispose();
            _transportLayer.Dispose();

            _config = null;

            Log.Instance.log($"Disconnected {this.Name}:{_comPort}.", LogSeverity.Debug);
        }

        public String InitUploadAndReturnUploadPort()
        {
            String result = _comPort;
            List<String> connectedPorts = SerialPort.GetPortNames().ToList();
            Disconnect();

            if (Board.Connection.ForceResetOnFirmwareUpdate)
            {
                SerialTransport tmpSerial = new SerialTransport()
                {
                    CurrentSerialSettings = { PortName = _comPort, BaudRate = 1200, DtrEnable = true } // object initializer
                };
 
                tmpSerial.Connect();
                tmpSerial.Disconnect();
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
            _cmdMessenger.Attach((int)Command.InputShiftRegisterChange, OnInputShiftRegisterChange);
            _cmdMessenger.Attach((int)Command.InputMultiplexerChange, OnInputMultiplexerChange);
            _cmdMessenger.Attach((int)Command.ButtonChange, OnButtonChange);
            _cmdMessenger.Attach((int)Command.AnalogChange, OnAnalogChange);
            _cmdMessenger.Attach((int)Command.DebugPrint, OnDebugPrint);
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

            if (!int.TryParse(pos, out int value))
            {
                Log.Instance.log($"Unable to convert {pos} to an integer.", LogSeverity.Error);
                return;
            }

            if (OnInputDeviceAction != null)
                OnInputDeviceAction(this, new InputEventArgs() { 
                    Serial = this.Serial, 
                    Name = Name, 
                    DeviceId = enc, 
                    DeviceLabel = enc, 
                    Type = DeviceType.Encoder, 
                    Value = value 
                });
            //addLog("Enc: " + enc + ":" + pos);
        }

        void OnInputShiftRegisterChange(ReceivedCommand arguments)
        {
            String deviceId = arguments.ReadStringArg();
            String strChannel = arguments.ReadStringArg();
            String strState = arguments.ReadStringArg();

            if (!int.TryParse(strChannel, out int channel))
            {
                Log.Instance.log($"Unable to convert {strChannel} to an integer.", LogSeverity.Error);
                return;
            }

            if (!int.TryParse(strState, out int state))
            {
                Log.Instance.log($"Unable to convert {strState} to an integer.", LogSeverity.Error);
                return;
            }

            if (OnInputDeviceAction != null)
                OnInputDeviceAction(this, new InputEventArgs() { 
                    Serial = this.Serial, 
                    Name = Name, 
                    DeviceId = deviceId, 
                    DeviceLabel = deviceId, 
                    Type = DeviceType.InputShiftRegister, 
                    ExtPin = channel, 
                    Value = state 
                });
        }

        void OnInputMultiplexerChange(ReceivedCommand arguments)
        {
            String deviceId = arguments.ReadStringArg();
            String strChannel = arguments.ReadStringArg();
            String strState = arguments.ReadStringArg();

            if (!int.TryParse(strChannel, out int channel))
            {
                Log.Instance.log($"Unable to convert {strChannel} to an integer.", LogSeverity.Error);
                return;
            }

            if (!int.TryParse(strState, out int state))
            {
                Log.Instance.log($"Unable to convert {strState} to an integer.", LogSeverity.Error);
                return;
            }

            if (OnInputDeviceAction != null)
                OnInputDeviceAction(this, new InputEventArgs() { 
                    Serial = this.Serial, 
                    Name = Name, 
                    DeviceId = deviceId, 
                    DeviceLabel = deviceId,
                    Type = DeviceType.InputMultiplexer, 
                    ExtPin = channel, 
                    Value = state
                });
        }

        // Callback function that prints the Arduino status to the console
        void OnButtonChange(ReceivedCommand arguments)
        {
            String button = arguments.ReadStringArg();
            String strState = arguments.ReadStringArg();

            if (!int.TryParse(strState, out int state)) {
                Log.Instance.log($"Unable to convert {strState} to an integer.", LogSeverity.Error);
                return;
            }

            OnInputDeviceAction?.Invoke(this, new InputEventArgs()
            {
                Serial = this.Serial,
                Name = Name,
                DeviceId = button,
                DeviceLabel = button,
                Type = DeviceType.Button,
                Value = state
            });
        }

        // Callback function that prints the Arduino status to the console
        void OnAnalogChange(ReceivedCommand arguments)
        {
            String name = arguments.ReadStringArg();
            String strValue = arguments.ReadStringArg();

            if (!int.TryParse(strValue, out int value))
            {
                Log.Instance.log($"Unable to convert {strValue} to an integer.", LogSeverity.Error);
                return;
            }

            OnInputDeviceAction?.Invoke(this, new InputEventArgs()
            {
                Serial = this.Serial,
                Name = Name,
                DeviceId = name,
                DeviceLabel = name,
                Type = DeviceType.AnalogInput,
                Value = value
            });
        }

        // Callback function that prints the Arduino Debug Print to the console
        // Up to 3 strings can be send from the firmware
        void OnDebugPrint(ReceivedCommand arguments)
        {
            String value1 = arguments.ReadStringArg();
            String value2 = arguments.ReadStringArg();
            String value3 = arguments.ReadStringArg();
            Log.Instance.log($"{this.Name}.debug: Firmware -> {value1} {value2} {value3}.", LogSeverity.Debug);
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
            String key = port + pin;

            if (!KeepAliveNeeded() && lastValue.ContainsKey(key) &&
                lastValue[key] == value.ToString()) return false;

            lastValue[key] = value.ToString();

            if (!outputs.ContainsKey(pin)) return false;

            outputs[pin].Set(value);

            return true;
        }

        public bool SetDisplay(string name, int module, byte points, byte mask, string value, bool reverse)
        {
            if (KeepAliveNeeded())
                ledModules[name].ClearState();

            ledModules[name].Display(module, value, points, mask, reverse);
            return true;
        }

        public bool SetDisplayBrightness(string name, int module, string value)
        {
            if (KeepAliveNeeded())
                ledModules[name].ClearState();

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
            lastValue[key] = stepperModules[stepper].Position().ToString();
            return true;
        }

        public bool SetLcdDisplay(string address, string value)
        {
            String key = "LCD_" + address;
            String cachedValue = value;

            if (!KeepAliveNeeded() && lastValue.ContainsKey(key) &&
                lastValue[key] == cachedValue) return false;

            lastValue[key] = cachedValue;

            lcdDisplays[address].Display(value);
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
                throw new IndexOutOfRangeException();
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

        internal bool setCustomDevice(string deviceName, string messageType, string value)
        {
            String key = "CustomDevice_" + deviceName + messageType;
            String cachedValue = value;

            if (!KeepAliveNeeded() && lastValue.ContainsKey(key) &&
                lastValue[key] == cachedValue) return false;

            lastValue[key] = cachedValue;

            customDevices[deviceName].Display(messageType, value);
            return true;
        }

        public bool Retrigger()
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
            MobiFlightModuleInfo devInfo = new MobiFlightModuleInfo()
            {
                Name = Name,
                Type = Type,
                Port = _comPort,
            };

            var command = new SendCommand((int)MobiFlightModule.Command.GetInfo, (int)MobiFlightModule.Command.Info, CommandTimeout);
            var InfoCommand = _cmdMessenger.SendCommand(command);

            if (InfoCommand.Ok)
            {
                // Workaround
                // the following two lines shall get removed
                // but at the moment something with the timing during startup is wrong.
                command = new SendCommand((int)MobiFlightModule.Command.GetInfo, (int)MobiFlightModule.Command.Info, CommandTimeout);
                InfoCommand = _cmdMessenger.SendCommand(command);

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

                Name = devInfo.Name;
                Version = devInfo.Version;
                Serial = devInfo.Serial;
            }

            // Get the board specifics based on the MobiFlight type returned by the firmware. If there's no match,
            // either because it is a generic Arduino without the firmware installed or because there
            // was no board definition file that matches the MobiFlight Type returned from the firmware,
            // then fall back to the Board type detected earlier via VID/PID.
            //
            // This check and assignment is done outside of the above if statement to catch cases
            // where there was no firmware installed.
            devInfo.Board = BoardDefinitions.GetBoardByMobiFlightType(devInfo.Type) ?? Board;
            Board = devInfo.Board;

            Log.Instance.log($"Retrieved board: {Type}, {Name}, {Version}, {Serial}.", LogSeverity.Debug);
            return devInfo;
        }

        public bool SaveName()
        {
            bool isOk = true;
            var command = new SendCommand((int)MobiFlightModule.Command.SetName, (int)MobiFlightModule.Command.Status, CommandTimeout);
            Log.Instance.log($"Save name: {(int)MobiFlightModule.Command.SetName} > {Name}.", LogSeverity.Debug);
            command.AddArgument(Name);
            ReceivedCommand StatusCommand = _cmdMessenger.SendCommand(command);

            isOk = StatusCommand.Ok;

            return isOk;
        }

        public bool SaveConfig()
        {
            bool isOk = SaveName();
            var command = new SendCommand((int)MobiFlightModule.Command.ResetConfig, (int)MobiFlightModule.Command.Status, CommandTimeout);
            Log.Instance.log("Reseting config.", LogSeverity.Debug);
            _cmdMessenger.SendCommand(command);

            foreach (string MessagePart in this.Config.ToInternal(this.Board.Connection.MessageSize - MessageSizeReductionValue))
            {
                Log.Instance.log($"Uploading config part {MessagePart}", LogSeverity.Debug);
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
                Log.Instance.log("Error on uploading.", LogSeverity.Error);
            }

            if (isOk)
            {
                command = new SendCommand((int)MobiFlightModule.Command.SaveConfig, (int)MobiFlightModule.Command.ConfigSaved, CommandTimeout);
                Log.Instance.log("Saving config.", LogSeverity.Debug);
                ReceivedCommand StatusCommand = _cmdMessenger.SendCommand(command);

                if (StatusCommand.Ok)
                {
                    command = new SendCommand((int)MobiFlightModule.Command.ActivateConfig, (int)MobiFlightModule.Command.ConfigActivated, CommandTimeout);
                    StatusCommand = _cmdMessenger.SendCommand(command);
                    isOk = StatusCommand.Ok;
                    if (isOk) Log.Instance.log("Config activated.", LogSeverity.Debug);
                    else Log.Instance.log("Config not activated successfully.", LogSeverity.Error);
                }
                else
                {
                    if (!isOk)
                    {
                        Log.Instance.log("Error on saving.", LogSeverity.Error);
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

            foreach (MobiFlightCustomDevice customDevice in customDevices.Values)
            {
                result.Add(customDevice);
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
            result[MobiFlightInputShiftRegister.TYPE] = inputShiftRegisters.Count;
            result[MobiFlightInputMultiplexer.TYPE] = inputMultiplexers.Count;
            result[MobiFlightCustomDevice.TYPE] = customDevices.Count;

            foreach(var device in customDevices.Values)
            {
                var customDeviceType = device.CustomDevice.Info.Type;
                var statisticsKey = $"CustomDevice.{customDeviceType}";
                if (!result.ContainsKey(statisticsKey))
                    result[statisticsKey] = 0;
                result[statisticsKey]++;
            }

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
            return Board.Pins.FindAll(x => x.isPWM);
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
            if (customDevices.Count > 0) result.Add(DeviceType.CustomDevice);

            return result;
        }

        public IEnumerable<DeviceType> GetConnectedInputDeviceTypes()
        {
            bool _hasButtons = false;
            bool _hasEncoder = false;
            bool _hasAnalog = false;
            bool _hasInputShiftRegisters = false;
            bool _hasInputMultiplexer = false;

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
                    case DeviceType.InputShiftRegister:
                        _hasInputShiftRegisters = true;
                        break;
                    case DeviceType.InputMultiplexer:
                        _hasInputMultiplexer = true;
                        break;
                    case DeviceType.AnalogInput:
                        _hasAnalog = true;
                        break;

                }
            }
            if (_hasButtons) result.Add(DeviceType.Button);
            if (_hasEncoder) result.Add(DeviceType.Encoder);
            if (_hasAnalog) result.Add(DeviceType.AnalogInput);
            if (_hasInputShiftRegisters) result.Add(DeviceType.InputShiftRegister);
            if (_hasInputMultiplexer) result.Add(DeviceType.InputMultiplexer);

            return result;
        }

        public IEnumerable<Config.BaseDevice> GetConnectedOutputDevices()
        {
            List<Config.BaseDevice> result = new List<Config.BaseDevice>();

            foreach (Config.BaseDevice dev in Config.Items)
            {
                switch (dev.Type)
                {
                    case DeviceType.Output:
                    case DeviceType.LedModule:
                    case DeviceType.Servo:
                    case DeviceType.Stepper:
                    case DeviceType.ShiftRegister:
                    case DeviceType.LcdDisplay:
                    case DeviceType.CustomDevice:
                        result.Add(dev);
                        break;
                }
            }

            result.Sort((a, b) => { return a.Name.CompareTo(b.Name); });
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
                    case DeviceType.InputShiftRegister:
                    case DeviceType.InputMultiplexer:
                    case DeviceType.AnalogInput:
                        result.Add(dev);
                        break;
                }
            }

            result.Sort((a, b) => { return a.Name.CompareTo(b.Name); });
            return result;
        }

        internal MobiFlightModuleInfo ToMobiFlightModuleInfo()
        {
            return new MobiFlightModuleInfo()
            {
                Serial = Serial,
                Name = Name,
                Type = Type,
                Port = Port,
                Version = Version,
                HardwareId = HardwareId,
                Board = Board
            };

        }

        protected bool KeepAliveNeeded()
        {
            if (lastUpdate.AddMinutes(KeepAliveIntervalInMinutes) < DateTime.UtcNow)
            {
                lastUpdate = DateTime.UtcNow;
                Log.Instance.log("Preventing entering EnergySaving mode: KeepAlive!", LogSeverity.Debug);
                return true;
            }

            return false;
        }

        public bool GenerateNewSerial()
        {
            System.Version minVersion = new System.Version("1.3.0");
            System.Version currentVersion = new System.Version(Version != null ? Version : "0.0.0");
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
            System.Version currentVersion = new System.Version(Version != null ? Version : "0.0.0");

            return (currentVersion.CompareTo(minVersion) >= 0);
        }

        public void Stop()
        {
            // Always clear the cache 
            // also in case maybe later something goes wrong
            // we will simply resend the value in that case
            // when we start again
            lastValue.Clear();

            // we have to make sure to not send messages
            // when we are not connected
            if (!this.connected) return;

            GetConnectedDevices().ForEach(device =>
            {
                //System.Threading.Thread.Sleep(10); 
                device.Stop();
            });
        }

        public List<MobiFlightPin> GetFreePins()
        {
            return GetPins(true);
        }

        // Returns a List<> of the pins used by the module
        public List<MobiFlightPin> GetPins(bool FreeOnly = false, bool ExcludeI2CDevices = false)
        {
            List<MobiFlightPin> ResultPins = new List<MobiFlightPin>();
            ResultPins.AddRange(Board.Pins.Select(x => new MobiFlightPin(x)));

            List<byte> usedPins = new List<byte>();

            foreach (Config.BaseDevice device in Config.Items)
            {
                String deviceName = device.Name;
                switch (device.Type)
                {
                    case DeviceType.LedModule:
                        usedPins.Add(Convert.ToByte((device as LedModule).ClkPin));
                        if ((device as LedModule).ModelType == LedModule.MODEL_TYPE_MAX72xx)
                        {
                            if ((device as LedModule).ClsPin != "")
                                usedPins.Add(Convert.ToByte((device as LedModule).ClsPin));
                        }
                        usedPins.Add(Convert.ToByte((device as LedModule).DinPin));
                        break;

                    case DeviceType.Stepper:
                        usedPins.Add(Convert.ToByte((device as Stepper).Pin1));
                        usedPins.Add(Convert.ToByte((device as Stepper).Pin2));
                        usedPins.Add(Convert.ToByte((device as Stepper).Pin3));
                        usedPins.Add(Convert.ToByte((device as Stepper).Pin4));

                        // We don't have to set the default 0 pin (for none auto zero)
                        if ((device as MobiFlight.Config.Stepper).BtnPin != "0")
                            usedPins.Add(Convert.ToByte((device as Stepper).BtnPin));
                        break;

                    case DeviceType.Servo:
                        usedPins.Add(Convert.ToByte((device as Servo).DataPin));
                        break;

                    case DeviceType.Button:
                        usedPins.Add(Convert.ToByte((device as Button).Pin));
                        break;

                    case DeviceType.Encoder:
                        usedPins.Add(Convert.ToByte((device as Config.Encoder).PinLeft));
                        usedPins.Add(Convert.ToByte((device as Config.Encoder).PinRight));
                        break;

                    case DeviceType.InputShiftRegister:
                        usedPins.Add(Convert.ToByte((device as InputShiftRegister).ClockPin));
                        usedPins.Add(Convert.ToByte((device as InputShiftRegister).DataPin));
                        usedPins.Add(Convert.ToByte((device as InputShiftRegister).LatchPin));
                        break;

                    case DeviceType.LcdDisplay:
                        if (ExcludeI2CDevices)
                        {
                            continue;
                        }

                        // Statically add correct I2C pins
                        foreach (MobiFlightPin pin in Board.Pins.FindAll(x => x.isI2C))
                        {
                            if (usedPins.Contains(Convert.ToByte(pin.Pin))) continue;
                            usedPins.Add(Convert.ToByte(pin.Pin));
                        }
                        break;

                    case DeviceType.Output:
                        usedPins.Add(Convert.ToByte((device as Output).Pin));
                        break;

                    case DeviceType.AnalogInput:
                        usedPins.Add(Convert.ToByte((device as AnalogInput).Pin));
                        break;

                    case DeviceType.ShiftRegister:
                        usedPins.Add(Convert.ToByte((device as ShiftRegister).ClockPin));
                        usedPins.Add(Convert.ToByte((device as ShiftRegister).LatchPin));
                        usedPins.Add(Convert.ToByte((device as ShiftRegister).DataPin));
                        break;

                    case DeviceType.InputMultiplexer:
                        usedPins.Add(Convert.ToByte((device as InputMultiplexer).DataPin));
                        usedPins.Add(Convert.ToByte((device as InputMultiplexer).Selector.PinSx[0]));
                        usedPins.Add(Convert.ToByte((device as InputMultiplexer).Selector.PinSx[1]));
                        usedPins.Add(Convert.ToByte((device as InputMultiplexer).Selector.PinSx[2]));
                        usedPins.Add(Convert.ToByte((device as InputMultiplexer).Selector.PinSx[3]));
                        break;

                    case DeviceType.CustomDevice:
                        (device as CustomDevice).ConfiguredPins.ForEach(p => usedPins.Add(Convert.ToByte((p))));
                        break;

                    // If the multiplexerDriver is to be handled as a regular device
                    // but explicitly defined by its own config line, following 'case' is required:
                    //case DeviceType.MultiplexerDriver:
                    //    usedPins.Add(Convert.ToByte((device as MultiplexerDriver).PinSx[0]));
                    //    usedPins.Add(Convert.ToByte((device as MultiplexerDriver).PinSx[1]));
                    //    usedPins.Add(Convert.ToByte((device as MultiplexerDriver).PinSx[2]));
                    //    usedPins.Add(Convert.ToByte((device as MultiplexerDriver).PinSx[3]));
                    //    break;

                    default:
                        throw new NotImplementedException();
                }
            }

            // Mark all the used pins as used in the result list.
            foreach (byte pinNo in usedPins) {
                MobiFlightPin pin = ResultPins.Find(resultPin => resultPin.Pin == pinNo);
                if (pin != null) pin.Used = true;
            }

            if (FreeOnly)
                ResultPins = ResultPins.FindAll(x => x.Used == false);

            return ResultPins;
        }

        public bool FirmwareRequiresUpdate()
        {
            return ToMobiFlightModuleInfo().FirmwareRequiresUpdate();
        }
    }
}
