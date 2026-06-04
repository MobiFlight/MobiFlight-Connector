using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MobiFlight.OpenAttitude
{
    /// <summary>
    /// FlightGear PropertyListener-compatible WebSocket server that the OpenAttitude
    /// panel can connect to. Clients subscribe to named property paths;
    /// call <see cref="BroadcastUpdate"/> to push values to interested clients.
    /// </summary>
    public class PropertyListenerServer : IDisposable
    {
        private readonly int _port;
        private WebSocketServer _server;

        private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _clients = new ConcurrentDictionary<Guid, IWebSocketConnection>();
        private readonly ConcurrentDictionary<Guid, HashSet<string>> _subscriptions = new ConcurrentDictionary<Guid, HashSet<string>>();

        public int Port => _port;
        public bool IsRunning { get; private set; }

        public PropertyListenerServer(int port) => _port = port;
        public string Url => $"ws://0.0.0.0:{_port}";

        public void Start()
        {
            FleckLog.LogAction = (level, msg, ex) =>
            {
                if (level >= LogLevel.Warn)
                    Log.Instance.log($"[OpenAttitude/WS/{level}] {msg} {ex?.Message}", LogSeverity.Warn);
            };

            _server = new WebSocketServer(Url);
            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    if (socket.ConnectionInfo.Path != "/PropertyListener")
                    {
                        socket.Close();
                        return;
                    }
                    _clients[socket.ConnectionInfo.Id] = socket;
                    _subscriptions[socket.ConnectionInfo.Id] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    Log.Instance.log($"[OpenAttitude] Client connected: {socket.ConnectionInfo.ClientIpAddress}", LogSeverity.Info);
                };

                socket.OnClose = () =>
                {
                    _clients.TryRemove(socket.ConnectionInfo.Id, out _);
                    _subscriptions.TryRemove(socket.ConnectionInfo.Id, out _);
                    Log.Instance.log($"[OpenAttitude] Client disconnected: {socket.ConnectionInfo.ClientIpAddress}", LogSeverity.Info);
                };

                socket.OnMessage = msg => HandleMessage(socket, msg);
                socket.OnError = ex => Log.Instance.log($"[OpenAttitude] WS error: {ex.Message}", LogSeverity.Error);
            });

            IsRunning = true;
            Log.Instance.log($"[OpenAttitude] Listening on ws://localhost:{_port}/PropertyListener", LogSeverity.Info);
        }

        private void HandleMessage(IWebSocketConnection socket, string raw)
        {
            PropertyListenerCommand cmd;
            try { cmd = JsonConvert.DeserializeObject<PropertyListenerCommand>(raw); }
            catch { return; }

            if (cmd == null || string.IsNullOrEmpty(cmd.Node)) return;
            var id = socket.ConnectionInfo.Id;

            switch (cmd.Command?.ToLowerInvariant())
            {
                case "addlistener":
                case "get":
                    if (!_subscriptions.TryGetValue(id, out var subs)) break;
                    lock (subs) { subs.Add(cmd.Node); }
                    break;

                case "removelistener":
                    if (!_subscriptions.TryGetValue(id, out var rsubs)) break;
                    lock (rsubs) { rsubs.Remove(cmd.Node); }
                    break;
            }
        }

        /// <summary>
        /// Pushes a value to every client subscribed to <paramref name="path"/>.
        /// </summary>
        public void BroadcastUpdate(string path, object value)
        {
            string type = value is string ? "string" : "double";
            var update = new PropertyListenerUpdate { Node = path, Path = path, Value = value, Type = type };
            string json = JsonConvert.SerializeObject(update);

            foreach (var kvp in _clients)
            {
                if (!_subscriptions.TryGetValue(kvp.Key, out var subs)) continue;
                bool subscribed;
                lock (subs) { subscribed = subs.Contains(path); }
                if (!subscribed || !kvp.Value.IsAvailable) continue;
                try { kvp.Value.Send(json); }
                catch { /* client disconnected mid-send */ }
            }
        }

        public void Stop()
        {
            _server?.Dispose();
            IsRunning = false;
        }

        public void Dispose() => Stop();
    }

    public class PropertyListenerCommand
    {
        [JsonProperty("command")] 
        public string Command { get; set; } = "";
        [JsonProperty("node")] 
        public string Node { get; set; } = "";
        [JsonProperty("value")] 
        public string Value { get; set; }
    }

    public class PropertyListenerUpdate
    {
        [JsonProperty("node")] 
        public string Node { get; set; } = "";
        [JsonProperty("path")] 
        public string Path { get; set; } = "";
        [JsonProperty("value")] 
        public object Value { get; set; } = 0.0;
        [JsonProperty("type")] 
        public string Type { get; set; } = "double";
    }
}