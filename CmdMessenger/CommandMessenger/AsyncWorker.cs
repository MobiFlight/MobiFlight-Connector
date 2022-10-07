using System;
using System.Threading;

namespace CommandMessenger
{
    public class AsyncWorker
    {
        public enum WorkerState
        {
            Stopped,
            Running,
            Suspended
        }

        /// <summary>
        /// Main worker method to do some work.
        /// </summary>
        /// <returns>true is there is more work to do, otherwise false and worker will wait until signalled with SignalWorker().</returns>
        public delegate bool AsyncWorkerJob();

        private bool _isFaulted;

        private volatile WorkerState _state = WorkerState.Stopped;
        private volatile WorkerState _requestedState = WorkerState.Stopped;

        private readonly object _lock = new object();
        private readonly EventWaiter _eventWaiter = new EventWaiter();

        private readonly AsyncWorkerJob _workerJob;

        private Thread _workerTask;

		public string Name { get; private set; }

        public WorkerState State { get { return _state; } }

        public bool IsRunning { get { return _state == WorkerState.Running; } }
        public bool IsSuspended { get { return _state == WorkerState.Suspended; } }

        public AsyncWorker(AsyncWorkerJob workerJob, string workerName = null)
        {
            if (workerJob == null) throw new ArgumentNullException("workerJob");
            _workerJob = workerJob;
            Name = workerName;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_state == WorkerState.Stopped)
                {
                    _requestedState = _state = WorkerState.Running;
                    _eventWaiter.Reset();

                    _workerTask = new Thread(() =>
                    {
                        while (true)
                        {
                            if (_state == WorkerState.Stopped) break;

                            bool haveMoreWork = false;
                            if (_state == WorkerState.Running)
                            {
                                try
                                {
                                    haveMoreWork = _workerJob();
                                }
                                catch
                                {
                                    _requestedState = _state = WorkerState.Stopped;
                                    _isFaulted = true;
                                    throw;
                                }

                                // Check if state has been changed in workerJob thread.
                                if (_requestedState != _state && _requestedState == WorkerState.Stopped)
                                {
                                    _state = _requestedState;
                                    break;
                                }
                            }

                            if (!haveMoreWork || _state == WorkerState.Suspended) _eventWaiter.WaitOne(Timeout.Infinite);
                            _state = _requestedState;
                        }
                    });
                    _workerTask.Name = Name;
                    _workerTask.IsBackground = true;

                    _workerTask.Start();
                    SpinWait.SpinUntil(() => _workerTask.IsAlive);
                }
                else
                {
                    throw new InvalidOperationException("The worker is already started.");
                }
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_state == WorkerState.Running || _state == WorkerState.Suspended)
                {
                    _requestedState = WorkerState.Stopped;

                    // Prevent deadlock by checking is we stopping from worker task or not.
                    if (Thread.CurrentThread.ManagedThreadId != _workerTask.ManagedThreadId)
                    {
                        _eventWaiter.Set();
                        _workerTask.Join();
                    }
                }
                else if (!_isFaulted)
                {
                    // Probably not needed, added as a precaution.
                    throw new InvalidOperationException("The worker is already stopped.");
                }
            }
        }

        public void Suspend()
        {
            lock (_lock)
            {
                if (_state == WorkerState.Running)
                {
                    _requestedState = WorkerState.Suspended;
                    _eventWaiter.Set();
                    SpinWait.SpinUntil(() => _requestedState == _state);
                }
                else
                {
                    // Probably not needed, added as a precaution.
                    throw new InvalidOperationException("The worker is not running.");
                }
            }
        }

        public void Resume()
        {
            lock (_lock)
            {
                if (_state == WorkerState.Suspended)
                {
                    _requestedState = WorkerState.Running;
                    _eventWaiter.Set();
                    SpinWait.SpinUntil(() => _requestedState == _state);
                }
                else
                {
                    // Probably not needed, added as a precaution.
                    throw new InvalidOperationException("The worker is not in suspended state.");
                }
            }
        }

        /// <summary>
        /// Signal worker to continue processing.
        /// </summary>
        public void Signal()
        {
            if (IsRunning) _eventWaiter.Set();
        }
    }
}
