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
    public class ReceiveCommandQueue : CommandQueue
    {

        private readonly QueueSpeed _queueSpeed = new QueueSpeed(0.5);

        /// <summary> Receive command queue constructor. </summary>
        /// <param name="disposeStack"> DisposeStack. </param>
        /// <param name="cmdMessenger"> The command messenger. </param>
        public ReceiveCommandQueue(DisposeStack disposeStack, CmdMessenger cmdMessenger )
            : base(disposeStack, cmdMessenger)
        {
            disposeStack.Push(this);
            QueueThread.Name = "ReceiveCommandQueue";
           // _queueSpeed.Name = "ReceiveCommandQueue";
        }

        /// <summary> Dequeue the received command. </summary>
        /// <returns> The received command. </returns>
        public ReceivedCommand DequeueCommand()
        {
            lock (Queue)
            {
                if (Queue.Count != 0)
                {
                    foreach (var generalStrategy in GeneralStrategies) { generalStrategy.OnDequeue(); }
                    var commandStrategy = Queue.Dequeue();
                    return (ReceivedCommand)commandStrategy.Command;                    
                }
                return null;
            }        
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
                
                var dequeueCommand = DequeueCommand();
                if (dequeueCommand != null)
                {
                    CmdMessenger.HandleMessage(dequeueCommand);
                }
            }
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
            lock (Queue)
            {
                // Process all generic enqueue strategies
                Queue.Enqueue(commandStrategy);
                foreach (var generalStrategy in GeneralStrategies) { generalStrategy.OnEnqueue(); }
            }
        }
    }
}
