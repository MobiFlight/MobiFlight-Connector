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
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CommandMessenger
{
    /// <summary>
    /// Starts a recurring action with fixed interval   
    /// If still running at next call, the action is skipped
    /// </summary>
    public class TimedAction : DisposableObject
    {
        /// <summary> Thread state.  </summary>
        private class ThreadState
        {
            public volatile bool IsRunning;
        }


        private readonly Action _action;	        // The action to execute
        private readonly Timer _actionTimer;	    // The action timer
        private readonly ThreadState _threadState;  // State of the thread

        /// <summary> Returns whether this object is running. </summary>
        /// <value> true if this object is running, false if not. </value>
        public bool IsRunning
        {
            get { return _threadState.IsRunning; }
        }

        /// <summary> Constructor. </summary>
        /// <param name="disposeStack">Dispose stack to add itself to</param>
        /// <param name="interval"> The execution interval. </param>
        /// <param name="action">   The action to execute. </param>
        public TimedAction(DisposeStack disposeStack, double interval, Action action)
        {
            disposeStack.Push(this);
            _action = action;
            _threadState = new ThreadState {IsRunning = false};


            _actionTimer = new Timer(interval) {Enabled = false, SynchronizingObject = null};
            _actionTimer.Elapsed += OnActionTimer;
        }


        /// <summary> Finaliser. </summary>
        ~TimedAction()
        {
            // Stop elapsed event handler
            StopAndWait();
            _actionTimer.Elapsed -= OnActionTimer;
            // Wait until last action has been executed or timeout
        }

        // On timer event run non-blocking action

        /// <summary> Executes the non-blocking action timer action. </summary>
        /// <param name="source"> Ignored. </param>
        /// <param name="e">      Ignored. </param>
        private void OnActionTimer(object source, ElapsedEventArgs e)
        {
            // Start background thread, but only if not yet running
            if (!_threadState.IsRunning)
            {
                RunNonBlockingAction(_action);
            }
        }

        // Execute the action if not already running

        /// <summary> Executes the non blocking action operation. </summary>
        /// <param name="action"> The action. </param>
        private void RunNonBlockingAction(Action action)
        {
            // Additional (non-blocking) test on _threadIsRunning
            // Request the lock for running background thread
            if (Monitor.TryEnter(_threadState))
            {
                try
                {
                    if (_actionTimer.Enabled)
                    {
                        action();
                    }
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("{0} Caught exception while running ActionBackground #1.", e);
                }
                finally
                {
                    // Ensure that the lock is released.
                    _threadState.IsRunning = false;
                    Monitor.Exit(_threadState);
                }
                return;
            }
            // Exit because Action is already running
            return;
        }

        /// <summary> Start timed actions. </summary>
        public void Start()
        {
            // Start interval events
            _actionTimer.Enabled = true;
        }

        /// <summary> Stop timed actions. </summary>
        public void Stop()
        {
            // Halt new interval events
            _actionTimer.Enabled = false;
        }

        /// <summary> Stop timed actions and wait until running function has finished. </summary>
        public void StopAndWait()
        {
            // Halt new interval events
            _actionTimer.Enabled = false;
            while (_threadState.IsRunning)
            {
            }
        }

        // Dispose
        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
        /// <param name="disposing"> true if resources should be disposed, false if not. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _actionTimer.Elapsed -= OnActionTimer;
            }
            base.Dispose(disposing);
        }
        

    }
}