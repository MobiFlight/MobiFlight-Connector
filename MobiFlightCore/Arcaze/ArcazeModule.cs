using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobiFlight;
using SimpleSolutions.Usb;

namespace MobiFlight
{
    public class ArcazeModule : IModule /*, IOutputModule */
    {
        public enum CacheType
        {
            outputCache = 1,
            inputCache = 2
        }

        /// <summary>
        /// Gets raised whenever connection is close
        /// </summary>
        public event EventHandler OnClose;
        /// <summary>
        /// Gets raised whenever connection is established
        /// </summary>
        public event EventHandler OnConnect;
        /// <summary>
        /// Gets raised whenever connection is lost
        /// </summary>
        public event EventHandler ConnectionLost;

        const int MAX_PIN_NUM = 40;
        DeviceInfoAndCache _info;
        ArcazeModuleSettings _settings;

        /// <summary>
        /// the look up table with the last set arcaze values
        /// </summary>
        Dictionary<string, string> lastArcazeValue = new Dictionary<string, string>();


        /// <summary>
        /// the look up table with the last set arcaze values
        /// </summary>
        Dictionary<string, int> lastArcazeGetValue = new Dictionary<string, int>();


        public ArcazeModule(DeviceInfoAndCache info, ArcazeModuleSettings settings)
        {
            _info = info;
            _settings = settings;
        }

        public bool Connected
        {
            get
            {
                return _info.m_arcazeHid.Info.Connected;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Connect()
        {
            // TBD: Only connect if in Adapters List
            _info.m_arcazeHid.Connect(_info.m_deviceInfo.Path);

            // Init cached port dirs/states
            for (int num = 0; num < _info.m_portDirs.Count(); num++) _info.m_portDirs[num] = 0;
            for (int num = 0; num < _info.m_portValues.Count(); num++) _info.m_portValues[num] = 0;

            _info.m_arcazeHid.Command.CmdInitExtensionPort(
            _settings.type,
            _settings.numModules,
            (_settings.type == ArcazeCommand.ExtModuleType.LedDriver3) ? 8 : 1,
            _settings.globalBrightness);            
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public IModuleInfo GetInfo()
        {
            return new ArcazeModuleInfo(_info.m_deviceInfo);
        }

        /// <summary>
        /// return valid pin numbers
        /// </summary>      
        /// <remarks>
        /// this should be refactored to the ArcazeIoBasic-class
        /// </remarks>
        public static String[] getPins()
        {
            String[] result = new String[(MAX_PIN_NUM / 2)];
            for (int i = 0; i < (MAX_PIN_NUM / 2); i++)
            {
                result[i] = String.Format("{0:00}", i + 1);
            }

            return result;
        }

        /// <summary>
        /// return valid ports
        /// </summary>  
        /// <remarks>
        /// this should be refactored to the ArcazeIoBasic-class
        /// </remarks>
        public static String[] getPorts()
        {
            return new String[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N" };
        }

        /// <summary>
        /// return valid addresses
        /// </summary>  
        /// <remarks>
        /// this should be refactored to the ArcazeIoBasic-class
        /// </remarks>
        public static String[] getDisplayAddresses()
        {
            return new String[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
        }

        public static String[] getDisplayConnectors()
        {
            return new String[] { "1", "2" };
        }

        public bool SetPin(int port, int pin, int value)
        {
            string portAndPin = port.ToString() + "_" + pin.ToString();
            string strValue = value.ToString();

            // if value has not changed since the last time, then we continue to next item to prevent 
            // unnecessary communication with Arcaze USB
            if (lastArcazeValue.ContainsKey(portAndPin) &&
                lastArcazeValue[portAndPin] == strValue) return false;

            // otherwise store it
            bool setOutputDirection = false;

            if (!lastArcazeValue.ContainsKey(portAndPin))
            {
                setOutputDirection = port < 2; // only set output direction for arcaze internal pins
            };

            lastArcazeValue[portAndPin] = strValue;

            DeviceInfoAndCache dev = _info;
            if (null != dev)
            {
                if (setOutputDirection)
                {
                    dev.m_arcazeHid.Command.CmdSetPinDirection(port, pin, 1);
                }
                // dev.m_arcazeHid.Command.CmdSetPin(io.Port, io.Pin, Int16.Parse(value));

                _info.m_arcazeHid.Command.WriteOutputPort(
                    0,
                    port,
                    pin,
                    (ushort)(value),
                    ArcazeCommand.OutputOperators.PlainWrite,
                    true);

            }
            return true;
        }

        public void setDisplay(string address, byte connector, List<string> digits, List<string> decimalPoints, string value)
        {
            String portAndPin = "LED" + address + connector.ToString();

            // if value has not changed since the last time, then we continue to next item to prevent 
            // unnecessary communication with Arcaze USB
            if (lastArcazeValue.ContainsKey(portAndPin) &&
                lastArcazeValue[portAndPin] == value) return;

            // otherwise store it
            if (!lastArcazeValue.ContainsKey(portAndPin))
            {
                _initLedDisplay(address);
            }

            lastArcazeValue[portAndPin] = value;

            try
            {
                DeviceInfoAndCache dev = _info;
                if (null != dev)
                {
                    ArcazeLedDigit led = new ArcazeLedDigit();

                    led.setConnector(connector);

                    foreach (string digit in digits)
                    {
                        led.setActive(ushort.Parse(digit));
                    }

                    foreach (string decimalPoint in decimalPoints)
                    {
                        led.setDecimalPoint(ushort.Parse(decimalPoint));
                    }

                    dev.m_arcazeHid.Command.CmdMax7219WriteDigits(
                            Int16.Parse(address.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber),
                            led.convert(value),
                            led.getMask()
                    );
                }
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_WritingDisplay"), e);
            }
        }

        /*
        public bool SetDisplay(int module, int pos, string value)
        {
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

                    foreach (string digit in digits)
                    {
                        led.setActive(ushort.Parse(digit));
                    }

                    foreach (string decimalPoint in decimalPoints)
                    {
                        led.setDecimalPoint(ushort.Parse(decimalPoint));
                    }

                    dev.m_arcazeHid.Command.CmdMax7219WriteDigits(
                            Int16.Parse(address.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber),
                            led.convert(value),
                            led.getMask()
                    );
                }
            }
            catch (Exception e)
            {
                throw new ArcazeCommandExecutionException(i18n._tr("ConfigErrorException_WritingDisplay"), e);
            }
        }
        */
        
        public string getValue(string portAndPin, string trigger) {
            string result = null;
            ArcazeIoBasic io = new ArcazeIoBasic(portAndPin);

            // check if value has already been read to prevent
            // unnecessary communication with Arcaze USB
            if (lastArcazeGetValue.ContainsKey(io.Port.ToString()))
            {
                int currentValues = lastArcazeGetValue[io.Port.ToString()];
                int bitValue = (int) (1 << io.Pin);
                result = ((currentValues & (1 << io.Pin)) == (1 << io.Pin)) ? "0" : "1";
                return result;
            }

            try
            {
                lastArcazeGetValue[io.Port.ToString()] =  _info.m_arcazeHid.Command.CmdReadPort(
                    io.Port
                );
                int currentValues = lastArcazeGetValue[io.Port.ToString()];
                result = ((currentValues & (1 << io.Pin)) == (1 << io.Pin)) ? "0" : "1";
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

        public void ClearCache(CacheType type = CacheType.inputCache|CacheType.outputCache)
        {
            if (type == CacheType.outputCache)
            {
                lastArcazeValue.Clear();
            }

            if (type == CacheType.inputCache)
                lastArcazeGetValue.Clear();
            
        }

        private void _initLedDisplay(string address)
        {
            ArcazeLedDisplayConfig cfg = new ArcazeLedDisplayConfig();
            cfg.address = address;
            cfg.mode = DecodeMode.Binary;
            cfg.scanLimit = 0x08;
            cfg.intensity = 0xff;

            cfg.intensity = (ushort)(_settings.globalBrightness / 16.0);
            
            try
            {
                DeviceInfoAndCache dev = _info;
                if (null != dev)
                {
                    // init the module 
                    // TODO: read settings from config
                    dev.m_arcazeHid.Command.CmdMax7219DisplayInit(
                        cfg.addressInternal(),
                        (ushort)cfg.mode,
                        cfg.intensity,
                        cfg.scanLimit
                    );

                    // set all digits to blank once
                    dev.m_arcazeHid.Command.CmdMax7219WriteDigits(
                        cfg.addressInternal(),
                        new List<byte>(8) { 0, 0, 0, 0, 0, 0, 0, 0 },
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

        internal void setBcd4056(List<string> list, string value)
        {
            ArcazeBcd4056 display = new ArcazeBcd4056();
            List<byte> vals = display.convert(value);
            ArcazeIoBasic io = new ArcazeIoBasic(list[0]);
            ArcazeIoBasic io1 = new ArcazeIoBasic(list[1]);
            ArcazeIoBasic io2 = new ArcazeIoBasic(list[2]);
            ArcazeIoBasic io3 = new ArcazeIoBasic(list[3]);
            ArcazeIoBasic io4 = new ArcazeIoBasic(list[4]);
            // set the value
            SetPin(io1.Port, io1.Pin, vals[0]);
            SetPin(io2.Port, io2.Pin, vals[1]);
            SetPin(io3.Port, io3.Pin, vals[2]);
            SetPin(io4.Port, io4.Pin, vals[3]);
            // store the value (strobe on -> off)
            SetPin(io.Port, io1.Pin, 1);
            SetPin(io.Port, io1.Pin, 0);
        }
    }
}
