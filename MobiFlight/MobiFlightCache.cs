using MobiFlight.Config;
using MobiFlight.Monitors;
using SharpDX;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight
{
    public delegate void ButtonEventHandler(object sender, InputEventArgs e);

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
        public event EventHandler OnAvailable;
        /// <summary>
        /// Gets raised whenever a new mobiflight module has connected
        /// </summary>
        public event EventHandler ModuleConnected;
        /// <summary>
        /// Gets raised whenever connection is lost
        /// </summary>
        public event EventHandler ModuleRemoved;
        /// <summary>
        /// Gets raised whenever the initial scan for modules is done
        /// </summary>
        public event EventHandler LookupFinished;


        private volatile List<MobiFlightModuleInfo> AvailableComModules = new List<MobiFlightModuleInfo>();
        Boolean isFirstTimeLookup = true;

        private bool _lookingUpModules = false;

        DateTime servoTime = DateTime.Now;
        /// <summary>
        /// list of known modules.
        /// 
        /// ConcurrentDictionary for better thread-safety
        /// </summary>
        ConcurrentDictionary<string, MobiFlightModule> Modules = new ConcurrentDictionary<string, MobiFlightModule>();
        ConcurrentDictionary<string, MobiFlightVariable> variables = new ConcurrentDictionary<string, MobiFlightVariable>();

        SerialPortMonitor SerialPortMonitor = new SerialPortMonitor();
        UsbDeviceMonitor UsbDeviceMonitor = new UsbDeviceMonitor();
        int progressValue = 0;

        public MobiFlightCache()
        {
        }

        public void Start()
        {
            SerialPortMonitor.PortAvailable += SerialPortMonitor_PortAvailable;
            SerialPortMonitor.PortUnavailable += SerialPortMonitor_PortUnavailable;
            SerialPortMonitor.Start();

            UsbDeviceMonitor.PortAvailable += UsbDeviceMonitor_PortAvailable;
            UsbDeviceMonitor.PortUnavailable += UsbDeviceMonitor_PortUnavailable;
            UsbDeviceMonitor.Start();

            // This timeout ensures that the LookupFinished is event called
            // even if we have no modules connected 
            // the event is needed so that the startup can finish
            Task.Delay(TimeSpan.FromMilliseconds(5000))
                .ContinueWith(_ => { 
                    if (0 == AvailableComModules.Count) {
                        isFirstTimeLookup = false;
                        LookupFinished?.Invoke(this, new EventArgs());
                        Log.Instance.log($"End looking up connected modules. No modules found.", LogSeverity.Debug);
                    } 
                });
        }

        private void SerialPortMonitor_PortUnavailable(object sender, PortDetails e)
        {
            Log.Instance.log($"Port disappeared: {e.Name}", LogSeverity.Debug);
            var disconnectedModule = AvailableComModules.Find(m => m.Port == e.Name);

            if (disconnectedModule == null) return;

            if (disconnectedModule.HasMfFirmware())
            {
                var module = Modules.Values.ToList().Find(m => m.Port == e.Name);
                if (module != null)
                {
                    module.Disconnect();
                    Modules.TryRemove(module.Serial, out MobiFlightModule removedModule);
                }
            }
            AvailableComModules.Remove(disconnectedModule);
            ModuleRemoved?.Invoke(disconnectedModule, EventArgs.Empty);
        }

        private async void SerialPortMonitor_PortAvailable(object sender, PortDetails portDetails)
        {
            Log.Instance.log($"Port detected: {portDetails.Name} {portDetails.Board.Info.FriendlyName}", LogSeverity.Debug);

            List<string> ignoredComPorts = getIgnoredPorts();
            if (ignoredComPorts.Contains(portDetails.Name))
            {
                OnIgnoredPortDetected(sender, portDetails);

                // in case all ports are connected
                // we have to check here
                // so that the startup can finish.
                await CheckIfLookUpFinished();
                return;
            }

            Task<MobiFlightModule> task = Task.Run(() =>
            {
                MobiFlightModule module = new MobiFlightModule(portDetails.Name, portDetails.Board);
                module.Connect();
                
                MobiFlightModuleInfo devInfo = module.GetInfo() as MobiFlightModuleInfo;
                // Store the hardware ID for later use
                devInfo.HardwareId = portDetails.HardwareId;
                module.HardwareId = devInfo.HardwareId;

                if (!module.HasMfFirmware())
                    module.Disconnect();

                return module;
            });

            var result = await task;

            OnCompatibleBoardDetected(result.ToMobiFlightModuleInfo());

            if (result.HasMfFirmware()) { 
                OnMobiFlightBoardDetected(result);
            }

            ModuleConnected?.Invoke(result, new EventArgs());

            await CheckIfLookUpFinished();
        }

        private async Task<bool> CheckIfLookUpFinished()
        {
            var currentModuleCount = AvailableComModules.Count;
            var allModulesDetected = await Task.Delay(TimeSpan.FromMilliseconds(3000))
                      .ContinueWith(_ => { return currentModuleCount == AvailableComModules.Count; });

            if (!allModulesDetected || !isFirstTimeLookup) return false;
            
            isFirstTimeLookup = false;

            Log.Instance.log($"End looking up connected modules. {AvailableComModules.Count} found.", LogSeverity.Debug);
            LookupFinished?.Invoke(this, new EventArgs());

            return true;
        }

        private void OnIgnoredPortDetected(object sender, PortDetails portDetails)
        {
            Log.Instance.log($"Skipping {portDetails.Name} since it is in the list of ports to ignore.", LogSeverity.Info);
            var ignoredPort = new MobiFlightModuleInfo()
            {
                Port = portDetails.Name,
                Type = "Ignored",
                Name = $"Ignored Device at Port {portDetails.Name}",
                Board = portDetails.Board,
                HardwareId = portDetails.HardwareId
            };

            AvailableComModules.Add(ignoredPort);
        }

        private void OnMobiFlightBoardDetected(MobiFlightModule module)
        {
            module.LoadConfig();
            RegisterModule(module, module.ToMobiFlightModuleInfo());
        }

        private void OnCompatibleBoardDetected(MobiFlightModuleInfo result)
        {
            AvailableComModules.Add(result);
        }

        private void UsbDeviceMonitor_PortUnavailable(object sender, PortDetails e)
        {
            Log.Instance.log($"USB device disappeared: {e.Name}", LogSeverity.Debug);
            SerialPortMonitor_PortUnavailable(sender, e);
        }

        private void UsbDeviceMonitor_PortAvailable(object sender, PortDetails e)
        {
            Log.Instance.log($"USB device detected: {e.Name} {e.Board.Info.FriendlyName}", LogSeverity.Debug);
            var info = new MobiFlightModuleInfo()
            {
                Type = e.Board.Info.FriendlyName,
                Board = e.Board,
                HardwareId = e.HardwareId,
                Name = e.Board.Info.FriendlyName,
                // It's important that this is the drive letter for the connected USB device. This is
                // used elsewhere in the flashing code to know that it wasn't connected via a COM
                // port and to skip the COM port toggle before flashing.
                Port = (e as UsbPortDetails)?.Path
            };

            // When in USB mode... we can only be a compatible board
            // and we don't have to check for MobiFlight board
            // because all MobiFlight boards are using COM ports 
            OnCompatibleBoardDetected(info);
            var result = new MobiFlightModule(info);
            ModuleConnected?.Invoke(result, new EventArgs());
        }

        /// <summary>
        /// indicates the status of the modules
        /// </summary>
        /// <returns>true if all modules are connected, false otherwise</returns>
        public bool Available()
        {
            bool result = (Modules.Count > 0);
            foreach (MobiFlightModule module in Modules.Values)
            {
                result &= module.Connected;
            }
            return result;
        }

        /// <summary>
        /// Returns a list of connected USB drives that are supported with MobiFlight and are in flash mode already,
        /// as opposed to being connected as COM port.
        /// </summary>
        /// <returns>The list of connected USB drives supported by MobiFlight.</returns>
        public static List<MobiFlightModuleInfo> FindConnectedUsbDevices()
        {
            var result = new List<MobiFlightModuleInfo>();
            var usbDeviceMonitor = new UsbDeviceMonitor();
            usbDeviceMonitor.Start();
            var task = Task
                // we need to wait for the timer to trigger
                .Delay(TimeSpan.FromMilliseconds(2000))
                .ContinueWith(_ => usbDeviceMonitor.DetectedPorts
                    .ForEach(p =>
                    {
                        result.Add(new MobiFlightModuleInfo()
                        {
                            Type = p.Board.Info.FriendlyName,
                            Board = p.Board,
                            HardwareId = p.HardwareId,
                            Name = p.Board.Info.FriendlyName,
                            // It's important that this is the drive letter for the connected USB device. This is
                            // used elsewhere in the flashing code to know that it wasn't connected via a COM
                            // port and to skip the COM port toggle before flashing.
                            Port = (p as UsbPortDetails)?.Path
                        });
                    })
                );
            
            task.Wait();
            usbDeviceMonitor.Stop();

            return result;
        }

        public bool updateConnectedModuleName(MobiFlightModule m)
        {
            if (AvailableComModules == null) return false;

            foreach (MobiFlightModuleInfo info in AvailableComModules)
            {
                if (info.Serial != m.Serial) continue;

                info.Name = m.Name;
                break;
            }

            return true;
        }

        public List<MobiFlightModuleInfo> GetDetectedCompatibleModules()
        {
            if (AvailableComModules.Count() == 0)
                    return new List<MobiFlightModuleInfo>();

            AvailableComModules.Sort(
                (item1, item2) => {
                    if (item1.Type == "Ignored" && item2.Type != "Ignored") return 1;
                    if (item1.Type != "Ignored" && item2.Type == "Ignored") return -1;
                    return item1.Name.CompareTo(item2.Name);
                });
            return AvailableComModules;
        }

        public IEnumerable<MobiFlightModule> GetModules()
        {
            if (!Available())
                return new List<MobiFlightModule>();
            return Modules.Values;
        }

        private List<string> getIgnoredPorts()
        {
            List<String> ports = new List<string>();
            if (Properties.Settings.Default.IgnoreComPorts)
            {
                ports = Properties.Settings.Default.IgnoredComPortsList.Split(',').ToList();
            }
            return ports;
        }

        /// <summary>
        /// When a MobiFlightModule is reset - then we remove it from the Modules list.
        /// It cannot be used with MobiFlight features anymore.
        /// 
        /// It is still maintained in the connectedComModules-list so that we could
        /// upload the firmware again.
        /// 
        /// The devInfo in that case is the old device info, it allows us to look it up.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="devInfo"></param>
        public void UnregisterModule(MobiFlightModule m, MobiFlightModuleInfo devInfo)
        {
            Log.Instance.log($"Unregistering module {m.Name}:{m.Port}.", LogSeverity.Debug);
            
            if (Modules.ContainsKey(devInfo.Serial))
            {
                Modules.TryRemove(devInfo.Serial, out MobiFlightModule removedModule);
                return;
            }
            
            Log.Instance.log($"Unregistering module {m.Name}:{m.Port} failed.", LogSeverity.Debug);
        }

        private void RegisterModule(MobiFlightModule m, MobiFlightModuleInfo devInfo, bool replace = false)
        {
            Log.Instance.log($"Registering module {m.Name}:{m.Port}.", LogSeverity.Debug);
            
            // Additional protection added for edge cases where this gets called after a failed firmware update, which resulted
            // in the exception reported in issue 611.
            if (String.IsNullOrEmpty(devInfo.Serial))
            {
                Log.Instance.log("A module with a null or empty serial number was specified and will not be registered.", LogSeverity.Error);
                return;
            }    

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
                    Log.Instance.log($"Duplicate serial number found {devInfo.Serial}. Module won't be added.", LogSeverity.Error);
                    return;
                }
            } else
            {      
                    Modules.TryAdd(devInfo.Serial, m);
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
        /// Disconnects all serial connections and stops all threads for safe shutdown.
        /// </summary>
        /// <returns>returns true if all modules were disconnected properly</returns>    
        public bool Shutdown()
        {
            Log.Instance.log("Disconnecting all modules.", LogSeverity.Debug);
            if (Available())
            {
                foreach (MobiFlightModule module in Modules.Values)
                {
                    module.Disconnect();
                }

                Closed(this, new EventArgs());
            }
            SerialPortMonitor.Stop();
            UsbDeviceMonitor.Stop();
            return !Available();
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
                if (module == null) return;

                if (name != null && name.Contains("|")) {
                    var pins = name.Split('|');
                    foreach(string pin in pins) {
                        if (!string.IsNullOrEmpty(pin.Trim()))
                        {
                            var iValue = (Int32)Math.Round(Double.Parse(value));
                            module.SetPin("base", pin, iValue);
                        }
                    };
                } else
                {
                    var iValue = (Int32)Math.Round(Double.Parse(value));
                    module.SetPin("base", name, iValue);
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
        public void setDisplay(string serial, string address, byte connector, List<string> digits, List<string> decimalPoints, string value, bool reverse)
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
                module.SetDisplay(address, connector - 1, ledDigit.getDecimalPoints(), (byte)ledDigit.getMask(), value, reverse);
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
                    var intValue = (Int32)Math.Round(Double.Parse(value));
                    module.SetDisplayBrightness(address, connector - 1, intValue.ToString());
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
                double dValue;
                
                if (!double.TryParse(value, out dValue)) return;

                int iValue = (int)dValue;

                module.SetServo(address, iValue, min, max, maxRotationPercent);
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_SettingServo"), e);
            }
        }

        public void setStepper(string serial, string address, string value, int inputRevolutionSteps, int outputRevolutionSteps, bool CompassMode, Int16 speed = 0, Int16 acceleration = 0)
        {
            try
            {
                if (!Modules.ContainsKey(serial)) return;

                MobiFlightModule module = Modules[serial];

                double dValue;
                if (!double.TryParse(value, out dValue)) return;

                int iValue = (int)dValue;


                if (module.GetStepper(address).OutputRevolutionSteps != outputRevolutionSteps)
                {
                    module.GetStepper(address).OutputRevolutionSteps = outputRevolutionSteps;
                }

                if (module.GetStepper(address).CompassMode != CompassMode)
                {
                    module.GetStepper(address).CompassMode = CompassMode;
                }

                if (speed>0) { module.GetStepper(address).Speed = speed; }
                if (acceleration > 0) { module.GetStepper(address).Acceleration = acceleration; }

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
                double dValue;
                if (!double.TryParse(value, out dValue)) return;

                int iValue = (int)Math.Round(dValue,0);
                module.setShiftRegisterOutput(shiftRegName, outputPin, iValue.ToString());
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
            foreach (MobiFlightModuleInfo moduleInfo in GetDetectedCompatibleModules())
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
            MobiFlightModuleInfo oldDevInfo = AvailableComModules.Find(delegate (MobiFlightModuleInfo devInfo)
            {
                return devInfo.Port == module.Port;
            }
            );

            if (oldDevInfo != null) AvailableComModules.Remove(oldDevInfo);
            AvailableComModules.Add(module.ToMobiFlightModuleInfo());
            
            if (module.HasMfFirmware())
                RegisterModule(module, module.ToMobiFlightModuleInfo(), true);
            else
                UnregisterModule(module, oldDevInfo);

            return module;
        }

        public MobiFlightModule GetModuleBySerial(string serial)
        {
            if (Modules.ContainsKey(serial)) return Modules[serial];

            return null;
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

        public Dictionary<String, List<BaseDevice>> FindAllConnectedSteppers()
        {
            var result = new Dictionary<String, List<BaseDevice>>();
            foreach (var module in Modules.Values)
            {
                var stepperList = module.GetConnectedOutputDevices().Where(p => p.Type == DeviceType.Stepper).ToList();
                if (stepperList.Count == 0) continue;
                
                result.Add(module.Name, stepperList);
            }
            return result;
        }

        internal void Set(string serial, OutputConfig.CustomDevice deviceConfig, string value, List<ConfigRefValue> configRefValues)
        {
            if (serial == null)
            {
                throw new ConfigErrorException("ConfigErrorException_SerialNull");
            };

            try
            {
                if (!Modules.ContainsKey(serial)) return;

                MobiFlightModule module = Modules[serial];
                module.setCustomDevice(deviceConfig.CustomName, deviceConfig.MessageType, value);
            }
            catch (Exception e)
            {
                throw new MobiFlight.ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_SetCustomDevice"), e);
            }
        }

        public void ResumeModuleScan()
        {
            SerialPortMonitor.Start();
            UsbDeviceMonitor.Start();
        }

        public void PauseModuleScan()
        {
            SerialPortMonitor.Stop();
            UsbDeviceMonitor.Stop();
        }
    }
}
