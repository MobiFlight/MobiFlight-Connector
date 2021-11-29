using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight
{
    public delegate void ButtonEventHandler(object sender, InputEventArgs e);
    public delegate void ModuleConnectEventHandler(object sender, String status, int progress);

    public class MobiFlightCache : MobiFlightCacheInterface, ModuleCacheInterface
    {    
        /// <summary>
        /// Gets raised whenever a button is pressed
        /// </summary>
        public event ButtonEventHandler OnButtonPressed;

        /// <summary>
        /// Gets raised whenever connection is close
        /// </summary>
        public event EventHandler Closed;
        /// <summary>
        /// Gets raised whenever connection is established
        /// </summary>
        public event EventHandler Connected;
        /// <summary>
        /// Gets raised whenever connection is lost
        /// </summary>
        public event EventHandler ConnectionLost;
        /// <summary>
        /// Gets raised whenever connection is established
        /// </summary>
        public event ModuleConnectEventHandler ModuleConnecting;
        /// <summary>
        /// Gets raised whenever the initial scan for modules is done
        /// </summary>
        public event EventHandler LookupFinished;


        private List<MobiFlightModuleInfo> connectedArduinoModules = null;
        Boolean isFirstTimeLookup = false;

        private bool _lookingUpModules = false;

        DateTime servoTime = DateTime.Now;
        /// <summary>
        /// list of known modules
        /// </summary>
        Dictionary<string, MobiFlightModule> Modules = new Dictionary<string, MobiFlightModule>();
        Dictionary<string, MobiFlightVariable> variables = new Dictionary<string, MobiFlightVariable>();

        /// <summary>
        /// indicates the status of the fsuipc connection
        /// </summary>
        /// <returns>true if connected, false if not</returns>
        public bool isConnected()
        {
            bool result = (Modules.Count > 0);
            foreach (MobiFlightModule module in Modules.Values)
            {
                result = result & module.Connected;
            }
            return result;
        }

        public bool updateConnectedModuleName(MobiFlightModule m)
        {
            if (connectedArduinoModules == null) return false;

            foreach (MobiFlightModuleInfo info in connectedArduinoModules)
            {
                if (info.Serial != m.Serial) continue;

                info.Name = m.Name;
                break;
            }

            return true;
        }

        public List<MobiFlightModuleInfo> GetDetectedArduinoModules()
        {
            if (connectedArduinoModules == null)
                    return new List<MobiFlightModuleInfo>();
            return connectedArduinoModules;
        }

        public async Task<IEnumerable<MobiFlightModule>> GetModulesAsync()
        {
            if (!isConnected())
                await connectAsync();
            return Modules.Values;
        }

        public IEnumerable<MobiFlightModule> GetModules()
        {
            if (!isConnected())
                return new List<MobiFlightModule>();
            return Modules.Values;
        }

        private static Dictionary<string, Board> getSupportedPorts()
        {
            var result = new Dictionary<string, Board>();

            RegistryKey regLocalMachine = Registry.LocalMachine;

            string[] regPaths = {
                    @"SYSTEM\CurrentControlSet\Enum\USB",
                    @"SYSTEM\CurrentControlSet\Enum\FTDIBUS"
            };

            foreach (var regPath in regPaths)
            {

                RegistryKey regUSB = regLocalMachine.OpenSubKey(regPath);
                if (regUSB == null) continue;

                foreach (String regDevice in regUSB.GetSubKeyNames())
                {
                    String message = null;
                    Log.Instance.log($"Checking for compatible module: {regDevice}", LogSeverity.Debug);

                    foreach (String regSubDevice in regUSB.OpenSubKey(regDevice).GetSubKeyNames())
                    {
                        Board board;
                        String FriendlyName = regUSB.OpenSubKey(regDevice).OpenSubKey(regSubDevice).GetValue("FriendlyName") as String;
                        if (FriendlyName == null) continue;

                        // Check to see if any board configurations exist
                        board = BoardDefinitions.GetBoardByFriendlyName(FriendlyName) ?? BoardDefinitions.GetBoardByHardwareId(regDevice);

                        // If no matching board definition is found at this point then it's an incompatible board and just keep going.
                        if (board == null)
                        {
                            Log.Instance.log($"Incompatible module skipped: {FriendlyName} - VID/PID: {regDevice}", LogSeverity.Debug);
                            continue;
                        }

                        // The board is a known type so test and see if a COM port is registered for it. If not, skip it.
                        String portName = regUSB.OpenSubKey(regDevice).OpenSubKey(regSubDevice).OpenSubKey("Device Parameters").GetValue("PortName") as String;

                        if (portName == null)
                        {
                            Log.Instance.log($"Arduino device has no port information: {regDevice}", LogSeverity.Debug);
                            continue;
                        }

                        // Safety check to ensure duplicate entires in the registry don't result in duplicate entires in the list.
                        if (result.ContainsKey(portName))
                        {
                            Log.Instance.log($"Duplicate entry for port: {board.Info.FriendlyName} {portName}", LogSeverity.Debug);
                            continue;
                        }

                        result.Add(portName, board);
                        Log.Instance.log($"Found potentially compatible module ({FriendlyName}): {regDevice}@{portName}", LogSeverity.Debug);
                    }
                }
            }

            return result;
        }

        private async  Task<List<MobiFlightModuleInfo>> LookupAllConnectedArduinoModulesAsync()
        {
            Log.Instance.log("MobiFlightCache.LookupAllConnectedArduinoModulesAsync: Start", LogSeverity.Debug);
            List<MobiFlightModuleInfo> result = new List<MobiFlightModuleInfo>();
            string[] connectedPorts = SerialPort.GetPortNames();

            if (_lookingUpModules) return result;
            _lookingUpModules = true;
            
            List<Task<MobiFlightModuleInfo>> tasks = new List<Task<MobiFlightModuleInfo>>();
            var supportedPorts = getSupportedPorts();
            List<string> connectingPorts = new List<string>();
            
            for (var i = 0; i != supportedPorts.Count; i++)
            {
                var port = supportedPorts.ElementAt(i);
                String portName = port.Key;
                Board board = port.Value;
                int progressValue = (i * 25) / supportedPorts.Count;

                if (!connectedPorts.Contains(portName))
                {
                    Log.Instance.log("MobiFlightCache.LookupAllConnectedArduinoModulesAsync: Port not connected ("+portName+")", LogSeverity.Debug);
                    continue;
                }
                if (connectingPorts.Contains(portName))
                {
                    Log.Instance.log("MobiFlightCache.LookupAllConnectedArduinoModulesAsync: Port already connecting (" + portName + ")", LogSeverity.Debug);
                    continue;
                }

                connectingPorts.Add(portName);

                tasks.Add(Task.Run(() =>
                {
                    MobiFlightModule tmp = new MobiFlightModule(portName, board);
                    ModuleConnecting?.Invoke(this, "Scanning Arduinos", progressValue);
                    tmp.Connect();
                    MobiFlightModuleInfo devInfo = tmp.GetInfo() as MobiFlightModuleInfo;

                    tmp.Disconnect();
                    ModuleConnecting?.Invoke(this, "Scanning Arduinos", progressValue + 5);

                    result.Add(devInfo);

                    return devInfo;
                }));
            }

            var infos = await Task.WhenAll(tasks);
  
            Log.Instance.log("MobiFlightCache.LookupAllConnectedArduinoModulesAsync: End", LogSeverity.Debug);

            _lookingUpModules = false;
            return result;
        }

        public async Task<bool> connectAsync(bool force=false)
        {
            if (isConnected() && force) { 
                disconnect(); 
            }
            
            if (connectedArduinoModules == null)
            {
                connectedArduinoModules = await LookupAllConnectedArduinoModulesAsync();
                isFirstTimeLookup = true;
            }

            Log.Instance.log("MobiFlightCache.connect: Clearing modules",LogSeverity.Debug);
            Modules.Clear();

            foreach (MobiFlightModuleInfo devInfo in connectedArduinoModules)
            {
                if (!devInfo.HasMfFirmware()) continue;

                MobiFlightModule m = new MobiFlightModule(devInfo.Port, devInfo.Board);
                RegisterModule(m, devInfo);
            }

            List<Task> connectTasks = new List<Task>();

            var i = 0;

            // Connect to all attached modules            
            foreach (MobiFlightModule module in Modules.Values)
            {
                int progressValue = (i * 50) / Modules.Values.Count;
                connectTasks.Add(Task.Run(() =>
                {
                    ModuleConnecting?.Invoke(this, "Connecting to MobiFlight Modules", progressValue);
                    module.Connect();
                    module.GetInfo();
                    ModuleConnecting?.Invoke(this, "Connecting to MobiFlight Modules", progressValue);
                }));
            }

            await Task.WhenAll(connectTasks);

            if (isFirstTimeLookup) {
                isFirstTimeLookup = false;
                LookupFinished?.Invoke(this, new EventArgs());
            }

            if (isConnected())
            {
                if (Connected != null)
                    this.Connected(this, new EventArgs());
                return true;
            }

            return false;
        }

        private void RegisterModule(MobiFlightModule m, MobiFlightModuleInfo devInfo, bool replace = false)
        {
            Log.Instance.log("MobiFlightCache.RegisterModule("+m.Name+":"+ m.Port +")", LogSeverity.Debug);
            
            if (Modules.ContainsKey(devInfo.Serial))
            {
                if (replace)
                {
                    // remove the existing handler
                    Modules[devInfo.Serial].OnInputDeviceAction -= new MobiFlightModule.InputDeviceEventHandler(module_OnButtonPressed);
                    Modules[devInfo.Serial] = m;                
                }
                else
                {
                    Log.Instance.log("Duplicate serial number found: " + devInfo.Serial + ". Module won't be added.", LogSeverity.Error);
                    return;
                }
            } else
            {                
                Modules.Add(devInfo.Serial, m);
            }

            // add the handler
            m.OnInputDeviceAction += new MobiFlightModule.InputDeviceEventHandler(module_OnButtonPressed);
        }

        public void module_OnButtonPressed(object sender, InputEventArgs e)
        {
            if (OnButtonPressed != null)
                OnButtonPressed(sender, e);
        }

        /// <summary>
        /// disconnects all modules
        /// </summary>
        /// <returns>returns true if all modules were disconnected properly</returns>    
        public bool disconnect()
        {
            Log.Instance.log("MobiFlightCache.disconnect()", LogSeverity.Debug);
            if (isConnected())
            {
                foreach (MobiFlightModule module in Modules.Values)
                {
                    module.Disconnect();
                }

                Closed(this, new EventArgs());
            }
            return !isConnected();
        }

        /// <summary>
        /// sets the pins in the mobiflight modules according to the given value
        /// </summary>
        /// <param name="serial">the device's serial</param>
        /// <param name="name">the port letter and pin number, e.g. A01</param>
        /// <param name="value">the value to be used</param>
        public void setValue(string serial, string name, string value)
        {
            if (serial == null)
            {
                throw new ConfigErrorException("ConfigErrorException_SerialNull");
            };

            try
            {
                if (name == null || name == "") return;

                if (!Modules.ContainsKey(serial)) return;

                MobiFlightModule module = GetModuleBySerial(serial);

                if (name != null && name.Contains("|")) {
                    var pins = name.Split('|');
                    foreach(string pin in pins) {
                        if (!string.IsNullOrEmpty(pin.Trim()))
                        {
                             module.SetPin("base", pin, Int16.Parse(value));
                        }
                    };
                } else
                {
                    module.SetPin("base", name, Int16.Parse(value));
                }

                
            }
            catch (ConfigErrorException e)
            {
                throw e;
            }
            catch (FormatException e)
            {
                // do nothing
                // maybe log this some time in the future
            }
            catch (Exception e)
            {
                throw e;
                //this.ConnectionLost(this, new EventArgs());
            }
        } //setValue()

        /// <summary>
        /// set the display module
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="address"></param>
        /// <param name="connector"></param>
        /// <param name="digits"></param>
        /// <param name="decimalPoints"></param>
        /// <param name="value"></param>
        public void setDisplay(string serial, string address, byte connector, List<string> digits, List<string> decimalPoints, string value)
        {
            if (serial == null)
            {
                throw new ConfigErrorException("ConfigErrorException_SerialNull");
            };

            if (address == null)
            {
                throw new ConfigErrorException("ConfigErrorException_AddressNull");
            }
            
            try
            {
                if (!Modules.ContainsKey(serial)) return;

                ArcazeLedDigit ledDigit = new ArcazeLedDigit();
                foreach (string digit in digits)
                {
                    ledDigit.setActive(ushort.Parse(digit));
                }

                foreach (string points in decimalPoints)
                {
                    ledDigit.setDecimalPoint(ushort.Parse(points));
                }

                MobiFlightModule module = Modules[serial];
                module.SetDisplay(address, connector - 1, ledDigit.getDecimalPoints(), (byte)ledDigit.getMask(), value);
            }
            catch (Exception e)
            {
                throw new MobiFlight.ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_WritingDisplay"), e);
            }
        }

        public void setDisplayBrightness(string serial, string address, byte connector, string value)
        {
            if (serial == null)
            {
                throw new ConfigErrorException("ConfigErrorException_SerialNull");
            };

            if (address == null)
            {
                throw new ConfigErrorException("ConfigErrorException_AddressNull");
            }

            try
            {
                if (!Modules.ContainsKey(serial)) return;

                ArcazeLedDigit ledDigit = new ArcazeLedDigit();
                MobiFlightModule module = Modules[serial];

                if (value != null)
                {
                    module.SetDisplayBrightness(address, connector - 1, value);
                }                
            }
            catch (Exception e)
            {
                throw new MobiFlight.ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_WritingDisplayBrightness"), e);
            }
        }

        private string GetValueForReference(string reference, List<ConfigRefValue> referenceList)
        {
            if (referenceList == null)
            {
                return null;
            }
            var found = referenceList.Find(x => x.ConfigRef.Ref.Equals(reference));
            return found?.Value;
        }

        public void setServo(string serial, string address, string value, int min, int max, byte maxRotationPercent)
        {
            try
            {
                if (!Modules.ContainsKey(serial)) return;

                MobiFlightModule module = Modules[serial];
                int iValue;
                if (!int.TryParse(value, out iValue)) return;

                module.SetServo(address, iValue, min, max, maxRotationPercent);
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_SettingServo"), e);
            }
        }

        public void setStepper(string serial, string address, string value, int inputRevolutionSteps, int outputRevolutionSteps, bool CompassMode)
        {
            try
            {
                if (!Modules.ContainsKey(serial)) return;

                MobiFlightModule module = Modules[serial];
                int iValue;
                if (!int.TryParse(value, out iValue)) return;
                if (module.GetStepper(address).OutputRevolutionSteps != outputRevolutionSteps)
                {
                    module.GetStepper(address).OutputRevolutionSteps = outputRevolutionSteps;
                }

                if (module.GetStepper(address).CompassMode != CompassMode)
                {
                    module.GetStepper(address).CompassMode = CompassMode;
                }
                module.SetStepper(address, iValue, inputRevolutionSteps);
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_SettingStepper"), e);
            }
        }

        public void resetStepper(string serial, string address)
        {
            try
            {
                MobiFlightModule module = Modules[serial];
                module.ResetStepper(address);
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_SettingServo"), e);
            }
        }

        /// <summary>
        /// set the display module
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="LcdConfig"></param>
        /// <param name="value"></param>
        /// <param name="replacements"></param>
        public void setLcdDisplay(string serial, OutputConfig.LcdDisplay LcdConfig, string value, List<ConfigRefValue> replacements)
        {
            if (serial == null)
            {
                throw new ConfigErrorException("ConfigErrorException_SerialNull");
            };

            if (LcdConfig.Address == null)
            {
                throw new ConfigErrorException("ConfigErrorException_AddressNull");
            }

            try
            {
                if (!Modules.ContainsKey(serial)) return;

                MobiFlightModule module = Modules[serial];
                // apply the replacement in our string
                MobiFlightLcdDisplay display = null;
                foreach (IConnectedDevice dev in module.GetConnectedDevices(LcdConfig.Address))
                {
                    if (dev.Type == DeviceType.LcdDisplay)
                    {
                        display = dev as MobiFlightLcdDisplay;
                    }
                }
                if (display == null) throw new Exception("LCD Display not found");
                String sValue = display.Apply(LcdConfig, value, replacements);

                module.SetLcdDisplay(LcdConfig.Address, sValue);
            }
            catch (Exception e)
            {
                throw new MobiFlight.ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_WritingDisplay"), e);
            }
        }

        public void setShiftRegisterOutput(string serial, string shiftRegName, string outputPin, string value)
        {
            if (serial == null)
            {
                throw new ConfigErrorException("ConfigErrorException_SerialNull");
            };

            if (outputPin == null)
            {
                throw new ConfigErrorException("ConfigErrorException_AddressNull");
            }

            try
            {
                if (!Modules.ContainsKey(serial)) return;

                MobiFlightModule module = Modules[serial];
                module.setShiftRegisterOutput(shiftRegName, outputPin, value);
            }
            catch (Exception e)
            {
                throw new MobiFlight.ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_WriteShiftRegisterOutput"), e);
            }
        }
      
        public void Flush()
        {
            // not implemented, don't throw exception either
        }


        public void Stop()
        {
            foreach (MobiFlightModule module in Modules.Values)
            {
                module.Stop();   
            }

            variables.Clear();
        }
        
        public IEnumerable<IModuleInfo> getModuleInfo()
        {
            List<IModuleInfo> result = new List<IModuleInfo>();
            foreach (MobiFlightModuleInfo moduleInfo in GetDetectedArduinoModules())
            {
                if (moduleInfo.HasMfFirmware())
                    result.Add(moduleInfo);
            }

            return result.OrderBy(module => module.Name);
        }

        public MobiFlightModule GetModule(string port)
        {
            foreach (MobiFlightModule module in Modules.Values)
            {
                if (module.Port == port) return module;
            }

            throw new IndexOutOfRangeException();
        }

        public MobiFlightModule RefreshModule(MobiFlightModule module)
        {
            MobiFlightModuleInfo oldDevInfo = connectedArduinoModules.Find(delegate(MobiFlightModuleInfo devInfo)
            {
                return devInfo.Port == module.Port;
            }
            );

            if (oldDevInfo != null) connectedArduinoModules.Remove(oldDevInfo);
            connectedArduinoModules.Add(module.ToMobiFlightModuleInfo());
            
            RegisterModule(module, module.ToMobiFlightModuleInfo(), true);

            return module;
        }

        public MobiFlightModule GetModuleBySerial(string serial)
        {
            if (Modules.ContainsKey(serial)) return Modules[serial];
            
            throw new IndexOutOfRangeException();
        }

        internal MobiFlightModule GetModule(MobiFlightModuleInfo moduleInfo)
        {
            foreach (MobiFlightModule module in Modules.Values)
            {
                if (module.Port == moduleInfo.Port) return module;                
            }

            if (Modules.ContainsKey(moduleInfo.Serial)) return Modules[moduleInfo.Serial];           

            throw new IndexOutOfRangeException();
        }

        public void SetMobiFlightVariable(MobiFlightVariable value)
        {
            variables[value.Name] = value;
        }

        public MobiFlightVariable GetMobiFlightVariable(String name)
        {
            if (!variables.Keys.Contains(name))
            {
                variables[name] = new MobiFlightVariable();
            }

            return variables[name];
        }

        public List<String> GetMobiFlightVariableNames()
        {
            return variables.Keys.ToList();
        }

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            result["Modules.Count"] = Modules.Values.Count();

            foreach(MobiFlightModule module in Modules.Values)
            {
                String key = "Modules." + module.Type;
                if (!result.ContainsKey(key)) result[key] = 0;
                result[key] += 1;

                foreach (String statKey in module.GetConnectedDevicesStatistics().Keys)
                {
                    if (!result.ContainsKey(statKey)) result[statKey] = 0;
                    result[statKey] += module.GetConnectedDevicesStatistics()[statKey];
                }
            }

            return result;
        }
    }
}
