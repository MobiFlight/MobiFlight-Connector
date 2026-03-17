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
using System.Threading;
using System.Windows.Forms;
using CommandMessenger.Queue;
using CommandMessenger.Transport;

namespace CommandMessenger
{
    public enum SendQueue
    {
        Default,
        InFrontQueue,
        AtEndQueue,
        WaitForEmptyQueue,        
        ClearQueue,
    }

    public enum ReceiveQueue
    {
        Default,
        WaitForEmptyQueue,
        ClearQueue,
    }

    public enum UseQueue
    {
        UseQueue,
        BypassQueue,
    }

    public enum BoardType
    {
        Bit16,
        Bit32,
    }

    /// <summary> Command messenger main class  </summary>
    public class CmdMessenger : IDisposable
    {
        private CommunicationManager _communicationManager;                 // The communication manager
        private MessengerCallbackFunction _defaultCallback;                 // The default callback
        private Dictionary<int, MessengerCallbackFunction> _callbackList;   // List of callbacks
        private SendCommandQueue _sendCommandQueue;                         // The queue of commands to be sent
        private ReceiveCommandQueue _receiveCommandQueue;                   // The queue of commands to be processed

        /// <summary> Definition of the messenger callback function. </summary>
        /// <param name="receivedCommand"> The received command. </param>
        public delegate void MessengerCallbackFunction(ReceivedCommand receivedCommand);

        /// <summary>
        /// Event handler for one or more lines received
        /// </summary>
        public event EventHandler<CommandEventArgs> NewLineReceived;
        
        /// <summary>
        /// Event handler for a new line sent
        /// </summary>
        public event EventHandler<CommandEventArgs> NewLineSent;                            

        /// <summary> Gets or sets a whether to print a line feed carriage return after each command. </summary>
        /// <value> true if print line feed carriage return, false if not. </value>
        public bool PrintLfCr
        {
            get { return _communicationManager.PrintLfCr; }
            set { _communicationManager.PrintLfCr = value; }
        }

        /// <summary>
        /// The control to invoke the callback on
        /// </summary>
        public Control ControlToInvokeOn { get; set; }

        /// <summary> Constructor. </summary>
        /// <param name="transport"> The transport layer. </param>
        /// <param name="boardType"> Embedded Processor type. Needed to translate variables between sides. </param>
        public CmdMessenger(ITransport transport, BoardType boardType = BoardType.Bit16)
        {
            Init(transport, boardType, ',', ';', '/', 60);
        }

        /// <summary> Constructor. </summary>
        /// <param name="transport"> The transport layer. </param>
        /// <param name="sendBufferMaxLength"> The maximum size of the send buffer</param>
        /// <param name="boardType"> Embedded Processor type. Needed to translate variables between sides. </param>
        public CmdMessenger(ITransport transport, int sendBufferMaxLength, BoardType boardType = BoardType.Bit16)
        {
            Init(transport, boardType, ',', ';', '/', sendBufferMaxLength);
        }

        /// <summary> Constructor. </summary>
        /// <param name="transport"> The transport layer. </param>
        /// <param name="boardType"> Embedded Processor type. Needed to translate variables between sides. </param>
        /// <param name="fieldSeparator"> The field separator. </param>
        public CmdMessenger(ITransport transport, BoardType boardType, char fieldSeparator)
        {
            Init(transport, boardType, fieldSeparator, ';', '/', 60);
        }

        /// <summary> Constructor. </summary>
        /// <param name="transport"> The transport layer. </param>
        /// <param name="boardType"> Embedded Processor type. Needed to translate variables between sides. </param>
        /// <param name="fieldSeparator"> The field separator. </param>
        /// <param name="sendBufferMaxLength"> The maximum size of the send buffer</param>
        public CmdMessenger(ITransport transport, BoardType boardType, char fieldSeparator, int sendBufferMaxLength)
        {
            Init(transport, boardType, fieldSeparator, ';', '/', sendBufferMaxLength);
        }

        /// <summary> Constructor. </summary>
        /// <param name="transport">   The transport layer. </param>
        /// <param name="boardType"> Embedded Processor type. Needed to translate variables between sides. </param>
        /// <param name="fieldSeparator">   The field separator. </param>
        /// <param name="commandSeparator"> The command separator. </param>
        public CmdMessenger(ITransport transport, BoardType boardType, char fieldSeparator, char commandSeparator)
        {
            Init(transport, boardType, fieldSeparator, commandSeparator, commandSeparator, 60);
        }

        /// <summary> Constructor. </summary>
        /// <param name="transport">   The transport layer. </param>
        /// <param name="boardType"> Embedded Processor type. Needed to translate variables between sides. </param>
        /// <param name="fieldSeparator">   The field separator. </param>
        /// <param name="commandSeparator"> The command separator. </param>
        /// <param name="escapeCharacter">  The escape character. </param>
        /// <param name="sendBufferMaxLength"> The maximum size of the send buffer</param>
        public CmdMessenger(ITransport transport, BoardType boardType, char fieldSeparator, char commandSeparator,
                            char escapeCharacter, int sendBufferMaxLength)
        {
            Init(transport, boardType, fieldSeparator, commandSeparator, escapeCharacter, sendBufferMaxLength);
        }

        /// <summary> Initializes this object. </summary>
        /// <param name="transport">   The transport layer. </param>
        /// <param name="boardType"> Embedded Processor type. Needed to translate variables between sides. </param>
        /// <param name="fieldSeparator">   The field separator. </param>
        /// <param name="commandSeparator"> The command separator. </param>
        /// <param name="escapeCharacter">  The escape character. </param>
        /// <param name="sendBufferMaxLength"> The maximum size of the send buffer</param>
        private void Init(ITransport transport, BoardType boardType, char fieldSeparator, char commandSeparator,
                          char escapeCharacter, int sendBufferMaxLength)
        {           
            ControlToInvokeOn = null;

            //Logger.Open(@"sendCommands.txt");
            Logger.DirectFlush = true;

            _receiveCommandQueue = new ReceiveCommandQueue(HandleMessage);
            _communicationManager = new CommunicationManager(transport, _receiveCommandQueue, boardType, commandSeparator, fieldSeparator, escapeCharacter);
            _sendCommandQueue = new SendCommandQueue(_communicationManager, sendBufferMaxLength);

            PrintLfCr = false;

            _receiveCommandQueue.NewLineReceived += (o, e) => InvokeNewLineEvent(NewLineReceived, e);
            _sendCommandQueue.NewLineSent        += (o, e) => InvokeNewLineEvent(NewLineSent, e);

            Escaping.EscapeChars(fieldSeparator, commandSeparator, escapeCharacter);
            _callbackList = new Dictionary<int, MessengerCallbackFunction>();

            _sendCommandQueue.Start();
            _receiveCommandQueue.Start();
        }

        /// <summary>
        /// Disposal of CmdMessenger
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Sets a control to invoke on. </summary>
        /// <param name="controlToInvokeOn"> The control to invoke on. </param>
        [Obsolete("Use ControlToInvokeOn property instead.")]
        public void SetControlToInvokeOn(Control controlToInvokeOn)
        {
            ControlToInvokeOn = controlToInvokeOn;
        }

        /// <summary>  Stop listening and end serial port connection. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Disconnect()
        {
            return _communicationManager.Disconnect();
        }

        /// <summary> Starts serial port connection and start listening. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Connect()
        {
            return _communicationManager.Connect();
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
        public long LastReceivedCommandTimeStamp
        {
            get
            {
                return _communicationManager.LastLineTimeStamp;
            }
        }

        /// <summary> Handle message. </summary>
        /// <param name="receivedCommand"> The received command. </param>
        private void HandleMessage(ReceivedCommand receivedCommand)
        {
            MessengerCallbackFunction callback = null;

            if (receivedCommand.Ok)
            {
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
                receivedCommand = new ReceivedCommand { CommunicationManager = _communicationManager };
            }

            InvokeCallBack(callback, receivedCommand);
        }

        /// <summary> Sends a command. 
        /// 		  If no command acknowledge is requested, the command will be send asynchronously: it will be put on the top of the send queue
        ///  		  If a  command acknowledge is requested, the command will be send synchronously:  the program will block until the acknowledge command 
        ///  		  has been received or the timeout has expired.
        ///  		  Based on ClearQueueState, the send- and receive-queues are left intact or are cleared</summary>
        /// <param name="sendCommand">       The command to sent. </param>
        /// <param name="sendQueueState">    Property to optionally clear/wait the send queue</param>
        /// <param name="receiveQueueState"> Property to optionally clear/wait the send queue</param>
        /// <returns> A received command. The received command will only be valid if the ReqAc of the command is true. </returns>
        public ReceivedCommand SendCommand(SendCommand sendCommand, SendQueue sendQueueState = SendQueue.InFrontQueue, ReceiveQueue receiveQueueState = ReceiveQueue.Default)
        {
            return SendCommand(sendCommand, sendQueueState, receiveQueueState, UseQueue.UseQueue);
        }

        /// <summary> Sends a command. 
        /// 		  If no command acknowledge is requested, the command will be send asynchronously: it will be put on the top of the send queue
        ///  		  If a  command acknowledge is requested, the command will be send synchronously:  the program will block until the acknowledge command 
        ///  		  has been received or the timeout has expired.
        ///  		  Based on ClearQueueState, the send- and receive-queues are left intact or are cleared</summary>
        /// <param name="sendCommand">       The command to sent. </param>
        /// <param name="sendQueueState">    Property to optionally clear/wait the send queue</param>
        /// <param name="receiveQueueState"> Property to optionally clear/wait the send queue</param>
        /// <param name="useQueue">          Property to optionally bypass the queue</param>
        /// <returns> A received command. The received command will only be valid if the ReqAc of the command is true. </returns>
        public ReceivedCommand SendCommand(SendCommand sendCommand, SendQueue sendQueueState, ReceiveQueue receiveQueueState, UseQueue useQueue)
        {
            var synchronizedSend = (sendCommand.ReqAc || useQueue == UseQueue.BypassQueue);

            // When waiting for an acknowledge, it is typically best to wait for the ReceiveQueue to be empty
            // This is thus the default state
            if (sendCommand.ReqAc && receiveQueueState == ReceiveQueue.Default)
            {
                receiveQueueState = ReceiveQueue.WaitForEmptyQueue;
            }

            if (sendQueueState == SendQueue.ClearQueue )
            {
                // Clear receive queue
                _receiveCommandQueue.Clear(); 
            }

            if (receiveQueueState == ReceiveQueue.ClearQueue )
            {
                // Clear send queue
                _sendCommandQueue.Clear();
            }

            // If synchronized sending, the only way to get command at end of queue is by waiting
            if (sendQueueState == SendQueue.WaitForEmptyQueue ||
                (synchronizedSend && sendQueueState == SendQueue.AtEndQueue)
            )
            {
                SpinWait.SpinUntil(() => _sendCommandQueue.IsEmpty);
            }
            
            if (receiveQueueState == ReceiveQueue.WaitForEmptyQueue)
            {
                SpinWait.SpinUntil(() => _receiveCommandQueue.IsEmpty);
            }

            if (synchronizedSend)
            {
                return SendCommandSync(sendCommand, sendQueueState);
            }
            
            if (sendQueueState != SendQueue.AtEndQueue)
            {
                // Put command at top of command queue
                _sendCommandQueue.SendCommand(sendCommand);
            }
            else
            {
                // Put command at bottom of command queue
                _sendCommandQueue.QueueCommand(sendCommand);
            }
            return new ReceivedCommand { CommunicationManager = _communicationManager };
        }

        /// <summary> Synchronized send a command. </summary>
        /// <param name="sendCommand">    The command to sent. </param>
        /// <param name="sendQueueState"> Property to optionally clear/wait the send queue. </param>
        /// <returns> . </returns>
        public ReceivedCommand SendCommandSync(SendCommand sendCommand, SendQueue sendQueueState)
        {
            // Directly call execute command
            var resultSendCommand = _communicationManager.ExecuteSendCommand(sendCommand, sendQueueState);
            InvokeNewLineEvent(NewLineSent, new CommandEventArgs(sendCommand));
            return resultSendCommand;            
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
        /// <param name="newLineHandler"> The event handler. </param>
        /// <param name="newLineArgs"></param>
        private void InvokeNewLineEvent(EventHandler<CommandEventArgs> newLineHandler, CommandEventArgs newLineArgs)
        {
            if (newLineHandler == null || (ControlToInvokeOn != null && ControlToInvokeOn.IsDisposed)) return;

            if (ControlToInvokeOn != null)
            {
                //Asynchronously call on UI thread
                ControlToInvokeOn.BeginInvoke((MethodInvoker)(() => newLineHandler(this, newLineArgs)));
            }
            else
            {
                //Directly call
                newLineHandler(this, newLineArgs);
            }
        }

        /// <summary> Helper function to Invoke or directly call callback function. </summary>
        /// <param name="messengerCallbackFunction"> The messenger callback function. </param>
        /// <param name="command">                   The command. </param>
        private void InvokeCallBack(MessengerCallbackFunction messengerCallbackFunction, ReceivedCommand command)
        {
            if (messengerCallbackFunction == null || (ControlToInvokeOn != null && ControlToInvokeOn.IsDisposed)) return;

            if (ControlToInvokeOn != null)
            {
                //Asynchronously call on UI thread
                ControlToInvokeOn.BeginInvoke(new MessengerCallbackFunction(messengerCallbackFunction), (object)command);
            }
            else
            {
                //Directly call
                messengerCallbackFunction(command);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ControlToInvokeOn = null;

                _communicationManager.Dispose();
                _sendCommandQueue.Dispose();
                _receiveCommandQueue.Dispose();
            }
        }
    }
}