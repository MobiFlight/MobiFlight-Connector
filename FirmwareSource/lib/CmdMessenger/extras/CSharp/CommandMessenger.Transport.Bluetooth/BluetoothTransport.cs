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
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

// Todo: 
// remove isconnected for speedup. 
// Test for disconnected bluetooth

namespace CommandMessenger.Transport.Bluetooth
{
    /// <summary>
    /// Manager for Bluetooth connection
    /// </summary>
    public class BluetoothTransport : ITransport
    {
        private const int BufferSize = 4096;

        private NetworkStream _stream;
        private readonly AsyncWorker _worker;
        private readonly object _readLock = new object();
        private readonly object _writeLock = new object();
        private readonly byte[] _readBuffer = new byte[BufferSize];
        private int _bufferFilled;
		private static BluetoothDeviceInfo _runningBluetoothDeviceInfo;
        private static bool _showAsConnected;
        private static bool _lazyReconnect;



        // Event queue for all listeners interested in NewLinesReceived events.
        public event EventHandler DataReceived;




        /// <summary>
        /// Gets or sets Bluetooth device info
        /// </summary>
        public BluetoothDeviceInfo CurrentBluetoothDeviceInfo { get; set; }

        /// <summary>
        /// Get or set Lazy reconnection status. Only reconnect if really necessary
        /// </summary>
        public bool LazyReconnect
        {
            get { return _lazyReconnect; }
            set { _lazyReconnect = value; }
        }

        /// <summary>
        /// Return local Bluetooth client
        /// </summary>
        public BluetoothClient BluetoothClient
        {
            get { return BluetoothUtils.LocalClient; }
        }

        /// <summary>
        /// Bluetooth transport constructor
        /// </summary>
        public BluetoothTransport()
        {
            _showAsConnected = false;
            _lazyReconnect = true;
            _worker = new AsyncWorker(Poll, "BluetoothTransport");
        }

        /// <summary>
        /// Bluetooth transport destructor
        /// </summary>
        ~BluetoothTransport()
        {
            Disconnect();
        }

        private bool Poll()
        {
            var bytes = UpdateBuffer();
            if (bytes > 0 && DataReceived != null) DataReceived(this, EventArgs.Empty);

            return true;
        }        

        /// <summary> Connects to a serial port defined through the current settings. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Connect()
        {
             
            // Reconnecting to the same device seems to fail a lot of the time, so see
            // if we can remain connected
            if (_runningBluetoothDeviceInfo!=null && _runningBluetoothDeviceInfo.DeviceAddress == CurrentBluetoothDeviceInfo.DeviceAddress && _lazyReconnect) {
                CurrentBluetoothDeviceInfo = _runningBluetoothDeviceInfo;
            } else {
                _runningBluetoothDeviceInfo = CurrentBluetoothDeviceInfo;
                //BluetoothClient.Close();
            }
            // Closing serial port if it is open
            //_stream = null;

            // set pin of device to connect with            
            // check if device is paired
            //CurrentBluetoothDeviceInfo.Refresh();
            try
            {
                if (!CurrentBluetoothDeviceInfo.Authenticated)
                {
                    //Console.WriteLine("Not authenticated");
                    _showAsConnected = false;
                    return _showAsConnected;
                }

                if (BluetoothClient.Connected && !LazyReconnect)
                {
                    //Previously connected, setting up new connection"
                    BluetoothUtils.UpdateClient();
                }

                // synchronous connection method
                if (!BluetoothClient.Connected || !_lazyReconnect)
                    BluetoothClient.Connect(CurrentBluetoothDeviceInfo.DeviceAddress, BluetoothService.SerialPort);

                if (!Open())
                {
                    // Desperate attempt: try full reset and open
                    _showAsConnected = UpdateConnectOpen();
                    return _showAsConnected;
                }

                // Check worker is not running as a precaution. This needs to be rechecked.
                if (!_worker.IsRunning) _worker.Start();

                _showAsConnected = true;
                return _showAsConnected;
            }
            catch (SocketException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                // Desperate attempt: try full reset and open
                _showAsConnected = UpdateConnectOpen();
                return _showAsConnected;
            }
        }

       
        private bool UpdateConnectOpen()
        {
            BluetoothUtils.UpdateClient();
            try
            {
                BluetoothClient.Connect(CurrentBluetoothDeviceInfo.DeviceAddress, BluetoothService.SerialPort);
            }
            catch
            {
                return false;
            }
            return Open();
        }

        /// <summary> Opens the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Open()
        {
            if (!BluetoothClient.Connected) return false;
            lock (_writeLock) { lock (_readLock) 
            {
                    _stream = BluetoothClient.GetStream();
                    _stream.ReadTimeout = 2000;
                    _stream.WriteTimeout = 1000;
            } }
            return true;
        }

        /// <summary>
        /// Returns connection status
        /// </summary>
        /// <returns>true when connected</returns>
        public bool IsConnected()
        {
            // In case of lazy reconnect we will pretend to be disconnected
            if (_lazyReconnect && !_showAsConnected) return false;
            // If not, test if we are connected
            return (BluetoothClient!=null) && BluetoothClient.Connected;
        }

        /// <summary>
        /// Returns opened stream status
        /// </summary>
        /// <returns>true when open</returns>
        public bool IsOpen()
        {
            // note: this does not always work. Perhaps do a scan
            return IsConnected() && (_stream != null);
        }


        /// <summary> Closes the Bluetooth stream port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Close()
        {
            lock (_writeLock)
            {
                lock (_readLock)
                {
                    // No closing needed
                    if (_stream == null) return true;
                    _stream.Close();
                    _stream = null;
                    return true;
                }
            }
        }

        /// <summary> Disconnect the bluetooth stream. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Disconnect()
        {
            _showAsConnected = false;
            // Check worker is running as a precaution. 
            if (_worker.IsRunning) _worker.Stop();
            if (_lazyReconnect) return true;
            return Close();
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
                //Do nothing
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int UpdateBuffer()
        {
            if (IsOpen())
            {
                try
                {

                    var nbrDataRead = _stream.Read(_readBuffer, _bufferFilled, (BufferSize - _bufferFilled));
                    lock (_readLock)
                    {
                        _bufferFilled += nbrDataRead;
                        //Console.WriteLine("buf: {0}", _bufferFilled.ToString().Length);
                    }
                    return _bufferFilled;
                }
                catch (IOException)
                {
                    //Console.WriteLine("buf: TO");
                    // Timeout (expected)
                }
            }
            else
            {
                // In case of no connection 
                // Sleep a bit otherwise CPU load will go through roof
                Thread.Sleep(25);
            }

            return _bufferFilled;
        }

        /// <summary> Reads the serial buffer into the string buffer. </summary>
        public byte[] Read()
        {
            //if (IsOpen())
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
            //return new byte[0];
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