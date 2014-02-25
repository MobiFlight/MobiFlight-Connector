using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using MobiFlight;

namespace ArcazeUSB
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

        DateTime servoTime = DateTime.Now;
        /// <summary>
        /// list of known modules
        /// </summary>
        List<MobiFlightModule> Modules = new List<MobiFlightModule>();
        Dictionary<string, MobiFlightModuleConfig> configs;

        /// <summary>
        /// indicates the status of the fsuipc connection
        /// </summary>
        /// <returns>true if connected, false if not</returns>
        public bool isConnected()
        {
            bool result = (Modules.Count > 0);
            foreach (MobiFlightModule module in Modules)
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
            if (connectedModules == null) connectedModules = lookupModules();
            return connectedModules;
        }

        public List<MobiFlightModule> GetModules()
        {
            if (!isConnected()) connect();
            return Modules;
        }

        private List<MobiFlightModuleInfo> lookupModules()
        {
            List<MobiFlightModuleInfo> result = new List<MobiFlightModuleInfo>();
            
            foreach (string portName in SerialPort.GetPortNames())
            {
                // we ignore the COM1 because it is normally an internal port and not one used
                // by the hardware
                if (portName == "COM1") continue;
                MobiFlightModule tmp = new MobiFlightModule(new MobiFlightModuleConfig() { ComPort = portName });
                tmp.Connect();
                MobiFlightModuleInfo devInfo = tmp.GetInfo() as MobiFlightModuleInfo;
                tmp.Disconnect();
                if (devInfo.Type != MobiFlightModuleInfo.TYPE_UNKNOWN)
                {
                    result.Add(devInfo);
                }
            }
            
            if (result.Count == 0)
            {
            //    result.Add(new MobiFlightModuleInfo() { Type = MobiFlightModuleInfo.TYPE_MEGA, Name = "Transponder Panel", Port = "COM3" });
            //    result.Add(new MobiFlightModuleInfo() { Type = MobiFlightModuleInfo.TYPE_MICRO, Name = "Stepper Module", Port = "COM13" });
            }

            return result;
        }

        public bool connect(bool force=false)
        {
            if (isConnected() && force) { 
                disconnect(); 
            }
            connectedModules = lookupModules();
            Modules.Clear();

            foreach (MobiFlightModuleInfo devInfo in connectedModules)
            {
                MobiFlightModule m = new MobiFlightModule(new MobiFlightModuleConfig() { ComPort = devInfo.Port });
                m.OnButtonPressed += new MobiFlightModule.ButtonEventHandler(module_OnButtonPressed);
                Modules.Add(m);
            }

            // Connect to all attached modules            
            foreach (MobiFlightModule module in Modules)
            {
                // module.UpdateConfig(
                module.Connect();
            }

            if (isConnected())
            {
                //Connected(this, new EventArgs());
                return true;
            }

            return false;
        }

        void module_OnButtonPressed(object sender, ButtonArgs e)
        {
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
                foreach (MobiFlightModule module in Modules)
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
        /// <param name="portAndPin">the port letter and pin number, e.g. A01</param>
        /// <param name="trigger">the trigger to be used</param>
        /// <param name="value">the value to be used</param>
        public void setValue(string serial, string portAndPin, string value)
        {
            try{
                if (portAndPin == null || portAndPin == "") return;
                int index = Int16.Parse(serial);
                // ensure no out of bounds
                if (index >= Modules.Count) return;

                MobiFlightModule module = Modules[index];
                MobiFlightIOBasic io = new MobiFlightIOBasic(portAndPin);

                module.SetPin(io.Port, io.Pin, Int16.Parse(value));
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
            
            String portAndPin = "LED0"+address;

            try
            {
                if (portAndPin == null || portAndPin == "") return;
                int index = Int16.Parse(serial);
                // ensure no out of bounds
                if (index >= Modules.Count) return;

                MobiFlightModule module = Modules[index];
                MobiFlightIOBasic io = new MobiFlightIOBasic(portAndPin);

                module.SetDisplay(io.Pin, 0, value);
            }
            catch (Exception e)
            {
                throw new ArcazeUSB.ArcazeCommandExecutionException(MainForm._tr("ConfigErrorException_WritingDisplay"), e);
            }
        }

        public void setServo(string serial, string address, string value)
        {
            String portAndPin = "SERVO0" + address;
            TimeSpan elapsed = DateTime.Now.Subtract(servoTime);
            //if (elapsed.TotalMilliseconds < 100) return;
            try
            {
                //
                if (portAndPin == null || portAndPin == "") return;
                int index = Int16.Parse(serial);
                // ensure no out of bounds
                if (index >= Modules.Count) return;

                MobiFlightModule module = Modules[index];
                MobiFlightIOBasic io = new MobiFlightIOBasic(portAndPin);

                int iValue;
                if (!int.TryParse(value, out iValue)) return;

                module.SetServo(io.Pin, iValue);
                servoTime = DateTime.Now;
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


        internal IEnumerable<IModuleInfo> getModuleInfo()
        {
            List<IModuleInfo> result = new List<IModuleInfo>();
            return getConnectedModules();
        }

        public MobiFlightModule GetModule(string port)
        {
            foreach (MobiFlightModule module in Modules)
            {
                if (module.Port == port) return module;
            }

            throw new IndexOutOfRangeException();
        }

        internal MobiFlightModule GetModule(MobiFlightModuleInfo moduleInfo)
        {
            foreach (MobiFlightModule module in Modules)
            {
                if (module.Port == moduleInfo.Port) return module;                
            }

            foreach (MobiFlightModule module in Modules)
            {
                if (module.Serial == moduleInfo.Serial) return module;
            }            

            throw new IndexOutOfRangeException();
        }
    }
}
