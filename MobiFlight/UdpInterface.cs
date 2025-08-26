using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class UdpInterface : IDisposable
    {
        private UdpClient _client;
        private IPEndPoint _remoteEndPoint;
        private bool _listening;

        public event Action<byte[]> DataReceived; // Changed from float[] to byte[]

        public UdpInterface(UdpSettings settings)
        {
            _client = new UdpClient(new IPEndPoint(IPAddress.Parse(settings.LocalIp), settings.LocalPort));
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(settings.RemoteIp), settings.RemotePort);
        }

        public void StartListening()
        {
            _listening = true;
            Task.Run(async () =>
            {
                while (_listening)
                {
                    try
                    {
                        var result = await _client.ReceiveAsync();
                        DataReceived?.Invoke(result.Buffer); // Send raw bytes
                    }
                    catch (ObjectDisposedException)
                    {
                        // Expected when stopping
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.log($"UdpInterface: Error receiving data: {ex.Message}", LogSeverity.Error);
                    }
                }
            });
        }

        public void StopListening()
        {
            _listening = false;
        }

        // Generic method for sending raw bytes
        public void Send(byte[] data)
        {
            try
            {
                _client.Send(data, data.Length, _remoteEndPoint);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"UdpInterface: Error sending data: {ex.Message}", LogSeverity.Error);
            }
        }

        // Convenience method for sending float arrays (backward compatibility)
        public void Send(float[] data)
        {
            var bytes = PackFloats(data);
            Send(bytes);
        }

        private byte[] PackFloats(float[] data)
        {
            var bytes = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
                Array.Copy(BitConverter.GetBytes(data[i]), 0, bytes, i * 4, 4);
            return bytes;
        }

        public void Dispose()
        {
            _listening = false;
            _client?.Dispose();
        }
    }
}