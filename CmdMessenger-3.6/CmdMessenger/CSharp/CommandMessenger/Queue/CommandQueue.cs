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
using System.Threading;

namespace CommandMessenger
{
    // Command queue base object. 
    public class CommandQueue : DisposableObject
    {
        protected readonly Thread QueueThread;
        protected readonly ListQueue<CommandStrategy> Queue = new ListQueue<CommandStrategy>();   // Buffer for commands
        protected readonly List<GeneralStrategy> GeneralStrategies = new List<GeneralStrategy>(); // Buffer for command independent strategies
        protected readonly CmdMessenger CmdMessenger;
        private ThreadRunStates _threadRunState;
        protected readonly EventWaiter EventWaiter;
        protected object ThreadRunStateLock = new object();

        /// <summary> Run state of thread running the queue.  </summary>
        public enum ThreadRunStates
        {
            Start,
            Started,
            Stop,
            Stopped,
            Abort,
        }

        /// <summary> Gets or sets the run state of the thread . </summary>
        /// <value> The thread run state. </value>
        public ThreadRunStates ThreadRunState  
        {
            set
            {
                lock (ThreadRunStateLock)
                {
                    _threadRunState = value;
                }
            }
            get
            {
                var result = ThreadRunStates.Start;
                lock (ThreadRunStateLock)
                {
                    result = _threadRunState;
                }
                return result;
            }
        }

        /// <summary> Gets or sets the run state of the thread . </summary>
        /// <value> The thread run state. </value>
        public int Count
        {
            get { return Queue.Count; }
        }

        /// <summary> command queue constructor. </summary>
        /// <param name="disposeStack"> DisposeStack. </param>
        /// <param name="cmdMessenger"> The command messenger. </param>
        public CommandQueue(DisposeStack disposeStack, CmdMessenger cmdMessenger) 
        {
            CmdMessenger = cmdMessenger;
            disposeStack.Push(this);

            EventWaiter = new EventWaiter();

            // Create queue thread and wait for it to start
            QueueThread = new Thread(ProcessQueue) {Priority = ThreadPriority.Normal};
            QueueThread.Start();
            while (!QueueThread.IsAlive && QueueThread.ThreadState!=ThreadState.Running)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(25));
            }
        }

        /// <summary> Process the queue. </summary>
        protected virtual void ProcessQueue()
        {
        }

        /// <summary> Clears the queue. </summary>
        public void Clear()
        {
            lock (Queue)
            {
                Queue.Clear();
            }
        }

        /// <summary> Queue the command wrapped in a command strategy. </summary>
        /// <param name="commandStrategy"> The command strategy. </param>
        public virtual void QueueCommand(CommandStrategy commandStrategy)
        {
        }

        /// <summary> Adds a general strategy. This strategy is applied to all queued and dequeued commands.  </summary>
        /// <param name="generalStrategy"> The general strategy. </param>
        public void AddGeneralStrategy(GeneralStrategy generalStrategy)
        {
            // Give strategy access to queue
            generalStrategy.CommandQueue = Queue;
            // Add to general strategy list
            GeneralStrategies.Add(generalStrategy);
        }

        /// <summary> Kills this object. </summary>
        public void Kill()
        {
            ThreadRunState = ThreadRunStates.Abort;
            EventWaiter.Quit();
            //Wait for thread to die
            Join(2000);
            if (QueueThread.IsAlive) QueueThread.Abort();
        }

        /// <summary> Joins the thread. </summary>
        /// <param name="millisecondsTimeout"> The milliseconds timeout. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Join(int millisecondsTimeout)
        {
            if (QueueThread.IsAlive == false) return true;
            return QueueThread.Join(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        // Dispose
        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
        /// <param name="disposing"> true if resources should be disposed, false if not. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Stop polling
                Kill();
            }
            base.Dispose(disposing);
        }

    }
}
