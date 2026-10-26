using MobiFlightMoza.Session;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace MobiFlight.Joysticks.Moza
{
    /// <summary>
    /// A MOZA device's CDC serial port. Thread-safe, since outputs are updated from both
    /// the joystick poll timer and the config executor.
    /// </summary>
    internal sealed class MozaCdcPort : IDisposable
    {
        private const int BaudRate = 2_000_000;
        private const int WriteTimeoutMilliseconds = 500;

        private readonly Lock Gate = new();
        private readonly Stream Stream;
        private readonly SerialPort Port;
        private byte[] LastFrame;
        private bool Disposed;

        internal MozaCdcPort(Stream stream, SerialPort port = null)
        {
            Stream = stream;
            Port = port;
        }

        /// <summary>Returns null (and logs why) if the port can't be opened.</summary>
        public static MozaCdcPort Open(string deviceName, int vendorId, int productId)
        {
            var ports = MozaPortLocator.FindPorts(vendorId, [productId]);
            if (ports.Count == 0)
            {
                Log.Instance.log($"{deviceName} - No CDC port found, backlight control is not available.", LogSeverity.Error);
                return null;
            }

            var port = new SerialPort(ports[0].PortName, BaudRate) { DtrEnable = true, WriteTimeout = WriteTimeoutMilliseconds };
            try
            {
                port.Open();
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                Log.Instance.log($"{deviceName} - Could not open {port.PortName}: {ex.Message}. Please close MOZA Cockpit to control the backlight from MobiFlight.", LogSeverity.Error);
                port.Dispose();
                return null;
            }

            return new MozaCdcPort(port.BaseStream, port);
        }

        public void Write(byte[] frame)
        {
            lock (Gate)
            {
                if (Disposed || frame.AsSpan().SequenceEqual(LastFrame)) return;

                Stream.Write(frame, 0, frame.Length);
                LastFrame = frame;
            }
        }

        public void Dispose()
        {
            lock (Gate)
            {
                if (Disposed) return;
                Disposed = true;

                if (Port is not null) Port.Dispose();
                else Stream.Dispose();
            }
        }
    }
}