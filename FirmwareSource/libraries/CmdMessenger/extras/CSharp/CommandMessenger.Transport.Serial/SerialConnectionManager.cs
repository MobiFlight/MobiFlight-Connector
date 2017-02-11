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
using System.Linq;
using System.Threading;

namespace CommandMessenger.Transport.Serial
{

    /// <summary>
    /// Class for storing last succesfull connection
    /// </summary>
    [Serializable]
    public class SerialConnectionManagerSettings 
    {
        public String Port{ get; set; }
        public int BaudRate { get; set; }
    }

    /// <summary>
    /// Connection manager for serial port connection
    /// </summary>
    public class SerialConnectionManager : ConnectionManager 
    {
        private enum ScanType { None, Quick, Thorough }

        private SerialConnectionManagerSettings _serialConnectionManagerSettings;
        private readonly ISerialConnectionStorer _serialConnectionStorer;
        private readonly SerialTransport _serialTransport;
        private readonly object _tryConnectionLock = new object();

        private ScanType _scanType = ScanType.None;

        /// <summary>
        /// Available serial port names in system.
        /// </summary>
        public string[] AvailableSerialPorts
        {
            get;
            private set;
        }

        /// <summary>
        /// In scan mode allow to try different baud rates besides that is configured in SerialSettings.
        /// </summary>
        public bool DeviceScanBaudRateSelection { get; set; }

        /// <summary>
        /// Connection manager for serial port connection
        /// </summary>
        public SerialConnectionManager(SerialTransport serialTransport, CmdMessenger cmdMessenger, int watchdogCommandId = 0, string uniqueDeviceId = null, ISerialConnectionStorer serialConnectionStorer = null) :
            base(cmdMessenger, watchdogCommandId, uniqueDeviceId)
        {
            
            if (serialTransport == null) 
                throw new ArgumentNullException("serialTransport", "Transport is null.");

            _serialTransport = serialTransport;
            _serialConnectionStorer = serialConnectionStorer;
            PersistentSettings = (_serialConnectionStorer != null);

            DeviceScanBaudRateSelection = true;

            UpdateAvailablePorts();

            _serialConnectionManagerSettings = new SerialConnectionManagerSettings();
            ReadSettings();
        }

        /// <summary>
        /// Try connection 
        /// </summary>
        /// <returns>Result</returns>
        private DeviceStatus TryConnection(string portName = null, int baudRate = int.MinValue)
        {
            lock (_tryConnectionLock)
            {
                // Save current port and baud rate
                string oldPortName = _serialTransport.CurrentSerialSettings.PortName;
                int oldBaudRate = _serialTransport.CurrentSerialSettings.BaudRate;

                // Update serial settings with new port and baud rate.
                if (portName != null) _serialTransport.CurrentSerialSettings.PortName = portName;
                if (baudRate != int.MinValue) _serialTransport.CurrentSerialSettings.BaudRate = baudRate;

                if (!_serialTransport.CurrentSerialSettings.IsValid())
                {
                    // Restore back previous settings if newly provided was invalid.
                    _serialTransport.CurrentSerialSettings.PortName = oldPortName;
                    _serialTransport.CurrentSerialSettings.BaudRate = oldBaudRate;

                    return DeviceStatus.NotAvailable;
                }

                Connected = false;

                Log(1, @"Trying serial port " + _serialTransport.CurrentSerialSettings.PortName + " at " + _serialTransport.CurrentSerialSettings.BaudRate + " bauds.");
                if (_serialTransport.Connect())
                {
                    // Calculate optimal timeout for command. It should be not less than Serial Port timeout. Lets add additional 250ms.
                    int optimalTimeout = _serialTransport.CurrentSerialSettings.Timeout + 250;
                    DeviceStatus status = ArduinoAvailable(optimalTimeout);

                    Connected = (status == DeviceStatus.Available);
                
                    if (Connected)
                    {
                        Log(1, "Connected to serial port " + _serialTransport.CurrentSerialSettings.PortName + " at " + _serialTransport.CurrentSerialSettings.BaudRate + " bauds.");
                        StoreSettings();
                    }
                    else
                    {
                        _serialTransport.Disconnect();
                    }

                    return status;
                }
			
                return DeviceStatus.NotAvailable;
            }
        }

        protected override void StartScan()
        {
            base.StartScan();

            if (ConnectionManagerMode == Mode.Scan)
            {
                UpdateAvailablePorts();
                _scanType = ScanType.None;
            }
        }

        //Try to connect using current connections settings and trigger event if succesful
        protected override void DoWorkConnect()
        {
            var activeConnection = false;

            try
            {
                activeConnection = TryConnection() == DeviceStatus.Available;
            }
            catch
            {
                // Do nothing
            }

            if (activeConnection)
            {
                ConnectionFoundEvent();
            } 
        }

        // Perform scan to find connected systems
        protected override void DoWorkScan()
        {
            // First try if currentConnection is open or can be opened
            var activeConnection = false;

            switch (_scanType)
            {
                case ScanType.None:
                    try
                    {
                        activeConnection = TryConnection() == DeviceStatus.Available;
                    }
                    catch
                    {
                        // Do nothing
                    }
                    _scanType = ScanType.Quick;
                    break;
                case ScanType.Quick:
                    try
                    {
                        activeConnection = QuickScan();
                    }
                    catch
                    {
                        // Do nothing
                    }
                    _scanType = ScanType.Thorough;
                    break;
                case ScanType.Thorough:
                    try
                    {
                        activeConnection = ThoroughScan();
                    }
                    catch
                    {
                        // Do nothing
                    }
                    _scanType = ScanType.Quick;
                    break;
            }

            // Trigger event when a connection was made
            if (activeConnection)
            {
                ConnectionFoundEvent();
            } 
        }

        private bool QuickScan()
        {            
            Log(3, "Performing quick scan.");

            if (PersistentSettings)
            {
                // Then try if last stored connection can be opened
                Log(3, "Trying last stored connection.");
                if (TryConnection(_serialConnectionManagerSettings.Port, _serialConnectionManagerSettings.BaudRate) == DeviceStatus.Available) 
                    return true;
            }

            // Quickly run through most used baud rates
            var commonBaudRates = DeviceScanBaudRateSelection 
                ? SerialUtils.CommonBaudRates 
                : new [] { _serialTransport.CurrentSerialSettings.BaudRate };
            foreach (var portName in AvailableSerialPorts)
            {
                // Get baud rates collection
                var baudRateCollection = DeviceScanBaudRateSelection
                    ? SerialUtils.GetSupportedBaudRates(portName)
                    : new[] { _serialTransport.CurrentSerialSettings.BaudRate };

                var baudRates = commonBaudRates.Where(baudRateCollection.Contains).ToList();
                if (baudRates.Any())
                {
                    Log(1, "Trying serial port " + portName + " using " + baudRateCollection.Length + " baud rate(s).");

                    //  Now loop through baud rate collection
                    foreach (var commonBaudRate in baudRates)
                    {
                        // Stop scanning if state was changed
                        if (ConnectionManagerMode != Mode.Scan) return false;

                        DeviceStatus status = TryConnection(portName, commonBaudRate);
                        if (status == DeviceStatus.Available) return true;
                        if (status == DeviceStatus.IdentityMismatch) break; // break the loop and continue to next port.
                    }
                }

                // If port list has changed, interrupt scan and test new ports first
                if (NewPortScan()) return true;
            }

            if (!AvailableSerialPorts.Any())
            {
                // Need to check for new ports if current ports list is empty
                if (NewPortScan()) return true;

                // Add small delay to reduce of Quick->Thorough->Quick->Thorough scan attempts - 400ms here + 100ms in main loop = ~500ms
                Thread.Sleep(400);
            }

            return false;
        }

        private bool ThoroughScan()
        {
            Log(1, "Performing thorough scan.");

            // Then try if last stored connection can be opened
            if (PersistentSettings && TryConnection(_serialConnectionManagerSettings.Port, _serialConnectionManagerSettings.BaudRate) == DeviceStatus.Available) 
                return true;

            // Slowly walk through 
            foreach (var portName in AvailableSerialPorts)
            {
                // Get baud rates collection
                var baudRateCollection = DeviceScanBaudRateSelection
                    ? SerialUtils.GetSupportedBaudRates(portName)
                    : new[] { _serialTransport.CurrentSerialSettings.BaudRate };

                //  Now loop through baud rate collection
				if (baudRateCollection.Any())
				{
                	Log(1, "Trying serial port " + portName + " using " + baudRateCollection.Length + " baud rate(s).");

	                foreach (var baudRate in baudRateCollection)
	                {
                        // Stop scanning if state was changed
                        if (ConnectionManagerMode != Mode.Scan) return false;

                        DeviceStatus status = TryConnection(portName, baudRate);
                        if (status == DeviceStatus.Available) return true;
                        if (status == DeviceStatus.IdentityMismatch) break; // break the loop and continue to next port.
	                }
				}

                // If port list has changed, interrupt scan and test new ports first
                if (NewPortScan()) return true;
            }

            if (!AvailableSerialPorts.Any())
            {
                // Need to check for new ports if current ports list is empty
                if (NewPortScan()) return true;

                // Add small delay to reduce of Quick->Thorough->Quick->Thorough scan attempts - 400ms here + 100ms in main loop = ~500ms
                Thread.Sleep(400);
            }

            return false;
        }

        private bool NewPortScan()
        {            
            // Then see if port list has changed
            var newPorts = NewPortInList();
            if (!newPorts.Any()) { return false; }

            //TODO: 4s - practical delay for Leonardo board, probably for other boards will be different. Need to investigate more on this.
            const int waitTime = 4000;
            Log(1, "New port(s) " + string.Join(",", newPorts) + " detected, wait for " + (waitTime / 1000.0) + "s before attempt to connect.");

            // Wait a bit before new port will be available then try to connect
            Thread.Sleep(waitTime);

            // Quickly run through most used ports
            var commonBaudRates = DeviceScanBaudRateSelection 
                ? SerialUtils.CommonBaudRates
                : new[] { _serialTransport.CurrentSerialSettings.BaudRate };

            foreach (var portName in newPorts)
            {
                // Get baud rates collection
                var baudRateCollection = DeviceScanBaudRateSelection
                    ? SerialUtils.GetSupportedBaudRates(portName)
                    : new[] { _serialTransport.CurrentSerialSettings.BaudRate };

                // First add commonBaudRates available
                var sortedBaudRates = commonBaudRates.Where(baudRateCollection.Contains).ToList();
                // Then add other BaudRates 
                sortedBaudRates.AddRange(baudRateCollection.Where(baudRate => !commonBaudRates.Contains(baudRate)));              

                foreach (var currentBaudRate in sortedBaudRates)
                {
                    // Stop scanning if state was changed
                    if (ConnectionManagerMode != Mode.Scan) return false;
                    
                    DeviceStatus status = TryConnection(portName, currentBaudRate);
                    if (status == DeviceStatus.Available) return true;
                    if (status == DeviceStatus.IdentityMismatch) break; // break the loop and continue to next port.
                }
            }

            return false;
        }

        private void UpdateAvailablePorts()
        {
            AvailableSerialPorts = SerialUtils.GetPortNames();
        }

        private List<string> NewPortInList()
        {
            var currentPorts = SerialUtils.GetPortNames();
            var newPorts = currentPorts.Except(AvailableSerialPorts).ToList();

            // Actualize ports collection
            AvailableSerialPorts = currentPorts;

            return newPorts;
        }

        protected override void StoreSettings()
        {
            if (!PersistentSettings) return;

            _serialConnectionManagerSettings.Port = _serialTransport.CurrentSerialSettings.PortName;
            _serialConnectionManagerSettings.BaudRate = _serialTransport.CurrentSerialSettings.BaudRate;

            _serialConnectionStorer.StoreSettings(_serialConnectionManagerSettings);
        }

        protected override sealed void ReadSettings()
        {
            if (!PersistentSettings) return;
            _serialConnectionManagerSettings = _serialConnectionStorer.RetrieveSettings();
        }
    }
}
