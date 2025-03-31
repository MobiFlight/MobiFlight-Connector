using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MobiFlight.Base;
using ProSimSDK;

namespace MobiFlight.ProSim
{
    public class ProSimCache : ProSimCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler<string> AircraftChanged;

        private bool _connected = false;
        private string _detectedAircraft = string.Empty;
        private System.Timers.Timer _aircraftNameTimer;

        // ProSim SDK object
        private ProSimConnect _connection;

        // Cache of subscribed DataRefs
        private ConcurrentDictionary<string, CachedDataRef> _subscribedDataRefs = new ConcurrentDictionary<string, CachedDataRef>();
        private ConcurrentDictionary<string, DataRefDescription> _dataRefDescriptions = new ConcurrentDictionary<string, DataRefDescription>();

        // Default update frequency (in milliseconds)
        private int DEFAULT_UPDATE_INTERVAL = 100;

        // Helper class to store cached values
        private class CachedDataRef
        {
            public string Path { get; set; }
            public object Value { get; set; }
            public DataRef DataRefObject { get; set; }
            public int UpdateInterval { get; set; }
            public DataRefDescription DataRefDescription { get; set; }
        }

        public ProSimCache()
        {
            _aircraftNameTimer = new System.Timers.Timer(5000);
            _aircraftNameTimer.Elapsed += (s, e) => { CheckForAircraftName(); };

            DEFAULT_UPDATE_INTERVAL = Properties.Settings.Default.PollInterval;
        }

        private void CheckForAircraftName()
        {
            try
            {
                if (!IsConnected()) return;
                
                // TODO: Finish aircraft connection event

                // Use our new subscription mechanism to get aircraft type
                //string aircraft = Convert.ToString(readDataref("aircraft.type"));
                
                //if (aircraft == _detectedAircraft) return;

                //_detectedAircraft = aircraft;
                //AircraftChanged?.Invoke(this, _detectedAircraft);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error checking aircraft name: {ex.Message}", LogSeverity.Error);
            }
        }

        public void Clear()
        {
            // Clear the cache if needed
        }

        public bool Connect()
        {
            try
            {
                // Initialize ProSim SDK connection
                _connection = new ProSimConnect();
                
                var host = !string.IsNullOrWhiteSpace(Properties.Settings.Default.ProSimHost)
                    ? Properties.Settings.Default.ProSimHost
                    : "localhost";

                _connection.Connect(host);
                

                _dataRefDescriptions = new ConcurrentDictionary<string, DataRefDescription>(
                    _connection.getDataRefDescriptions().ToDictionary(drd => drd.Name)
                );

                // Register connection events
                _connection.onDisconnect += () => {
                    _connected = false;
                    ConnectionLost?.Invoke(this, new EventArgs());
                };

                _connected = true;
                _aircraftNameTimer.Start();
                Connected?.Invoke(this, new EventArgs());
                
                Log.Instance.log("Connected to ProSim", LogSeverity.Info);
                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Failed to connect to ProSim: {ex.Message}", LogSeverity.Error);
                _connected = false;
                return false;
            }
        }

        public bool Disconnect()
        {
            if (_connected)
            {
                _aircraftNameTimer.Stop();
                
                // Clear all DataRefs
                lock (_subscribedDataRefs)
                {
                    _subscribedDataRefs.Clear();
                }
                
                _connected = false;
                _connection = null;
                Closed?.Invoke(this, new EventArgs());
            }
            return !_connected;
        }

        public bool IsConnected()
        {
            return _connected && _connection != null && _connection.isConnected;
        }

        private void cacheDataref(string datarefPath)
        {
            // Create a new subscription for this DataRef
            var cachedRef = new CachedDataRef
            {
                Path = datarefPath,
                Value = 0,
                UpdateInterval = DEFAULT_UPDATE_INTERVAL
            };

            // Create DataRef object with SDK
            cachedRef.DataRefObject = new DataRef(datarefPath, cachedRef.UpdateInterval, _connection);

            // Set up callback to update the value when it changes
            cachedRef.DataRefObject.onDataChange += (dataRef) => {
                try
                {
                    cachedRef.Value = dataRef.value;
                    Log.Instance.log($"Updated cached value for {datarefPath}: {cachedRef.Value}", LogSeverity.Debug);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Error in DataRef callback: {ex.Message}", LogSeverity.Error);
                }
            };

            cachedRef.DataRefDescription = _dataRefDescriptions[datarefPath];

            // Register for updates
            //cachedRef.DataRefObject.Register();

            // Add to our dictionary
            if (!_subscribedDataRefs.TryAdd(datarefPath, cachedRef))
            {
                Log.Instance.log($"Failed to subscribe to DataRef: {datarefPath}", LogSeverity.Debug);

            }
            else
            {
                Log.Instance.log($"Subscribed to DataRef: {datarefPath}", LogSeverity.Debug);
            }
        }

        public double readDataref(string datarefPath)
        {
            if (!IsConnected())
            {
                return 0;
            }

            try
            {
                // Check if we already have this DataRef subscribed

                
                if (!_subscribedDataRefs.ContainsKey(datarefPath))
                {
                    cacheDataref(datarefPath);
                }
                
                // Return the cached value (continuously updated by the subscription)
                var value = _subscribedDataRefs[datarefPath].Value;
                var returnValue = (value == null) ? 0 : Convert.ToDouble(value);


                return returnValue;
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error reading dataref {datarefPath}: {ex.Message}", LogSeverity.Error);
                
                // Check if connection was lost
                if (_connection == null || !_connection.isConnected)
                {
                    _connected = false;
                    ConnectionLost?.Invoke(this, new EventArgs());
                }
                
                return 0;
            }
        }

        public void writeDataref(string datarefPath, object value)
        {
            if (!IsConnected())
            {
                return;
            }

            try
            {
                // For writes, we can use either a cached DataRef or create a temporary one
                CachedDataRef dataRef;

                if (!_subscribedDataRefs.ContainsKey(datarefPath))
                {
                    cacheDataref(datarefPath);
                }
                
                dataRef = _subscribedDataRefs[datarefPath];

                var transformedValue = value;
                var targetDataType = dataRef.DataRefDescription.DataType;

                if (targetDataType == "System.Int32")
                {
                    transformedValue = Convert.ToInt32(value);
                }
                else if (targetDataType == "System.Double")
                {
                    transformedValue = Convert.ToDouble(value);
                }
                else if (targetDataType == "System.Boolean")
                {
                    transformedValue = Convert.ToInt32(value) > 0;
                }

                dataRef.DataRefObject.value = transformedValue;
                Log.Instance.log($"Written value to {datarefPath}: {transformedValue}", LogSeverity.Debug);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error writing to dataref {datarefPath}: {ex.Message}", LogSeverity.Error);
                
                // Check if connection was lost
                if (_connection == null || !_connection.isConnected)
                {
                    _connected = false;
                    ConnectionLost?.Invoke(this, new EventArgs());
                }
            }
        }
    }
} 