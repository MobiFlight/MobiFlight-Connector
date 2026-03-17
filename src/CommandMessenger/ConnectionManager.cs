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
using System.Threading;
using System.Windows.Forms;

namespace CommandMessenger
{
    public class ConnectionManagerProgressEventArgs : EventArgs
    {
        public int Level { get; set; }
        public String Description { get; set; }
    }

    public enum Mode
    {
        Wait,
        Connect,
        Scan,
        Watchdog
    }

    public enum DeviceStatus
    {
        NotAvailable,
        Available,
        IdentityMismatch
    }

    public abstract class ConnectionManager : IDisposable 
    {
        public event EventHandler<EventArgs> ConnectionTimeout;
        public event EventHandler<EventArgs> ConnectionFound;
        public event EventHandler<ConnectionManagerProgressEventArgs> Progress;

        protected Mode ConnectionManagerMode = Mode.Wait;

        private readonly CmdMessenger _cmdMessenger;
        private readonly AsyncWorker _worker;
        private readonly int _identifyCommandId;
        private readonly string _uniqueDeviceId;

        private long _lastCheckTime;
        private long _nextTimeOutCheck;
        private uint _watchdogTries;
        private bool _watchdogEnabled;

        /// <summary>
        /// Is connection manager currently connected to device.
        /// </summary>
        public bool Connected { get; protected set; }

        public int WatchdogTimeout { get; set; }
        public int WatchdogRetryTimeout { get; set; }
        public uint WatchdogTries { get; set; }

        /// <summary>
        /// Enables or disables connection watchdog functionality using identify command and unique device id.
        /// </summary>
        public bool WatchdogEnabled
        {
            get { return _watchdogEnabled; }
            set
            {
                if (value && string.IsNullOrEmpty(_uniqueDeviceId))
                    throw new InvalidOperationException("Watchdog can't be enabled without Unique Device ID.");
                _watchdogEnabled = value;
            }
        }

        /// <summary>
        /// Enables or disables device scanning. 
        /// When disabled, connection manager will try to open connection to the device configured in the setting.
        /// - For SerialConnection this means scanning for (virtual) serial ports, 
        /// - For BluetoothConnection this means scanning for a device on RFCOMM level
        /// </summary>
        public bool DeviceScanEnabled { get; set; }

        /// <summary>
        /// Enables or disables storing of last connection configuration in persistent file.
        /// </summary>
        public bool PersistentSettings { get;  set; }

        protected ConnectionManager(CmdMessenger cmdMessenger, int identifyCommandId = 0, string uniqueDeviceId = null)
        {
            if (cmdMessenger == null)
                throw new ArgumentNullException("cmdMessenger", "Command Messenger is null.");

            _cmdMessenger = cmdMessenger;
            _identifyCommandId = identifyCommandId;
            _uniqueDeviceId = uniqueDeviceId;

            WatchdogTimeout = 3000;
            WatchdogRetryTimeout = 1500;        
            WatchdogTries = 3;
            WatchdogEnabled = false;

            PersistentSettings = false;
            DeviceScanEnabled = true;

            _worker = new AsyncWorker(DoWork, "ConnectionManager");

            if (!string.IsNullOrEmpty(uniqueDeviceId))
                _cmdMessenger.Attach(identifyCommandId, OnIdentifyResponse);
        }

        /// <summary>
        /// Start connection manager.
        /// </summary>
        public virtual void StartConnectionManager()
        {
            if (!_worker.IsRunning) _worker.Start();

            if (DeviceScanEnabled)
            {
                StartScan();
            }
            else
            {
                StartConnect();
            }
        }

        /// <summary>
        /// Stop connection manager.
        /// </summary>
        public virtual void StopConnectionManager()
        {
            if (_worker.IsRunning) _worker.Stop();
            Disconnect();
        }

        protected virtual void ConnectionFoundEvent()
        {
            ConnectionManagerMode = Mode.Wait;

            if (WatchdogEnabled) StartWatchDog();

            InvokeEvent(ConnectionFound, EventArgs.Empty);
        }

        protected virtual void ConnectionTimeoutEvent()
        {
            ConnectionManagerMode = Mode.Wait;

            Disconnect();

            InvokeEvent(ConnectionTimeout, EventArgs.Empty);

            if (WatchdogEnabled)
            {
                StopWatchDog();

                if (DeviceScanEnabled)
                {
                    StartScan();
                }
                else
                {
                    StartConnect();
                }
            }
        }

        protected virtual void Log(int level, string logMessage)
        {
            var args = new ConnectionManagerProgressEventArgs {Level = level, Description = logMessage};
            InvokeEvent(Progress, args);
        }

        protected virtual void OnIdentifyResponse(ReceivedCommand responseCommand)
        {
            if (responseCommand.Ok && !string.IsNullOrEmpty(_uniqueDeviceId))
            {
                ValidateDeviceUniqueId(responseCommand);
            }
        }

        private void InvokeEvent<TEventHandlerArguments>(EventHandler<TEventHandlerArguments> eventHandler,
            TEventHandlerArguments eventHandlerArguments) where TEventHandlerArguments : EventArgs
        {
            var ctrlToInvoke = _cmdMessenger.ControlToInvokeOn;

            if (eventHandler == null || (ctrlToInvoke != null && ctrlToInvoke.IsDisposed)) return;
            if (ctrlToInvoke != null )
            {
                try { ctrlToInvoke.BeginInvoke((MethodInvoker)(() => eventHandler(this, eventHandlerArguments))); } catch { }
            }
            else
            {   
				//Invoke here             
                try { eventHandler.BeginInvoke(this, eventHandlerArguments, null, null); } catch { }
                
            }
        }

        private bool DoWork()
        {
            // Switch between waiting, device scanning and watchdog 
            switch (ConnectionManagerMode)
            {
                case Mode.Scan:
                    DoWorkScan();
                    break;
                case Mode.Connect:
                    DoWorkConnect();
                    break;
                case Mode.Watchdog:
                    DoWorkWatchdog();
                    break;
            }

            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        ///  Check if Arduino is available
        /// </summary>
        /// <param name="timeOut">Timout for waiting on response</param>
        /// <returns>Check result.</returns>
        protected DeviceStatus ArduinoAvailable(int timeOut)
        {
            var challengeCommand = new SendCommand(_identifyCommandId, _identifyCommandId, timeOut);
            var responseCommand = _cmdMessenger.SendCommand(challengeCommand, SendQueue.InFrontQueue, ReceiveQueue.Default, UseQueue.BypassQueue);

            if (responseCommand.Ok && !string.IsNullOrEmpty(_uniqueDeviceId))
            {
                return ValidateDeviceUniqueId(responseCommand) ? DeviceStatus.Available : DeviceStatus.IdentityMismatch;
            }

            return responseCommand.Ok ? DeviceStatus.Available : DeviceStatus.NotAvailable;
        }

        /// <summary>
        ///  Check if Arduino is available
        /// </summary>
        /// <param name="timeOut">Timout for waiting on response</param>
        /// <param name="tries">Number of tries</param>
        /// <returns>Check result.</returns>
        protected DeviceStatus ArduinoAvailable(int timeOut, int tries)
        {
            for (var i = 1; i <= tries; i++)
            {
                Log(3, "Polling Arduino, try # " + i);

                DeviceStatus status = ArduinoAvailable(timeOut);
                if (status == DeviceStatus.Available 
                    || status == DeviceStatus.IdentityMismatch) return status;
            }
            return DeviceStatus.NotAvailable;
        }

        protected virtual bool ValidateDeviceUniqueId(ReceivedCommand responseCommand)
        {
            bool valid = _uniqueDeviceId == responseCommand.ReadStringArg();
            if (!valid)
            {
                Log(3, "Invalid device response. Device ID mismatch.");
            }

            return valid;
        }

        //Try to connect using current connections settings
        protected abstract void DoWorkConnect();

        // Perform scan to find connected systems
        protected abstract void DoWorkScan();

        protected virtual void DoWorkWatchdog()
        {
            var lastLineTimeStamp = _cmdMessenger.LastReceivedCommandTimeStamp;
            var currentTimeStamp = TimeUtils.Millis;

            // If timeout has not elapsed, wait till next watch time
            if (currentTimeStamp < _nextTimeOutCheck) return;

            // if a command has been received recently, set next check time
            if (lastLineTimeStamp >= _lastCheckTime)
            {
                Log(3, "Successful watchdog response.");
                _lastCheckTime = currentTimeStamp;
                _nextTimeOutCheck = _lastCheckTime + WatchdogTimeout;
                _watchdogTries = 0;
                return;
            }

            // Apparently, other side has not reacted in time
            // If too many tries, notify and stop
            if (_watchdogTries >= WatchdogTries)
            {
                Log(2, "Watchdog received no response after final try #" + WatchdogTries);
                _watchdogTries = 0;
                ConnectionManagerMode = Mode.Wait;
                ConnectionTimeoutEvent();
                return;
            }

            // We'll try another time
            // We queue the command in order to not be intrusive, but put it in front to get a quick answer
            _cmdMessenger.SendCommand(new SendCommand(_identifyCommandId));
            _watchdogTries++;

            _lastCheckTime = currentTimeStamp;
            _nextTimeOutCheck = _lastCheckTime + WatchdogRetryTimeout;
            Log(3, _watchdogTries == 1 ? 
                "Watchdog detected no communication for " + WatchdogTimeout/1000.0 + "s, asking for response" 
                : "Watchdog received no response, performing try #" + _watchdogTries);
        }

        /// <summary>
        /// Disconnect from Arduino
        /// </summary>
        /// <returns>true if sucessfully disconnected</returns>
        private bool Disconnect()
        {
            if (Connected)
            {
                Connected = false;
                return _cmdMessenger.Disconnect();
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Start watchdog. Will check if connection gets interrupted
        /// </summary>
        protected virtual void StartWatchDog()
        {
            if (ConnectionManagerMode != Mode.Watchdog && Connected)
            {
                Log(1, "Starting Watchdog.");
                _lastCheckTime = TimeUtils.Millis;
                _nextTimeOutCheck = _lastCheckTime + WatchdogTimeout;
                _watchdogTries = 0;

                ConnectionManagerMode = Mode.Watchdog;
            }
        }

        /// <summary>
        /// Stop watchdog.
        /// </summary>
        protected virtual void StopWatchDog()
        {
            if (ConnectionManagerMode == Mode.Watchdog)
            {
                Log(1, "Stopping Watchdog.");
                ConnectionManagerMode = Mode.Wait;
            }
        }

        /// <summary>
        /// Start scanning for devices
        /// </summary>
        protected virtual void StartScan()
        {
            if (ConnectionManagerMode != Mode.Scan && !Connected)
            {
                Log(1, "Starting device scan.");
                ConnectionManagerMode = Mode.Scan;
            }
        }

        /// <summary>
        /// Stop scanning for devices
        /// </summary>
        protected virtual void StopScan()
        {
            if (ConnectionManagerMode == Mode.Scan)
            {
                Log(1, "Stopping device scan.");
                ConnectionManagerMode = Mode.Wait;
            }
        }

        /// <summary>
        /// Start connect to device
        /// </summary>
        protected virtual void StartConnect()
        {
            if (ConnectionManagerMode != Mode.Connect && !Connected)
            {
                Log(1, "Start connecting to device.");
                ConnectionManagerMode = Mode.Connect;
            }
        }

        /// <summary>
        /// Stop connect to device
        /// </summary>
        protected virtual void StopConnect()
        {
            if (ConnectionManagerMode == Mode.Connect)
            {
                Log(1, "Stop connecting to device.");
                ConnectionManagerMode = Mode.Wait;
            }
        }

        protected virtual void StoreSettings() { }

        protected virtual void ReadSettings() { }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopConnectionManager();
            }
        }
    }
}


