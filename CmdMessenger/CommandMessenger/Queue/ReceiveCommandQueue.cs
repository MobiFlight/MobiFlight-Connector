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

namespace CommandMessenger.Queue
{
    /// <summary> Queue of received commands.  </summary>
    public class ReceiveCommandQueue : CommandQueue
    {
        public delegate void HandleReceivedCommandDelegate(ReceivedCommand receivedCommand);

        public event EventHandler<CommandEventArgs> NewLineReceived;

        private readonly HandleReceivedCommandDelegate _receivedCommandHandler;
        private readonly ReceivedCommandSignal _receivedCommandSignal = new ReceivedCommandSignal();

        public ReceiveCommandQueue(HandleReceivedCommandDelegate receivedCommandHandler)
        {
            _receivedCommandHandler = receivedCommandHandler;
        }

        /// <summary> Dequeue the received command. </summary>
        /// <returns> The received command. </returns>
        public ReceivedCommand DequeueCommand()
        {
            lock (Queue)
            {
                return DequeueCommandInternal();
            }        
        }

        protected override bool ProcessQueue()
        {
            ReceivedCommand dequeueCommand;
            bool hasMoreWork;

            lock (Queue)
            {
                dequeueCommand = DequeueCommandInternal();
                hasMoreWork = !IsEmpty;
            }

            if (dequeueCommand != null)
            {
                _receivedCommandHandler(dequeueCommand);
            }

            return hasMoreWork;
        }

        public void PrepareForCmd(int cmdId, SendQueue sendQueueState)
        {
            _receivedCommandSignal.PrepareForWait(cmdId, sendQueueState);
        }

        public ReceivedCommand WaitForCmd(int timeOut)
        {
            return _receivedCommandSignal.WaitForCmd(timeOut);
        }

        /// <summary> Queue the received command. </summary>
        /// <param name="receivedCommand"> The received command. </param>
        public void QueueCommand(ReceivedCommand receivedCommand)
        {
            QueueCommand(new CommandStrategy(receivedCommand));
        }

        /// <summary> Queue the command wrapped in a command strategy. </summary>
        /// <param name="commandStrategy"> The command strategy. </param>
        public override void QueueCommand(CommandStrategy commandStrategy)
        {
            if (IsSuspended)
            {
                // Directly send this command to waiting thread
                var addToQueue = _receivedCommandSignal.ProcessCommand((ReceivedCommand)commandStrategy.Command);
                // check if the item needs to be added to the queue for later processing. If not return directly
                if (!addToQueue) return;
            }

            lock (Queue)
            {
                // Process all generic enqueue strategies
                Queue.Enqueue(commandStrategy);
                foreach (var generalStrategy in GeneralStrategies) { generalStrategy.OnEnqueue(); }
            }

            if (!IsSuspended)
            {
                // Give a signal to indicate that a new item has been queued
                SignalWorker();
                if (NewLineReceived != null) NewLineReceived(this, new CommandEventArgs(commandStrategy.Command));
            }
        }

        private ReceivedCommand DequeueCommandInternal()
        {
            ReceivedCommand receivedCommand = null;
            if (!IsEmpty)
            {
                foreach (var generalStrategy in GeneralStrategies) { generalStrategy.OnDequeue(); }
                var commandStrategy = Queue.Dequeue();
                receivedCommand = (ReceivedCommand)commandStrategy.Command;
            }
            return receivedCommand;
        }
    }
}
