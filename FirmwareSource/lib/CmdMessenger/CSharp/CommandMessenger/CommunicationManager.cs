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
using System.Text;
using CommandMessenger.TransportLayer;

namespace CommandMessenger
{
    /// <summary>Fas
    /// Manager for serial port data
    /// </summary>
    public class CommunicationManager : DisposableObject
    {
        private ITransport _transport;
        public readonly Encoding StringEncoder = Encoding.GetEncoding("ISO-8859-1");	// The string encoder
        private string _buffer = "";	                                                // The buffer

        private ReceiveCommandQueue _receiveCommandQueue;
        private IsEscaped _isEscaped;                                           // The is escaped
        private char _fieldSeparator;                                       // The field separator
        private char _commandSeparator;                                     // The command separator
        private char _escapeCharacter;                                      // The escape character


        /// <summary> Default constructor. </summary>
        /// /// <param name="disposeStack"> The DisposeStack</param>
        /// <param name="transport"> The Transport Layer</param>
        /// <param name="receiveCommandQueue"></param>
        public CommunicationManager(DisposeStack disposeStack, ITransport transport, ReceiveCommandQueue receiveCommandQueue)
        {
            Initialize(disposeStack,transport, receiveCommandQueue, ';', ',', '/');
        }

        /// <summary> Constructor. </summary>
        /// <param name="receiveCommandQueue"></param>
        /// <param name="commandSeparator">    The End-Of-Line separator. </param>
        /// <param name="fieldSeparator"></param>
        /// <param name="escapeCharacter"> The escape character. </param>
        /// <param name="disposeStack"> The DisposeStack</param>
        /// <param name="transport"> The Transport Layer</param>
        public CommunicationManager(DisposeStack disposeStack,ITransport transport, ReceiveCommandQueue receiveCommandQueue, char commandSeparator,  char fieldSeparator, char escapeCharacter)
        {
            Initialize(disposeStack, transport, receiveCommandQueue, commandSeparator, fieldSeparator, escapeCharacter);
        }

        /// <summary> Finaliser. </summary>
        ~CommunicationManager()
        {
            Dispose(false);
        }

        /// <summary> Initializes this object. </summary>
        /// <param name="receiveCommandQueue"></param>
        /// <param name="commandSeparator">    The End-Of-Line separator. </param>
        /// <param name="fieldSeparator"></param>
        /// <param name="escapeCharacter"> The escape character. </param>
        /// <param name="disposeStack"> The DisposeStack</param>
        /// /// <param name="transport"> The Transport Layer</param>
        public void Initialize(DisposeStack disposeStack, ITransport transport, ReceiveCommandQueue receiveCommandQueue, char commandSeparator, char fieldSeparator, char escapeCharacter)
        {
            disposeStack.Push(this);
            _transport = transport;
            _receiveCommandQueue = receiveCommandQueue;
            _transport.NewDataReceived += NewDataReceived;
            _commandSeparator = commandSeparator;
            _fieldSeparator = fieldSeparator;
            _escapeCharacter = escapeCharacter;

            _isEscaped = new IsEscaped();           
        }

        #region Fields

        /// <summary> Gets or sets the time stamp of the last received line. </summary>
        /// <value> time stamp of the last received line. </value>
        public long LastLineTimeStamp { get; set; }

        #endregion

        #region Properties


        #endregion

        #region Event handlers

        /// <summary> Serial port data received. </summary>
        private void NewDataReceived(object o, EventArgs e)
        {
            ParseLines();
        }

        #endregion

        #region Methods

        /// <summary> Connects to a serial port defined through the current settings. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool StartListening()
        {
            return _transport.StartListening();
        }

        /// <summary> Stops listening to the serial port. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool StopListening()
        {
            return _transport.StopListening();
        }

        /// <summary> Writes a string to the serial port. </summary>
        /// <param name="value"> The string to write. </param>
        public void WriteLine(string value)
        {
            byte[] writeBytes = StringEncoder.GetBytes(value + '\n');
            _transport.Write(writeBytes);
        }

        /// <summary> Writes a parameter to the serial port followed by a NewLine. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="value"> The value. </param>
        public void WriteLine<T>(T value)
        {        
            var writeString = value.ToString();
            byte[] writeBytes = StringEncoder.GetBytes(writeString + '\n');
            _transport.Write(writeBytes);
        }

        /// <summary> Writes a parameter to the serial port. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="value"> The value. </param>
        public void Write<T>(T value)
        {
            var writeString = value.ToString();
            byte[] writeBytes = StringEncoder.GetBytes(writeString);
            _transport.Write(writeBytes);
        }


        /// <summary> Reads the serial buffer into the string buffer. </summary>
        private void ReadInBuffer()
        {
            var data = _transport.Read();
            _buffer += StringEncoder.GetString(data);
        }

        private void ParseLines()
        {
            LastLineTimeStamp = TimeUtils.Millis;
            ReadInBuffer();
            var currentLine = ParseLine();
            while (!String.IsNullOrEmpty(currentLine))
            {
                ProcessLine(currentLine);
                currentLine = ParseLine();
            }
            //Send data to whom ever interested
            //if (newDataAvailable && NewLinesReceived != null)
            //    NewLinesReceived(this, null);
        }

        /// <summary> Converts lines on . </summary>
        public void ProcessLine(string line)
        {            
                // Read line from raw buffer and make command
                var currentReceivedCommand = ParseMessage(line);
                currentReceivedCommand.rawString = line;
                // Set time stamp
                currentReceivedCommand.TimeStamp = LastLineTimeStamp;
                // And put on queue
                _receiveCommandQueue.QueueCommand(currentReceivedCommand);

        }

        /// <summary> Parse message. </summary>
        /// <param name="line"> The received command line. </param>
        /// <returns> The received command. </returns>
        private ReceivedCommand ParseMessage(string line)
        {
            // Trim and clean line
            var cleanedLine = line.Trim('\r', '\n');
            cleanedLine = Escaping.Remove(cleanedLine, _commandSeparator, _escapeCharacter);

            return
                new ReceivedCommand(Escaping.Split(cleanedLine, _fieldSeparator, _escapeCharacter,
                                    StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary> Reads a single line from the serial buffer, if complete. </summary>
        /// <returns> Whether a complete line was present in the serial buffer. </returns>
        private string ParseLine()
        {

                if (_buffer != "")
                {
                    // Check if an End-Of-Line is present in the string, and split on first
                    //var i = _buffer.IndexOf(CommandSeparator);
                    var i = FindNextEol();
                    if (i >= 0 && i < _buffer.Length)
                    {
                        var line = _buffer.Substring(0, i + 1);
                        if (!String.IsNullOrEmpty(line))
                        {
                            _buffer = _buffer.Substring(i + 1);
                            return line;
                        }
                        _buffer = _buffer.Substring(i + 1);
                        return "";
                    }
                }
                return "";

        }

        /// <summary> Searches for the next End-Of-Line. </summary>
        /// <returns> The the location in the string of the next End-Of-Line. </returns>
        private int FindNextEol()
        {
            int pos = 0;
            while (pos < _buffer.Length)
            {
                var escaped = _isEscaped.EscapedChar(_buffer[pos]);
                if (_buffer[pos] == _commandSeparator && !escaped)
                {
                    return pos;
                }
                pos++;
            }
            return pos;
        }

        // Dispose
        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
        /// <param name="disposing"> true if resources should be disposed, false if not. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {                
                // Stop polling
                _transport.NewDataReceived -= NewDataReceived;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}