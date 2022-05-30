using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlaneConnector;

namespace MobiFlight.xplane
{
    internal class XplaneCache
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;

        private bool _simConnectConnected = false;
        private bool _connected = false;

        XPlaneConnector.XPlaneConnector Connector = null;

        Dictionary<String, DataRefElement> SubscribedDataRefs = new Dictionary<String, DataRefElement>();

        public bool Connect()
        {
            if (Connector == null) Connector = new XPlaneConnector.XPlaneConnector();
            
            Connector.OnLog += (m) =>
            {
                Log.Instance.log(m, LogSeverity.Debug);
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

        internal void Start()
        {
            SubscribedDataRefs.Clear();
            Connector?.Start();
        }

        internal void Stop()
        {
            Connector?.Stop();
        }

        public float readDataRef(string dataRefPath)
        {
            if (!SubscribedDataRefs.ContainsKey(dataRefPath))
            {
                SubscribedDataRefs.Add(dataRefPath, new DataRefElement() { DataRef = dataRefPath, Frequency = 5, Value = 0 });
                Connector.Subscribe(SubscribedDataRefs[dataRefPath], 5, (e, v) => {
                    SubscribedDataRefs[e.DataRef].Value = v;
                });
            }
            return SubscribedDataRefs[dataRefPath].Value;
        }

        public void writeDataRef(string dataRefPath, float value)
        {
            Connector.SetDataRefValue(dataRefPath, value);
        }

        public void sendCommand(string command)
        {
            XPlaneCommand xPlaneCommand = new XPlaneCommand(command, command);
            Connector.SendCommand(xPlaneCommand);
        }
    }
}
