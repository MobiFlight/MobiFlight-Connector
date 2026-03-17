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

namespace CommandMessenger.Transport
{
    /// <summary> Interface for transport layer.  </summary>
    public interface ITransport: IDisposable    
    {
        /// <summary>
        /// Connect transport 
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// Disconnect transport 
        /// </summary>
        /// <returns></returns>
        bool Disconnect();

        /// <summary>
        /// Returns connection status
        /// </summary>
        /// <returns>true when connected</returns>
        bool IsConnected();

        /// <summary>
        /// Bytes read over transport
        /// </summary>
        /// <returns></returns>
        byte[] Read();

        /// <summary>
        /// Write bytes over transport
        /// </summary>
        /// <param name="buffer"></param>
        void Write(byte[] buffer);

        /// <summary>
        /// Bytes have been received event. 
        /// </summary>
        event EventHandler DataReceived; 
    }
}
