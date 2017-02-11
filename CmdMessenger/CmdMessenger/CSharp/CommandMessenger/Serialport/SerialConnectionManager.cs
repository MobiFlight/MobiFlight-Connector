#region CmdMessenger - MIT - (c) 2014 Thijs Elenbaas.
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

  Copyright 2014 - Thijs Elenbaas
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace CommandMessenger.Serialport
{

    /// <summary>
    /// Class for storing last succesfull connection
    /// </summary>
    [Serializable()]
    public class LastConnectedSetting 
    {
        public String Port{ get; set; }
        public int BaudRate { get; set; }
    }

    /// <summary>
    /// Connection manager for serial port connection
    /// </summary>
    public class SerialConnectionManager :  ConnectionManager 
    {
        const string SettingsFileName = @"LastConnectedSerialSetting.cfg";
        private LastConnectedSetting _lastConnectedSetting;
        private readonly SerialTransport _serialTransport;
        private int _scanType = 0;

        // The control to invoke the callback on
        private readonly object _tryConnectionLock = new object();

        /// <summary>
        /// Connection manager for serial port connection
        /// </summary
        public SerialConnectionManager(SerialTransport serialTransport, CmdMessenger cmdMessenger, int challengeCommandId, int responseCommandId) :
            base(cmdMessenger, challengeCommandId, responseCommandId)
        {
            WatchdogTimeOut = 2000;
            WatchdogRetryTimeOut = 1000;
            MaxWatchdogTries = 3;

            if (serialTransport == null) return;
            if (cmdMessenger    == null) return;

            _serialTransport = serialTransport;

            _lastConnectedSetting = new LastConnectedSetting();
            ReadSettings();
            _serialTransport.UpdatePortCollection();

            StartConnectionManager();
        }

        /// <summary>
        /// Try connection given specific port name & baud rate
        /// </summary>
        /// <param name="portName">Port name</param>
        /// <param name="baudRate">Baud rate</param>
        /// <param name="timeOut">Time out for response</param>
        /// <returns>true if succesfully connected</returns>
        public bool TryConnection(string portName, int baudRate, int timeOut)
        {
            // Try specific port name & baud rate

            _serialTransport.CurrentSerialSettings.PortName = portName;
            _serialTransport.CurrentSerialSettings.BaudRate = baudRate;
            return TryConnection(timeOut); 
        }

        /// <summary>
        /// Try connection 
        /// </summary>
        /// <param name="timeOut">Time out for response</param>
        /// <returns>true if succesfully connected</returns>
        public bool TryConnection(int timeOut)
        {
            lock(_tryConnectionLock)
            Connected = false;
            Log(1, @"Trying serial port " + _serialTransport.CurrentSerialSettings.PortName + @" baud rate " + _serialTransport.CurrentSerialSettings.BaudRate);
            if (_serialTransport.Connect())
            {
                Connected = (ArduinoAvailable(timeOut,2));
                
                if (Connected)
                {
                    Log(1, "Connected at serial port " + _serialTransport.CurrentSerialSettings.PortName + @" baud rate " + _serialTransport.CurrentSerialSettings.BaudRate);
                    StoreSettings();
                }
                return Connected;
            }
            return false;
        }


        // Single scan on foreground thread
        public bool SingleScan()
        {
            if (QuickScan()) return true;
            if (ThoroughScan()) return true;
            return false;
        }

        protected override void DoWorkScan()
        {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "BluetoothConnectionManager";
            var activeConnection = false;
            
            {
                if (_scanType == 0)
                {
                    _scanType = 1;
                    try { activeConnection = QuickScan(); }
                    catch { }
                }
                else if (_scanType == 1)
                {
                    _scanType = 0;
                    try { activeConnection = ThoroughScan(); }
                    catch { }
                }
            }

            // Trigger event when a connection was made
            if (activeConnection)
            {
                ConnectionManagerState = ConnectionManagerStates.Wait;
                ConnectionFoundEvent();

            } 
        }

        public bool QuickScan()
        {            
            Log(3, "Performing quick scan");
            const int longTimeOut =  1000;
            const int shortTimeOut = 500;

            // First try if currentConnection is open or can be opened
            if (TryConnection(longTimeOut)) return true;

            // Then try if last stored connection can be opened
            Log(3, "Trying last stored connection");
            if (TryConnection(_lastConnectedSetting.Port, _lastConnectedSetting.BaudRate, longTimeOut)) return true;

            // Then see if port list has changed
            //if (NewPortInList().Count > 0) { _scanType = 2; return false; }

            // Quickly run through most used ports
            int[] commonBaudRates =
                {
                    115200, // Arduino Uno, Mega, with AT8u2 USB
                    57600,  // Arduino Duemilanove, FTDI Serial
                    9600    // Often used as default, but slow!
                };
            _serialTransport.UpdatePortCollection();
            for (var port = _serialTransport.CurrentSerialSettings.PortNameCollection.Length- 1; port >= 0; port--)
            {
                // If port list has changed, interrupt scan and test new ports first
                if (NewPortScan()) return true;

                var portName = _serialTransport.CurrentSerialSettings.PortNameCollection[port];
                // First set port name
                _serialTransport.CurrentSerialSettings.PortName = portName;
                // Now update BaudRate Collection
                _serialTransport.UpdateBaudRateCollection();
                var baudRateCollection =_serialTransport.CurrentSerialSettings.BaudRateCollection;

                //  Now loop through baud rate collection
                foreach (var commonBaudRate in commonBaudRates)
                {

                    if (_serialTransport.CurrentSerialSettings.BaudRateCollection.Contains(commonBaudRate))
                    {

                        Log(1,
                            "Trying Port" + portName + ", possible speeds " +
                            baudRateCollection.Count + " " +
                            (baudRateCollection.Count > commonBaudRates.Length ? ", trying " + commonBaudRates.Length : "")
                            );
                        if (TryConnection(portName,commonBaudRate, shortTimeOut)) return true;
                        Thread.Sleep(25); 
                    }                    
                }
            }
            return false;
        }

        public bool ThoroughScan()
        {
            Console.WriteLine("Performing thorough scan");
            Log(1, "Performing thorough scan");
            // First try last used connection
            const int longTimeOut = 1000;
            const int shortTimeOut = 500;

            // First try if currentConnection is open or can be opened
            if (TryConnection(longTimeOut)) return true;

            // Then try if last stored connection can be opened
            if (TryConnection(_lastConnectedSetting.Port, _lastConnectedSetting.BaudRate, longTimeOut)) return true;



            // Slowly walk through 
            _serialTransport.UpdatePortCollection();
            foreach (var portName in _serialTransport.CurrentSerialSettings.PortNameCollection)
            {


                // First set port name
                _serialTransport.CurrentSerialSettings.PortName = portName;
                // update BaudRate Collection
                _serialTransport.UpdateBaudRateCollection();
                //  Now loop through baud rate collection
                var baudRateCollection = _serialTransport.CurrentSerialSettings.BaudRateCollection;

                Log(1, "Trying Port" + portName + ", possible speeds " + baudRateCollection.Count);

                foreach (var baudRate in baudRateCollection)
                {
                    // If port list has changed, interrupt scan and test new ports first
                    if (NewPortScan()) return true;
                    {
                        if (TryConnection(portName,baudRate, shortTimeOut))
                            return true;
                        Thread.Sleep(100); 
                    }
                }
            }
            return false;
        }

        public bool NewPortScan()
        {            
            const int shortTimeOut = 200;

            // Then see if port list has changed
            var newPorts = NewPortInList();
            if (newPorts.Count == 0) { return false; }

            Console.WriteLine("Trying new ports");
            Log(1, "Trying new ports");

            // Quickly run through most used ports
            int[] commonBaudRates =
                {
                    115200, // Arduino Uno, Mega, with AT8u2 USB
                    57600,  // Arduino Duemilanove, FTDI Serial
                    9600    // Often used as default, but slow!
                };
            _serialTransport.UpdatePortCollection();
            foreach (var portName in newPorts)
            {
                // First set port name
                _serialTransport.CurrentSerialSettings.PortName = portName;
                // Now update BaudRate Collection
                _serialTransport.UpdateBaudRateCollection();
                //  Now loop through baud rate collection
                var allBaudRates = _serialTransport.CurrentSerialSettings.BaudRateCollection;
                // First add commonBaudRates available
                var sortedBaudRates = commonBaudRates.Where(allBaudRates.Contains).ToList();
                // Then add other BaudRates 
                sortedBaudRates.AddRange(allBaudRates.Where(baudRate => !commonBaudRates.Contains(baudRate)));              

                foreach (var currentBaudRate in sortedBaudRates)
                {
                        if (TryConnection(portName, currentBaudRate, shortTimeOut)) return true;
                        Thread.Sleep(100);
                }
            }
            return false;
        }


        private List<string> NewPortInList()
        {
            var oldPortCollection = _serialTransport.CurrentSerialSettings.PortNameCollection;
            var portCollection    = SerialPort.GetPortNames();
            return portCollection.Where(port => !oldPortCollection.Any(port.Contains)).ToList();
        }

        private void StoreSettings()
        {
            _lastConnectedSetting.Port = _serialTransport.CurrentSerialSettings.PortName;
            _lastConnectedSetting.BaudRate = _serialTransport.CurrentSerialSettings.BaudRate;

            var fileStream = File.Create(SettingsFileName);
            var serializer = new BinaryFormatter();
            serializer.Serialize(fileStream,_lastConnectedSetting);
            fileStream.Close();
        }

        private void ReadSettings()
        {
            // Read from file

            _lastConnectedSetting.Port = "COM1";
            _lastConnectedSetting.BaudRate = 115200;
            if (File.Exists(SettingsFileName))
            {
                var fileStream = File.OpenRead(SettingsFileName);
                var deserializer = new BinaryFormatter();
                _lastConnectedSetting = (LastConnectedSetting)deserializer.Deserialize(fileStream);
                fileStream.Close();
            }
        }

    }
}
