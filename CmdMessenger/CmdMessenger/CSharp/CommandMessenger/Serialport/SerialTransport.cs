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

using System;
using System.IO.Ports;
using System.Reflection;
using System.Linq;
using System.Threading;
using CommandMessenger.TransportLayer;

namespace CommandMessenger.Serialport
{
    public enum ThreadRunStates
    {
        Start,
        Stop,
        Abort,
    }

    /// <summary>Fas
    /// Manager for serial port data
    /// </summary>
    public class SerialTransport : DisposableObject, ITransport
    {
        private readonly QueueSpeed _queueSpeed = new QueueSpeed(4,10);
        private Thread _queueThread;
        private ThreadRunStates _threadRunState;
        private readonly object _threadRunStateLock = new object();
        private readonly object _serialReadWriteLock = new object();

        /// <summary> Gets or sets the run state of the thread . </summary>
        /// <value> The thread run state. </value>
        public ThreadRunStates ThreadRunState  
        {
            set
            {
                lock (_threadRunStateLock)
                {
                    _threadRunState = value;
                }
            }
            get
            {
                ThreadRunStates result;
                lock (_threadRunStateLock)
                {
                    result = _threadRunState;
                }
                return result;
            }
        }

        /// <summary> Default constructor. </summary>
        public SerialTransport()
        {          
            Initialize();
        }

        /// <summary> Initializes this object. </summary>
        public void Initialize()
        {            
           // _queueSpeed.Name = "Serial";
            // Find installed serial ports on hardware
            _currentSerialSettings.PortNameCollection = SerialPort.GetPortNames();         

            // If serial ports are found, we select the first one
            if (_currentSerialSettings.PortNameCollection.Length > 0)
                _currentSerialSettings.PortName = _currentSerialSettings.PortNameCollection[0];

            // Create queue thread and wait for it to start
            
            _queueThread = new Thread(ProcessQueue)
                {
                    Priority = ThreadPriority.Normal, 
                    Name = "Serial"
                };
            ThreadRunState = ThreadRunStates.Start;
            _queueThread.Start();
            while (!_queueThread.IsAlive) { Thread.Sleep(50); }
        }

        #region Fields

        private SerialPort _serialPort;                                         // The serial port
        private SerialSettings _currentSerialSettings = new SerialSettings();   // The current serial settings
        public event EventHandler NewDataReceived;                              // Event queue for all listeners interested in NewLinesReceived events.

        #endregion

        #region Properties

        /// <summary> Gets or sets the current serial port settings. </summary>
        /// <value> The current serial settings. </value>
        public SerialSettings CurrentSerialSettings
        {
            get { return _currentSerialSettings; }
            set { _currentSerialSettings = value; }
        }

        /// <summary> Gets the serial port. </summary>
        /// <value> The serial port. </value>
        public SerialPort SerialPort
        {
            get { return _serialPort; }
        }

        #endregion

        #region Methods

        protected  void ProcessQueue()
        {
            // Endless loop
            while (ThreadRunState != ThreadRunStates.Abort)
            {
                Poll(ThreadRunState);
            }
            _queueSpeed.Sleep(50);
        }        

        public void StartPolling()
        {
            ThreadRunState = ThreadRunStates.Start;
        }

        public void StopPolling()
        {
            ThreadRunState = ThreadRunStates.Stop;
        }

        private void Poll(ThreadRunStates threadRunState)
        {
            var bytes = BytesInBuffer();
            _queueSpeed.SetCount(bytes);
            _queueSpeed.CalcSleepTimeWithoutLoad();
            _queueSpeed.Sleep();
            if (threadRunState == ThreadRunStates.Start)
            {
                if (bytes > 0)
                {
                    if (NewDataReceived != null) NewDataReceived(this, null);
                }
            }
        }

        public void Poll()
        {
            Poll(ThreadRunStates.Start);
        }

        /// <summary> Connects to a serial port defined through the current settings. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Connect()
        {
            // Closing serial port if it is open

            //if (IsOpen()) Close();
            Close();

            // Setting serial port settings
            _serialPort = new SerialPort(
                _currentSerialSettings.PortName,
                _currentSerialSettings.BaudRate,
                _currentSerialSettings.Parity,
                _currentSerialSettings.DataBits,
                _currentSerialSettings.StopBits)
                {
                    DtrEnable = _currentSerialSettings.DtrEnable,
                    WriteTimeout = 1000
                };


            // Subscribe to event and open serial port for data
            ThreadRunState = ThreadRunStates.Start;
            return Open();
        }

        /// <summary> Opens the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Open()
        {            
            if(_serialPort != null && PortExists() && !_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    return _serialPort.IsOpen;
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsConnected()
        {
            return IsOpen();
        }

        /// <summary> Queries if a given port exists. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool PortExists()
        {
            return SerialPort.GetPortNames().Contains(_serialPort.PortName);
        }

        /// <summary> Closes the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Close()
        {
            try
            {
                if (SerialPort == null || !PortExists()) return false;
                if (!_serialPort.IsOpen) return true;
                _serialPort.Close();
                return true;
            }
            catch
            {
                return false;
            }            
        }

        /// <summary> Query ifthe serial port is open. </summary>
        /// <returns> true if open, false if not. </returns>
        public bool IsOpen()
        {
            try
            {
                return _serialPort != null && PortExists() && _serialPort.IsOpen;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Stops listening to the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Disconnect()
        {
            ThreadRunState = ThreadRunStates.Stop;
            var state = Close();
            return state;
        }

        /// <summary> Writes a parameter to the serial port. </summary>
        /// <param name="buffer"> The buffer to write. </param>
        public void Write(byte[] buffer)
        {
            try
            {
                if (IsOpen())
                {
                    lock (_serialReadWriteLock)
                    {
                        _serialPort.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary> Retrieves the possible baud rates for the currently selected serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool UpdateBaudRateCollection()
        {
            try
            {
                _currentSerialSettings.UpdateBaudRateCollection(0);
                Close();
                _serialPort = new SerialPort(_currentSerialSettings.PortName);
                if (Open())
                {
                    var fieldInfo = _serialPort.BaseStream.GetType()
                                               .GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fieldInfo != null)
                    {
                        object p = fieldInfo.GetValue(_serialPort.BaseStream);
                        var fieldInfoValue = p.GetType()
                                              .GetField("dwSettableBaud",
                                                        BindingFlags.Instance | BindingFlags.NonPublic |
                                                        BindingFlags.Public);
                        if (fieldInfoValue != null)
                        {
                            var dwSettableBaud = (Int32) fieldInfoValue.GetValue(p);
                            Close();
                            _currentSerialSettings.UpdateBaudRateCollection(dwSettableBaud);
                        }
                    }
                }
            }
            catch
            {                
                return false;
            }
            return true;
        }


        public void UpdatePortCollection()
        {
            _currentSerialSettings.PortNameCollection = SerialPort.GetPortNames();  
        }

        /// <summary> Reads the serial buffer into the string buffer. </summary>
        public byte[] Read()
        {
            var buffer = new byte[0];
            if (IsOpen())
            {
                try
                {
                    lock (_serialReadWriteLock)
                    {
                        var dataLength = _serialPort.BytesToRead;
                        buffer = new byte[dataLength];
                        int nbrDataRead = _serialPort.Read(buffer, 0, dataLength);
                        if (nbrDataRead == 0) return new byte[0];
                    }
                }
                catch
                { }
            }
            return buffer;
        }

        /// <summary> Gets the bytes in buffer. </summary>
        /// <returns> Bytes in buffer </returns>
        public int BytesInBuffer()
        {
            return IsOpen()? _serialPort.BytesToRead:0;
        }

        /// <summary> Kills this object. </summary>
        public void Kill()
        {
            // Signal thread to stop
            ThreadRunState = ThreadRunStates.Abort;

            //Wait for thread to die
            Join(1200);
            if (_queueThread.IsAlive) _queueThread.Abort();

            // Releasing serial port 
            if (IsOpen()) Close();
            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;
            }

        }

        /// <summary> Joins the thread. </summary>
        /// <param name="millisecondsTimeout"> The milliseconds timeout. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Join(int millisecondsTimeout)
        {
            if (_queueThread.IsAlive == false) return true;
            return _queueThread.Join(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        // Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Kill();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}