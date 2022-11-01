using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleSolutions.Usb;
using MobiFlight;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class ArcazeCache : ModuleCacheInterface
    {
        public bool Enabled { get; set; }
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

        const int MAX_DEVICE_NUM    = 5;

        /// <summary>
        /// the arcaze module interface
        /// </summary>
        private ArcazeHid arcazeHid = new ArcazeHid();

        Dictionary<String, ArcazeModule> Modules = new Dictionary<String, ArcazeModule>();

        /// <summary>
        /// the available arcaze modules
        /// </summary>
        private List<DeviceInfo> presentArcaze = new List<DeviceInfo>(5);
        private List<DeviceInfoAndCache> attachedArcaze = new List<DeviceInfoAndCache>(5);

        /// <summary>
        /// the settings about the extension modules and global init stuff
        /// </summary>
        private Dictionary<string, ArcazeModuleSettings> arcazeSettings = new Dictionary<string, ArcazeModuleSettings>();

        /// <summary>
        /// the used led displays
        /// </summary>
        private Dictionary<string, ArcazeLedDisplayConfig> ledDisplays = new Dictionary<string, ArcazeLedDisplayConfig>();
        private bool _initialized;

        public void updateModuleSettings(Dictionary<string, ArcazeModuleSettings> arcazeSettings)
        {
            this.arcazeSettings = arcazeSettings;            
        }

        /// <summary>
        /// indicates the status of the module connection
        /// </summary>
        /// <returns>true if connected, false if not</returns>
        public bool isConnected()
        {
            bool result = (Modules.Count > 0);

            if (!Enabled) return result;

            foreach (ArcazeModule module in Modules.Values)
            {
                result = result & module.Connected;
            }
            return result;

            /*
            bool result = (attachedArcaze.Count > 0);
            foreach (DeviceInfoAndCache dev in attachedArcaze)
            {
                result = result & dev.m_arcazeHid.Info.Connected;
            }
            return result;            
             */
        }

        /// <summary>
        /// call the Connect method on each present module and reset the cached states for each module
        /// </summary>
        /// <returns>returns true if arcaze modules were connected successfully otherwise returns false</returns>
        public bool connect(bool force=false)
        {
            if (!Enabled) return false;

            if (isConnected() && force)
            {
                disconnect();
            }
            
            if (!_initialized) _initialize();

            foreach (ArcazeModule module in Modules.Values)
            {
                module.Connect();
            }

            if (isConnected())
            {
                this.Connected(this, new EventArgs());
                return true;
            }

            return false;
        }

        void m_arcazeHid_DeviceRemoved(object sender, HidEventArgs e)
        {
            this.ConnectionLost(this, new EventArgs());
        }

        void m_arcazeHid_OurDeviceRemoved(object sender, HidEventArgs e)
        {
            this.ConnectionLost(this, new EventArgs());
        }

        /// <summary>
        /// convenient method to look up modules and connect to them with a single method call
        /// </summary>
        /// <returns>returns true if all present modules were connected properly</returns>
        protected bool _initialize()
        {
            _initialized = false;
            if (_initialized = _lookupArcazeModules())
            {
                _initialized = connect();                
            }
            return _initialized;
        } //_initializeArcaze()

        /// <summary>
        /// disconnects all arcaze modules
        /// </summary>
        /// <returns>returns true if all modules were disconnected properly</returns>    
        public bool disconnect()
        {
            if (!Enabled) return true;

            if (isConnected())
            {
                foreach (DeviceInfoAndCache dev in attachedArcaze)
                {
                    dev.m_arcazeHid.Disconnect();
                }
                arcazeHid.Disconnect();
                _initialized = false;
                Closed(this, new EventArgs());
            }
            return !isConnected();
        }

        public void Clear()
        {
            if (!Enabled) return;

            foreach (ArcazeModule module in Modules.Values)
            {
                module.ClearCache();
            }
        }

        /// <summary>
        /// gathers infos about the connected modules and stores information in different objects
        /// </summary>
        /// <returns>returns true if there are modules present</returns>
        private bool _lookupArcazeModules()
        {
            List<DeviceInfo> allArcaze = new List<DeviceInfo>(MAX_DEVICE_NUM);

            arcazeHid.Find(allArcaze);
            presentArcaze = arcazeHid.RemoveSameSerialDevices(allArcaze);

            Modules.Clear();
            
            foreach (DeviceInfo dev in presentArcaze)
            {
                if (arcazeSettings.ContainsKey(dev.Serial))
                    Modules.Add(dev.Serial, new ArcazeModule(new DeviceInfoAndCache(dev), arcazeSettings[dev.Serial]));
                else
                    Modules.Add(dev.Serial, new ArcazeModule(new DeviceInfoAndCache(dev), new ArcazeModuleSettings()));
            }

            return Modules.Count > 0;
        } //lookupArcazeModules()

        public IEnumerable<IModuleInfo> getModuleInfo()
        {
            List<IModuleInfo> result = new List<IModuleInfo>();

            if (!Enabled) return result;


            if (!_initialized && Modules.Count == 0) _lookupArcazeModules();
            foreach (ArcazeModule module in Modules.Values)
            {
                result.Add(module.GetInfo());
            }
            return result.OrderBy(m => m.Name);
        }

        /// <summary>
        /// reads the pin's status in the arcaze module
        /// </summary>
        /// <param name="serial">the device's serial</param>
        /// <param name="portAndPin">the port letter and pin number, e.g. A01</param>
        /// <param name="trigger">the trigger to be used</param>        
        public string getValue(string serial, string portAndPin, string trigger)
        {
            string result = null;

            if (portAndPin == null || portAndPin == "")
            {
                throw new ArgumentException("Port and pin is not valid.");                
            } //if

            try
            {
                result = Modules[serial].getValue(portAndPin, trigger);
            }
            catch (Exception e)
            {
                //throw e;
                //this.ConnectionLost(this, new EventArgs());
            }

            return result;
        }

        /// <summary>
        /// sets the pins in the arcaze modules according to the given value
        /// </summary>
        /// <param name="serial">the device's serial</param>
        /// <param name="portAndPin">the port letter and pin number, e.g. A01</param>
        /// <param name="value">the value to be used</param>
        public void setValue (string serial, string portAndPin, string value)
        {
            if (portAndPin == null || portAndPin == "") return;
            ArcazeIoBasic io = new ArcazeIoBasic(portAndPin);

            try
            {
                Modules[serial].SetPin(io.Port, io.Pin, UInt16.Parse(value));
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
                //throw e;
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
                Modules[serial].setDisplay(address, connector, digits, decimalPoints, value);
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_WritingDisplay"), e);
            }
        }        

        public void clearGetValues()
        {
            foreach (ArcazeModule module in Modules.Values)
            {
                module.ClearCache(ArcazeModule.CacheType.inputCache);
            }

            ledDisplays.Clear();
        }

        internal void flush()
        {
            throw new NotImplementedException();
        }

        internal void setBcd4056(string serial, List<string> list, string value)
        {
            Modules[serial].setBcd4056(list, value);
        }

        /// <summary>
        /// rebuilt Arcaze module settings from the stored configuration
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ArcazeModuleSettings> GetArcazeModuleSettings()
        {
            List<ArcazeModuleSettings> moduleSettings = new List<ArcazeModuleSettings>();
            Dictionary<string, ArcazeModuleSettings> result = new Dictionary<string, ArcazeModuleSettings>();

            if (!Enabled || "" == Properties.Settings.Default.ModuleSettings) 
                return result;

            try
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(List<ArcazeModuleSettings>));
                System.IO.StringReader w = new System.IO.StringReader(Properties.Settings.Default.ModuleSettings);
                moduleSettings = (List<ArcazeModuleSettings>)SerializerObj.Deserialize(w);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Deserialize problem: {ex.Message}", LogSeverity.Error);
            }

            foreach (ArcazeModuleSettings setting in moduleSettings)
            {
                result[setting.serial] = setting;
            }

            return result;
        }
    }
}
