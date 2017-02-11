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

namespace CommandMessenger.Queue
{
    // Command queue base object. 
    public abstract class CommandQueue : IDisposable
    {
        private readonly AsyncWorker _worker;

        protected readonly ListQueue<CommandStrategy> Queue = new ListQueue<CommandStrategy>();   // Buffer for commands
        protected readonly List<GeneralStrategy> GeneralStrategies = new List<GeneralStrategy>(); // Buffer for command independent strategies

        public bool IsRunning { get { return _worker.IsRunning; } }
        public bool IsSuspended { get { return _worker.IsSuspended; } }

        /// <summary>Gets count of records in queue. NOT THREAD-SAFE.</summary>
        public int Count
        {
            get { return Queue.Count; }
        }

        /// <summary>Gets is queue is empty. NOT THREAD-SAFE.</summary>
        public bool IsEmpty
        {
            get { return Queue.Count == 0; }
        }

        /// <summary> Clears the queue. </summary>
        public void Clear()
        {
            lock (Queue) Queue.Clear();
        }

        protected CommandQueue()
        {
            _worker = new AsyncWorker(ProcessQueue, "CommandQueue");
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

        /// <summary> 
        /// Queue the command wrapped in a command strategy. 
        /// Call SignalWaiter method to continue processing of queue.
        /// </summary>
        /// <param name="commandStrategy"> The command strategy. </param>
        public abstract void QueueCommand(CommandStrategy commandStrategy);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            _worker.Start();
        }

        public void Stop()
        {
            _worker.Stop();
            Clear();
        }

        public void Suspend()
        {
            _worker.Suspend();
        }

        public void Resume()
        {
            _worker.Resume();
        }

        protected void SignalWorker()
        {
            _worker.Signal();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        /// <summary> Process the queue. </summary>
        protected abstract bool ProcessQueue();
    }
}
