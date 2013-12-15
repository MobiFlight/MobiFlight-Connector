using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleSolutions.Usb;

namespace ArcazeUSB
{
    public class ArcazeCache
    {
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
        const int MAX_PIN_NUM       = 40;

        /// <summary>
        /// the look up table with the last set arcaze values
        /// </summary>
        Dictionary<string, Dictionary<string, string>>
            lastArcazeValue = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// the look up table with the last set arcaze values
        /// </summary>
        Dictionary<string, Dictionary<string, int>>
            lastArcazeGetValue = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>
        /// the arcaze module interface
        /// </summary>
        private ArcazeHid arcazeHid = new ArcazeHid();

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
        /// indicates the status of the fsuipc connection
        /// </summary>
        /// <returns>true if connected, false if not</returns>
        public bool isConnected()
        {
            bool result = (attachedArcaze.Count > 0);
            foreach (DeviceInfoAndCache dev in attachedArcaze)
            {
                result = result & dev.m_arcazeHid.Info.Connected;
            }
            return result;            
        }

        /// <summary>
        /// call the Connect method on each present module and reset the cached states for each module
        /// </summary>
        /// <returns>returns true if arcaze modules were connected successfully otherwise returns false</returns>
        public bool connect(bool force=false)
        {
            if (isConnected()&&force) { disconnect(); }
            if (!_initialized) _initialize();

            // Connect to all attached modules            
            foreach (DeviceInfoAndCache dev in attachedArcaze)
            {
                // TBD: Only connect if in Adapters List
                dev.m_arcazeHid.Connect(dev.m_deviceInfo.Path);

                // Init cached port dirs/states
                for (int num = 0; num < dev.m_portDirs.Count(); num++) dev.m_portDirs[num] = 0;
                for (int num = 0; num < dev.m_portValues.Count(); num++) dev.m_portValues[num] = 0;

                if (arcazeSettings.ContainsKey(dev.m_deviceInfo.Serial)) {
                    ArcazeModuleSettings settings = arcazeSettings[dev.m_deviceInfo.Serial];
                    dev.m_arcazeHid.Command.CmdInitExtensionPort(
                    settings.type,
                    settings.numModules,
                    (settings.type == ArcazeCommand.ExtModuleType.LedDriver3) ? 8 : 1,
                    settings.globalBrightness);                
                } else {
                    // fallback if no config found
                    dev.m_arcazeHid.Command.CmdInitExtensionPort(
                    ArcazeCommand.ExtModuleType.LedDriver3,
                    1,
                    8,
                    Properties.Settings.Default.LedIntensity);                
                }
            }

            _initializePortDirections();

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
            lastArcazeValue.Clear();
            lastArcazeGetValue.Clear();
            ledDisplays.Clear();
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

            attachedArcaze.Clear();
            foreach (DeviceInfo dev in presentArcaze)
            {
                DeviceInfoAndCache newDev = new DeviceInfoAndCache(dev);
                attachedArcaze.Add(newDev);
            }

            return attachedArcaze.Count > 0;
        } //lookupArcazeModules()

        public IEnumerable<DeviceInfo> getDeviceInfo()
        {
            if (!_initialized && presentArcaze.Count==0) _lookupArcazeModules();            
            return presentArcaze;
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

            ArcazeIoBasic io = new ArcazeIoBasic(portAndPin);

            // check if value has already been read to prevent
            // unnecessary communication with Arcaze USB
            if (lastArcazeGetValue.ContainsKey(serial) &&
                lastArcazeGetValue[serial].ContainsKey(io.Port.ToString()))
            {
                int currentValues = lastArcazeGetValue[serial][io.Port.ToString()];
                int bitValue = (int) (1 << io.Pin);
                result = ((currentValues & (1 << io.Pin)) == (1 << io.Pin)) ? "0" : "1";
                return result;
            }

            if (!lastArcazeGetValue.ContainsKey(serial))
            {
                lastArcazeGetValue[serial] = new Dictionary<string, int>();
            }            

            try
            {
                DeviceInfoAndCache dev = SelectArcazeBySerial(serial);
                if (null != dev)
                {
                    lastArcazeGetValue[serial][io.Port.ToString()] =  dev.m_arcazeHid.Command.CmdReadPort(
                        io.Port
                    );
                    int currentValues = lastArcazeGetValue[serial][io.Port.ToString()];
                    result = ((currentValues & (1 << io.Pin)) == (1 << io.Pin)) ? "0" : "1";
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

            return result;
        }

        /// <summary>
        /// sets the pins in the arcaze modules according to the given value
        /// </summary>
        /// <param name="serial">the device's serial</param>
        /// <param name="portAndPin">the port letter and pin number, e.g. A01</param>
        /// <param name="trigger">the trigger to be used</param>
        /// <param name="value">the value to be used</param>
        public void setValue (string serial, string portAndPin, string trigger, string value)
        {
            if (portAndPin == null || portAndPin == "") return;

            // if value has not changed since the last time, then we continue to next item to prevent 
            // unnecessary communication with Arcaze USB
            if (lastArcazeValue.ContainsKey(serial) &&
                lastArcazeValue[serial].ContainsKey(portAndPin) &&
                lastArcazeValue[serial][portAndPin] == value) return;

            // otherwise store it
            bool setOutputDirection = false;

            // determine the correct arcaze usb pin defined by string, e.g. "A01"
            ArcazeIoBasic io = new ArcazeIoBasic(portAndPin);                  
            
            if (!lastArcazeValue.ContainsKey(serial))
            {
                lastArcazeValue[serial] = new Dictionary<string, string>();
            }

            if (!lastArcazeValue[serial].ContainsKey(portAndPin))
            {
                setOutputDirection = io.Port < 2; // only set output direction for arcaze internal pins
            };

            lastArcazeValue[serial][portAndPin] = value;

            try
            {
                DeviceInfoAndCache dev = SelectArcazeBySerial(serial);
                if (null != dev)
                {
                    if (setOutputDirection )
                    {
                        dev.m_arcazeHid.Command.CmdSetPinDirection(io.Port, io.Pin, 1);
                    }
                    // dev.m_arcazeHid.Command.CmdSetPin(io.Port, io.Pin, Int16.Parse(value));
                    
                    dev.m_arcazeHid.Command.WriteOutputPort(
                        0, 
                        io.Port, 
                        io.Pin, 
                        UInt16.Parse(value), 
                        ArcazeCommand.OutputOperators.PlainWrite, 
                        true);
                    
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

            String cacheSerial = serial + "LED" + address;
            String portAndPin = connector.ToString();            

            // if value has not changed since the last time, then we continue to next item to prevent 
            // unnecessary communication with Arcaze USB
            if (lastArcazeValue.ContainsKey(cacheSerial) &&
                lastArcazeValue[cacheSerial].ContainsKey(portAndPin) &&
                lastArcazeValue[cacheSerial][portAndPin] == value) return;

            // otherwise store it
            if (!lastArcazeValue.ContainsKey(cacheSerial))
            {
                lastArcazeValue[cacheSerial] = new Dictionary<string, string>();
                _initLedDisplay(serial, address);
            }

            if (!lastArcazeValue[cacheSerial].ContainsKey(portAndPin))
            {
                // nothing to do here right now
            }

            lastArcazeValue[cacheSerial][portAndPin] = value;

            try
            {
                DeviceInfoAndCache dev = SelectArcazeBySerial(serial);
                if (null != dev)
                {
                    ArcazeLedDigit led = new ArcazeLedDigit();

                    led.setConnector(connector);

                    foreach (string digit in digits) {
                        led.setActive(ushort.Parse(digit));
                    }

                    foreach (string decimalPoint in decimalPoints) {
                        led.setDecimalPoint (ushort.Parse(decimalPoint));
                    }

                    dev.m_arcazeHid.Command.CmdMax7219WriteDigits(
                            Int16.Parse(address.Replace("0x",""), System.Globalization.NumberStyles.HexNumber),
                            led.convert(value),
                            led.getMask()
                    );
                }
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(MainForm._tr("ConfigErrorException_WritingDisplay"), e);
            }
        }

        private void _initLedDisplay(string serial, string address)
        {
            ArcazeLedDisplayConfig cfg = new ArcazeLedDisplayConfig();
            cfg.address = address;
            cfg.mode = DecodeMode.Binary;
            cfg.scanLimit = 0x08;
            cfg.intensity = 0xff;

            if (arcazeSettings.ContainsKey(serial))
            {
                cfg.intensity = (ushort) (arcazeSettings[serial].globalBrightness / 16.0);
            }

            try
            {
                DeviceInfoAndCache dev = SelectArcazeBySerial(serial);
                if (null != dev)
                {
                    // init the module 
                    // TODO: read settings from config
                    dev.m_arcazeHid.Command.CmdMax7219DisplayInit(
                        cfg.addressInternal(),
                        (ushort) cfg.mode,
                        cfg.intensity,
                        cfg.scanLimit
                    );
                  
                    // set all digits to blank once
                    dev.m_arcazeHid.Command.CmdMax7219WriteDigits(
                        cfg.addressInternal(),
                        new List<byte>(8) { 0,0,0,0,0,0,0,0 },
                        0xff
                    );
                }
            }
            catch (FormatException e)
            {
                // do nothing
                // maybe log this some time in the future                
            }
            catch (Exception e)
            {
                this.ConnectionLost(this, new EventArgs());
            }            
        }

        /// <summary>
        /// returns handle of the according aracaze module
        /// </summary>
        public DeviceInfoAndCache SelectArcazeBySerial(String serial)
        {
            DeviceInfoAndCache result = null;

            foreach (DeviceInfoAndCache dev in attachedArcaze)
                if (dev.m_deviceInfo.Serial == serial)
                    result = dev;

            return result;
        } //SelectArcazeBySerial()

        /// <summary>
        /// this method initializes the port directions depending on the current config
        /// </summary>
        /// <remarks>
        /// @stephan: please check if this needs to be done and when it needs to be done
        /// </remarks>
        private void _initializePortDirections()
        {
            // Set Arcaze Port Directions
            //   This is dirty!
            //   => TBD: Reduce max number of USB accesses to 4 per module:
            //    * Collect directions in a common variable per port + per module
            //    * read PortDirs from module, 
            //    * change, 
            //    * write back.
            //   But currently there is no ReadPortDir command available to do this...

            //foreach (Adapter adp in activeAdapters)
            //{
            //    if (adp.m_arcazePort >= DeviceInfoAndCache.ARCAZE_NUM_PORTS || adp.m_arcazePort < 0)
            //        continue;

            //    // Check if this module is available, if not do nothing for this adapter
            //    arcaze = SelectArcazeBySerial(adp.m_arcazeSerial);

            //    if (arcaze == null)
            //    {
            //        Log("Arcaze Module with Serial " + adp.m_arcazeSerial + " not found!");
            //        continue;
            //    }

            //    Int32 mask = 1;
            //    for (Int32 bit = 0; bit < 32; bit++)
            //    {
            //        if ((adp.m_arcazeMask & mask) > 0)
            //        {
            //            arcaze.m_portDirs[adp.m_arcazePort] |= mask;
            //            arcaze.m_arcazeHid.Command.CmdSetPinDirection(adp.m_arcazePort, bit, 1);
            //        }
            //        mask <<= 1;
            //    }
            //}
        } //initializePortDirections()

        /// <summary>
        /// return valid pin numbers
        /// </summary>      
        /// <remarks>
        /// this should be refactored to the ArcazeIoBasic-class
        /// </remarks>
        public String[] getPins()
        {
            String[] result = new String[(MAX_PIN_NUM / 2)];
            for (int i = 0; i < (MAX_PIN_NUM / 2); i++)
            {
                result[i] = String.Format("{0:00}", i+1);                
            }

            return result;
        }

        /// <summary>
        /// return valid ports
        /// </summary>  
        /// <remarks>
        /// this should be refactored to the ArcazeIoBasic-class
        /// </remarks>
        public String[] getPorts ()
        {
            return new String[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N" };            
        }

        public void clearGetValues()
        {
            lastArcazeGetValue.Clear();
        }

        internal void flush()
        {
            throw new NotImplementedException();
        }

        internal void setBcd4056(string serial, List<string> list, string trigger, string value)
        {
            ArcazeBcd4056 display = new ArcazeBcd4056();
            List<byte> vals = display.convert(value);            
            // set the value
            setValue(serial, list[1], trigger, vals[0].ToString());
            setValue(serial, list[2], trigger, vals[1].ToString());
            setValue(serial, list[3], trigger, vals[2].ToString());
            setValue(serial, list[4], trigger, vals[3].ToString());
            // store the value
            setValue(serial, list[0], trigger, "1");
            setValue(serial, list[0], trigger, "0");
        }
    }
}
