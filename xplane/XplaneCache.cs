using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using XPlaneConnector;

namespace MobiFlight.xplane
{
    public class XplaneCache : XplaneCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler OnUpdateFrequencyPerSecondChanged;
        public event EventHandler<string> AircraftChanged;

        private DateTime LastHeartBeat = DateTime.MinValue;
        private readonly TimeSpan HeartbeatTimeout = TimeSpan.FromSeconds(5);
        private CancellationTokenSource cancellationTokenSource;

        private bool _simConnectConnected = false;
        private bool _connected = false;
        private int _updateFrequencyPerSecond = 10;
        private string _detectedAircraft = string.Empty;
        public int UpdateFrequencyPerSecond
        {
            get { return _updateFrequencyPerSecond; }
            set
            {
                if (_updateFrequencyPerSecond == value) return;
                _updateFrequencyPerSecond = value;
                OnUpdateFrequencyPerSecondChanged?.Invoke(value, new EventArgs());
            }
        }

        XPlaneConnector.XPlaneConnector Connector = null;

        Dictionary<String, DataRefElement> SubscribedDataRefs = new Dictionary<String, DataRefElement>();

        public XplaneCache()
        {
            OnUpdateFrequencyPerSecondChanged += (v, e) =>
            {
                Log.Instance.log($"update frequency changed: {v} per second.", LogSeverity.Debug);
                Connector?.Stop();
                Connector?.Start();
            };
        }

        public bool Connect()
        {
            // create the connector only now
            // because it will start the connection immediately
            if (Connector == null) Connector = new XPlaneConnector.XPlaneConnector();
#if DEBUG
            Connector.OnLog += (m) =>
            {
                // Log.Instance.log(m, LogSeverity.Debug);
            };
#endif

            SubscribedDataRefs.Clear();
            StartMonitoringHeartBeat();

            return _connected;
        }

        private void StartMonitoringHeartBeat()
        {
            if (Connector == null) return;
            if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested) return;

            Connector.Start();
            Connector.Unsubscribe("sim/time/local_time_sec");
            Connector.Subscribe(new DataRefElement() { DataRef = "sim/time/local_time_sec", Frequency = 1, Value = 0 }, 1, (e, v) =>
            {
                if (!_connected)
                {
                    _connected = true;
                    Connected?.Invoke(this, new EventArgs());
                }
                UpdateHeartBeat();
                CheckForAircraftName();
            });
            CheckForHeartBeatTimeout();

            Log.Instance.log("Heartbeat monitoring started", LogSeverity.Debug);
        }

        private void CheckForHeartBeatTimeout()
        {
            cancellationTokenSource = new CancellationTokenSource();
            InitHeartBeat();

            Task.Run(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationTokenSource.Token).ConfigureAwait(false); // Check heartbeat every second
                    var elapsedSinceLastHeartBeat = DateTime.Now - LastHeartBeat;
                    if (elapsedSinceLastHeartBeat > HeartbeatTimeout)
                    {
                        OnHeartBeatTimeout();
                        return;
                    }
                }
                Log.Instance.log("Heartbeat monitoring stopped with CancellationToken", LogSeverity.Debug);
            }, cancellationTokenSource.Token);
        }

        public void StopMonitoringHeartBeat()
        {
            cancellationTokenSource?.Cancel();
            Log.Instance.log("Heartbeat monitoring stopped", LogSeverity.Debug);
        }

        private void OnHeartBeatTimeout()
        {
            if (_connected)
            {
                Log.Instance.log("Heartbeat timeout", LogSeverity.Warn);
                StopMonitoringHeartBeat();
                Stop();
                Disconnect();
            }
        }

        public void CheckForAircraftName()
        {
            if (!_connected) return;
            UpdateAircraftSubscription();
        }

        private void InitHeartBeat()
        {
            LastHeartBeat = DateTime.Now;
        }

        private void UpdateHeartBeat()
        {
            LastHeartBeat = DateTime.Now;
#if DEBUG
            Log.Instance.log("Heartbeat received", LogSeverity.Debug);
#endif  
        }

        private void UpdateAircraftSubscription()
        {
            var datarefAircraftName = new StringDataRefElement();
            datarefAircraftName.DataRef = "sim/aircraft/view/acf_ui_name";
            datarefAircraftName.Frequency = 1;
            datarefAircraftName.Value = string.Empty;
            datarefAircraftName.StringLenght = 64;

            Connector.Unsubscribe(datarefAircraftName.DataRef);
            Connector.Subscribe(datarefAircraftName, 1, (e1, v1) =>
            {
                if (_detectedAircraft == v1) return;
                Log.Instance.log($"sim/aircraft/view/acf_ui_name = {v1}", LogSeverity.Debug);
                _detectedAircraft = v1;
                AircraftChanged?.Invoke(this, _detectedAircraft);
            });
        }

        public bool Disconnect()
        {
            if (_connected)
            {
                _connected = false;
                Connector.Dispose();
                Connector = null;

                Closed?.Invoke(this, new EventArgs());
            }
            _detectedAircraft = string.Empty;
            return _connected;
        }

        public bool IsConnected()
        {
            return _connected;
        }

        public void Start()
        {
            SubscribedDataRefs.Clear();
            Connector?.Start();
            StartMonitoringHeartBeat();
        }

        public void Stop()
        {
            StopMonitoringHeartBeat();
            Connector?.Stop();
        }

        public void Clear()
        {
            SubscribedDataRefs.Clear();
        }

        public float readDataRef(string dataRefPath)
        {
            if (Connector == null) return 0;

            if (!SubscribedDataRefs.ContainsKey(dataRefPath))
            {
                SubscribedDataRefs.Add(dataRefPath, new DataRefElement() { DataRef = dataRefPath, Frequency = UpdateFrequencyPerSecond, Value = 0 });
                Connector.Subscribe(SubscribedDataRefs[dataRefPath], UpdateFrequencyPerSecond, (e, v) =>
                {
                    SubscribedDataRefs[e.DataRef].Value = v;
                });
            }
            return SubscribedDataRefs[dataRefPath].Value;
        }

        public void writeDataRef(string dataRefPath, float value)
        {
            Connector?.SetDataRefValue(dataRefPath, value);
        }

        public void sendCommand(string command)
        {
            XPlaneCommand xPlaneCommand = new XPlaneCommand(command, command);
            Connector?.SendCommand(xPlaneCommand);
        }
    }
}
