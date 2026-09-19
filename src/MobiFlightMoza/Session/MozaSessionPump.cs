using System;
using System.IO;
using System.Threading;

namespace MobiFlightMoza.Session
{
    /// <summary>
    /// What <see cref="MozaSessionPump"/> drives: bytes off a real stream and the real
    /// clock. <see cref="MozaScreenSession"/> satisfies this without any change beyond
    /// declaring it - everything below the pump is pure and already takes an explicit
    /// <c>now</c>. The interface exists purely so tests can drive the pump against a
    /// scripted double, without a full protocol handshake, to check its own timing and
    /// error-handling behavior in isolation.
    /// </summary>
    internal interface IMozaSessionDriver
    {
        void Start();
        void OnBytesReceived(byte[] data, int count, DateTime now);
        void Tick(DateTime now);
        bool IsShutdownComplete { get; }
    }

    /// <summary>
    /// Drives an <see cref="IMozaSessionDriver"/> on a dedicated background thread: a
    /// bounded blocking read feeds bytes in, and every loop iteration - including one that
    /// times out with nothing to read - ticks the driver with the real clock, so
    /// heartbeats and retransmits keep firing while the device is idle. Mirrors
    /// <c>MobiFlight.Joysticks.HidReportReceiver</c>'s dedicated-thread pattern.
    /// </summary>
    internal class MozaSessionPump
    {
        private const int ReadPollTimeoutMilliseconds = 200;
        private const int StopWaitMilliseconds = ReadPollTimeoutMilliseconds * 5;

        /// <summary>
        /// One pump generation. The loop only ever checks its own session, so a reader
        /// that outlives a <see cref="Stop"/> (because it was mid-iteration when the join
        /// timed out) can neither be revived by a following <see cref="Start"/> nor stop
        /// the new generation.
        /// </summary>
        private class PumpSession
        {
            public volatile bool Active = true;
            public bool CallbackErrorLogged;
        }

        private readonly object StartStopLock = new object();
        private volatile PumpSession ActiveSession;
        private Thread PumpThread;

        public bool IsRunning
        {
            get
            {
                var session = ActiveSession;
                return session != null && session.Active;
            }
        }

        /// <summary>
        /// Starts the background pump loop; does nothing if already running.
        /// </summary>
        /// <param name="stream">Open CDC stream. The pump sets its read timeout but does not own it; closing it remains the caller's responsibility.</param>
        /// <param name="driver">The session to drive - <see cref="IMozaSessionDriver.Start"/> is called once, from the pump thread, before the read loop begins.</param>
        /// <param name="bufferSize">Read buffer size.</param>
        /// <param name="onError">Called at most once, if a stream read fails or the driver keeps throwing; may be null.</param>
        /// <param name="threadName">Optional thread name to identify the device in debugging tools.</param>
        public void Start(Stream stream, IMozaSessionDriver driver, int bufferSize, Action<Exception> onError = null, string threadName = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufferSize));

            lock (StartStopLock)
            {
                if (IsRunning) return;

                byte[] buffer = new byte[bufferSize];
                stream.ReadTimeout = ReadPollTimeoutMilliseconds;
                var session = new PumpSession();
                ActiveSession = session;

                // A dedicated thread, not a pool thread - same reasoning as HidReportReceiver.
                PumpThread = new Thread(() => PumpLoop(session, stream, driver, buffer, onError))
                {
                    IsBackground = true,
                    Name = threadName ?? "MozaSessionPump"
                };
                PumpThread.Start();
            }
        }

        public void Stop()
        {
            Thread pumpThread;
            lock (StartStopLock)
            {
                var session = ActiveSession;
                if (session == null || !session.Active) return;

                session.Active = false;
                pumpThread = PumpThread;
                PumpThread = null;
            }

            // A handler stopping from inside the loop must not wait on its own thread.
            if (pumpThread != null && pumpThread != Thread.CurrentThread)
            {
                pumpThread.Join(StopWaitMilliseconds);
            }
        }

        private void PumpLoop(PumpSession session, Stream stream, IMozaSessionDriver driver, byte[] buffer, Action<Exception> onError)
        {
            driver.Start();

            while (session.Active)
            {
                int count = 0;
                try
                {
                    count = stream.Read(buffer, 0, buffer.Length);
                }
                catch (TimeoutException)
                {
                    // No bytes within the poll window - expected. Falls through to Tick
                    // below regardless, so heartbeats/retransmits fire while idle.
                }
                catch (Exception ex)
                {
                    // A stream closed after Stop() was requested is an orderly shutdown,
                    // not an error worth reporting.
                    if (session.Active)
                    {
                        session.Active = false;
                        try { onError?.Invoke(ex); } catch { }
                    }
                    return;
                }

                DateTime now = DateTime.Now;
                try
                {
                    if (count > 0) driver.OnBytesReceived(buffer, count, now);
                    driver.Tick(now);
                }
                catch (Exception ex)
                {
                    // A faulty driver must not take down the pump loop, but a silently
                    // dead session is undebuggable - report the first failure.
                    if (!session.CallbackErrorLogged)
                    {
                        session.CallbackErrorLogged = true;
                        try { onError?.Invoke(ex); } catch { }
                    }
                }

                if (driver.IsShutdownComplete)
                {
                    session.Active = false;
                    return;
                }
            }
        }
    }
}
