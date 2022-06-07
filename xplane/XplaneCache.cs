using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlaneConnector;

namespace MobiFlight.xplane
{
    public class XplaneCache : XplaneCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler OnUpdateFrequencyPerSecondChanged;

        private bool _simConnectConnected = false;
        private bool _connected = false;
        private int _updateFrequencyPerSecond = 10;
        public int UpdateFrequencyPerSecond { 
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

        public bool Connect()
        {
            if (Connector == null) Connector = new XPlaneConnector.XPlaneConnector();
            
            Connector.OnLog += (m) =>
            {
                Log.Instance.log(m, LogSeverity.Debug);
            };

            OnUpdateFrequencyPerSecondChanged += (v, e) =>
            {
                Log.Instance.log($"XplaneCache: update frequency changed: {v} per second.", LogSeverity.Info);
                Connector?.Stop();
                Connector?.Start();
            };

            SubscribedDataRefs.Clear();
            _connected = true;

            Connected?.Invoke(this, new EventArgs());
          
            return _connected;
        }
        public bool Disconnect()
        {
            _connected = false;
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
        }

        public void Stop()
        {
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
                Connector.Subscribe(SubscribedDataRefs[dataRefPath], UpdateFrequencyPerSecond, (e, v) => {
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
