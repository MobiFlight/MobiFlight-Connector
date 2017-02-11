using System;
using System.Net.Sockets;

namespace CommandMessenger.Transport.Network
{
    /// <summary>
    /// Used with network devices.
    /// Example: socat tcp-l:1234,reuseaddr,fork file:/dev/ttyACM0,nonblock,raw,echo=0,waitlock=/var/run/tty,b115200
    /// </summary>
    public class TcpTransport : ITransport
    {
        private const int BufferSize = 4096;

        private readonly object _serialReadWriteLock = new object();
        private readonly object _readLock = new object();
        private readonly byte[] _readBuffer = new byte[BufferSize];
        private int _bufferFilled;
        private int _timeout;

        private TcpClient _tcpClient;
        public event EventHandler DataReceived;

        public string Host { get; private set; }
        public int Port { get; private set; }

        public int Timeout
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                if (_tcpClient != null)
                {
                    _tcpClient.ReceiveTimeout = _tcpClient.SendTimeout = _timeout;
                }
            }
        }

        public TcpTransport(string host, int port)
        {
            Timeout = 1000;

            Host = host;
            Port = port;
        }

        public bool Connect()
        {
            if (IsConnected())
                throw new InvalidOperationException("Already connected.");

            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.ReceiveTimeout = _tcpClient.SendTimeout = _timeout;
                _tcpClient.Connect(Host, Port);
            }
            catch (SocketException)
            {
                return false;
            }

            _tcpClient.Client.BeginReceive(_readBuffer, 0, BufferSize, SocketFlags.None, OnBytesReceived, this);

            return true;
        }

        public bool IsConnected()
        {
            return _tcpClient != null && _tcpClient.Client != null && _tcpClient.Connected;
        }

        public bool Disconnect()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }

            return true;
        }

        public void Write(byte[] buffer)
        {
            if (IsConnected())
            {
                lock (_serialReadWriteLock)
                {
                    _tcpClient.Client.Send(buffer);
                }
            }
        }

        public byte[] Read()
        {
            if (IsConnected())
            {
                byte[] buffer;
                lock (_readLock)
                {
                    buffer = new byte[_bufferFilled];
                    Array.Copy(_readBuffer, buffer, _bufferFilled);
                    _bufferFilled = 0;
                }
                return buffer;
            }

            return new byte[0];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnBytesReceived(IAsyncResult result)
        {
            if (!IsConnected()) return;

            lock (_readLock)
            {
                int nbrDataRead = _tcpClient.Client.EndReceive(result);
                if (nbrDataRead <= 0)
                {
                    Disconnect();
                    return;
                }

                _bufferFilled += nbrDataRead;

                if (nbrDataRead > 0) DataReceived(this, EventArgs.Empty);

                _tcpClient.Client.BeginReceive(_readBuffer, 0, BufferSize, SocketFlags.None, OnBytesReceived, this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }
    }
}
