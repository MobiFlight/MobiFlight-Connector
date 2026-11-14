#region CmdMessenger - MIT - (c) 2013 Thijs Elenbaas.
/*
  CmdMessenger - library that provides command based messaging

  Permission is hereby granted, free of charge, to any person obtaining
  a copy of this software and associated documentation files (the
  "Software"), to deal in the Software without restriction, including
  without limitation the rights to use, copy, modify, merge, publish,
  distribute, sublicense, and/or sell copies of the Software, and to
  permit persons to whom the Software is furnished to do so, subject to
  the following conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  Copyright 2013 - Thijs Elenbaas
*/
#endregion

using CommandMessenger;
using CommandMessenger.Transport;
using CommandMessenger.Transport.Serial;
using System;
using System.IO.Ports;

namespace MobiFlight.Transport
{
    public enum Status
    {
        New,
        Connected,
        Disconnected,
        Timeout,
        PortBusy,
        Failed,
        PortNotFound
    }

    /// <summary>Fas
    /// Manager for serial port data
    /// </summary>
    public class SerialTransport : ITransport
    {
        private const int BufferSize = 4096;
        private const int ReadTimeout = 1000;

        private readonly AsyncWorker _worker;
        private readonly object _serialReadWriteLock = new object();
        private readonly object _readLock = new object();
        private readonly byte[] _readBuffer = new byte[BufferSize];
        private int _bufferFilled;

        private SerialPort _serialPort;

        private SerialSettings _currentSerialSettings = new SerialSettings();

        // Event queue for all listeners interested in NewLinesReceived events.
        public event EventHandler DataReceived;

        // Raised when opening or writing to the port throws, e.g. when another application holds it.
        public event EventHandler<Tuple<string, Exception>> ConnectionFailed;

        /// <summary> Gets or sets the current serial port settings. </summary>
        /// <value> The current serial settings. </value>
        public SerialSettings CurrentSerialSettings
        {
            get { return _currentSerialSettings; }
            set { _currentSerialSettings = value; }
        }

        private volatile bool _connected;
        private volatile Status _status = Status.New;
        public Status Status { get { return _status; } private set { _status = value; } }

        public SerialTransport()
        {
            _worker = new AsyncWorker(Poll, "SerialTransport");
        }

        private bool Poll()
        {
            var bytes = UpdateBuffer();
            if (bytes > 0 && DataReceived != null) DataReceived(this, EventArgs.Empty);

            // Return true as we always have work to do here. The delay is achieved by SerialPort.Read timeout.
            return true;
        }

        /// <summary> Connects to a serial port defined through the current settings. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Connect()
        {
            if (!_currentSerialSettings.IsValid())
                throw new InvalidOperationException("Unable to open connection - serial settings invalid.");

            if (IsConnected())
                throw new InvalidOperationException("Serial port is already opened.");

            // Setting serial port settings
            _serialPort = new SerialPort(
                _currentSerialSettings.PortName,
                _currentSerialSettings.BaudRate,
                _currentSerialSettings.Parity,
                _currentSerialSettings.DataBits,
                _currentSerialSettings.StopBits)
            {
                DtrEnable = _currentSerialSettings.DtrEnable,
                WriteTimeout = _currentSerialSettings.Timeout,
                ReadTimeout = ReadTimeout,
            };

            _connected = Open();
            if (_connected) _worker.Start();

            return _connected;
        }

        /// <summary> Query if the serial port is open. </summary>
        /// <returns> true if open, false if not. </returns>
        public bool IsConnected()
        {
            return _connected && _serialPort.IsOpen;
        }

        /// <summary> Stops listening to the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Disconnect()
        {
            bool result = Close();
            if (_connected)
            {
                _connected = false;
                _worker.Stop();
            }
            return result;
        }

        /// <summary> Writes a parameter to the serial port. </summary>
        /// <param name="buffer"> The buffer to write. </param>
        public void Write(byte[] buffer)
        {
            if (!IsConnected()) return;

            try
            {
                lock (_serialReadWriteLock)
                {
                    _serialPort.Write(buffer, 0, buffer.Length);
                }
            }
            catch (TimeoutException ex)
            {
                Status = Status.Timeout;
                ConnectionFailed?.Invoke(
                    this,
                    Tuple.Create<string, Exception>(
                        $"{_serialPort.PortName}: Timeout when writing to serial port",
                        ex
                    )
                );
            }
            catch (Exception e)
            {
                Disconnect();
                Status = Status.Disconnected;
                ConnectionFailed?.Invoke(
                    this,
                    Tuple.Create<string, Exception>(
                        $"{_serialPort.PortName}: Serial port disconnected",
                        e
                    )
                );
            }
        }

        /// <summary> Reads the serial buffer into the string buffer. </summary>
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

        /// <summary> Opens the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private bool Open()
        {
            if (!PortExists())
            {
                Status = Status.PortNotFound;
                ConnectionFailed?.Invoke(this, Tuple.Create<string, Exception>(
                    $"{_serialPort.PortName}: Serial port not found",
                    null
                ));

                return false;
            }

            if (!_connected && !_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();

                    if (_serialPort.IsOpen)
                    {
                        Status = Status.Connected;
                        return true;
                    }

                    Status = Status.Disconnected;
                    return _serialPort.IsOpen;
                }
                catch (UnauthorizedAccessException uaEx)
                {
                    Status = Status.PortBusy;
                    ConnectionFailed?.Invoke(this, Tuple.Create<string, Exception>(
                        $"{_serialPort.PortName}: Access denied to serial port",
                        uaEx
                    ));
                }
                catch (Exception ex)
                {
                    Status = Status.Failed;
                    ConnectionFailed?.Invoke(this, 
                        new Tuple<string, Exception>(
                            $"{_serialPort.PortName}: Failed to open serial port",
                            ex
                        )
                    );
                }
            }

            return false;
        }

        /// <summary> Closes the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private bool Close()
        {
            if (!IsConnected()) return false;

            try
            {
                _serialPort.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Queries if a current port exists. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        private bool PortExists()
        {
            return SerialUtils.PortExists(_serialPort.PortName);
        }

        private int UpdateBuffer()
        {
            if (IsConnected())
            {
                try
                {
                    lock (_readLock)
                    {
                        var nbrDataRead = _serialPort.Read(_readBuffer, _bufferFilled, (BufferSize - _bufferFilled));
                        _bufferFilled += nbrDataRead;
                    }
                    return _bufferFilled;
                }
                catch (TimeoutException)
                {
                    // Timeout (expected)
                }
                catch
                {
                    Disconnect();
                }
            }

            return 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
                _serialPort?.Dispose();
            }
        }
    }
}