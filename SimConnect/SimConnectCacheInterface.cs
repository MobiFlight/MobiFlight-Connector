using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.SimConnect
{
    class SimConnectCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;

       /* public void Clear()
        {
            throw new NotImplementedException();
        }*/

        public bool connect()
        {
            throw new NotImplementedException();
        }

        public bool disconnect()
        {
            throw new NotImplementedException();
        }

        public bool isConnected()
        {
            throw new NotImplementedException();
        }

        public void sendEventID(int eventID, int param)
        {
        }
    }
}
