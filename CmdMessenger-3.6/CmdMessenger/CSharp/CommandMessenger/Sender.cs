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

namespace CommandMessenger
{
    public class Sender
    {
        readonly CommunicationManager _communicationManager;
        readonly ReceiveCommandQueue _receiveCommandQueue;
        private readonly Object _sendCommandDataLock = new Object();        // The process serial data lock
                
        /// <summary> Gets or sets the current received command. </summary>
        /// <value> The current received command. </value>
        public ReceivedCommand CurrentReceivedCommand { get; private set; }

        /// <summary> Gets or sets a whether to print a line feed carriage return after each command. </summary>
        /// <value> true if print line feed carriage return, false if not. </value>
        public bool PrintLfCr { get; set; }

        public Sender(CommunicationManager communicationManager, ReceiveCommandQueue receiveCommandQueue)
        {
            _communicationManager = communicationManager;
            _receiveCommandQueue = receiveCommandQueue;
        }
        
        /// <summary> Directly executes the send command operation. </summary>
        /// <param name="sendCommand">    The command to sent. </param>
        /// <param name="sendQueueState"> Property to optionally clear the send and receive queues. </param>
        /// <returns> A received command. The received command will only be valid if the ReqAc of the command is true. </returns>
        public ReceivedCommand ExecuteSendCommand(SendCommand sendCommand, SendQueue sendQueueState)
        {
            // Disable listening, all callbacks are disabled until after command was sent

            ReceivedCommand ackCommand;
            lock (_sendCommandDataLock)
            {

                if (PrintLfCr)
                    _communicationManager.WriteLine(sendCommand.CommandString());
                else
                    _communicationManager.Write(sendCommand.CommandString());
                ackCommand = sendCommand.ReqAc ? BlockedTillReply(sendCommand.AckCmdId, sendCommand.Timeout, sendQueueState) : new ReceivedCommand();                
                }
                return ackCommand;
            }

        /// <summary> Directly executes the send string operation. </summary>
        /// <param name="commandsString"> The string to sent. </param>
        /// <param name="sendQueueState"> Property to optionally clear the send and receive queues. </param>
        /// <returns> The received command is added for compatibility. It will not yield a response. </returns>
        public ReceivedCommand ExecuteSendString(String commandsString, SendQueue sendQueueState)
        {
            lock (_sendCommandDataLock)
            {
                if (PrintLfCr)
                    _communicationManager.WriteLine(commandsString);
                else
                {
                    _communicationManager.Write(commandsString);
                }
            }            
            return new ReceivedCommand();
        }

                /// <summary> Blocks until acknowledgement reply has been received. </summary>
        /// <param name="ackCmdId"> acknowledgement command ID </param>
        /// <param name="timeout">  Timeout on acknowledge command. </param>
        /// <param name="sendQueueState"></param>
        /// <returns> A received command. </returns>
        private ReceivedCommand BlockedTillReply(int ackCmdId, int timeout, SendQueue sendQueueState)
        {
            // Disable invoking command callbacks
            _receiveCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Stop;

            // Disable thread based polling of Serial Interface
            //_communicationManager.StopPolling();

            var start = TimeUtils.Millis;
            var time = start;
            var acknowledgeCommand = new ReceivedCommand();
            while ((time - start < timeout) && !acknowledgeCommand.Ok)
            {
                time = TimeUtils.Millis;                                
                // Force updating the transport buffer
                //_communicationManager.UpdateTransportBuffer();
                // Yield to other threads in order to process data in the buffer
                Thread.Yield();
                // Check if an acknowledgment command has come in
                acknowledgeCommand = CheckForAcknowledge(ackCmdId, sendQueueState);
            }

            // Re enable thread based polling of Serial Interface
            //_communicationManager.StartPolling();

            // Re-enable invoking command callbacks
            _receiveCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Start;
            return acknowledgeCommand;
        }

        /// <summary> Listen to the receive queue and check for a specific acknowledge command. </summary>
        /// <param name="ackCmdId">        acknowledgement command ID. </param>
        /// <param name="sendQueueState"> Property to optionally clear the send and receive queues. </param>
        /// <returns> The first received command that matches the command ID. </returns>
        private ReceivedCommand CheckForAcknowledge(int ackCmdId, SendQueue sendQueueState)
        {
            // Read command from received queue
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
                if (sendQueueState != SendQueue.ClearQueue)
                {
                    // Add to queue for later processing
                    _receiveCommandQueue.QueueCommand(CurrentReceivedCommand);
                }
            }
            // Return not Ok received command
            return new ReceivedCommand();
        }

    }
}
