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
using System.ComponentModel;
using System.IO.Ports;
using System.Reflection;
using System.Linq;
using System.Threading;

namespace CommandMessenger.TransportLayer
{
    public enum ThreadRunStates
    {
        Start,
        Stop,
    }

    /// <summary>Fas
    /// Manager for serial port data
    /// </summary>
    public class SerialTransport : DisposableObject, ITransport
    {
        private readonly QueueSpeed _queueSpeed = new QueueSpeed(4);
        private Thread _queueThread;

        public ThreadRunStates ThreadRunState = ThreadRunStates.Start;

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
            _currentSerialSettings.PropertyChanged += CurrentSerialSettingsPropertyChanged;

            // If serial ports are found, we select the first one
            if (_currentSerialSettings.PortNameCollection.Length > 0)
                _currentSerialSettings.PortName = _currentSerialSettings.PortNameCollection[0];

            // Create queue thread and wait for it to start
            _queueThread = new Thread(ProcessQueue) { Priority = ThreadPriority.Normal };
            _queueThread.Name = "Serial";
            _queueThread.Start();
            while (!_queueThread.IsAlive) { }
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

        #region Event handlers

        /// <summary> Current serial settings property changed. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Property changed event information. </param>
        private void CurrentSerialSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // if serial port is changed, a new baud query is issued
            if (e.PropertyName.Equals("PortName"))
                UpdateBaudRateCollection();
        }

        protected  void ProcessQueue()
        {
            // Endless loop
            while (ThreadRunState == ThreadRunStates.Start)
            {
                var bytes = BytesInBuffer();
                    _queueSpeed.SetCount(bytes);
                    _queueSpeed.CalcSleepTimeWithoutLoad();
                    _queueSpeed.Sleep();
                if (bytes > 0)
                {                    
                    if (NewDataReceived != null) NewDataReceived(this, null);
                }                    
            }
            _queueSpeed.Sleep(50);
        }

        #endregion

        #region Methods

        /// <summary> Connects to a serial port defined through the current settings. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool StartListening()
        {
            // Closing serial port if it is open

            if (IsOpen()) Close();

            // Setting serial port settings
            _serialPort = new SerialPort(
                _currentSerialSettings.PortName,
                _currentSerialSettings.BaudRate,
                _currentSerialSettings.Parity,
                _currentSerialSettings.DataBits,
                _currentSerialSettings.StopBits);
            _serialPort.DtrEnable = _currentSerialSettings.DataTerminalReady;
            // Subscribe to event and open serial port for data
            ThreadRunState = ThreadRunStates.Start;
            return Open();
        }

        /// <summary> Opens the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Open()
        {
            try
            {
                if (SerialPort != null && PortExists())
                {
                    _serialPort.Open();
                    return _serialPort.IsOpen;
                }
            }
            catch
            {
                return false;
            }
            return false;
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
                if (SerialPort != null && PortExists())
                {
                    _serialPort.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return true;
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
        public bool StopListening()
        {
            ThreadRunState = ThreadRunStates.Start;
            //_pollBuffer.StopAndWait();
            return Close();
        }

        /// <summary> Writes a parameter to the serial port. </summary>
        /// <param name="buffer"> The buffer to write. </param>
        public void Write(byte[] buffer)
        {
            try
            {
                //if (IsOpen())
                //{
                    _serialPort.Write(buffer, 0, buffer.Length);
                //}
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

        /// <summary> Reads the serial buffer into the string buffer. </summary>
        public byte[] Read()
        {
            var buffer = new byte[0];
            //if (IsOpen())
            {
                try
                {
                    var dataLength = _serialPort.BytesToRead;
                    buffer = new byte[dataLength];
                    int nbrDataRead = _serialPort.Read(buffer, 0, dataLength);
                    if (nbrDataRead == 0) return new byte[0];
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
            try
            {                
                return _serialPort.BytesToRead;
            }
            catch (Exception)
            {
                return 0;
            }
            //return IsOpen()? _serialPort.BytesToRead:0;
        }

        // Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _queueThread.Abort();
                _queueThread.Join();
                _currentSerialSettings.PropertyChanged -= CurrentSerialSettingsPropertyChanged;
                // Releasing serial port 
                if (IsOpen()) Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}