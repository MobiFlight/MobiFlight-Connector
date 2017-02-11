using System;

namespace CommandMessenger.Transport.Network
{
    public class TcpConnectionManager : ConnectionManager
    {
        private readonly TcpTransport _transport;
        private readonly object _tryConnectionLock = new object();

        public TcpConnectionManager(TcpTransport tcpTransport, CmdMessenger cmdMessenger, int identifyCommandId = 0, string uniqueDeviceId = null) 
            : base(cmdMessenger, identifyCommandId, uniqueDeviceId)
        {
            DeviceScanEnabled = false;

            if (tcpTransport == null)
                throw new ArgumentNullException("tcpTransport", "Transport is null.");

            _transport = tcpTransport;

            ReadSettings();
        }

        protected override void DoWorkConnect()
        {
            lock (_tryConnectionLock)
            {
                Connected = false;

                if (_transport.Connect())
                {
                    int optimalTimeout = _transport.Timeout + 250;
                    DeviceStatus status = ArduinoAvailable(optimalTimeout);

                    Connected = (status == DeviceStatus.Available);

                    if (Connected)
                    {
                        Log(1, string.Format("Connected to {0}:{1}.", _transport.Host, _transport.Port));
                        StoreSettings();

                        ConnectionFoundEvent();
                    }
                    else
                    {
                        _transport.Disconnect();
                    }
                }
            }
        }

        protected override void DoWorkScan()
        {
            throw new NotSupportedException("ScanMode not supported by TcpConnectionManager.");
        }
    }
}
