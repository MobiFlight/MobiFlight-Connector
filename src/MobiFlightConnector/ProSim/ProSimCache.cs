using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
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
        private IGraphQLWebSocketClient _connection;

        // Subscription to the current connection's websocket state changes
        private IDisposable _connectionStateSubscription;

        // Heartbeat timer to keep WebSocket connection active
        private Timer _heartbeatTimer;

        // Cache of subscribed DataRefs
        private Dictionary<string, CachedDataRef> _subscribedDataRefs = new Dictionary<string, CachedDataRef>();
        private Dictionary<string, DataRefDescription> _dataRefDescriptions = new Dictionary<string, DataRefDescription>();

        // Queue for writes that need to wait for data definitions refresh
        private readonly ConcurrentQueue<(string datarefPath, object value)> _pendingWrites = new ConcurrentQueue<(string, object)>();

        // The in-flight refresh; concurrent callers share this task. Guarded by _refreshLock.
        private Task<bool> _refreshTask;

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

                // Retry attempts must not leave orphaned clients behind whose state
                // handlers would keep mutating _connected/_dataRefDescriptions.
                DisposeConnection();

                var connection = new GraphQLHttpClient($"http://{host}:{port}/graphql", new NewtonsoftJsonSerializer());
                _connection = connection;
                connection.InitializeWebsocketConnection();

                _connectionStateSubscription = connection.WebsocketConnectionState.Subscribe(state =>
                {
                    if (state == GraphQLWebsocketConnectionState.Connected)
                    {
                        Log.Instance.log("Connected to ProSim GraphQL WebSocket!", LogSeverity.Debug);
                        _connected = true;

                        // Start heartbeat timer to keep WebSocket active
                        StartHeartbeat();

                        Connected?.Invoke(this, new EventArgs());
                    }
                    else if (state == GraphQLWebsocketConnectionState.Disconnected)
                    {
                        if (_connected)
                        {
                            _connected = false;
                            // Stop heartbeat timer
                            StopHeartbeat();
                            // Clear data definitions on disconnection
                            lock (_cacheLock)
                            {
                                _dataRefDescriptions.Clear();
                            }
                            // Drop the in-flight refresh so the next connect starts a
                            // fresh one instead of coalescing onto this dead connection's task
                            ResetRefreshState();
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
            lock (_cacheLock)
            {
                _dataRefDescriptions = new Dictionary<string, DataRefDescription>();
            }
        }

        private void DisposeConnection()
        {
            _connectionStateSubscription?.Dispose();
            _connectionStateSubscription = null;

            var oldConnection = _connection as IDisposable;
            _connection = null;

            try
            {
                oldConnection?.Dispose();
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error disposing previous ProSim connection: {ex.Message}", LogSeverity.Debug);
            }
        }

        private void ResetRefreshState()
        {
            lock (_refreshLock)
            {
                _refreshTask = null;
            }
        }

        private void StartHeartbeat()
        {
            StopHeartbeat(); // Ensure we don't have multiple timers
            
            _heartbeatTimer = new Timer(5000); // 5 seconds
            _heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            _heartbeatTimer.Start();
            Log.Instance.log("Started WebSocket heartbeat timer", LogSeverity.Debug);
        }

        private void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogTaskFault(RunHeartbeatAsync(), "Heartbeat task failed", LogSeverity.Debug);
        }

        private static void LogTaskFault(Task task, string errorMessage, LogSeverity severity = LogSeverity.Error)
        {
            task.ContinueWith(t =>
            {
                Log.Instance.log($"{errorMessage}: {t.Exception?.GetBaseException().Message}", severity);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private async Task RunHeartbeatAsync()
        {
            var connection = _connection;
            if (!IsConnected() || connection == null)
            {
                return;
            }

            try
            {
                // Send a lightweight introspection query to keep WebSocket active
                await connection.SendQueryAsync<object>(new GraphQL.GraphQLRequest
                {
                    Query = "{ __typename }"
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Heartbeat failed: {ex.Message}", LogSeverity.Debug);
            }
        }

        private void StopHeartbeat()
        {
            if (_heartbeatTimer != null)
            {
                _heartbeatTimer.Stop();
                _heartbeatTimer.Dispose();
                _heartbeatTimer = null;
                Log.Instance.log("Stopped WebSocket heartbeat timer", LogSeverity.Debug);
            }
        }

        private void SubscribeToDataRef(string datarefPath)
        {
            lock (_subscriptions)
            {
                // Skip if we already have a subscription for this dataref
                if (_subscriptions.ContainsKey(datarefPath))
                    return;

                // Disconnect flips the connection state before it takes this lock to
                // clean up, so a null check here keeps us off a disposed client and
                // prevents entries from being added behind Disconnect's back.
                var connection = _connection;
                if (!IsConnected() || connection == null)
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

                var dataRefObservable = connection.CreateSubscriptionStream<DataRefSubscriptionResult>(new GraphQL.GraphQLRequest
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
            }

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
                    var newCachedRef = new CachedDataRef
                    {
                        Path = datarefPath,
                        Value = value,
                    };

                    // Set DataRefDescription if available
                    if (_dataRefDescriptions.TryGetValue(datarefPath, out var description))
                    {
                        newCachedRef.DataRefDescription = description;
                    }

                    _subscribedDataRefs[datarefPath] = newCachedRef;
                }
            }
        }

        private readonly Dictionary<string, (string method, string graphqlType)> mutationLookup = new Dictionary<string, (string, string)>
        {
            { "System.Int32", ("writeInt", "Int!") },
            { "System.Double", ("writeFloat", "Float!") },
            { "System.Boolean", ("writeBool", "Boolean!") }
        };

        private void WriteOutValue(string datarefPath, object value)
        {
            try
            {
                if (!IsConnected() || _connection == null)
                {
                    return;
                }

                DataRefDescription description = null;
                lock (_cacheLock)
                {
                    if (!_dataRefDescriptions.TryGetValue(datarefPath, out description))
                    {
                        return;
                    }
                }

                if (!description.CanWrite)
                {
                    return;
                }

                if (!mutationLookup.TryGetValue(description.DataType, out var mutation))
                {
                    return;
                }

                var (method, graphqlType) = mutation;

                var query = $@"
mutation ($name: String!, $value: {graphqlType}) {{
	dataRef {{
		{method}(name: $name, value: $value)
	}}
}}";

                RunWriteMutationAsync(datarefPath, value, query);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error preparing ProSim write for {datarefPath}: {ex.Message}", LogSeverity.Error);
            }
        }

        private void RunWriteMutationAsync(string datarefPath, object value, string query)
        {
            var connection = _connection;
            if (connection == null)
            {
                return;
            }

            LogTaskFault(connection.SendMutationAsync<object>(new GraphQL.GraphQLRequest
            {
                Query = query,
                Variables = new { name = datarefPath, value }
            }), $"Failed to write ProSim dataref {datarefPath}");
        }

        public Task<bool> RefreshDataDefinitionsAsync()
        {
            lock (_refreshLock)
            {
                if (_refreshTask != null && !_refreshTask.IsCompleted)
                {
                    return _refreshTask;
                }

                var refreshTask = RefreshDataDefinitionsInternalAsync();
                _refreshTask = refreshTask;
                refreshTask.ContinueWith(completedTask =>
                {
                    lock (_refreshLock)
                    {
                        if (ReferenceEquals(_refreshTask, completedTask))
                        {
                            _refreshTask = null;
                        }
                    }

                    // Drain after the task is cleared so writes queued during the
                    // refresh cannot race the drain and get stranded.
                    if (completedTask.Status == TaskStatus.RanToCompletion && completedTask.Result)
                    {
                        ProcessPendingWrites();
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);

                return refreshTask;
            }
        }

        private async Task<bool> RefreshDataDefinitionsInternalAsync()
        {
            try
            {
                var refreshConnection = _connection;
                if (!IsConnected() || refreshConnection == null)
                {
                    Log.Instance.log("Skipping ProSim data definitions refresh because the connection is not available.", LogSeverity.Warn);
                    return false;
                }

                Log.Instance.log("Refreshing ProSim data definitions...", LogSeverity.Debug);
                
                var dataRefDescriptions = await refreshConnection.SendQueryAsync<DataRefData>(new GraphQL.GraphQLRequest
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
                }).ConfigureAwait(false);

                if (dataRefDescriptions?.Data?.DataRef?.DataRefDescriptions == null)
                {
                    Log.Instance.log("ProSim data definitions refresh returned no data definitions.", LogSeverity.Error);
                    return false;
                }

                var newDataRefDescriptions = dataRefDescriptions.Data.DataRef.DataRefDescriptions.ToDictionary(drd => drd.Name);

                // Disconnect paths flip the connection state before clearing the cache
                // under _cacheLock, so checking inside the lock guarantees a stale
                // refresh either bails here or is cleared right after the swap.
                lock (_cacheLock)
                {
                    if (!IsConnected() || !ReferenceEquals(refreshConnection, _connection))
                    {
                        Log.Instance.log("Discarding ProSim data definitions because the connection changed during refresh.", LogSeverity.Warn);
                        return false;
                    }

                    _dataRefDescriptions = newDataRefDescriptions;
                }

                Log.Instance.log($"Refreshed {newDataRefDescriptions.Count} data definitions", LogSeverity.Debug);

                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error refreshing ProSim data definitions: {ex.Message}", LogSeverity.Error);
                return false;
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
                    lock (_cacheLock)
                    {
                        if (_dataRefDescriptions.Count == 0)
                        {
                            continue;
                        }

                        if (!_dataRefDescriptions.ContainsKey(pendingWrite.datarefPath))
                        {
                            continue;
                        }
                    }

                    WriteOutValue(pendingWrite.datarefPath, pendingWrite.value);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Error processing pending ProSim write for {pendingWrite.datarefPath}: {ex.Message}", LogSeverity.Error);
                }
            }
        }

        public bool Disconnect()
        {
            if (_connected)
            {
                _connected = false;
                DisposeConnection();

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

                ResetRefreshState();

                // Stop heartbeat timer
                StopHeartbeat();

                lock (_subscriptions)
                {
                    foreach (var subscription in _subscriptions.Values)
                    {
                        subscription.Dispose();
                    }
                    // Remove the disposed entries so datarefs re-subscribe after a reconnect
                    _subscriptions.Clear();
                }

                Closed?.Invoke(this, new EventArgs());
            }
            return !_connected;
        }

        public bool IsConnected()
        {
            return _connected && _connection != null;
        }

        public object readDataref(string datarefPath)
        {
            if (!IsConnected())
            {
                return (double)0;
            }

            try
            {
                bool isSubscribed;
                lock (_subscriptions)
                {
                    isSubscribed = _subscriptions.ContainsKey(datarefPath);
                }

                if (!isSubscribed)
                {
                    SubscribeToDataRef(datarefPath);
                    return (double)0;
                }

                lock (_subscribedDataRefs)
                {
                    if (!_subscribedDataRefs.ContainsKey(datarefPath))
                    {
                        // Wait for data to be returned by the subscription
                        return (double)0;
                    }

                    // Cache the dictionary value to avoid redundant lookups
                    var subscribedDataRef = _subscribedDataRefs[datarefPath];
                    var value = subscribedDataRef.Value;

                    if (subscribedDataRef.DataRefDescription.DataType == "System.String")
                    {
                        return value;
                    }

                    var returnValue = (value == null) ? 0 : Convert.ToDouble(value);

                    return returnValue;
                }

            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error reading dataref {datarefPath}: {ex.Message}", LogSeverity.Error);
                return (double)0;
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
                DataRefDescription description = null;
                var queueWrite = false;
                lock (_cacheLock)
                {
                    if (_dataRefDescriptions.Count == 0)
                    {
                        _pendingWrites.Enqueue((datarefPath, value));
                        queueWrite = true;
                    }
                    else if (!_dataRefDescriptions.TryGetValue(datarefPath, out description))
                    {
                        return;
                    }
                }

                if (queueWrite)
                {
                    Log.Instance.log($"Queued write for {datarefPath} until data definitions are available", LogSeverity.Debug);
                    // Joins the in-flight refresh if one is running, starts a new one
                    // otherwise; the refresh drains the queue on success.
                    TriggerRefreshInBackground();
                    return;
                }

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
                        transformedValue = Convert.ToBoolean(value);
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Failed to transform ProSim write value for {datarefPath}: {ex.Message}", LogSeverity.Error);
                    return;
                }

                WriteOutValue(datarefPath, transformedValue);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error writing ProSim dataref {datarefPath}: {ex.Message}", LogSeverity.Error);
            }
        }

        private void TriggerRefreshInBackground()
        {
            LogTaskFault(RefreshAndWarnAsync(), "Background ProSim refresh failed");
        }

        private async Task RefreshAndWarnAsync()
        {
            var refreshSuccessful = await RefreshDataDefinitionsAsync().ConfigureAwait(false);
            if (!refreshSuccessful)
            {
                Log.Instance.log("Background ProSim refresh completed without data definitions.", LogSeverity.Warn);
            }
        }

        public Dictionary<string, DataRefDescription> GetDataRefDescriptions()
        {
            lock (_cacheLock)
            {
                return new Dictionary<string, DataRefDescription>(_dataRefDescriptions);
            }
        }
    }
} 
