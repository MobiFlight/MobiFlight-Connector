using MobiFlight.xplane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlaneConnector;
using MobiFlight.Base;

namespace MobiFlightUnitTests.mock.xplane
{
    internal class XplaneCacheMock : XplaneCacheInterface
    {
        public List<DataRefElement> Writes = new List<DataRefElement>();
        public List<DataRefElement> Reads = new List<DataRefElement>();
        public List<XPlaneCommand> Commands = new List<XPlaneCommand>();

        Dictionary<String, DataRefElement> SubscribedDataRefs = new Dictionary<String, DataRefElement>();
        private bool _connected;

        public event EventHandler Closed;
        public event EventHandler ConnectionLost;
        public event EventHandler Connected;

        internal void Clear()
        {
            Writes.Clear();
            Reads.Clear();
            Commands.Clear();
            SubscribedDataRefs.Clear();
        }
        
        public bool Connect()
        {
            _connected = true;
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
            // do nothing
        }

        public void Stop()
        {
            // do nothing
        }

        public float readDataRef(string dataRefPath)
        {
            if (!SubscribedDataRefs.ContainsKey(dataRefPath))
            {
                SubscribedDataRefs.Add(dataRefPath, new DataRefElement() { DataRef = dataRefPath, Frequency = 5, Value = 0 });
            }
            Reads.Add(SubscribedDataRefs[dataRefPath]);
            return SubscribedDataRefs[dataRefPath].Value;
        }

        public void writeDataRef(string dataRefPath, float value)
        {
            if (!SubscribedDataRefs.ContainsKey(dataRefPath))
            {
                SubscribedDataRefs.Add(dataRefPath, new DataRefElement() { DataRef = dataRefPath, Frequency = 5, Value = 0 });
            }
            SubscribedDataRefs[dataRefPath].Value = value;
            Writes.Add(SubscribedDataRefs[dataRefPath]);
        }

        public void sendCommand(string command)
        {
            Commands.Add(new XPlaneCommand(command, null));
        }

        void CacheInterface.Clear()
        {
            throw new NotImplementedException();
        }
    }
}
