using System;

namespace MobiFlight.Base
{
    public interface CacheInterface
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
        event EventHandler<string> AircraftChanged;

        void Clear();

        /// <summary>
        /// indicates the status of the fsuipc connection
        /// </summary>
        /// <returns>true if connected, false if not</returns>
        bool IsConnected();

        /// <summary>
        /// initializes and opens connection to fsuipc
        /// </summary>
        bool Connect();

        /// <summary>
        /// disconnects from fsuipc
        /// </summary>        
        bool Disconnect();
    }
}
