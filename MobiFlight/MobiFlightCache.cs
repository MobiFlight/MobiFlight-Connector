using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using MobiFlight;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace MobiFlight
{
    public class MobiFlightCache : ModuleCacheInterface
    {
        public delegate void ButtonEventHandler(object sender, ButtonArgs e);
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

        private List<MobiFlightModuleInfo> connectedModules = null;

        private bool _lookingUpModules = false;

        DateTime servoTime = DateTime.Now;
        /// <summary>
        /// list of known modules
        /// </summary>
        Dictionary<string, MobiFlightModule> Modules = new Dictionary<string, MobiFlightModule>();
        Dictionary<string, MobiFlightModuleConfig> configs;

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

        public bool updateModuleSettings(Dictionary<string, MobiFlightModuleConfig> configs)
        {
            this.configs = configs;
            return true;
        }

        public List<MobiFlightModuleInfo> getConnectedModules()
        {
            if (connectedModules == null)
            {
                connectedModules = lookupModules();
            }
            return connectedModules;
        }

        public IEnumerable<MobiFlightModule> GetModules()
        {
            if (!isConnected()) connect();
            return Modules.Values;
        }

        private static List<Tuple<string, string>> getArduinoPorts()
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            String[] arduinoVidPids = { 
                MobiFlightModuleInfo.PIDVID_MICRO, // Micro
                MobiFlightModuleInfo.PIDVID_MEGA,  // Mega
                MobiFlightModuleInfo.PIDVID_MEGA_10,  // Mega
                MobiFlightModuleInfo.PIDVID_MEGA_CLONE,  // Mega
                MobiFlightModuleInfo.PIDVID_MEGA_CLONE_1,  // Mega
                MobiFlightModuleInfo.PIDVID_MEGA_CLONE_2  // Mega
            };
            Regex regEx = new Regex( "^(" + string.Join("|", arduinoVidPids) + ")" );

            RegistryKey regLocalMachine = Registry.LocalMachine;
            RegistryKey regUSB = regLocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\USB");
            foreach (String regDevice in regUSB.GetSubKeyNames())
            {
                if (regEx.Match(regDevice).Success)
                {
                    String VidPid = regEx.Match(regDevice).Value;
                    foreach (String regSubDevice in regUSB.OpenSubKey(regDevice).GetSubKeyNames())
                    {
                        // we have found an existing entry 
                        // let's check if it is currently connected#
                        try
                        {
                            //String val = regUSB.OpenSubKey(regDevice).OpenSubKey(regSubDevice).OpenSubKey("Control").GetValue("ActiveService") as String;
                            String portName = regUSB.OpenSubKey(regDevice).OpenSubKey(regSubDevice).OpenSubKey("Device Parameters").GetValue("PortName") as String;
                            if (portName != null)
                                result.Add(new Tuple<string, string>(portName, VidPid));
                        }
                        catch (Exception e)
                        {
                            // not available therefore not connected
                            String message = "Arduino device not connected -skipping: " + regDevice;
                            Log.Instance.log(message, LogSeverity.Debug);
                            continue;
                        }
                    }
                }
                else
                {
                    String message = "No arduino device -skipping: " + regDevice;
                    Log.Instance.log(message, LogSeverity.Debug);
                }
            }
            return result;
        }

        private List<MobiFlightModuleInfo> lookupModules()
        {
            List<MobiFlightModuleInfo> result = new List<MobiFlightModuleInfo>();
            string[] connectedPorts = SerialPort.GetPortNames();

            if (_lookingUpModules) return result;
            _lookingUpModules = true;

            foreach (Tuple<string, string> item in getArduinoPorts())
            {
                String portName = item.Item1;

                if (!connectedPorts.Contains(portName)) continue;
                MobiFlightModule tmp = new MobiFlightModule(new MobiFlightModuleConfig() { ComPort = portName });
                tmp.Connect();
                MobiFlightModuleInfo devInfo = tmp.GetInfo() as MobiFlightModuleInfo;
                tmp.Disconnect();

                if (devInfo.Type == MobiFlightModuleInfo.TYPE_UNKNOWN)
                {
                    devInfo.SetTypeByVidPid(item.Item2);
                }

                result.Add(devInfo);
            }
            _lookingUpModules = false;

            return result;
        }

        public bool connect(bool force=false)
        {
            if (isConnected() && force) { 
                disconnect(); 
            }
            if (connectedModules==null)
                connectedModules = lookupModules();

            Modules.Clear();

            foreach (MobiFlightModuleInfo devInfo in connectedModules)
            {
                if (!devInfo.HasMfFirmware()) continue;

                MobiFlightModule m = new MobiFlightModule(new MobiFlightModuleConfig() { ComPort = devInfo.Port });
                RegisterModule(m, devInfo);
            }

            // Connect to all attached modules            
            foreach (MobiFlightModule module in Modules.Values)
            {
                module.Connect();
                module.GetInfo();
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
            m.OnButtonPressed += new MobiFlightModule.ButtonEventHandler(module_OnButtonPressed);

            if (Modules.ContainsKey(devInfo.Serial))
            {
                if (replace)
                {
                    Modules[devInfo.Serial] = m;
                }
                else
                {
                    Log.Instance.log("Duplicate serial number found: " + devInfo.Serial + ". Module won't be added.", LogSeverity.Error);
                }
                return;
            }

            Modules.Add(devInfo.Serial, m);
        }

        public void module_OnButtonPressed(object sender, ButtonArgs e)
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
            try{
                if (name == null || name == "") return;

                MobiFlightModule module = GetModuleBySerial(serial);
                if (module == null) return;

                module.SetPin("base", name, Int16.Parse(value));
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
                throw new MobiFlight.ArcazeCommandExecutionException(MainForm._tr("ConfigErrorException_WritingDisplay"), e);
            }
        }

        public void setServo(string serial, string address, string value, int min, int max, byte maxRotationPercent)
        {
            try
            {
                MobiFlightModule module = Modules[serial];
                int iValue;
                if (!int.TryParse(value, out iValue)) return;

                module.SetServo(address, iValue, min, max, maxRotationPercent);
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(MainForm._tr("ConfigErrorException_SettingServo"), e);
            }
        }

        public void setStepper(string serial, string address, string value, int inputRevolutionSteps, int outputRevolutionSteps)
        {
            try
            {
                MobiFlightModule module = Modules[serial];
                int iValue;
                if (!int.TryParse(value, out iValue)) return;
                if (module.GetStepper(address).OutputRevolutionSteps != outputRevolutionSteps)
                {
                    module.GetStepper(address).OutputRevolutionSteps = outputRevolutionSteps;
                }
                module.SetStepper(address, iValue, inputRevolutionSteps);
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(MainForm._tr("ConfigErrorException_SettingServo"), e);
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
                throw new ArcazeCommandExecutionException(MainForm._tr("ConfigErrorException_SettingServo"), e);
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
        }
        
        internal IEnumerable<IModuleInfo> getModuleInfo()
        {
            List<IModuleInfo> result = new List<IModuleInfo>();
            foreach (MobiFlightModuleInfo moduleInfo in getConnectedModules())
            {
                if (moduleInfo.HasMfFirmware())
                    result.Add(moduleInfo);
            }

            return result;
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
            MobiFlightModuleInfo oldDevInfo = connectedModules.Find(delegate(MobiFlightModuleInfo devInfo)
            {
                return devInfo.Port == module.Port;
            }
            );

            if (oldDevInfo != null) connectedModules.Remove(oldDevInfo);
            connectedModules.Add(module.ToMobiFlightModuleInfo());

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
    }
}
