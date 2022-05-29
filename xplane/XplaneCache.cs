using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.xplane
{
    internal class XplaneCache
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;

        private bool _simConnectConnected = false;
        private bool _connected = false;

        public bool Connect()
        {
            return false;
        }
        public bool Disconnect()
        {
            return false;
        }

        public int readDataRef(string dataRefPath)
        {
            return 0;
        }

        public void writeDataRef(string dataRefPath, int value)
        {

        }

        public void sendCommand(string commmand)
        {

        }
    }
}
