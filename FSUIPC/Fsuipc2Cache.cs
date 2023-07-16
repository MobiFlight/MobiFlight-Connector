using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using FSUIPC;

namespace MobiFlight.FSUIPC
{
    public class Fsuipc2Cache : FSUIPCCacheInterface
    {

        public event EventHandler Closed;

        public event EventHandler Connected;

        public event EventHandler ConnectionLost;

        Dictionary<Int32, Offset<Byte>> __cacheByte = new Dictionary<Int32, Offset<Byte>>();        
        Dictionary<Int32, Offset<Int16>> __cacheShort = new Dictionary<Int32, Offset<Int16>>();
        //Dictionary<Int32, Offset<UInt16>> __cacheUShort = new Dictionary<Int32, Offset<UInt16>>();
        Dictionary<Int32, Offset<Int32>> __cacheInt = new Dictionary<Int32, Offset<Int32>>();
        //Dictionary<Int32, Offset<UInt32>> __cacheUInt = new Dictionary<Int32, Offset<UInt32>>();
        Dictionary<Int32, Offset<Single>> __cacheFloat = new Dictionary<Int32, Offset<Single>>();
        Dictionary<Int32, Offset<Int64>> __cacheLong = new Dictionary<Int32, Offset<Int64>>();
        //Dictionary<Int32, Offset<UInt64>> __cacheULong = new Dictionary<Int32, Offset<UInt64>>();
        Dictionary<Int32, Offset<Double>> __cacheDouble = new Dictionary<Int32, Offset<Double>>();
        Dictionary<Int32, Offset<String>> __cacheString = new Dictionary<Int32, Offset<String>>();

        private readonly Offset<Int32> __macroParam = new Offset<Int32>("macro", 0x0d6c, true);
        private readonly Offset<string> __macroName = new Offset<string>("macro", 0xd70, 40, true);

        HashSet<int> __cacheByteWriteOnly = new HashSet<int>();
        HashSet<int> __cacheShortWriteOnly = new HashSet<int>();
        HashSet<int> __cacheIntWriteOnly = new HashSet<int>();
        HashSet<int> __cacheStringWriteOnly = new HashSet<int>();

        long lastProcessedMs = 0;

        public MobiFlight.FlightSimConnectionMethod FlightSimConnectionMethod { get; set; } = MobiFlight.FlightSimConnectionMethod.NONE;
        public MobiFlight.FlightSimType FlightSim = FlightSimType.NONE;
        
        bool _offsetsRegistered = false;
        bool __isProcessed = false;

        public Fsuipc2Cache()
        {
        }

        public void Clear()
        {
            __isProcessed = false;
        }

        public bool IsConnected()
        {
            return FSUIPCConnection.IsOpen;
        }

        public bool Connect()
        {
            try {
                // Attempt to open a connection to FSUIPC 
                // (running on any version of Flight Sim)     
                if (!IsConnected())
                {
                    FSUIPCConnection.Open();
                    this.Connected(this, new EventArgs());
                    // Opened OK 
                }
            } catch (FSUIPCException ex) {            
                // Badness occurred - 
                // show the error message 
                if (ex.Message == "FSUIPC Error #1: FSUIPC_ERR_OPEN. The connection to FSUIPC is already open.")
                {
                    this.Connected(this, new EventArgs());
                }
                else if (ex.FSUIPCErrorCode == FSUIPCError.FSUIPC_ERR_NOFS)
                {
                    Log.Instance.log("No FSUIPC found.", LogSeverity.Debug);
                }
                else
                {
                    Log.Instance.log($"FSUIPC Exception: {ex.Message}", LogSeverity.Error);
                }
            } catch (Exception ex)
            {
                Log.Instance.log($"Exception: {ex.Message}", LogSeverity.Error);
            }
            return IsConnected();
        }

        public bool Disconnect()
        {
            try
            {
                if (IsConnected())
                {
                    FSUIPCConnection.Close();
                    this.Closed(this, new EventArgs());
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return !IsConnected();
        }

        protected void _process() {
            // test the cache and gather data from fsuipc if necessary
            if (IsConnected() && _offsetsRegistered && !__isProcessed) {
                try
                {
                    FSUIPCConnection.Process();
                }
                catch (Exception e)
                {
                    this.ConnectionLost(this, new EventArgs());
                    throw e;
                }
               __isProcessed = true;
            }
        }

        public long getValue(int offset, byte size)
        {
            long result = 0;
            if (!IsConnected()) return result;

            _process();

            switch (size)
            {
                case 1:
                    if (!__cacheByte.ContainsKey(offset))
                    {
                        __cacheByte[offset] = new Offset<Byte>(offset);
                        _offsetsRegistered = true;
                        try
                        {
                            FSUIPCConnection.Process();
                        }
                        catch (Exception e)
                        {
                            this.ConnectionLost(this, new EventArgs());
                            throw e;
                        }                            
                    }
                    result = Convert.ToInt64(__cacheByte[offset].Value);
                    break;
                case 2:
                    if (!__cacheShort.ContainsKey(offset))
                    {
                        __cacheShort[offset] = new Offset<Int16>(offset);
                        _offsetsRegistered = true;
                        try
                        {
                            FSUIPCConnection.Process();
                        }
                        catch (Exception e)
                        {
                            this.ConnectionLost(this, new EventArgs());
                            throw e;
                        }                            
                    }
                    result = Convert.ToInt64(__cacheShort[offset].Value);
                    break;
                case 4:
                    if (!__cacheInt.ContainsKey(offset))
                    {
                        __cacheInt[offset] = new Offset<Int32>(offset);
                        _offsetsRegistered = true;
                        try
                        {
                            FSUIPCConnection.Process();
                        }
                        catch (Exception e)
                        {
                            this.ConnectionLost(this, new EventArgs());
                            throw e;
                        }                            
                    }
                    result =  Convert.ToInt64(__cacheInt[offset].Value);
                    break;
                case 8:
                    if (!__cacheLong.ContainsKey(offset))
                    {
                        __cacheLong[offset] = new Offset<Int64>(offset);
                        _offsetsRegistered = true;
                        try
                        {
                            FSUIPCConnection.Process();
                        }
                        catch (Exception e)
                        {
                            this.ConnectionLost(this, new EventArgs());
                            throw e;
                        }
                    }
                    result = __cacheLong[offset].Value;
                    break;            
            } //switch
            
            return result;
        }

        /*
         * NOT IMPLEMENTED
         * 
        public ulong getUValue(int offset, byte size)
        {
            ulong result = 0;
            _process();

            switch (size)
            {
                case 2:
                    if (!__cacheUShort.ContainsKey(offset))
                    {
                        __cacheUShort[offset] = new Offset<UInt16>(offset);
                        _offsetsRegistered = true;
                        try
                        {
                            FSUIPCConnection.Process();
                        }
                        catch (Exception e)
                        {
                            this.ConnectionLost(this, new EventArgs());
                            throw e;
                        }
                    }
                    result = Convert.ToUInt64(__cacheUShort[offset].Value);
                    break;
                case 4:
                    if (!__cacheUInt.ContainsKey(offset))
                    {
                        __cacheUInt[offset] = new Offset<UInt32>(offset);
                        _offsetsRegistered = true;
                        try
                        {
                            FSUIPCConnection.Process();
                        }
                        catch (Exception e)
                        {
                            this.ConnectionLost(this, new EventArgs());
                            throw e;
                        }
                    }
                    result = Convert.ToUInt64(__cacheUInt[offset].Value);
                    break;
                case 8:
                    if (!__cacheULong.ContainsKey(offset))
                    {
                        __cacheULong[offset] = new Offset<UInt64>(offset);
                        _offsetsRegistered = true;
                        try
                        {
                            FSUIPCConnection.Process();
                        }
                        catch (Exception e)
                        {
                            this.ConnectionLost(this, new EventArgs());
                            throw e;
                        }
                    }
                    result = __cacheULong[offset].Value;
                    break;
            } //switch

            return result;
        }
         * */

        public long getLongValue(int offset, byte size)
        {
            long result = 0;
            if (!IsConnected()) return result;

            _process();

            if (!__cacheLong.ContainsKey(offset))
            {
                __cacheLong[offset] = new Offset<Int64>(offset);
                _offsetsRegistered = true;
                try
                {
                    FSUIPCConnection.Process();
                }
                catch (Exception e)
                {
                    this.ConnectionLost(this, new EventArgs());
                    throw e;
                }
            }
            result = __cacheLong[offset].Value;
            
            return result;
        }

        public double getFloatValue(int offset, byte size)
        {
            double result = 0.0;
            if (!IsConnected()) return result;

            _process();
            if (!__cacheFloat.ContainsKey(offset))
            {
                __cacheFloat[offset] = new Offset<float>(offset);
                _offsetsRegistered = true;
                try
                {
                    FSUIPCConnection.Process();
                }
                catch (Exception e)
                {
                    this.ConnectionLost(this, new EventArgs());
                    throw e;
                }
            }
            result = __cacheFloat[offset].Value;

            return result;
        }

        public double getDoubleValue(int offset, byte size)
        {
            double result = 0.0;
            if (!IsConnected()) return result;

            _process();
            if (!__cacheDouble.ContainsKey(offset))
            {
                __cacheDouble[offset] = new Offset<Double>(offset);
                _offsetsRegistered = true;
                try
                {
                    FSUIPCConnection.Process();
                }
                catch (Exception e)
                {
                    this.ConnectionLost(this, new EventArgs());
                    throw e;
                }
            }
            result = __cacheDouble[offset].Value;

            return result;
        }

        public string getStringValue(int offset, byte size)
        {
            String result = "";
            if (!IsConnected()) return result;

            _process();

            if (!__cacheString.ContainsKey(offset))
            {
                __cacheString[offset] = new Offset<String>(offset, 255);
                _offsetsRegistered = true;
                try
                {
                    FSUIPCConnection.Process();
                }
                catch (Exception e)
                {
                    this.ConnectionLost(this, new EventArgs());
                    throw e;
                }
            }
            result = __cacheString[offset].Value;

            return result;
            //_process();            
        }

        public void setOffset(int offset, byte value)
        {
            if (!__cacheByte.ContainsKey(offset))
            {
                __cacheByte[offset] = new Offset<Byte>(offset);
                _offsetsRegistered = true;
            }

            __cacheByte[offset].Value = value;
        }

        public void setOffset(int offset, short value)
        {
            if (!__cacheShort.ContainsKey(offset))
            {
                __cacheShort[offset] = new Offset<Int16>(offset);
                _offsetsRegistered = true;
            }

            __cacheShort[offset].Value = value;
        }

        public void setOffset(int offset, int value, bool writeOnly = false)
        {
            if (!__cacheInt.ContainsKey(offset))
            {
                __cacheInt[offset] = new Offset<Int32>(offset, writeOnly);
                _offsetsRegistered = true;
            }

            __cacheInt[offset].Value = value;
        }

        public void setOffset(int offset, float value)
        {
            if (!__cacheFloat.ContainsKey(offset))
            {
                __cacheFloat[offset] = new Offset<float>(offset);
                _offsetsRegistered = true;
            }

            __cacheFloat[offset].Value = value;
        }

        public void setOffset(int offset, double value)
        {
            if (!__cacheDouble.ContainsKey(offset))
            {
                __cacheDouble[offset] = new Offset<double>(offset);
                _offsetsRegistered = true;
            }

            __cacheDouble[offset].Value = value;
        }

        public void setOffset(int offset, string value)
        {
            if (!__cacheString.ContainsKey(offset))
            {
                __cacheString[offset] = new Offset<String>(offset,value.Length);
                _offsetsRegistered = true;
            }

            __cacheString[offset].Value = value;
        }

        public void executeMacro(string macroName, int paramValue)
        {
            __macroParam.Value = paramValue;
            __macroName.Value = macroName;
            try {
                FSUIPCConnection.Process("macro");
            }
            catch (Exception e)
            {
                this.ConnectionLost(this, new EventArgs());
                throw e;
            }
        }

        public void setEventID(int eventID, int param)
        {
            try {
                FSUIPCConnection.SendControlToFS(eventID, param);
            }
            catch (Exception e)
            {
                this.ConnectionLost(this, new EventArgs());
                throw e;
            }
        }

        public void setEventID(string eventID)
        {
            throw new NotImplementedException();
        }

        public void Write()
        {
            try
            {
                // if we don't check on this
                // the FSUIPC connection will 
                // throw an exception in case that
                // we have no offset registered
                if (_offsetsRegistered && IsConnected())
                {
                    FSUIPCConnection.Process();
                    long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    lastProcessedMs = milliseconds;
                }
            }
            catch (Exception e)
            {
                this.ConnectionLost(this, new EventArgs());
                throw e;
            }
        }

    }
}
