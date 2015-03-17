using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public interface SourceCacheInterface
    {
        /// <summary>
        /// Gets raised whenever connection is close
        /// </summary>
        event EventHandler Closed;
        /// <summary>
        /// Gets raised whenever connection is established
        /// </summary>
        event EventHandler Connected;
        /// <summary>
        /// Gets raised whenever connection is lost
        /// </summary>
        event EventHandler ConnectionLost;

        void Clear();

        /// <summary>
        /// indicates the status of the fsuipc connection
        /// </summary>
        /// <returns>true if connected, false if not</returns>
        bool isConnected();

        /// <summary>
        /// initializes and opens connection to fsuipc
        /// </summary>
        bool connect();

        /// <summary>
        /// disconnects from fsuipc
        /// </summary>        
        bool disconnect();

        long getValue(Int32 offset, byte size);

        //ulong getUValue(Int32 offset, byte size);

        long getLongValue(Int32 offset, byte size);
        
        double getFloatValue(Int32 offset, byte size);

        string getStringValue(Int32 offset, byte size);
    }
}
