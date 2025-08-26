namespace FliteSimFfbMock
{
    using System.Net;
    using System.Net.Sockets;

    public class FfbUdpSimulator : IDisposable
    {
        private readonly int receivePort;
        private readonly int sendPort;
        private readonly string targetIp;
        private UdpClient? receiver;
        private UdpClient? sender;
        private IPEndPoint? sendEndpoint;
        private CancellationTokenSource? cts;
        private Task? receiveTask;

        public FfbUdpSimulator(int receivePort, int sendPort, string targetIp)
        {
            this.receivePort = receivePort;
            this.sendPort = sendPort;
            this.targetIp = targetIp;
        }

        public void Start()
        {
            cts = new CancellationTokenSource();
            receiver = new UdpClient(receivePort);
            sender = new UdpClient();
            sendEndpoint = new IPEndPoint(IPAddress.Parse(targetIp), sendPort);

            receiveTask = Task.Run(() => ReceiveLoop(cts.Token));
        }

        public void Stop()
        {
            cts?.Cancel();
            receiver?.Dispose();
            sender?.Dispose();
            receiveTask = null;
        }

        public void SendValue(float value, int channel = 0)
        {
            var data = new float[10];
            data[channel] = value;
            var bytes = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
                Array.Copy(BitConverter.GetBytes(data[i]), 0, bytes, i * 4, 4);
            sender?.Send(bytes, bytes.Length, sendEndpoint!);
            Console.WriteLine($"Sent value {value} on channel {channel}.");
        }

        public void TriggerHandshake()
        {
            var data = new float[10];
            data[0] = 123.456f; // Example handshake value
            var bytes = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
                Array.Copy(BitConverter.GetBytes(data[i]), 0, bytes, i * 4, 4);
            sender?.Send(bytes, bytes.Length, sendEndpoint!);
            Console.WriteLine("Handshake triggered.");
        }

        public void TriggerEndMessage()
        {
            var data = new float[10];
            data[0] = -999.999f; // Example end value
            var bytes = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
                Array.Copy(BitConverter.GetBytes(data[i]), 0, bytes, i * 4, 4);
            sender?.Send(bytes, bytes.Length, sendEndpoint!);
            Console.WriteLine("End message triggered.");
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var result = await receiver!.ReceiveAsync();
                if (result.Buffer.Length == 43 * 4)
                {
                    var floats = new float[43];
                    for (int i = 0; i < 43; i++)
                        floats[i] = BitConverter.ToSingle(result.Buffer, i * 4);
                    Console.WriteLine($"Received 43 floats: [{string.Join(", ", floats[..Math.Min(8, floats.Length)])} ...]");
                }
                else
                {
                    Console.WriteLine($"Received {result.Buffer.Length} bytes (unexpected size)");
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
