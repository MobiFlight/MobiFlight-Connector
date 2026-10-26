using MobiFlightMoza.Session;
using System;
using System.IO;
using System.IO.Ports;

namespace MobiFlight.Joysticks.Moza
{
    /// <summary>
    /// A MOZA device's CDC serial port: opened lazily, reopened with a throttle after a failure,
    /// and never rewrites a frame identical to the last one it sent.
    /// </summary>
    internal sealed class MozaCdcPort : IDisposable
    {
        private const int BaudRate = 2_000_000;

        private readonly string DeviceName;
        private readonly Func<Stream> OpenStream;
        private readonly TimeSpan RetryInterval;
        private SerialPort Port;
        private Stream Stream;
        private byte[] LastFrame;
        private DateTime NextAttempt = DateTime.MinValue;
        private bool OpenFailureLogged;

        public bool IsOpen => Stream is not null;

        public MozaCdcPort(string deviceName, int vendorId, int productId)
        {
            DeviceName = deviceName;
            RetryInterval = TimeSpan.FromSeconds(2);
            OpenStream = () => OpenSerialPort(vendorId, productId);
        }

        // Testing seam: lets tests substitute the stream and the retry throttle.
        internal MozaCdcPort(string deviceName, Func<Stream> openStream, TimeSpan retryInterval)
        {
            DeviceName = deviceName;
            OpenStream = openStream;
            RetryInterval = retryInterval;
        }

        /// <summary>Returns false if the frame could not be sent (port missing or write failed); try again later.</summary>
        public bool TryWrite(byte[] frame)
        {
            if (!TryOpen()) return false;
            if (frame.AsSpan().SequenceEqual(LastFrame)) return true;

            try
            {
                Stream.Write(frame, 0, frame.Length);
                LastFrame = frame;
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
            {
                Log.Instance.log($"{DeviceName} - Failed to write to CDC port: {ex.Message}", LogSeverity.Error);
                Dispose();
                NextAttempt = DateTime.Now + RetryInterval;
                return false;
            }
        }

        public void Dispose()
        {
            if (Port is not null) Port.Dispose();
            else Stream?.Dispose();

            Port = null;
            Stream = null;
            LastFrame = null;
        }

        private bool TryOpen()
        {
            if (IsOpen) return true;
            if (DateTime.Now < NextAttempt) return false;

            Stream = OpenStream();
            if (IsOpen) return true;

            Log.Instance.log($"{DeviceName} - CDC port not found yet, will retry.", LogSeverity.Debug);
            NextAttempt = DateTime.Now + RetryInterval;
            return false;
        }

        private Stream OpenSerialPort(int vendorId, int productId)
        {
            var ports = MozaPortLocator.FindPorts(vendorId, [productId]);
            if (ports.Count == 0) return null;

            var port = new SerialPort(ports[0].PortName, BaudRate) { DtrEnable = true };
            try
            {
                port.Open();
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                // Typically held by MOZA's own software; logged once, retried quietly.
                if (!OpenFailureLogged)
                {
                    OpenFailureLogged = true;
                    Log.Instance.log($"{DeviceName} - Could not open {port.PortName}: {ex.Message}", LogSeverity.Error);
                }
                port.Dispose();
                return null;
            }

            OpenFailureLogged = false;
            Port = port;
            return port.BaseStream;
        }
    }
}