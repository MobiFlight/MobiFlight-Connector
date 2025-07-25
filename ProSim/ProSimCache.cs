using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace MobiFlight.ProSim
{
    public class ProSimCache : ProSimCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler<string> AircraftChanged;

        private bool _connected = false;

        private readonly object _cacheLock = new object();
        private readonly object _refreshLock = new object();

        private readonly Dictionary<string, IDisposable> _subscriptions = new Dictionary<string, IDisposable>();

        // ProSim SDK object
        private GraphQLHttpClient _connection;

        // Cache of subscribed DataRefs
        private Dictionary<string, CachedDataRef> _subscribedDataRefs = new Dictionary<string, CachedDataRef>();
        private Dictionary<string, DataRefDescription> _dataRefDescriptions = new Dictionary<string, DataRefDescription>();

        // Queue for writes that need to wait for data definitions refresh
        private readonly ConcurrentQueue<(string datarefPath, object value)> _pendingWrites = new ConcurrentQueue<(string, object)>();
        
        // Flag to track if refresh is in progress
        private volatile bool _refreshInProgress = false;

        // Helper class to store cached values
        private class CachedDataRef
        {
            public string Path { get; set; }
            public object Value { get; set; }
            public DataRef DataRefObject { get; set; }
            public int UpdateInterval { get; set; }
            public DataRefDescription DataRefDescription { get; set; }
        }

        public bool Connect()
        {
            try
            {
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
                        
                        // Refresh data definitions on connection
                        RefreshDataDefinitionsAsync().ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Log.Instance.log($"Failed to refresh data definitions: {task.Exception?.GetBaseException().Message}", LogSeverity.Error);
                            }
                        });
                        
                        Connected?.Invoke(this, new EventArgs());
                    }
                    else if (state == GraphQLWebsocketConnectionState.Disconnected)
                    {
                        if (_connected)
                        {
                            _connected = false;
                            // Clear data definitions on disconnection
                            lock (_cacheLock)
                            {
                                _dataRefDescriptions.Clear();
                            }
                            ConnectionLost?.Invoke(this, new EventArgs());
                        }
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Failed to connect to ProSim: {ex.Message}", LogSeverity.Error);
                _connected = false;
                return false;
            }
        }

        public void Clear()
        {
            _dataRefDescriptions = new Dictionary<string, DataRefDescription>();
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
            // Check if refresh is in progress
            if (_refreshInProgress)
            {
                // Queue the write for later processing
                _pendingWrites.Enqueue((datarefPath, value));
                Log.Instance.log($"Queued write for {datarefPath} during data definitions refresh", LogSeverity.Debug);
                return;
            }

            WriteOutValueInternal(datarefPath, value);
        }

        private void WriteOutValueInternal(string datarefPath, object value)
        {
            try
            {
                if (!IsConnected() || _connection == null)
                {
                    return;
                }

                if (!_dataRefDescriptions.TryGetValue(datarefPath, out var description))
                {
                    return;
                }

                if (!description.CanWrite)
                {
                    return;
                }

                if (!mutationLookup.TryGetValue(description.DataType, out var method))
                {
                    return;
                }

                Task.Run(async () => {
                    try
                    {
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
                    }
                    catch
                    {
                        // Ignore all errors
                    }
                });
            }
            catch
            {
                // Ignore all errors
            }
        }

        private async Task RefreshDataDefinitionsAsync()
        {
            lock (_refreshLock)
            {
                if (_refreshInProgress)
                {
                    return;
                }
                _refreshInProgress = true;
            }

            try
            {
                if (!IsConnected() || _connection == null)
                {
                    return;
                }

                Log.Instance.log("Refreshing ProSim data definitions...", LogSeverity.Debug);
                
                var dataRefDescriptions = await _connection.SendQueryAsync<DataRefData>(new GraphQL.GraphQLRequest
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
                    }"
                });

                if (dataRefDescriptions?.Data?.DataRef?.DataRefDescriptions == null)
                {
                    return;
                }

                var newDataRefDescriptions = dataRefDescriptions.Data.DataRef.DataRefDescriptions.ToDictionary(drd => drd.Name);

                lock (_cacheLock)
                {
                    _dataRefDescriptions = newDataRefDescriptions;
                }

                Log.Instance.log($"Refreshed {_dataRefDescriptions.Count} data definitions", LogSeverity.Debug);

                ProcessPendingWrites();
            }
            catch
            {
                // Ignore all errors
            }
            finally
            {
                lock (_refreshLock)
                {
                    _refreshInProgress = false;
                }
            }
        }

        private void ProcessPendingWrites()
        {
            if (!IsConnected())
            {
                return;
            }

            var totalPending = _pendingWrites.Count;

            if (totalPending == 0)
            {
                return;
            }

            Log.Instance.log($"Processing {totalPending} pending writes after data definitions refresh", LogSeverity.Debug);

            while (_pendingWrites.TryDequeue(out var pendingWrite))
            {
                try
                {
                    if (_dataRefDescriptions.Count == 0)
                    {
                        continue;
                    }

                    if (!_dataRefDescriptions.ContainsKey(pendingWrite.datarefPath))
                    {
                        continue;
                    }

                    WriteOutValueInternal(pendingWrite.datarefPath, pendingWrite.value);
                }
                catch
                {
                    // Ignore all errors
                }
            }
        }

        public bool Disconnect()
        {
            if (_connected)
            {                
                lock (_subscribedDataRefs)
                {
                    _subscribedDataRefs.Clear();
                }

                lock (_cacheLock)
                {
                    _dataRefDescriptions.Clear();
                }

                // Clear pending writes
                while (_pendingWrites.TryDequeue(out _)) { }

                lock (_refreshLock)
                {
                    _refreshInProgress = false;
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
                if (_dataRefDescriptions.Count == 0)
                {
                    if (_refreshInProgress)
                    {
                        _pendingWrites.Enqueue((datarefPath, value));
                        return;
                    }

                    _pendingWrites.Enqueue((datarefPath, value));
                    RefreshDataDefinitionsAsync().ConfigureAwait(false);
                    return;
                }

                if (!_dataRefDescriptions.ContainsKey(datarefPath))
                {
                    return;
                }
                
                var description = _dataRefDescriptions[datarefPath];

                if (!description.CanWrite)
                {
                    return;
                }

                var transformedValue = value;
                var targetDataType = description.DataType;

                try
                {
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
                }
                catch
                {
                    return;
                }

                WriteOutValue(datarefPath, transformedValue);
            }
            catch
            {
                // Ignore all errors
            }
        }

        public Dictionary<string, DataRefDescription> GetDataRefDescriptions()
        {
            return new Dictionary<string, DataRefDescription>(_dataRefDescriptions);
        }
    }
} 