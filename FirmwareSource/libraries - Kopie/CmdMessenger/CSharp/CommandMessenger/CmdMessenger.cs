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
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using CommandMessenger.TransportLayer;

namespace CommandMessenger
{
    public enum ClearQueue
    {
        KeepQueue,
        ClearSendQueue,
        ClearReceivedQueue,
        ClearSendAndReceivedQueue,
    }

    /// <summary> Command messenger main class  </summary>
    public class CmdMessenger : DisposableObject
    {
        
        public EventHandler NewLinesReceived;                               // Event handler for new lines received
        public EventHandler NewLineReceived;	                            // Event handler for a new line received
        public EventHandler NewLineSent;	                                // The new line sent
        private readonly Object _sendCommandDataLock = new Object();        // The process serial data lock
       
        private CommunicationManager _communicationManager;                 // The communication manager
        private char _fieldSeparator;                                       // The field separator
        private char _commandSeparator;                                     // The command separator

        private MessengerCallbackFunction _defaultCallback;                 // The default callback
        private Dictionary<int, MessengerCallbackFunction> _callbackList;   // List of callbacks

        private SendCommandQueue _sendCommandQueue;                         // The queue of commands to be sent
        private ReceiveCommandQueue _receiveCommandQueue;                   // The queue of commands to be processed

        //private Logger _sendCommandLogger = new Logger(@"d:\sendCommands.txt");
        /// <summary> Definition of the messenger callback function. </summary>
        /// <param name="receivedCommand"> The received command. </param>
        public delegate void MessengerCallbackFunction(ReceivedCommand receivedCommand);

        /// <summary> Gets or sets a whether to print a line feed carriage return after each command. </summary>
        /// <value> true if print line feed carriage return, false if not. </value>
        public bool PrintLfCr { get; set; }

        /// <summary> Gets or sets the current received command line. </summary>
        /// <value> The current received line. </value>
        public String CurrentReceivedLine { get; private set; }

        /// <summary> Gets or sets the current received command. </summary>
        /// <value> The current received command. </value>
        public ReceivedCommand CurrentReceivedCommand { get; private set; }

        /// <summary> Gets or sets the currently sent line. </summary>
        /// <value> The currently sent line. </value>
        public String CurrentSentLine { get; private set; }

        /// <summary> Gets or sets the log file of send commands. </summary>
        /// <value> The logfile name for send commands. </value>
        //public String LogFileSendCommands
        //{
        //    get { return _sendCommandLogger.LogFileName; }
        //    set { _sendCommandLogger.LogFileName = value; }
        //}

        /// <summary> Gets or sets the log file of receive commands. </summary>
        /// <value> The logfile name for receive commands. </value>
        public String LogFileReceiveCommands { get; set; }

        // The control to invoke the callback on
        private Control _controlToInvokeOn; 

        /// <summary> Constructor. </summary>
        /// <param name="transport"> The transport layer. </param>
        public CmdMessenger(ITransport transport)
        {
            Init(transport, ',', ';', '/');
        }

        /// <summary> Constructor. </summary>
        /// <param name="transport"> The transport layer. </param>
        /// <param name="fieldSeparator"> The field separator. </param>
        public CmdMessenger(ITransport transport, char fieldSeparator)
        {
            Init(transport, fieldSeparator, ';', '/');
        }

        /// <summary> Constructor. </summary>
        /// <param name="transport">   The transport layer. </param>
        /// <param name="fieldSeparator">   The field separator. </param>
        /// <param name="commandSeparator"> The command separator. </param>
        public CmdMessenger(ITransport transport, char fieldSeparator, char commandSeparator)
        {
            Init(transport, fieldSeparator, commandSeparator, commandSeparator);
        }

        /// <summary> Constructor. </summary>
        /// <param name="transport">   The transport layer. </param>
        /// <param name="fieldSeparator">   The field separator. </param>
        /// <param name="commandSeparator"> The command separator. </param>
        /// <param name="escapeCharacter">  The escape character. </param>
        public CmdMessenger(ITransport transport, char fieldSeparator, char commandSeparator,
                            char escapeCharacter)
        {
            Init(transport, fieldSeparator, commandSeparator, escapeCharacter);
        }

        /// <summary> Initialises this object. </summary>
        /// <param name="transport">   The transport layer. </param>
        /// <param name="fieldSeparator">   The field separator. </param>
        /// <param name="commandSeparator"> The command separator. </param>
        /// <param name="escapeCharacter">  The escape character. </param>
        private void Init(ITransport transport, char fieldSeparator, char commandSeparator,
                          char escapeCharacter)
        {           
            _controlToInvokeOn = null;

            _sendCommandQueue = new SendCommandQueue(DisposeStack, this);
            _receiveCommandQueue = new ReceiveCommandQueue(DisposeStack, this);
            _communicationManager = new CommunicationManager(DisposeStack, transport, _receiveCommandQueue, commandSeparator, fieldSeparator, escapeCharacter);

            _fieldSeparator = fieldSeparator;
            _commandSeparator = commandSeparator;

            Escaping.EscapeChars(fieldSeparator, commandSeparator, escapeCharacter);
            _callbackList = new Dictionary<int, MessengerCallbackFunction>();
            PrintLfCr = false;
        }

        public void SetSingleCore()
        {
            Process proc = Process.GetCurrentProcess();
            //var t = proc.Threads[0];
            foreach (ProcessThread pt in proc.Threads)
            {
                if (pt.ThreadState != ThreadState.Terminated)
                {
                    try
                    {
                        pt.IdealProcessor = 0;
                        pt.ProcessorAffinity = (IntPtr) 1;
                    }
                    catch (Exception)
                    {
                    }

                }
            }
        }
        /// <summary> Sets a control to invoke on. </summary>
        /// <param name="controlToInvokeOn"> The control to invoke on. </param>
        public void SetControlToInvokeOn(Control controlToInvokeOn)
        {
            _controlToInvokeOn = controlToInvokeOn;
        }

        /// <summary>  Stop listening and end serial port connection. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool StopListening()
        {
            return _communicationManager.StopListening();
        }

        /// <summary> Starts serial port connection and start listening. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool StartListening()
        {
            if (_communicationManager.StartListening())
            {
                // Timestamp of this command is same as time stamp of serial line
                LastLineTimeStamp = _communicationManager.LastLineTimeStamp;
                return true;
            }
            return false;
        }

        /// <summary> Attaches default callback for unsupported commands. </summary>
        /// <param name="newFunction"> The callback function. </param>
        public void Attach(MessengerCallbackFunction newFunction)
        {
            _defaultCallback = newFunction;
        }

        /// <summary> Attaches default callback for certain Message ID. </summary>
        /// <param name="messageId">   Command ID. </param>
        /// <param name="newFunction"> The callback function. </param>
        public void Attach(int messageId, MessengerCallbackFunction newFunction)
        {
            _callbackList[messageId] = newFunction;
        }

        /// <summary> Gets or sets the time stamp of the last command line received. </summary>
        /// <value> The last line time stamp. </value>
        public long LastLineTimeStamp { get; private set; }


        /// <summary> Handle message. </summary>
        /// <param name="receivedCommand"> The received command. </param>
        public void HandleMessage(ReceivedCommand receivedCommand)
        {
            CurrentReceivedLine = receivedCommand.rawString;
            // Send message that a new line has been received and is due to be processed
            InvokeEvent(NewLineReceived);

            MessengerCallbackFunction callback = null;
            if (receivedCommand.Ok)
            {
                //receivedCommand = new ReceivedCommand(commandString);
                if (_callbackList.ContainsKey(receivedCommand.CmdId))
                {
                    callback = _callbackList[receivedCommand.CmdId];
                }
                else
                {
                    if (_defaultCallback != null) callback = _defaultCallback;
                }
            }
            else
            {
                // Empty command
                receivedCommand = new ReceivedCommand();
            }
            InvokeCallBack(callback, receivedCommand);
        }

        /// <summary> Sends a command. 
        /// 		  If no command acknowledge is requested, the command will be send asynchronously: it will be put on the top of the send queue
        ///  		  If a  command acknowledge is requested, the command will be send synchronously:  the program will block until the acknowledge command 
        ///  		  has been received or the timeout has expired. </summary>
        /// <param name="sendCommand"> The command to sent. </param>
        public ReceivedCommand SendCommand(SendCommand sendCommand)
        {
            return SendCommand(sendCommand, ClearQueue.KeepQueue);
        }

        /// <summary> Sends a command. 
        /// 		  If no command acknowledge is requested, the command will be send asynchronously: it will be put on the top of the send queue
        ///  		  If a  command acknowledge is requested, the command will be send synchronously:  the program will block until the acknowledge command 
        ///  		  has been received or the timeout has expired.
        ///  		  Based on ClearQueueState, the send- and receive-queues are left intact or are cleared</summary>
        /// <param name="sendCommand"> The command to sent. </param>
        /// <param name="clearQueueState"> Property to optionally clear the send and receive queues</param>
        /// <returns> A received command. The received command will only be valid if the ReqAc of the command is true. </returns>
        public ReceivedCommand SendCommand(SendCommand sendCommand, ClearQueue clearQueueState)
        {
            //_sendCommandLogger.LogLine(CommandToString(sendCommand) + _commandSeparator);

            if (clearQueueState == ClearQueue.ClearReceivedQueue || 
                clearQueueState == ClearQueue.ClearSendAndReceivedQueue)
            {
                // Clear receive queue
                _receiveCommandQueue.Clear(); 
            }

            if (clearQueueState == ClearQueue.ClearSendQueue || 
                clearQueueState == ClearQueue.ClearSendAndReceivedQueue)
            {
                // Clear send queue
                _sendCommandQueue.Clear();
            }

            if (sendCommand.ReqAc)
            {
                // Directly call execute command
                return ExecuteSendCommand(sendCommand, clearQueueState);
            }
            
            // Put command at top of command queue
            _sendCommandQueue.SendCommand(sendCommand);
            return new ReceivedCommand();
        }

        /// <summary> Directly executes the send command operation. </summary>
        /// <param name="sendCommand">     The command to sent. </param>
        /// <param name="clearQueueState"> Property to optionally clear the send and receive queues. </param>
        /// <returns> A received command. The received command will only be valid if the ReqAc of the command is true. </returns>
        public ReceivedCommand ExecuteSendCommand(SendCommand sendCommand, ClearQueue clearQueueState)
        {
            // Disable listening, all callbacks are disabled until after command was sent

            lock (_sendCommandDataLock)
            {

                CurrentSentLine = CommandToString(sendCommand);
 

                if (PrintLfCr)
                    _communicationManager.WriteLine(CurrentSentLine + _commandSeparator);
                else
                {
                    _communicationManager.Write(CurrentSentLine + _commandSeparator);
                }
                InvokeEvent(NewLineSent);
                var ackCommand = sendCommand.ReqAc ? BlockedTillReply(sendCommand.AckCmdId, sendCommand.Timeout, clearQueueState) : new ReceivedCommand();
                return ackCommand;
            }

        }

        string CommandToString(Command command)
        {
            var commandString = command.CmdId.ToString(CultureInfo.InvariantCulture);

            foreach (var argument in command.Arguments)
            {
                commandString += _fieldSeparator + argument;
            }
            return commandString;
        }

        /// <summary> Put the command at the back of the sent queue.</summary>
        /// <param name="sendCommand"> The command to sent. </param>
        public void QueueCommand(SendCommand sendCommand)
        {
            _sendCommandQueue.QueueCommand(sendCommand);
        }

        /// <summary> Put  a command wrapped in a strategy at the back of the sent queue.</summary>
        /// <param name="commandStrategy"> The command strategy. </param>
        public void QueueCommand(CommandStrategy commandStrategy)
        {
            _sendCommandQueue.QueueCommand(commandStrategy);
        }

        /// <summary> Adds a general command strategy to the receive queue. This will be executed on every enqueued and dequeued command.  </summary>
        /// <param name="generalStrategy"> The general strategy for the receive queue. </param>
        public void AddReceiveCommandStrategy(GeneralStrategy generalStrategy) 
        {
            _receiveCommandQueue.AddGeneralStrategy(generalStrategy);
        }

        /// <summary> Adds a general command strategy to the send queue. This will be executed on every enqueued and dequeued command.  </summary>
        /// <param name="generalStrategy"> The general strategy for the send queue. </param>
        public void AddSendCommandStrategy(GeneralStrategy generalStrategy)
        {
            _sendCommandQueue.AddGeneralStrategy(generalStrategy);
        }

        /// <summary> Clears the receive queue. </summary>
        public void ClearReceiveQueue()
        {
            _receiveCommandQueue.Clear();
        }

        /// <summary> Clears the send queue. </summary>
        public void ClearSendQueue()
        {
            _sendCommandQueue.Clear();
        }

        /// <summary> Helper function to Invoke or directly call event. </summary>
        /// <param name="eventHandler"> The event handler. </param>
        private void InvokeEvent(EventHandler eventHandler)
        {
            try
            {
                if (eventHandler != null)
                {
                    if (_controlToInvokeOn != null && _controlToInvokeOn.InvokeRequired)
                    {
                        //Asynchronously call on UI thread
                        _controlToInvokeOn.Invoke(eventHandler, null);
                    }
                    else
                    {
                        //Directly call
                        eventHandler(this, null);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary> Helper function to Invoke or directly call callback function. </summary>
        /// <param name="messengerCallbackFunction"> The messenger callback function. </param>
        /// <param name="command">                   The command. </param>
        private void InvokeCallBack(MessengerCallbackFunction messengerCallbackFunction, ReceivedCommand command)
        {
            if (messengerCallbackFunction != null)
            {
                if (_controlToInvokeOn != null && _controlToInvokeOn.InvokeRequired)
                {
                    //Asynchronously call on UI thread
                    _controlToInvokeOn.Invoke(new MessengerCallbackFunction(messengerCallbackFunction), (object) command);
                }
                else
                {
                    //Directly call
                    messengerCallbackFunction(command);
                }
            }
        }

        /// <summary> Blocks until acknowlegdement reply has been received. </summary>
        /// <param name="ackCmdId"> acknowledgement command ID </param>
        /// <param name="timeout">  Timeout on acknowlegde command. </param>
        /// <param name="clearQueueState"></param>
        /// <returns> . </returns>
        private ReceivedCommand BlockedTillReply(int ackCmdId, int timeout, ClearQueue clearQueueState)
        {
            // Disable invoking command callbacks
            _receiveCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Stop;

            var start = TimeUtils.Millis;
            var time = start;
            var acknowledgeCommand = new ReceivedCommand();
            while ((time - start < timeout) && !acknowledgeCommand.Ok)
            {
                time = TimeUtils.Millis;
                acknowledgeCommand = CheckForAcknowledge(ackCmdId, clearQueueState);
            }

            // Re-enable invoking command callbacks
            _receiveCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Start;
            return acknowledgeCommand;
        }

        /// <summary> Listen to the receive queue and check for a specific acknowledge command. </summary>
        /// <param name="ackCmdId">        acknowledgement command ID. </param>
        /// <param name="clearQueueState"> Property to optionally clear the send and receive queues. </param>
        /// <returns> The first received command that matches the command ID. </returns>
        private ReceivedCommand CheckForAcknowledge(int ackCmdId, ClearQueue clearQueueState)
        {
            // Read single command from received queue
            CurrentReceivedCommand = _receiveCommandQueue.DequeueCommand();
            if (CurrentReceivedCommand != null)
            {
                // Check if received command is valid
                if (!CurrentReceivedCommand.Ok) return CurrentReceivedCommand;

                // If valid, check if is same as command we are waiting for
                if (CurrentReceivedCommand.CmdId == ackCmdId)
                {
                    // This is command we are waiting for, so return
                    return CurrentReceivedCommand;
                }
                
                // This is not command we are waiting for
                if (clearQueueState == ClearQueue.KeepQueue || clearQueueState == ClearQueue.ClearSendQueue)
                {
                    // Add to queue for later processing
                    _receiveCommandQueue.QueueCommand(CurrentReceivedCommand);
                }
            }
            // Return not Ok received command
            return new ReceivedCommand();
        }

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
        /// <param name="disposing"> true if resources should be disposed, false if not. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controlToInvokeOn = null;
                _receiveCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Stop;
            }
            base.Dispose(disposing);
        }
    }
}