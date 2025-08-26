using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FliteSimFfbMock
{
    public class FfbUdpSimulator : IDisposable
    {
        private const ushort FRAME_HEADER = 0xAA55;
        private const string INIT_FLAG = "INIT";
        private const string ACKN_FLAG = "ACKN";
        private const string QUIT_FLAG = "QUIT";
        private const string ACKQ_FLAG = "ACKQ";
        private const float PROTOCOL_VERSION = 1.0f;

        private readonly int receivePort;
        private readonly int sendPort;
        private readonly string targetIp;
        private UdpClient? receiver;
        private UdpClient? sender;
        private IPEndPoint? sendEndpoint;
        private CancellationTokenSource? cts;
        private Task? receiveTask;

        // Protocol state
        private bool _handshakeCompleted = false;
        private DateTime _lastHandshakeAttempt = DateTime.MinValue;
        private int _handshakeAttempts = 0;
        private const int MAX_HANDSHAKE_ATTEMPTS = 3;
        private const int HANDSHAKE_TIMEOUT_MS = 3000;

        // Control data simulation
        private float[] _controlData = new float[13]; // 13-float control data
        private float[] _lastFlightData = new float[30]; // Track changes in received flight data
        private bool _autoControlEnabled = false;
        private DateTime _startTime = DateTime.Now;

        // Control data channel labels for display
        private readonly string[] _controlChannelNames = new string[]
        {
            "Override Pitch", "Override Roll", "Override Heading",
            "Pitch Ratio", "Roll Ratio", "Heading Ratio",
            "Pitch Trim Ctrl", "Roll Trim Ctrl", "Yaw Trim Ctrl",
            "Elevator Trim", "Aileron Trim", "Rudder Trim",
            "Autopilot Disconnect"
        };

        // Flight data channel labels for display (first 10 for brevity)
        private readonly string[] _flightChannelNames = new string[]
        {
            "XPlane Version", "Flight Time", "Paused", "Mach Number", "AoA",
            "Vertical Speed", "Wind Speed", "Wind Direction", "Magnetic Heading", "Surface Type"
        };

        public bool IsConnected => _handshakeCompleted;
        public bool AutoControlEnabled => _autoControlEnabled;

        public FfbUdpSimulator(int receivePort, int sendPort, string targetIp)
        {
            this.receivePort = receivePort;
            this.sendPort = sendPort;
            this.targetIp = targetIp;
            InitializeControlData();
        }

        private void InitializeControlData()
        {
            // Set some default values
            _controlData[0] = 1.0f; // Override pitch
            _controlData[1] = 1.0f; // Override roll  
            _controlData[2] = 1.0f; // Override heading
            // Ratios start at 0.0 (neutral)
            // Trim controls and autopilot start at 0.0
        }

        public void Start()
        {
            cts = new CancellationTokenSource();
            receiver = new UdpClient(receivePort);
            sender = new UdpClient();
            sendEndpoint = new IPEndPoint(IPAddress.Parse(targetIp), sendPort);

            receiveTask = Task.Run(() => ReceiveLoop(cts.Token));
            Console.WriteLine($"FFB Simulator started - Listening on {receivePort}, sending to {targetIp}:{sendPort}");
        }

        public void Stop()
        {
            if (_handshakeCompleted)
            {
                SendQuit(0.0f); // Normal shutdown
            }
            
            _autoControlEnabled = false;
            cts?.Cancel();
            receiver?.Dispose();
            sender?.Dispose();
            receiveTask = null;
            _handshakeCompleted = false;
            Console.WriteLine("FFB Simulator stopped.");
        }

        public void InitiateHandshake()
        {
            if (_handshakeAttempts >= MAX_HANDSHAKE_ATTEMPTS)
            {
                Console.WriteLine("Maximum handshake attempts reached.");
                return;
            }

            var packet = CreateProtocolPacket(INIT_FLAG, PROTOCOL_VERSION);
            sender?.Send(packet, packet.Length, sendEndpoint!);
            _lastHandshakeAttempt = DateTime.Now;
            _handshakeAttempts++;
            
            Console.WriteLine($"Handshake attempt {_handshakeAttempts}/{MAX_HANDSHAKE_ATTEMPTS} sent.");
        }

        public void SendQuit(float reason = 0.0f)
        {
            var packet = CreateProtocolPacket(QUIT_FLAG, reason);
            sender?.Send(packet, packet.Length, sendEndpoint!);
            Console.WriteLine($"Quit message sent (reason: {reason}).");
        }

        public void SendControlData()
        {
            if (!_handshakeCompleted)
            {
                Console.WriteLine("Cannot send control data - handshake not completed.");
                return;
            }

            if (_autoControlEnabled)
            {
                UpdateAutoControl();
            }

            var bytes = PackFloats(_controlData);
            sender?.Send(bytes, bytes.Length, sendEndpoint!);
            Console.WriteLine($"Control data sent: Pitch={_controlData[3]:F2}, Roll={_controlData[4]:F2}, Yaw={_controlData[5]:F2}");
        }

        public void SetControlValue(int channel, float value)
        {
            if (channel >= 0 && channel < _controlData.Length)
            {
                _controlData[channel] = value;
                var channelName = channel < _controlChannelNames.Length ? _controlChannelNames[channel] : $"Channel {channel}";
                Console.WriteLine($"Set {channelName} = {value:F3}");
            }
            else
            {
                Console.WriteLine($"Invalid channel {channel}. Valid range: 0-{_controlData.Length - 1}");
            }
        }

        public void ToggleAutoControl()
        {
            _autoControlEnabled = !_autoControlEnabled;
            if (_autoControlEnabled)
            {
                _startTime = DateTime.Now;
                Console.WriteLine("Auto control simulation started - rolling motion enabled.");
            }
            else
            {
                Console.WriteLine("Auto control simulation stopped.");
            }
        }

        private void UpdateAutoControl()
        {
            var elapsed = (DateTime.Now - _startTime).TotalSeconds;
            
            // Simulate gentle rolling motion
            _controlData[3] = (float)(Math.Sin(elapsed * 0.5) * 0.3);  // Pitch
            _controlData[4] = (float)(Math.Sin(elapsed * 0.7) * 0.5);  // Roll  
            _controlData[5] = (float)(Math.Sin(elapsed * 0.3) * 0.2);  // Yaw
        }

        public void CheckHandshakeTimeout()
        {
            if (!_handshakeCompleted && 
                _handshakeAttempts > 0 && 
                _handshakeAttempts < MAX_HANDSHAKE_ATTEMPTS &&
                DateTime.Now.Subtract(_lastHandshakeAttempt).TotalMilliseconds > HANDSHAKE_TIMEOUT_MS)
            {
                Console.WriteLine("Handshake timeout, retrying...");
                InitiateHandshake();
            }
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = await receiver!.ReceiveAsync();
                    ProcessReceivedData(result.Buffer);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving data: {ex.Message}");
                }
            }
        }

        private void ProcessReceivedData(byte[] buffer)
        {
            // Check for protocol messages (handshake/quit)
            if (buffer.Length >= 10 && BitConverter.ToUInt16(buffer, 0) == FRAME_HEADER)
            {
                HandleProtocolMessage(buffer);
            }
            // Check for flight data from MOBI (30 floats = 120 bytes)
            else if (buffer.Length == 30 * 4)
            {
                if (!_handshakeCompleted)
                {
                    Console.WriteLine("Received flight data but handshake not completed.");
                    return;
                }

                var flightData = ParseFloats(buffer);
                DisplayFlightDataChanges(flightData);
            }
            else
            {
                Console.WriteLine($"Received unexpected data size: {buffer.Length} bytes");
            }
        }

        private void HandleProtocolMessage(byte[] buffer)
        {
            var flag = Encoding.ASCII.GetString(buffer, 2, 4);
            var value = BitConverter.ToSingle(buffer, 6);

            switch (flag)
            {
                case INIT_FLAG:
                    Console.WriteLine($"Received handshake request (version: {value})");
                    SendHandshakeAck();
                    break;

                case ACKN_FLAG:
                    Console.WriteLine($"Received handshake acknowledgment (version: {value})");
                    _handshakeCompleted = true;
                    break;

                case QUIT_FLAG:
                    Console.WriteLine($"Received quit request (reason: {value})");
                    SendQuitAck(value);
                    break;

                case ACKQ_FLAG:
                    Console.WriteLine($"Received quit acknowledgment (reason: {value})");
                    break;

                default:
                    Console.WriteLine($"Unknown protocol flag: {flag}");
                    break;
            }
        }

        private void SendHandshakeAck()
        {
            var packet = CreateProtocolPacket(ACKN_FLAG, PROTOCOL_VERSION);
            sender?.Send(packet, packet.Length, sendEndpoint!);
            _handshakeCompleted = true;
            Console.WriteLine("Handshake completed successfully.");
        }

        private void SendQuitAck(float reason)
        {
            var packet = CreateProtocolPacket(ACKQ_FLAG, reason);
            sender?.Send(packet, packet.Length, sendEndpoint!);
            Console.WriteLine($"Quit acknowledgment sent (reason: {reason}).");
        }

        private void DisplayFlightDataChanges(float[] newFlightData)
        {
            bool hasChanges = false;
            var changes = new List<string>();

            for (int i = 0; i < Math.Min(newFlightData.Length, _lastFlightData.Length); i++)
            {
                if (Math.Abs(newFlightData[i] - _lastFlightData[i]) > 0.001f)
                {
                    var channelName = i < _flightChannelNames.Length ? _flightChannelNames[i] : $"Ch{i}";
                    changes.Add($"{channelName}={newFlightData[i]:F3}");
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                Console.WriteLine($"Flight data changes: {string.Join(", ", changes)}");
                Array.Copy(newFlightData, _lastFlightData, Math.Min(newFlightData.Length, _lastFlightData.Length));
            }
        }

        private byte[] CreateProtocolPacket(string flag, float value)
        {
            var packet = new byte[10]; // 2 + 4 + 4 bytes
            var headerBytes = BitConverter.GetBytes(FRAME_HEADER);
            var flagBytes = Encoding.ASCII.GetBytes(flag);
            var valueBytes = BitConverter.GetBytes(value);

            Array.Copy(headerBytes, 0, packet, 0, 2);
            Array.Copy(flagBytes, 0, packet, 2, 4);
            Array.Copy(valueBytes, 0, packet, 6, 4);
            return packet;
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
            Stop();
        }
    }
}
