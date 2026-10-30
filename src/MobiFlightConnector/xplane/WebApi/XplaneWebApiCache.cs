using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobiFlight.xplane.WebApi
{
    public class XplaneWebApiCache : XplaneCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler<string> AircraftChanged;

        public int UpdateFrequencyPerSecond { get; set; } = 10;

        internal const int DiscoveryPageSize = 1000;

        private static readonly TimeSpan DiscoveryTimeout = TimeSpan.FromSeconds(15);

        private readonly string _host;
        private readonly int _port;
        private readonly TimeSpan _probeTimeout;
        private readonly HttpMessageHandler _testHttpMessageHandler;

        private readonly Lock _stateLock = new();
        private readonly SemaphoreSlim _sendLock = new(1, 1);

        private readonly Lazy<HttpClient> _httpClient;
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _receiveLoopCts;
        private TaskCompletionSource<bool> _connectedSignal;

        private bool _connected;
        private int _reqId;
        private string _detectedAircraft = string.Empty;

        private readonly Dictionary<string, long> _datarefIdsByName = new();
        private readonly Dictionary<string, long> _commandIdsByName = new();
        private readonly HashSet<long> _subscribedIds = [];
        private readonly Dictionary<long, float[]> _lastFloatValues = new();

        private long _heartbeatTimeId = -1;
        private long _heartbeatAircraftNameId = -1;

        public XplaneWebApiCache(string host, int port, TimeSpan probeTimeout)
        {
            _host = host;
            _port = port;
            _probeTimeout = probeTimeout;
            _httpClient = new Lazy<HttpClient>(CreateHttpClient);
        }

        internal XplaneWebApiCache(string host, int port, TimeSpan probeTimeout, HttpMessageHandler httpMessageHandler)
            : this(host, port, probeTimeout)
        {
            _testHttpMessageHandler = httpMessageHandler;
        }

        public bool Connect()
        {
            CleanupTransport();

            try
            {
                return ConnectAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Instance.log($"X-Plane Web API connect failed: {ex.Message}", LogSeverity.Debug);
                return false;
            }
        }

        private async Task<bool> ConnectAsync()
        {
            return await TryDiscoverCapabilitiesAsync().ConfigureAwait(false)
                   && await ConnectWebSocketAndWaitForHeartbeatAsync().ConfigureAwait(false);
        }

        private HttpClient CreateHttpClient()
        {
            var client = _testHttpMessageHandler != null
                ? new HttpClient(_testHttpMessageHandler, disposeHandler: false)
                : new HttpClient();

            client.BaseAddress = new Uri($"http://{_host}:{_port}");
            client.Timeout = Timeout.InfiniteTimeSpan;
            return client;
        }

        private async Task<bool> TryDiscoverCapabilitiesAsync()
        {
            List<DatarefListEntry> firstDatarefPage;
            using (var probeCts = new CancellationTokenSource(_probeTimeout))
            {
                try
                {
                    firstDatarefPage = await FetchDiscoveryPageAsync<DatarefListEntry>(
                        XplaneWebApiConstants.DatarefsPath, 0, probeCts.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"X-Plane Web API not reachable at {_host}:{_port}: {ex.Message}",
                        LogSeverity.Debug);
                    return false;
                }
            }

            if (firstDatarefPage == null) return false;

            using var discoveryCts = new CancellationTokenSource(DiscoveryTimeout);
            var allDatarefs = await FetchRemainingDiscoveryPagesAsync(firstDatarefPage,
                XplaneWebApiConstants.DatarefsPath, discoveryCts.Token).ConfigureAwait(false);

            lock (_stateLock)
            {
                _datarefIdsByName.Clear();
                foreach (var entry in allDatarefs)
                {
                    _datarefIdsByName[entry.Name] = entry.Id;
                }
            }

            try
            {
                var firstCommandPage = await FetchDiscoveryPageAsync<CommandListEntry>(
                    XplaneWebApiConstants.CommandsPath, 0, discoveryCts.Token).ConfigureAwait(false);
                var allCommands = firstCommandPage != null
                    ? await FetchRemainingDiscoveryPagesAsync(firstCommandPage, XplaneWebApiConstants.CommandsPath,
                        discoveryCts.Token).ConfigureAwait(false)
                    : [];

                lock (_stateLock)
                {
                    _commandIdsByName.Clear();
                    foreach (var entry in allCommands)
                    {
                        _commandIdsByName[entry.Name] = entry.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"X-Plane Web API commands discovery failed: {ex.Message}", LogSeverity.Debug);
            }

            return true;
        }

        private async Task<List<T>> FetchRemainingDiscoveryPagesAsync<T>(List<T> firstPage, string path,
            CancellationToken token)
        {
            var all = new List<T>(firstPage);
            var start = DiscoveryPageSize;

            while (firstPage.Count == DiscoveryPageSize)
            {
                try
                {
                    firstPage = await FetchDiscoveryPageAsync<T>(path, start, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"X-Plane Web API paged discovery of {path} stopped early: {ex.Message}",
                        LogSeverity.Debug);
                    break;
                }

                if (firstPage == null) break;

                all.AddRange(firstPage);
                start += DiscoveryPageSize;
            }

            return all;
        }

        private async Task<List<T>> FetchDiscoveryPageAsync<T>(string path, int start, CancellationToken token)
        {
            var response = await _httpClient.Value
                .GetAsync($"{path}?start={start}&limit={DiscoveryPageSize}", token).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                Log.Instance.log($"X-Plane Web API GET {path} returned {(int)response.StatusCode}.", LogSeverity.Debug);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
            return (JsonConvert.DeserializeObject<PagedListResponse<T>>(json) ?? new PagedListResponse<T>()).Data;
        }

        private async Task<bool> ConnectWebSocketAndWaitForHeartbeatAsync()
        {
            _webSocket = new ClientWebSocket();
            _connectedSignal = new TaskCompletionSource<bool>();

            try
            {
                using var cts = new CancellationTokenSource(_probeTimeout);
                var uri = new Uri($"ws://{_host}:{_port}{XplaneWebApiConstants.WebSocketPath}");
                await _webSocket.ConnectAsync(uri, cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"X-Plane Web API WebSocket connect failed: {ex.Message}", LogSeverity.Debug);
                _webSocket.Dispose();
                _webSocket = null;
                return false;
            }

            _receiveLoopCts = new CancellationTokenSource();
            _ = ReceiveLoopAsync(_webSocket, _receiveLoopCts.Token);

            await SubscribeHeartbeatAsync().ConfigureAwait(false);

            try
            {
                return await _connectedSignal.Task.WaitAsync(_probeTimeout).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        private async Task SubscribeHeartbeatAsync()
        {
            var ids = new List<long>();

            lock (_stateLock)
            {
                if (_datarefIdsByName.TryGetValue(XplaneWebApiConstants.HeartbeatTimeDataRef, out var timeId))
                {
                    _heartbeatTimeId = timeId;
                    _subscribedIds.Add(timeId);
                    ids.Add(timeId);
                }

                if (_datarefIdsByName.TryGetValue(XplaneWebApiConstants.HeartbeatAircraftNameDataRef,
                        out var aircraftId))
                {
                    _heartbeatAircraftNameId = aircraftId;
                    _subscribedIds.Add(aircraftId);
                    ids.Add(aircraftId);
                }
            }

            if (ids.Count == 0)
            {
                Log.Instance.log("X-Plane Web API: heartbeat datarefs not found in discovery list.", LogSeverity.Debug);
                return;
            }

            await SendWebSocketMessageAsync(XplaneWebApiMessageParser.BuildSubscribeMessage(NextReqId(), ids))
                .ConfigureAwait(false);
        }

        private async Task ReceiveLoopAsync(ClientWebSocket webSocket, CancellationToken token)
        {
            var buffer = new byte[8192];

            try
            {
                while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
                {
                    using var messageStream = new MemoryStream();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer, token).ConfigureAwait(false);
                        if (result.MessageType == WebSocketMessageType.Close) return;
                        messageStream.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);

                    HandleMessage(Encoding.UTF8.GetString(messageStream.ToArray()));
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                Log.Instance.log($"X-Plane Web API receive loop ended: {ex.Message}", LogSeverity.Debug);
            }
            finally
            {
                if (_connected)
                {
                    _connected = false;
                    ConnectionLost?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void HandleMessage(string json)
        {
            if (XplaneWebApiMessageParser.TryParseResult(json, out var reqId, out var success, out var errorCode,
                    out var errorMessage))
            {
                if (!success)
                {
                    Log.Instance.log($"X-Plane Web API request {reqId} failed: {errorCode} {errorMessage}",
                        LogSeverity.Debug);
                }

                return;
            }

            if (!XplaneWebApiMessageParser.TryParseDatarefUpdate(json, out var floatValues, out var stringValues))
                return;

            lock (_stateLock)
            {
                foreach (var kvp in floatValues) _lastFloatValues[kvp.Key] = kvp.Value;
            }

            if (!_connected && _heartbeatTimeId != -1 && floatValues.ContainsKey(_heartbeatTimeId))
            {
                _connected = true;
                _connectedSignal?.TrySetResult(true);
                Connected?.Invoke(this, EventArgs.Empty);
            }

            if (_heartbeatAircraftNameId == -1 || !stringValues.TryGetValue(_heartbeatAircraftNameId,
                                                   out var aircraftName)
                                               || _detectedAircraft == aircraftName) return;

            _detectedAircraft = aircraftName;
            AircraftChanged?.Invoke(this, _detectedAircraft);
        }

        public bool Disconnect()
        {
            var wasConnected = _connected;

            if (wasConnected)
            {
                _detectedAircraft = string.Empty;
                AircraftChanged?.Invoke(this, _detectedAircraft);
            }

            _connected = false;
            CleanupTransport();

            if (wasConnected)
            {
                Closed?.Invoke(this, EventArgs.Empty);
            }

            return _connected;
        }

        private void CleanupTransport()
        {
            _receiveLoopCts?.Cancel();
            _receiveLoopCts = null;

            try
            {
                _webSocket?.Abort();
            }
            catch (Exception)
            {
                // ignored
            }

            _webSocket?.Dispose();
            _webSocket = null;

            lock (_stateLock)
            {
                _subscribedIds.Clear();
                _lastFloatValues.Clear();
            }
        }

        public bool IsConnected()
        {
            return _connected;
        }

        public void Start()
        {
            UnsubscribeAll();
        }

        public void Stop()
        {
            UnsubscribeAll();
        }

        public void Clear()
        {
            UnsubscribeAll();
        }

        private void UnsubscribeAll()
        {
            List<long> ids;
            lock (_stateLock)
            {
                ids = [.. _subscribedIds];
                _subscribedIds.Clear();
            }

            if (ids.Count == 0) return;

            FireAndForget(
                SendWebSocketMessageAsync(XplaneWebApiMessageParser.BuildUnsubscribeMessage(NextReqId(), ids)),
                "Failed to unsubscribe X-Plane Web API datarefs");
        }

        public float readDataRef(string dataRefPath)
        {
            var (name, index) = XplaneDataRefPathParser.Parse(dataRefPath);

            if (!TryResolveDatarefId(name, out var id)) return 0f;

            EnsureSubscribed(id);

            lock (_stateLock)
            {
                if (!_lastFloatValues.TryGetValue(id, out var values)) return 0f;

                var i = index ?? 0;
                if (i < 0 || i >= values.Length) return 0f;

                return values[i];
            }
        }

        private void EnsureSubscribed(long id)
        {
            lock (_stateLock)
            {
                if (!_subscribedIds.Add(id)) return;
            }

            FireAndForget(
                SendWebSocketMessageAsync(XplaneWebApiMessageParser.BuildSubscribeMessage(NextReqId(), [id])),
                $"Failed to subscribe to X-Plane Web API dataref id {id}");
        }

        public void writeDataRef(string dataRefPath, float value)
        {
            var (name, index) = XplaneDataRefPathParser.Parse(dataRefPath);

            if (!TryResolveDatarefId(name, out var id)) return;

            FireAndForget(
                SendWebSocketMessageAsync(
                    XplaneWebApiMessageParser.BuildSetValuesMessage(NextReqId(), id, value, index)),
                $"Failed to write X-Plane Web API dataref {dataRefPath}");
        }

        public void sendCommand(string command)
        {
            if (!TryResolveCommandId(command, out var id))
            {
                Log.Instance.log($"X-Plane Web API: unknown command '{command}'.", LogSeverity.Debug);
                return;
            }

            FireAndForget(
                SetCommandActiveAsync(id, true, duration: 0),
                $"Failed to activate X-Plane Web API command {command}");
        }

        private Task SetCommandActiveAsync(long id, bool isActive, double? duration = null)
        {
            return SendWebSocketMessageAsync(
                XplaneWebApiMessageParser.BuildCommandSetActiveMessage(NextReqId(), id, isActive, duration));
        }

        internal bool TryResolveDatarefId(string name, out long id)
        {
            lock (_stateLock)
            {
                return _datarefIdsByName.TryGetValue(name, out id);
            }
        }

        internal bool TryResolveCommandId(string name, out long id)
        {
            lock (_stateLock)
            {
                return _commandIdsByName.TryGetValue(name, out id);
            }
        }

        private async Task SendWebSocketMessageAsync(string json)
        {
            if (_webSocket?.State != WebSocketState.Open)
                return;

            var bytes = Encoding.UTF8.GetBytes(json);

            await _sendLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        /// <summary>
        /// Generates a request id in a thread safe manner.
        /// </summary>
        /// <returns></returns>
        private int NextReqId() => Interlocked.Increment(ref _reqId);

        private static void FireAndForget(Task task, string errorMessage)
        {
            task.ContinueWith(
                t =>
                {
                    Log.Instance.log($"{errorMessage}: {t.Exception?.GetBaseException().Message}", LogSeverity.Debug);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}