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
using System.Collections.Generic;
using System.IO.Ports;
using System.ComponentModel;

namespace CommandMessenger
{
    /// <summary>
    /// Class containing properties related to a serial port 
    /// </summary>
    public class SerialSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string _portName = "";
        int _baudRate = 115200;
        readonly List<int> _baudRateCollection = new List<int>();
        Parity _parity = Parity.None;
        int _dataBits = 8;
        int[] _dataBitsCollection = new[] { 5, 6, 7, 8 };
        StopBits _stopBits = StopBits.One;
        private bool _dtrEnable = false;

        #region Properties
        /// <summary>
        /// The port to use (for example, COM1).
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set
            {
                if (!_portName.Equals(value))
                {
                    _portName = value;
                }
            }
        }
        /// <summary>
        /// The baud rate.
        /// </summary>
        public int BaudRate
        {
            get { return _baudRate; }
            set 
            {
                if (_baudRate != value)
                {
                    _baudRate = value;                    
                }
            }
        }

        /// <summary>
        /// One of the Parity values.
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
            set 
            {
                if (_parity != value)
                {
                    _parity = value;
                }
            }
        }
        /// <summary>
        /// The data bits value.
        /// </summary>
        public int DataBits
        {
            get { return _dataBits; }
            set
            {
                if (_dataBits != value)
                {
                    _dataBits = value;
                }
            }
        }
        /// <summary>
        /// One of the StopBits values.
        /// </summary>
        public StopBits StopBits
        {
            get { return _stopBits; }
            set
            {
                if (_stopBits != value)
                {
                    _stopBits = value;
                }
            }
        }

        /// <summary>
        /// Set Data Terminal Ready.
        /// </summary>
        public bool DtrEnable
        {
            get { return _dtrEnable; }
            set
            {
                if (_dtrEnable != value)
                {
                    _dtrEnable = value;
                }
            }
        }

        /// <summary>
        /// Available ports on the computer
        /// </summary>
        public string[] PortNameCollection { get; set; }

        /// <summary>
        /// Available baud rates for current serial port
        /// </summary>
        public List<int> BaudRateCollection
        {
            get { return _baudRateCollection; }
        }

        /// <summary>
        /// Available data bits setting
        /// </summary>
        public int[] DataBitsCollection
        {
            get { return _dataBitsCollection; }
            set { _dataBitsCollection = value; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Updates the range of possible baud rates for device
        /// </summary>
        /// <param name="possibleBaudRates">dwSettableBaud parameter from the COMMPROP Structure</param>
        /// <returns>An updated list of values</returns>
        public void UpdateBaudRateCollection(int possibleBaudRates)
        {
            // ReSharper disable InconsistentNaming
            const int BAUD_075    = 0x00000001;	// The fifth baud 07
            const int BAUD_110    = 0x00000002;
            const int BAUD_150    = 0x00000008;
            const int BAUD_300    = 0x00000010;
            const int BAUD_600    = 0x00000020;
            const int BAUD_1200   = 0x00000040;
            const int BAUD_1800   = 0x00000080;
            const int BAUD_2400   = 0x00000100;
            const int BAUD_4800   = 0x00000200;
            const int BAUD_7200   = 0x00000400;
            const int BAUD_9600   = 0x00000800;
            const int BAUD_14400  = 0x00001000;
            const int BAUD_19200  = 0x00002000;
            const int BAUD_38400  = 0x00004000;
            const int BAUD_56K    = 0x00008000;
            const int BAUD_57600  = 0x00040000;
            const int BAUD_115200 = 0x00020000;
            const int BAUD_128K   = 0x00010000;

            _baudRateCollection.Clear();

            if ((possibleBaudRates & BAUD_075) > 0)
                _baudRateCollection.Add(75);
            if ((possibleBaudRates & BAUD_110) > 0)
                _baudRateCollection.Add(110);
            if ((possibleBaudRates & BAUD_150) > 0)
                _baudRateCollection.Add(150);
            if ((possibleBaudRates & BAUD_300) > 0)
                _baudRateCollection.Add(300);
            if ((possibleBaudRates & BAUD_600) > 0)
                _baudRateCollection.Add(600);
            if ((possibleBaudRates & BAUD_1200) > 0)
                _baudRateCollection.Add(1200);
            if ((possibleBaudRates & BAUD_1800) > 0)
                _baudRateCollection.Add(1800);
            if ((possibleBaudRates & BAUD_2400) > 0)
                _baudRateCollection.Add(2400);
            if ((possibleBaudRates & BAUD_4800) > 0)
                _baudRateCollection.Add(4800);
            if ((possibleBaudRates & BAUD_7200) > 0)
                _baudRateCollection.Add(7200);
            if ((possibleBaudRates & BAUD_9600) > 0)
                _baudRateCollection.Add(9600);
            if ((possibleBaudRates & BAUD_14400) > 0)
                _baudRateCollection.Add(14400);
            if ((possibleBaudRates & BAUD_19200) > 0)
                _baudRateCollection.Add(19200);
            if ((possibleBaudRates & BAUD_38400) > 0)
                _baudRateCollection.Add(38400);
            if ((possibleBaudRates & BAUD_56K) > 0)
                _baudRateCollection.Add(56000);
            if ((possibleBaudRates & BAUD_57600) > 0)
                _baudRateCollection.Add(57600);
            if ((possibleBaudRates & BAUD_115200) > 0)
                _baudRateCollection.Add(115200);
            if ((possibleBaudRates & BAUD_128K) > 0)
                _baudRateCollection.Add(128000);

            SendPropertyChangedEvent("BaudRateCollection");
        }

        /// <summary>
        /// Send a PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of changed property</param>
        private void SendPropertyChangedEvent(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
