using System;

namespace MobiFlight.xplane
{
    public class XplaneAggregateCache : XplaneCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler<string> AircraftChanged;

        private const string DefaultHost = "127.0.0.1";
        private const int DefaultPort = 8086;
        private static readonly TimeSpan DefaultProbeTimeout = TimeSpan.FromSeconds(1.5);

        private readonly XplaneCacheInterface _webApiCache;
        private readonly XplaneCacheInterface _udpCache;
        private readonly bool _webApiEnabled;

        private XplaneCacheInterface _active;

        public int UpdateFrequencyPerSecond
        {
            get;
            set
            {
                field = value;
                _webApiCache.UpdateFrequencyPerSecond = value;
                _udpCache.UpdateFrequencyPerSecond = value;
            }
        } = 10;

        public XplaneAggregateCache()
            : this(
                new WebApi.XplaneWebApiCache(DefaultHost, DefaultPort, DefaultProbeTimeout),
                new XplaneCache(),
                webApiEnabled: true)
        {
        }

        public XplaneAggregateCache(XplaneCacheInterface webApiCache, XplaneCacheInterface udpCache, bool webApiEnabled)
        {
            _webApiCache = webApiCache;
            _udpCache = udpCache;
            _webApiEnabled = webApiEnabled;

            WireEvents(_webApiCache);
            WireEvents(_udpCache);
        }

        private void WireEvents(XplaneCacheInterface inner)
        {
            inner.Closed += (s, e) => { if (ReferenceEquals(inner, _active)) Closed?.Invoke(this, e); };
            inner.Connected += (s, e) => { if (ReferenceEquals(inner, _active)) Connected?.Invoke(this, e); };
            inner.ConnectionLost += (s, e) => { if (ReferenceEquals(inner, _active)) ConnectionLost?.Invoke(this, e); };
            inner.AircraftChanged += (s, e) => { if (ReferenceEquals(inner, _active)) AircraftChanged?.Invoke(this, e); };
        }

        public bool Connect()
        {
            if (_webApiEnabled)
            {
                _active = _webApiCache;
                if (_webApiCache.Connect())
                {
                    Log.Instance.log("X-Plane connected via Web API (WebSocket).", LogSeverity.Info);
                    return true;
                }

                Log.Instance.log("X-Plane Web API not reachable, falling back to UDP.", LogSeverity.Debug);
            }

            _active = _udpCache;
            if (_udpCache.Connect())
            {
                Log.Instance.log("X-Plane connected via UDP.", LogSeverity.Info);
                return true;
            }

            _active = null;
            Log.Instance.log("X-Plane: neither Web API nor UDP connected.", LogSeverity.Debug);
            return false;
        }

        public bool Disconnect()
        {
            var result = _active != null && _active.Disconnect();
            _active = null;
            return result;
        }

        public bool IsConnected()
        {
            return _active != null && _active.IsConnected();
        }

        public void Start()
        {
            _active?.Start();
        }

        public void Stop()
        {
            _active?.Stop();
        }

        public void Clear()
        {
            _webApiCache.Clear();
            _udpCache.Clear();
        }

        public float readDataRef(string dataRefPath)
        {
            return _active?.readDataRef(dataRefPath) ?? 0f;
        }

        public void writeDataRef(string dataRefPath, float value)
        {
            _active?.writeDataRef(dataRefPath, value);
        }

        public void sendCommand(string command)
        {
            _active?.sendCommand(command);
        }

        public void beginCommand(string command)
        {
            _active?.beginCommand(command);
        }

        public void endCommand(string command)
        {
            _active?.endCommand(command);
        }
    }
}
