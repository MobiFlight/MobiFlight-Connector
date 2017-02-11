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

using System.IO.Ports;

namespace CommandMessenger.Transport.Serial
{
    /// <summary>
    /// Class containing serial port configuration
    /// </summary>
    public class SerialSettings
    {
        #region Properties

        /// <summary>
        /// The port to use (for example: COM1 or /dev/ttyACM1).
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// Port baud rate.
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// One of the Parity values.
        /// </summary>
        public Parity Parity { get; set; }

        /// <summary>
        /// The data bits value.
        /// </summary>
        public int DataBits { get; set; }

        /// <summary>
        /// One of the StopBits values.
        /// </summary>
        public StopBits StopBits { get; set; }

        /// <summary>
        /// Set Data Terminal Ready.
        /// </summary>
        public bool DtrEnable { get; set; }

        /// <summary>
        /// Timeout for read and write operations to serial port.
        /// </summary>
        public int Timeout { get; set; }

        #endregion

        public SerialSettings()
        {
            StopBits = StopBits.One;
            DataBits = 8;
            Parity = Parity.None;
            BaudRate = 9600;
            PortName = string.Empty;
            Timeout = 500;              // 500ms is default value for SerialPort
        }

        /// <summary>
        /// Check is serial settings configured properly.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(PortName) && BaudRate > 0;
        }
    }
}
