using System;

namespace MobiFlight
{
    /// <summary>
    /// Interface for UDP communication that can be implemented by both real and mock implementations
    /// </summary>
    public interface IUdpInterface : IDisposable
    {
        event Action<byte[]> DataReceived;
        
        void StartListening();
        void StopListening();
        void Send(byte[] data);
        void Send(float[] data);
    }
}