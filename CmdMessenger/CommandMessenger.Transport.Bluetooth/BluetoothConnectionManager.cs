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
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

// todo: User added common PINs and per-device PINs

namespace CommandMessenger.Transport.Bluetooth
{  
    /// <summary>
    /// Class for storing last succesful connection
    /// </summary>
    [Serializable]
    public class BluetoothConnectionManagerSettings 
    {
        public BluetoothAddress BluetoothAddress { get; set; }
        public Dictionary<BluetoothAddress, string> StoredDevicePins { get; set; }

        public BluetoothConnectionManagerSettings()
        {
            StoredDevicePins = new Dictionary<BluetoothAddress, string>();
        }
    }

    /// <summary>
    /// Connection manager for Bluetooth devices
    /// </summary>
    public class BluetoothConnectionManager : ConnectionManager
    {
        private static readonly List<string> CommonDevicePins = new List<string>
            {                
                "0000",                
                "1111",
                "1234",
            };

        private enum ScanType { None, Quick, Thorough }

        private BluetoothConnectionManagerSettings _bluetoothConnectionManagerSettings;
        private readonly IBluetoothConnectionStorer _bluetoothConnectionStorer;
        private readonly BluetoothTransport _bluetoothTransport;
        private ScanType _scanType;

        // The control to invoke the callback on
        private readonly object _tryConnectionLock = new object();
        private readonly List<BluetoothDeviceInfo> _deviceList;
        private List<BluetoothDeviceInfo> _prevDeviceList;

        /// <summary>
        /// Lookup dictionary of Pincode per device
        /// </summary>
        public Dictionary<string, string> DevicePins { get; set; }

        /// <summary>
        /// List of Pincodes tried for unknown devices
        /// </summary>
        public List<string> GeneralPins { get; set; }

        /// <summary>
        /// Connection manager for Bluetooth devices
        /// </summary>
        public BluetoothConnectionManager(BluetoothTransport bluetoothTransport, CmdMessenger cmdMessenger, int watchdogCommandId = 0, string uniqueDeviceId = null, IBluetoothConnectionStorer bluetoothConnectionStorer = null) :
            base(cmdMessenger, watchdogCommandId, uniqueDeviceId)
        {
            if (bluetoothTransport == null) 
                throw new ArgumentNullException("bluetoothTransport", "Transport is null.");

            _bluetoothTransport = bluetoothTransport;

            _bluetoothConnectionManagerSettings = new BluetoothConnectionManagerSettings();
            _bluetoothConnectionStorer = bluetoothConnectionStorer;
            PersistentSettings = (_bluetoothConnectionStorer != null);
            ReadSettings();

            _deviceList = new List<BluetoothDeviceInfo>();
            _prevDeviceList = new List<BluetoothDeviceInfo>();

            DevicePins = new Dictionary<string, string>();
            GeneralPins = new List<string>();
        }

        //Try to connect using current connections settings and trigger event if succesful
        protected override void DoWorkConnect()
        {
            const int timeOut = 1000;
            var activeConnection = false;

            try
            {
                activeConnection = TryConnection(timeOut);
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
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "BluetoothConnectionManager";
            var activeConnection = false;

            // Starting scan
            if (_scanType == ScanType.None) 
            {
                _scanType = ScanType.Quick;
            }

            switch (_scanType)
            {
                case ScanType.Quick:
                    try { activeConnection = QuickScan(); } catch {
                        //Do nothing 
                    }
                    _scanType = ScanType.Thorough;
                    break;
                case ScanType.Thorough:
                    try { activeConnection = ThoroughScan(); } catch {
                        //Do nothing 
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

        // Quick scan of available devices
        private void QuickScanDevices()
        {
            // Fast
            _prevDeviceList = _deviceList;
            _deviceList.Clear();
            _deviceList.AddRange(_bluetoothTransport.BluetoothClient.DiscoverDevices(255, true, true, false, false));
        }

        // Thorough scan of available devices
        private void ThorougScanForDevices()
        {
            // Slow
            _deviceList.Clear();
            _deviceList.AddRange(_bluetoothTransport.BluetoothClient.DiscoverDevices(65536, true, true, true, true));
        }

        // Pair a Bluetooth device
        private bool PairDevice(BluetoothDeviceInfo device)
        {
            if (device.Authenticated) return true;
            Log(2, "Trying to pair device " + device.DeviceName + " (" + device.DeviceAddress + ") ");

            // Check if PIN  for this device has been injected in ConnectionManager  
            string adress = device.DeviceAddress.ToString();

            var matchedDevicePin = FindPin(adress);
            if (matchedDevicePin!=null)
            {
                
                Log(3, "Trying known key for device " + device.DeviceName);
                if (BluetoothSecurity.PairRequest(device.DeviceAddress, matchedDevicePin))
                {
                    Log(2, "Pairing device " + device.DeviceName + " succesful! ");
                    return true;
                }
                // When trying PINS, you really need to wait in between
                Thread.Sleep(1000);
            }  

            // Check if PIN has been previously found and stored
            if (_bluetoothConnectionManagerSettings.StoredDevicePins.ContainsKey(device.DeviceAddress))
            {
                Log(3, "Trying stored key for device " + device.DeviceName );
                if (BluetoothSecurity.PairRequest(device.DeviceAddress, _bluetoothConnectionManagerSettings.StoredDevicePins[device.DeviceAddress]))
                {
                    Log(2, "Pairing device " + device.DeviceName + " succesful! ");
                    return true;
                }
                // When trying PINS, you really need to wait in between
                Thread.Sleep(1000);   
            }

            // loop through general pins PIN numbers that have been injected to see if they pair
            foreach (string devicePin in GeneralPins)
            {

                Log(3, "Trying known general pin " + devicePin + " for device " + device.DeviceName);
                var isPaired = BluetoothSecurity.PairRequest(device.DeviceAddress, devicePin);
                if (isPaired)
                {
                    _bluetoothConnectionManagerSettings.StoredDevicePins[device.DeviceAddress] = devicePin;
                    Log(2, "Pairing device " + device.DeviceName + " succesful! ");
                    return true;
                }
                // When trying PINS, you really need to wait in between
                Thread.Sleep(1000);
            }

            // loop through common PIN numbers to see if they pair
            foreach (string devicePin in CommonDevicePins)
            {               
                Log(3, "Trying common pin " + devicePin + " for device " + device.DeviceName);
                var isPaired = BluetoothSecurity.PairRequest(device.DeviceAddress, devicePin);
                if (isPaired)
                {
                    _bluetoothConnectionManagerSettings.StoredDevicePins[device.DeviceAddress] = devicePin;
                    StoreSettings();
                    Log(2, "Pairing device " + device.DeviceName + " succesful! ");
                    return true;
                }
                // When trying PINS, you really need to wait in between
                Thread.Sleep(1000);
            }

            Log(2, "Pairing device " + device.DeviceName + " unsuccesfull ");
            return true;
        }

        // Find the pin code for a Bluetooth adress
        private string FindPin(string adress)
        {
            return (from devicePin in DevicePins where BluetoothUtils.StripBluetoothAdress(devicePin.Key) == adress select devicePin.Value).FirstOrDefault();
        }

        private bool TryConnection(BluetoothAddress bluetoothAddress, int timeOut)
        {
            if (bluetoothAddress == null) return false;
            // Find
            return (from bluetoothDeviceInfo in _deviceList where bluetoothDeviceInfo.DeviceAddress == bluetoothAddress select TryConnection(bluetoothDeviceInfo, timeOut)).FirstOrDefault();
        }

        private bool TryConnection(BluetoothDeviceInfo bluetoothDeviceInfo, int timeOut)
        {
            // Try specific settings
            _bluetoothTransport.CurrentBluetoothDeviceInfo = bluetoothDeviceInfo;
            return TryConnection(timeOut); 
        }

        private bool TryConnection(int timeOut)
        {
            lock (_tryConnectionLock)
            {
                // Check if an (old) connection exists
                if (_bluetoothTransport.CurrentBluetoothDeviceInfo == null) return false;

                Connected = false;
                Log(1, @"Trying Bluetooth device " + _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName);
                if (_bluetoothTransport.Connect())
                {
                    Log(3,
                        @"Connected with Bluetooth device " + _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName +
                        ", requesting response");

                    DeviceStatus status = ArduinoAvailable(timeOut, 5);
                    Connected = (status == DeviceStatus.Available);

                    if (Connected)
                    {
                        Log(1,
                            "Connected with Bluetooth device " +
                            _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName);
                        StoreSettings();
                    }
                    else
                    {
                        Log(3,
                            @"Connected with Bluetooth device " +
                            _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName + ", received no response");
                    }
                    return Connected;
                }
                else
                {
                    Log(3,
                    @"No connection made with Bluetooth device " + _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName );
                }
                return false;
            }
        }

        protected override void StartScan()
        {
            base.StartScan();

            if (ConnectionManagerMode == Mode.Scan)
            {
                _scanType = ScanType.None;
            }
        }
    
        private bool QuickScan()
        {            
            Log(3, "Performing quick scan");
            const int longTimeOut =  1000;
            const int shortTimeOut = 1000;

            // First try if currentConnection is open or can be opened
            if (TryConnection(longTimeOut)) return true;

            // Do a quick rescan of all devices in range
            QuickScanDevices();

            if (PersistentSettings)
            {
                // Then try if last stored connection can be opened
                Log(3, "Trying last stored connection");
                if (TryConnection(_bluetoothConnectionManagerSettings.BluetoothAddress, longTimeOut)) return true;
            }

            // Then see if new devices have been added to the list 
            if (NewDevicesScan()) return true;

            foreach (var device in _deviceList)
            {

                Thread.Sleep(100); // Bluetooth devices seem to work more reliably with some waits 
                Log(1, "Trying Device " + device.DeviceName + " (" + device.DeviceAddress + ") " );
                if (TryConnection(device, shortTimeOut)) return true;
            }

            return false;
        }

        private bool ThoroughScan()
        {
            Log(3, "Performing thorough scan");
            const int longTimeOut = 1000;
            const int shortTimeOut = 1000;

            // First try if currentConnection is open or can be opened
            if (TryConnection(longTimeOut)) return true;

            // Do a quick rescan of all devices in range
            ThorougScanForDevices();

            // Then try if last stored connection can be opened
            Log(3, "Trying last stored connection");
            if (TryConnection(_bluetoothConnectionManagerSettings.BluetoothAddress,  longTimeOut)) return true;

            // Then see if new devices have been added to the list 
            if (NewDevicesScan()) return true;

            foreach (var device in _deviceList)
            {
                Thread.Sleep(100); // Bluetooth devices seem to work more reliably with some waits
                if (PairDevice(device))
                {
                    Log(1, "Trying Device " + device.DeviceName + " (" + device.DeviceAddress + ") ");
                    if (TryConnection(device, shortTimeOut)) return true;        
                }
            }
            return false;
        }

        private bool NewDevicesScan()
        {            
            const int shortTimeOut = 200;

            // Then see if port list has changed
            var newDevices = NewDevicesInList();
            if (newDevices.Count == 0) { return false; }
            
            Log(1, "Trying new devices");

            foreach (var device in newDevices)
            {
                if (TryConnection(device, shortTimeOut)) return true;
                Thread.Sleep(100);
            }
            return false;
        }

        private List<BluetoothDeviceInfo> NewDevicesInList()
        {
            return (from device in _deviceList from prevdevice in _prevDeviceList where device.DeviceAddress != prevdevice.DeviceAddress select device).ToList();
        }

        protected override void StoreSettings()
        {
            if (!PersistentSettings) return;
            _bluetoothConnectionManagerSettings.BluetoothAddress = _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceAddress;

            _bluetoothConnectionStorer.StoreSettings(_bluetoothConnectionManagerSettings);
        }

        protected override sealed void ReadSettings()
        {
            if (!PersistentSettings) return;
            _bluetoothConnectionManagerSettings = _bluetoothConnectionStorer.RetrieveSettings();
        }
    }
}
