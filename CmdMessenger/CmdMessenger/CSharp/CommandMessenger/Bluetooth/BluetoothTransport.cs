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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using CommandMessenger.TransportLayer;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace CommandMessenger.Bluetooth
{
    public enum ThreadRunStates
    {
        Start,
        Stop,
        Abort,
    }

    /// <summary>
    /// Manager for Bluetooth connection
    /// </summary>
    public class BluetoothTransport : DisposableObject, ITransport
    {
        //private BluetoothEndPoint _localEndpoint;
        //private BluetoothClient _localClient;
        //private BluetoothComponent _localComponent;
        private NetworkStream _stream;
        //private Guid _guid = Guid.NewGuid();          
        //private Guid_guid = new Guid("{00112233-4455-6677-8899-aabbccddeeff}");
        private readonly QueueSpeed _queueSpeed = new QueueSpeed(4,10);
        private Thread _queueThread;
        private ThreadRunStates _threadRunState;
        private readonly object _threadRunStateLock = new object();
        private readonly object _readLock = new object();
        private readonly object _writeLock = new object();
        private const int BufferMax = 4096;
        readonly byte[] _readBuffer = new byte[BufferMax];
        private int _bufferFilled;

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
        public BluetoothTransport()
        {          
            Initialize();
        }

        ~BluetoothTransport()
        {
            Kill();
        }

        /// <summary> Initializes this object. </summary>
        public void Initialize()
        {            
            // _queueSpeed.Name = "Bluetooth";

            _queueThread = new Thread(ProcessQueue)
                {
                    Priority = ThreadPriority.Normal,
                    Name = "Bluetooth"
                };
            ThreadRunState = ThreadRunStates.Start;
            _queueThread.Start();
            while (!_queueThread.IsAlive) { Thread.Sleep(50); }
        }

        #region Fields

        public event EventHandler NewDataReceived;                              // Event queue for all listeners interested in NewLinesReceived events.

        #endregion

        #region Properties

        /// <summary> Gets or sets the current serial port settings. </summary>
        /// <value> The current serial settings. </value>
        public BluetoothDeviceInfo CurrentBluetoothDeviceInfo { get; set; }

        public BluetoothClient BluetoothClient
        {
            get { return BluetoothUtils.LocalClient; }
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
            var bytes = UpdateBuffer();
            if (threadRunState == ThreadRunStates.Start)
            {
                if (bytes > 0)
                {
                    // Send an event
                    if (NewDataReceived != null) NewDataReceived(this, null);
                    // Signal so that processes waiting on this continue
                }
            }
        }

        /// <summary> Polls for bluetooth device for data. </summary>
        public void Poll()
        {
            Poll(ThreadRunStates.Start);
        }

        /// <summary> Connects to a serial port defined through the current settings. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Connect()
        {
            // Closing serial port if it is open
            _stream = null;

            // set pin of device to connect with            
            // check if device is paired
            //CurrentBluetoothDeviceInfo.Refresh();
            try
            {
                if (!CurrentBluetoothDeviceInfo.Authenticated)
                {
                    //Console.WriteLine("Not authenticated");
                    return false;
                }

                if (BluetoothClient.Connected)
                {
                    //Console.WriteLine("Previously connected, setting up new connection");
                    BluetoothUtils.UpdateClient();
                }

                // synchronous connection method
                BluetoothClient.Connect(CurrentBluetoothDeviceInfo.DeviceAddress, BluetoothService.SerialPort);
                //Console.WriteLine("New connection");
                //BluetoothClient.Connect(CurrentBluetoothDeviceInfo.DeviceAddress, _guid);
                //BluetoothUtils.ConnectDevice(CurrentBluetoothDeviceInfo,null);

                if (!Open())
                {
                    Console.WriteLine("Stream not opened");
                    return false;
                }

                // Subscribe to event and open serial port for data
                ThreadRunState = ThreadRunStates.Start;
                return true;
            }
            catch (SocketException)
            {
                //Console.WriteLine("Socket exception while trying to connect");
                return false;
            }
            catch (InvalidOperationException)
            {
                BluetoothUtils.UpdateClient();
                return false;
            }
        }

        /// <summary> Opens the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Open()
        {
            if (!BluetoothClient.Connected) return false;
            _stream = BluetoothClient.GetStream();
            _stream.ReadTimeout = 5;
            return (true);
        }

        public bool IsConnected()
        {
            // note: this does not always work. Perhaps do a scan
            return BluetoothClient.Connected;
        }

        public bool IsOpen()
        {
            // note: this does not always work. Perhaps do a scan
            return IsConnected() && (_stream != null);
        }


        /// <summary> Closes the Bluetooth stream port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Close()
        {
            // No closing needed
            if (_stream == null) return true;
            _stream.Close();
            _stream = null;
            return true;
        }

        /// <summary> Disconnect the bluetooth stream. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Disconnect()
        {
            ThreadRunState = ThreadRunStates.Stop;
            var state = Close();
            return state;
        }

        /// <summary> Writes a byte array to the bluetooth stream. </summary>
        /// <param name="buffer"> The buffer to write. </param>
        public void Write(byte[] buffer)
        {
            try
            {
                if (IsOpen())
                {
                    lock (_writeLock)
                    {
                        _stream.Write(buffer,0,buffer.Length);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary> Retrieves the address of the local bluetooth radio. </summary>
        /// <returns> The address of the local bluetooth radio. </returns>
        public BluetoothAddress RetreiveLocalBluetoothAddress()
        {
            var primaryRadio = BluetoothRadio.PrimaryRadio;
            if (primaryRadio == null) return null;
            return primaryRadio.LocalAddress;
        }

        private int UpdateBuffer()
        {
            if (IsOpen())
            {
                try
                {
                    lock (_readLock)
                    {
                        //if (_stream.DataAvailable)
                        {
                            var nbrDataRead = _stream.Read(_readBuffer, _bufferFilled, (BufferMax - _bufferFilled));
                            _bufferFilled += nbrDataRead;
                        }
                    }
                    return _bufferFilled;
                }
                catch (IOException)
                {
                    // Timeout (expected)
                }
            }
            return 0;
        }

        /// <summary> Reads the serial buffer into the string buffer. </summary>
        public byte[] Read()
        {
            if (IsOpen())
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

        /// <summary> Gets the bytes in buffer. </summary>
        /// <returns> Bytes in buffer </returns>
        public int BytesInBuffer()
        {
            return IsOpen() ? _bufferFilled : 0;
        }

        /// <summary> Kills this object. </summary>
        public void Kill()
        {
            // Signal thread to abort
            ThreadRunState = ThreadRunStates.Abort;

            //Wait for thread to die
            Join(500);
            if (_queueThread.IsAlive) _queueThread.Abort();

            // Releasing stream
            if (IsOpen()) Close();

            // component is used to manage device discovery
            //_localComponent.Dispose();
            
            // client is used to manage connections
            //_localClient.Dispose();
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