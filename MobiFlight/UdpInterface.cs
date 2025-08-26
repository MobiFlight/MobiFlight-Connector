using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class UdpSettings
    {
        public string LocalIp { get; set; } = "127.0.0.1";
        public int LocalPort { get; set; } = 5555;
        public string RemoteIp { get; set; } = "127.0.0.1";
        public int RemotePort { get; set; } = 5556;
    }

    public class UdpInterface : IDisposable
    {
        private UdpClient _client;
        private IPEndPoint _remoteEndPoint;
        private bool _listening;

        public event Action<float[]> DataReceived;

        public UdpInterface(UdpSettings settings)
            : this(settings.LocalIp, settings.LocalPort, settings.RemoteIp, settings.RemotePort)
        {
        }

        private UdpInterface(string localIp, int localPort, string remoteIp, int remotePort)
        {
            _client = new UdpClient(new IPEndPoint(IPAddress.Parse(localIp), localPort));
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);
        }

        public void StartListening()
        {
            _listening = true;
            Task.Run(async () =>
            {
                while (_listening)
                {
                    var result = await _client.ReceiveAsync();
                    var floats = ParseFloats(result.Buffer);
                    DataReceived?.Invoke(floats);
                }
            });
        }

        public void StopListening()
        {
            _listening = false;
        }

        public void Send(float[] data)
        {
            var bytes = PackFloats(data);
            _client.Send(bytes, bytes.Length, _remoteEndPoint);
        }

        private float[] ParseFloats(byte[] buffer)
        {
            int count = buffer.Length / 4;
            float[] result = new float[count];
            for (int i = 0; i < count; i++)
                result[i] = BitConverter.ToSingle(buffer, i * 4);
            return result;
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