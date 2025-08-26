using System;
using System.Text;

namespace MobiFlight.Joysticks.FliteSim
{
    /// <summary>
    /// Handles the FliteSim FFB UDP protocol including handshake, data conversion, and connection management
    /// </summary>
    internal class FliteSimProtocol : IDisposable
    {
        private const ushort FRAME_HEADER = 0xAA55;
        private const string INIT_FLAG = "INIT";
        private const string ACKN_FLAG = "ACKN";
        private const string QUIT_FLAG = "QUIT";
        private const string ACKQ_FLAG = "ACKQ";
        private const float PROTOCOL_VERSION = 1.0f;

        private readonly UdpInterface _udpInterface;
        private bool _handshakeCompleted = false;
        private DateTime _lastHandshakeAttempt = DateTime.MinValue;
        private int _handshakeAttempts = 0;
        private const int MAX_HANDSHAKE_ATTEMPTS = 3;
        private const int HANDSHAKE_TIMEOUT_MS = 3000;

        // Events
        public event Action HandshakeCompleted;
        public event Action<float[]> ControlDataReceived; // 13 floats from FFB
        public event Action ConnectionLost;

        public bool IsConnected => _handshakeCompleted;

        public FliteSimProtocol(UdpSettings settings)
        {
            _udpInterface = new UdpInterface(settings);
            _udpInterface.DataReceived += OnRawDataReceived;
        }

        public void StartListening()
        {
            _udpInterface.StartListening();
        }

        public void StopListening()
        {
            _udpInterface.StopListening();
        }

        /// <summary>
        /// Initiate handshake with FFB device
        /// </summary>
        public void InitiateHandshake()
        {
            if (_handshakeAttempts >= MAX_HANDSHAKE_ATTEMPTS)
            {
                Log.Instance.log("FliteSimProtocol: Maximum handshake attempts reached", LogSeverity.Error);
                return;
            }

            var packet = CreateProtocolPacket(INIT_FLAG, PROTOCOL_VERSION);
            _udpInterface.Send(packet);
            _lastHandshakeAttempt = DateTime.Now;
            _handshakeAttempts++;
            
            Log.Instance.log($"FliteSimProtocol: Handshake attempt {_handshakeAttempts}/{MAX_HANDSHAKE_ATTEMPTS}", LogSeverity.Info);
        }

        /// <summary>
        /// Send acknowledgment to FFB handshake
        /// </summary>
        public void SendHandshakeAck()
        {
            var packet = CreateProtocolPacket(ACKN_FLAG, PROTOCOL_VERSION);
            _udpInterface.Send(packet);
            _handshakeCompleted = true;
            HandshakeCompleted?.Invoke();
            Log.Instance.log("FliteSimProtocol: Handshake completed", LogSeverity.Info);
        }

        /// <summary>
        /// Send flight simulation data (30 floats) to FFB device
        /// </summary>
        /// <param name="data">30-element float array with flight data</param>
        public void SendFlightData(float[] data)
        {
            if (!_handshakeCompleted)
            {
                Log.Instance.log("FliteSimProtocol: Cannot send flight data - handshake not completed", LogSeverity.Warn);
                return;
            }

            if (data.Length != 30)
            {
                Log.Instance.log($"FliteSimProtocol: Invalid flight data size - expected 30, got {data.Length}", LogSeverity.Error);
                return;
            }

            var bytes = PackFloats(data);
            _udpInterface.Send(bytes);
        }

        /// <summary>
        /// Send quit message to FFB device
        /// </summary>
        /// <param name="reason">Quit reason (0=normal, 1=user abort, 2=error)</param>
        public void SendQuit(float reason = 0.0f)
        {
            var packet = CreateProtocolPacket(QUIT_FLAG, reason);
            _udpInterface.Send(packet);
            Log.Instance.log($"FliteSimProtocol: Quit message sent (reason: {reason})", LogSeverity.Info);
        }

        /// <summary>
        /// Check if handshake has timed out and retry if needed
        /// </summary>
        public void CheckHandshakeTimeout()
        {
            if (!_handshakeCompleted && 
                _handshakeAttempts > 0 && 
                _handshakeAttempts < MAX_HANDSHAKE_ATTEMPTS &&
                DateTime.Now.Subtract(_lastHandshakeAttempt).TotalMilliseconds > HANDSHAKE_TIMEOUT_MS)
            {
                Log.Instance.log("FliteSimProtocol: Handshake timeout, retrying...", LogSeverity.Warn);
                InitiateHandshake();
            }
        }

        private void OnRawDataReceived(byte[] buffer)
        {
            // Check for protocol messages (handshake/quit)
            if (buffer.Length >= 10 && BitConverter.ToUInt16(buffer, 0) == FRAME_HEADER)
            {
                HandleProtocolMessage(buffer);
            }
            // Check for control data from FFB (13 floats = 52 bytes)
            else if (buffer.Length == 13 * 4)
            {
                if (!_handshakeCompleted)
                {
                    Log.Instance.log("FliteSimProtocol: Received control data but handshake not completed", LogSeverity.Warn);
                    return;
                }

                var floats = ParseFloats(buffer);
                ControlDataReceived?.Invoke(floats);
            }
            else
            {
                Log.Instance.log($"FliteSimProtocol: Received unexpected data size: {buffer.Length} bytes", LogSeverity.Debug);
            }
        }

        private void HandleProtocolMessage(byte[] buffer)
        {
            var flag = Encoding.ASCII.GetString(buffer, 2, 4);
            var value = BitConverter.ToSingle(buffer, 6);

            switch (flag)
            {
                case INIT_FLAG:
                    Log.Instance.log($"FliteSimProtocol: Received handshake request (version: {value})", LogSeverity.Info);
                    SendHandshakeAck();
                    break;

                case ACKN_FLAG:
                    Log.Instance.log($"FliteSimProtocol: Received handshake acknowledgment (version: {value})", LogSeverity.Info);
                    _handshakeCompleted = true;
                    HandshakeCompleted?.Invoke();
                    break;

                case QUIT_FLAG:
                    Log.Instance.log($"FliteSimProtocol: Received quit request (reason: {value})", LogSeverity.Info);
                    SendQuitAck(value);
                    ConnectionLost?.Invoke();
                    break;

                case ACKQ_FLAG:
                    Log.Instance.log($"FliteSimProtocol: Received quit acknowledgment (reason: {value})", LogSeverity.Info);
                    ConnectionLost?.Invoke();
                    break;

                default:
                    Log.Instance.log($"FliteSimProtocol: Unknown protocol flag: {flag}", LogSeverity.Warn);
                    break;
            }
        }

        private void SendQuitAck(float reason)
        {
            var packet = CreateProtocolPacket(ACKQ_FLAG, reason);
            _udpInterface.Send(packet);
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
            if (_handshakeCompleted)
            {
                SendQuit(0.0f); // Normal shutdown
            }
            StopListening();
            _udpInterface?.Dispose();
        }
    }
}