using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FsuipcSdk;

namespace ArcazeUSB
{
    class FsuipcCache : SourceCacheInterface
    {
        /// <summary>
        /// Gets raised whenever connection is close
        /// </summary>
        public event EventHandler Closed;
        /// <summary>
        /// Gets raised whenever connection is established
        /// </summary>
        public event EventHandler Connected;

        public event EventHandler ConnectionLost;


        /// <summary>
        /// indicates whether fsuipc is connected or not
        /// </summary>
        protected bool fsuipcIsConnected = false;

        /// <summary>
        /// the following lines were taken from the fsuipc sdk examples
        /// </summary>
        Fsuipc fsuipc = new Fsuipc();	    // Our main fsuipc object!
        bool fsuipcResult = false;			// Return boolean for FSUIPC method calls        
        int fsuipcVersion = 0;				// Any version of FS is OK
        long fsuipcRefResult = -1;				// Variable to hold returned results        
        int fsuipcToken = -1;

        Dictionary<Int32, Int32> __cache = new Dictionary<Int32, Int32>();
        public void Clear()
        {
            __cache.Clear();
        } //clear()

        /// <summary>
        /// indicates the status of the fsuipc connection
        /// </summary>
        /// <returns>true if connected, false if not</returns>
        public bool isConnected()
        {
            return fsuipcIsConnected;
        } //isConnected()

        /// <summary>
        /// initializes and opens connection to fsuipc
        /// </summary>
        public bool connect()
        {
            fsuipc.FSUIPC_Initialization();
            fsuipcIsConnected = fsuipc.FSUIPC_Open(fsuipcVersion, ref fsuipcRefResult);

            // update icon
            // @todo: raise event and handle event
            if (fsuipcIsConnected)
            {
                this.Connected(this, new EventArgs());                
            }
            else
            {
                this.Closed(this, new EventArgs());                
            }

            return fsuipcIsConnected;
        } //_connectFsuipc

        /// <summary>
        /// disconnects from fsuipc
        /// </summary>        
        public bool disconnect()
        {
            if (!fsuipcIsConnected) return fsuipcIsConnected;

            fsuipc.FSUIPC_Close();
            fsuipcIsConnected = false;
            return fsuipcIsConnected;
        } //_disconnectFsuipc

        public long getValue(Int32 offset, byte size)
        {
            // test the cache and gather data from fsuipc if necessary
            if (!__cache.ContainsKey(offset))
            {                
                try
                {
                    fsuipcRefResult = 0;
                    fsuipcResult = fsuipc.FSUIPC_Read(offset, size, ref fsuipcToken, ref fsuipcRefResult);
                    fsuipcResult = fsuipc.FSUIPC_Process(ref fsuipcRefResult);
                    fsuipcResult = fsuipc.FSUIPC_Get(ref fsuipcToken, ref fsuipcRefResult);

                    __cache[offset] = fsuipcRefResult;
                }
                catch (Exception e)
                {
                    this.ConnectionLost(this, new EventArgs());
                    return 0;
                }
            }

            return __cache[offset];
        } //getValue()        


        public long getLongValue(int offset, byte size)
        {
            throw new NotImplementedException();
        }

        public double getFloatValue(int offset, byte size)
        {
            throw new NotImplementedException();
        }

        public string getStringValue(int offset, byte size)
        {
            throw new NotImplementedException();
        }
    }
}
