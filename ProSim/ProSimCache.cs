using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using MobiFlight.Base;

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

        private readonly object _cacheLock = new object();


        private readonly Dictionary<string, IDisposable> _subscriptions = new Dictionary<string, IDisposable>();


        // ProSim SDK object
        private GraphQLHttpClient _connection;

        // Cache of subscribed DataRefs
        private Dictionary<string, CachedDataRef> _subscribedDataRefs = new Dictionary<string, CachedDataRef>();
        private Dictionary<string, DataRefDescription> _dataRefDescriptions = new Dictionary<string, DataRefDescription>();

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

        private void SubscribeToDataRef(string datarefPath)
        {
            // Skip if we already have a subscription for this dataref
            if (_subscriptions.ContainsKey(datarefPath))
                return;

            var subscription = @"
                subscription ($names: [String!]!) {
                  dataRefs(names: $names) {
                    name
                    value
                  }
                }";

            var variables = new
            {
                names = new[] { datarefPath }
            };

            var dataRefObservable = _connection.CreateSubscriptionStream<DataRefSubscriptionResult>(new GraphQL.GraphQLRequest
            {
                Query = subscription,
                Variables = variables
            });

            var disposable = dataRefObservable.Subscribe(response =>
            {
                if (response?.Data != null)
                {
                    var dataRef = response.Data.DataRefs;
                    UpdateCachedValue(dataRef.Name, dataRef.Value);
                }
            });

            _subscriptions[datarefPath] = disposable;

            Log.Instance.log($"Created subscription for dataref: {datarefPath}", LogSeverity.Debug);
        }

        private void UpdateCachedValue(string datarefPath, object value)
        {
            lock (_cacheLock)
            {
                if (_subscribedDataRefs.TryGetValue(datarefPath, out var cachedRef))
                {
                    // Update existing cache entry
                    cachedRef.Value = value;
                }
                else
                {
                    // Create new cache entry
                    _subscribedDataRefs[datarefPath] = new CachedDataRef
                    {
                        Path = datarefPath,
                        Value = value,
                    };
                }
            }
        }

        private readonly Dictionary<string, string> mutationLookup = new Dictionary<string, string>
        {
            { "System.Int32", "writeInt" },
            { "System.Double", "writeFloat" },
            { "System.Boolean", "writeBoolean" }
        };

        private void WriteOutValue(string datarefPath, object value)
        {
            if (_dataRefDescriptions.TryGetValue(datarefPath, out var description))
            {
                if (mutationLookup.TryGetValue(description.DataType, out var method)) {

                    Task.Run(async () => {
                        var query = $@"
mutation {{
	dataRef {{
		{method}(name: ""{datarefPath}"", value: {value})
	}}
}}";
                        await _connection.SendMutationAsync<object>(new GraphQL.GraphQLRequest
                        {
                            Query = query
                        });
                    });

                }
            }


        }

        public bool Connect()
        {
            try
            {
                // Initialize ProSim SDK connection
                
                var host = !string.IsNullOrWhiteSpace(Properties.Settings.Default.ProSimHost)
                    ? Properties.Settings.Default.ProSimHost
                    : "localhost";

                var port = Properties.Settings.Default.ProSimPort;

                _connection = new GraphQLHttpClient($"http://{host}:{port}/graphql", new NewtonsoftJsonSerializer());
                _connection.InitializeWebsocketConnection();

                _connection.WebsocketConnectionState.Subscribe(state =>
                {
                    if (state == GraphQLWebsocketConnectionState.Connected)
                    {
                        Log.Instance.log("Connected to ProSim GraphQL WebSocket!", LogSeverity.Debug);
                        _connected = true;
                    }
                    //else if (state == GraphQLWebsocketConnectionState.Disconnected)
                    //{
                    //    _connected = false;
                    //    ConnectionLost?.Invoke(this, new EventArgs());
                    //}
                });

                Task.Run(() =>
                {
                    var dataRefDescriptions =  _connection.SendQueryAsync<DataRefData>(new GraphQL.GraphQLRequest
                    {
                        Query = @"
                {
                    dataRef {
                    dataRefDescriptions: list {
                    		name
                    		description
                    		canRead
                    		canWrite
                    		dataType
                    		dataUnit
                        __typename
                    }
                    __typename
                    }
                }
                "
                    }).Result;

                    _dataRefDescriptions = dataRefDescriptions.Data.DataRef.DataRefDescriptions.ToDictionary(drd => drd.Name);

                });

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

                foreach (var subscription in _subscriptions.Values)
                {
                    subscription.Dispose();
                }

                _connected = false;
                _connection = null;
                Closed?.Invoke(this, new EventArgs());
            }
            return !_connected;
        }

        

        public bool IsConnected()
        {
            return _connected && _connection != null;
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

                
                if (!_subscriptions.ContainsKey(datarefPath))
                {
                    SubscribeToDataRef(datarefPath);
                    return 0;
                }

                if (!_subscribedDataRefs.ContainsKey(datarefPath))
                {
                    // Wait for data to be returned by the subscription
                    return 0;
                }
                
                // Return the cached value (continuously updated by the subscription)
                var value = _subscribedDataRefs[datarefPath].Value;
                var returnValue = (value == null) ? 0 : Convert.ToDouble(value);


                return returnValue;
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error reading dataref {datarefPath}: {ex.Message}", LogSeverity.Error);
                
                
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

                if (!_dataRefDescriptions.ContainsKey(datarefPath))
                {
                    // Probably an error, unsure
                    return;
                }
                
                var description = _dataRefDescriptions[datarefPath];

                var transformedValue = value;
                var targetDataType = description.DataType;

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

                WriteOutValue(datarefPath, transformedValue);
                Log.Instance.log($"Written value to {datarefPath}: {transformedValue}", LogSeverity.Debug);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error writing to dataref {datarefPath}: {ex.Message}", LogSeverity.Error);     
            }
        }
    }
} 