using System;
using FSUIPC;
using System.Collections.Concurrent;

namespace MobiFlight.FSUIPC
{
    // List of the FSUIPC error codes and meanings
    //
    //    FSUIPC_ERR_OK
    //    FSUIPC_ERR_OPEN       Attempt to Open when already Open
    //    FSUIPC_ERR_NOFS       Cannot link to FSUIPC or WideClient
    //    FSUIPC_ERR_REGMSG     Failed to Register common message with Windows
    //    FSUIPC_ERR_ATOM       Failed to create Atom for mapping filename
    //    FSUIPC_ERR_MAP        Failed to create a file mapping object
    //    FSUIPC_ERR_VIEW       Failed to open a view to the file map
    //    FSUIPC_ERR_VERSION    Incorrect version of FSUIPC, or not FSUIPC
    //    FSUIPC_ERR_WRONGFS    Sim is not version requested
    //    FSUIPC_ERR_NOTOPEN    Call cannot execute, link not Open
    //    FSUIPC_ERR_NODATA     Call cannot execute: no requests accumulated
    //    FSUIPC_ERR_TIMEOUT    IPC timed out all retries
    //    FSUIPC_ERR_SENDMSG    IPC sendmessage failed all retries
    //    FSUIPC_ERR_DATA       IPC request contains bad data
    //    FSUIPC_ERR_RUNNING    Maybe running on WideClient, but FS not running on Server, or wrong FSUIPC
    //    FSUIPC_ERR_SIZE       Read or Write request cannot be added, memory for Process is full

    public class Fsuipc2Cache : FSUIPCCacheInterface
    {
        private static readonly object _fsuipcLock = new object();

        public event EventHandler Closed;

        public event EventHandler Connected;

        public event EventHandler ConnectionLost;
        public event EventHandler<string> AircraftChanged;

        readonly ConcurrentDictionary<Int32, Offset<Byte>> __cacheByte = new ConcurrentDictionary<Int32, Offset<Byte>>();
        readonly ConcurrentDictionary<Int32, Offset<Int16>> __cacheShort = new ConcurrentDictionary<Int32, Offset<Int16>>();
        readonly ConcurrentDictionary<Int32, Offset<Int32>> __cacheInt = new ConcurrentDictionary<Int32, Offset<Int32>>();
        readonly ConcurrentDictionary<Int32, Offset<Single>> __cacheFloat = new ConcurrentDictionary<Int32, Offset<Single>>();
        readonly ConcurrentDictionary<Int32, Offset<Int64>> __cacheLong = new ConcurrentDictionary<Int32, Offset<Int64>>();
        readonly ConcurrentDictionary<Int32, Offset<Double>> __cacheDouble = new ConcurrentDictionary<Int32, Offset<Double>>();
        readonly ConcurrentDictionary<Int32, Offset<String>> __cacheString = new ConcurrentDictionary<Int32, Offset<String>>();

        private readonly Offset<Int32> __macroParam = new Offset<Int32>("macro", 0x0d6c, true);
        private readonly Offset<string> __macroName = new Offset<string>("macro", 0xd70, 40, true);

        private long lastProcessedMs = 0;

        public MobiFlight.FlightSimConnectionMethod FlightSimConnectionMethod { get; set; } = MobiFlight.FlightSimConnectionMethod.NONE;
        public MobiFlight.FlightSimType FlightSim = FlightSimType.NONE;

        private bool _offsetsRegistered = false;
        private bool __isProcessed = false;

        private string _detectedAircraft = string.Empty;
        private System.Timers.Timer _aircraftNameTimer;

        public Fsuipc2Cache()
        {
            _aircraftNameTimer = new System.Timers.Timer(5000);
            _aircraftNameTimer.Elapsed += (s, e) => { CheckForAircraftName(); };
        }

        public void Clear()
        {
            __isProcessed = false;
        }

        public bool IsConnected()
        {
            return FSUIPCConnection.IsOpen;
        }

        private void CheckForAircraftName()
        {
            Clear();
            var aircraft = getStringValue(0x3D00, 255);
            if (aircraft == _detectedAircraft) return;

            _detectedAircraft = aircraft;
            AircraftChanged?.Invoke(this, _detectedAircraft);
        }

        public bool Connect()
        {
            try
            {
                // Attempt to open a connection to FSUIPC
                // (running on any version of Flight Sim)
                if (!IsConnected())
                {
                    FSUIPCConnection.Open();
                    Connected?.Invoke(this, EventArgs.Empty);
                    // Opened OK
                    _aircraftNameTimer.Enabled = true;
                }

            }
            catch (FSUIPCException ex)
            {
                // Badness occurred -
                // show the error message
                if (ex.FSUIPCErrorCode == FSUIPCError.FSUIPC_ERR_OPEN)
                {
                    Connected?.Invoke(this, EventArgs.Empty);
                }
                else if (ex.FSUIPCErrorCode == FSUIPCError.FSUIPC_ERR_NOFS)
                {
                    // We can enable this again once we have throttling for the log in place
                    // But it doesn't make sense to log this every 10s
                    // Log.Instance.log("No FSUIPC found.", LogSeverity.Debug);
                }
                else
                {
                    Log.Instance.log($"FSUIPC Exception: {ex.Message}", LogSeverity.Error);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Exception: {ex.Message}", LogSeverity.Error);
            }
            return IsConnected();
        }

        public bool Disconnect()
        {
            try
            {
                _aircraftNameTimer.Enabled = false;

                if (IsConnected())
                {
                    FSUIPCConnection.Close();
                    Closed?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                Log.Instance.log($"Exception during FSUIPC disconnect: {e.Message}", LogSeverity.Error);
                return false;
            }

            return !IsConnected();
        }

        protected void _processThreadSafe(bool forceRefresh = false)
        {
            // test the cache and gather data from fsuipc if necessary
            if (IsConnected() && _offsetsRegistered && (!__isProcessed || forceRefresh))
            {
                try
                {
                    lock (_fsuipcLock)
                    {
                        FSUIPCConnection.Process();
                        lastProcessedMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        __isProcessed = true;
                    }
                }
                catch (Exception e)
                {
                    ConnectionLost?.Invoke(this, EventArgs.Empty);
                    throw e;
                }
            }
        }

        public long getValue(int offset, byte size)
        {
            bool cacheChanged = false;
            long result = 0;
            if (!IsConnected()) return result;

            switch (size)
            {
                case 1:
                    __cacheByte.GetOrAdd(offset, k =>
                    {
                        _offsetsRegistered = cacheChanged = true;
                        return new Offset<Byte>(k);
                    });
                    _processThreadSafe(cacheChanged);
                    result = Convert.ToInt64(__cacheByte[offset].Value);
                    break;
                case 2:
                    __cacheShort.GetOrAdd(offset, k =>
                    {
                        _offsetsRegistered = cacheChanged = true;
                        return new Offset<Int16>(k);
                    });
                    _processThreadSafe(cacheChanged);
                    result = Convert.ToInt64(__cacheShort[offset].Value);
                    break;
                case 4:
                    __cacheInt.GetOrAdd(offset, k =>
                    {
                        _offsetsRegistered = cacheChanged = true;
                        return new Offset<Int32>(k);
                    });
                    _processThreadSafe(cacheChanged);
                    result = Convert.ToInt64(__cacheInt[offset].Value);
                    break;
                case 8:
                    __cacheLong.GetOrAdd(offset, k =>
                    {
                        _offsetsRegistered = cacheChanged = true;
                        return new Offset<Int64>(k);
                    });
                    _processThreadSafe(cacheChanged);
                    result = __cacheLong[offset].Value;
                    break;
            }

            return result;
        }

        public long getLongValue(int offset, byte size)
        {
            bool cacheChanged = false;
            long result = 0;
            if (!IsConnected()) return result;

            __cacheLong.GetOrAdd(offset, k =>
            {
                _offsetsRegistered = cacheChanged = true;
                return new Offset<Int64>(k);
            });

            _processThreadSafe(cacheChanged);
            result = __cacheLong[offset].Value;

            return result;
        }

        public double getFloatValue(int offset, byte size)
        {
            bool cacheChanged = false;
            double result = 0.0;
            if (!IsConnected()) return result;

            __cacheFloat.GetOrAdd(offset, k =>
            {
                _offsetsRegistered = cacheChanged = true;
                return new Offset<float>(k);
            });
            _processThreadSafe(cacheChanged);
            result = __cacheFloat[offset].Value;

            return result;
        }

        public double getDoubleValue(int offset, byte size)
        {
            bool cacheChanged = false;
            double result = 0.0;
            if (!IsConnected()) return result;

            __cacheDouble.GetOrAdd(offset, k =>
            {
                _offsetsRegistered = cacheChanged = true;
                return new Offset<Double>(k);
            });
            _processThreadSafe(cacheChanged);
            result = __cacheDouble[offset].Value;

            return result;
        }

        public string getStringValue(int offset, byte size)
        {
            bool cacheChanged = false;
            String result = "";
            if (!IsConnected()) return result;

            __cacheString.GetOrAdd(offset, k =>
            {
                _offsetsRegistered = cacheChanged = true;
                return new Offset<String>(k, 255);
            });
            _processThreadSafe(cacheChanged);
            result = __cacheString[offset].Value;

            return result;
        }

        public void setOffset(int offset, byte value)
        {
            lock (_fsuipcLock)
            {
                __cacheByte.GetOrAdd(offset, k =>
                {
                    _offsetsRegistered = true;
                    return new Offset<Byte>(k);
                });

                __cacheByte[offset].ActionAtNextProcess = OffsetAction.Write;
                __cacheByte[offset].Value = value;
            }
        }

        public void setOffset(int offset, short value)
        {
            lock (_fsuipcLock)
            {
                __cacheShort.GetOrAdd(offset, k =>
                {
                    _offsetsRegistered = true;
                    return new Offset<Int16>(k);
                });

                __cacheShort[offset].ActionAtNextProcess = OffsetAction.Write;
                __cacheShort[offset].Value = value;
            }
        }

        public void setOffset(int offset, int value, bool writeOnly = false)
        {
            lock (_fsuipcLock)
            {
                __cacheInt.GetOrAdd(offset, k =>
                {
                    _offsetsRegistered = true;
                    return new Offset<Int32>(k, writeOnly);
                });

                __cacheInt[offset].ActionAtNextProcess = OffsetAction.Write;
                __cacheInt[offset].Value = value;
            }
        }

        public void setOffset(int offset, float value)
        {
            lock (_fsuipcLock)
            {
                __cacheFloat.GetOrAdd(offset, k =>
                {
                    _offsetsRegistered = true;
                    return new Offset<float>(k);
                });

                __cacheFloat[offset].ActionAtNextProcess = OffsetAction.Write;
                __cacheFloat[offset].Value = value;
            }
        }

        public void setOffset(int offset, double value)
        {
            lock (_fsuipcLock)
            {
                __cacheDouble.GetOrAdd(offset, k =>
                {
                    _offsetsRegistered = true;
                    return new Offset<double>(k);
                });

                __cacheDouble[offset].ActionAtNextProcess = OffsetAction.Write;
                __cacheDouble[offset].Value = value;
            }
        }

        public void setOffset(int offset, string value)
        {
            lock (_fsuipcLock)
            {
                // +1 needed because fsuipc string must end with 0x00 and last char is auto set by library
                int stringLength = value.Length + 1;
                __cacheString.AddOrUpdate(offset,
                    k =>
                    {
                        _offsetsRegistered = true;
                        return new Offset<String>(k, stringLength);
                    },
                    (k, existing) =>
                    {
                        if (existing.DataLength != stringLength)
                        {
                            existing.Disconnect();
                            return new Offset<String>(k, stringLength);
                        }
                        return existing;
                    });

                __cacheString[offset].ActionAtNextProcess = OffsetAction.Write;
                __cacheString[offset].Value = value;
            }
        }

        public void executeMacro(string macroName, int paramValue)
        {
            try
            {
                lock (_fsuipcLock)
                {
                    __macroParam.Value = paramValue;
                    __macroName.Value = macroName;
                    FSUIPCConnection.Process("macro");
                }
            }
            catch (Exception e)
            {
                ConnectionLost?.Invoke(this, EventArgs.Empty);
                throw e;
            }
        }

        public void setEventID(int eventID, int param)
        {
            try
            {
                lock (_fsuipcLock)
                {
                    FSUIPCConnection.SendControlToFS(eventID, param);
                }
            }
            catch (Exception e)
            {
                ConnectionLost?.Invoke(this, EventArgs.Empty);
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
                // Force FSUIPC to process the offsets marked as write only,
                // even if they haven't been read yet (and thus aren't in the cache)
                _processThreadSafe(true);
            }
            catch (Exception e)
            {
                ConnectionLost?.Invoke(this, EventArgs.Empty);
                throw e;
            }
        }

    }
}