using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private bool _simConnectConnected = false;
        private bool _connected = false;
        private int _updateFrequencyPerSecond = 10;
        private string _detectedAircraft = string.Empty;
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
                // Log.Instance.log(m, LogSeverity.Debug);
            };

            OnUpdateFrequencyPerSecondChanged += (v, e) =>
            {
                Log.Instance.log($"update frequency changed: {v} per second.", LogSeverity.Debug);
                Connector?.Stop();
                Connector?.Start();
            };

            SubscribedDataRefs.Clear();
            _connected = true;

            Connected?.Invoke(this, new EventArgs());

            CheckForAircraftName();

            return _connected;
        }

        public void CheckForAircraftName()
        {
            if (!_connected) return;
            // just start the connector
            Connector?.Start();
            Connector.Subscribe(new DataRefElement() { DataRef = "sim/aircraft/view/acf_ui_name[0]", Frequency = 1, Value = 0 }, 1, (e, v) =>
            {
                Log.Instance.log($"sim/aircraft/view/acf_ui_name[0] = {v}", LogSeverity.Debug);
                UpdateAircraftSubscription();
            });
            Connector.Subscribe(new DataRefElement() { DataRef = "sim/aircraft/view/acf_ui_name[2]", Frequency = 1, Value = 0 }, 1, (e, v) =>
            {
                Log.Instance.log($"sim/aircraft/view/acf_ui_name[2] = {v}", LogSeverity.Debug);
                UpdateAircraftSubscription();
            });
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
                _detectedAircraft = v1;
                AircraftChanged?.Invoke(this, _detectedAircraft);
            });
        }

        public bool Disconnect()
        {
            if (_connected)
            {
                _connected = false;
                Closed?.Invoke(this, new EventArgs());
            }
            
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
            CheckForAircraftName();
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
