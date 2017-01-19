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

namespace CommandMessenger
{
    /// <summary> Queue of received commands.  </summary>
    class SendCommandQueue : CommandQueue
    {
        private readonly QueueSpeed _queueSpeed = new QueueSpeed(0.5);

        /// <summary> send command queue constructor. </summary>
        /// <param name="disposeStack"> DisposeStack. </param>
        /// <param name="cmdMessenger"> The command messenger. </param>
        public SendCommandQueue(DisposeStack disposeStack, CmdMessenger cmdMessenger)
            : base(disposeStack, cmdMessenger)
        {
            QueueThread.Name = "SendCommandQueue";
           // _queueSpeed.Name = "SendCommandQueue";
        }

        /// <summary> Process the queue. </summary>
        protected override void ProcessQueue()
        {
            // Endless loop
            while (ThreadRunState == ThreadRunStates.Start)
            {
                _queueSpeed.SetCount(Queue.Count);
                _queueSpeed.CalcSleepTime();
                _queueSpeed.Sleep();
                SendCommandFromQueue();
            }
        }

        /// <summary> Sends all commands currently on queue. </summary>
        private void SendCommandFromQueue()
        {         
            CommandStrategy commandStrategy;
            lock (Queue)
            {
                commandStrategy = Queue.Count != 0 ? Queue.Peek() : null;
                // Process command specific dequeue strategy
                if (commandStrategy != null)
                { commandStrategy.DeQueue(); }

                // Process all generic dequeue strategies
                foreach (var generalStrategy in GeneralStrategies) { generalStrategy.OnDequeue(); }

            }
            // Send command
            if (commandStrategy != null && commandStrategy.Command != null)
                CmdMessenger.ExecuteSendCommand((SendCommand)commandStrategy.Command, ClearQueue.KeepQueue);    
     
        }

        /// <summary> Sends a command. Note that the command is put at the front of the queue </summary>
        /// <param name="sendCommand"> The command to sent. </param>
        public void SendCommand(SendCommand sendCommand)
        {
            // Add command to front of queue
            QueueCommand(new TopCommandStrategy(sendCommand));
        }

        /// <summary> Queue the send command. </summary>
        /// <param name="sendCommand"> The command to sent. </param>
        public void QueueCommand(SendCommand sendCommand)
        {            
            QueueCommand(new CommandStrategy(sendCommand));
        }

        /// <summary> Queue the send command wrapped in a command strategy. </summary>
        /// <param name="commandStrategy"> The command strategy. </param>
        public override void QueueCommand(CommandStrategy commandStrategy)
        {
            
            lock (Queue)
            {
                // Process commandStrategy enqueue associated with command
                commandStrategy.CommandQueue = Queue;
                commandStrategy.ThreadRunState = ThreadRunState;

                commandStrategy.Enqueue();

                // Process all generic enqueue strategies
                foreach (var generalStrategy in GeneralStrategies) { generalStrategy.OnEnqueue(); }

            }
        }
    }
}
