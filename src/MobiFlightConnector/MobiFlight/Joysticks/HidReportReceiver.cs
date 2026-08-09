using System;
using System.IO;
using System.Threading;

namespace MobiFlight.Joysticks
{
    /// <summary>
    /// Reads HID input reports on a dedicated background thread using plain blocking reads.
    /// <para>
    /// This deliberately bypasses HidSharp's <c>HidDeviceInputReceiver</c>: its
    /// <c>BeginRead</c> chain queues every single read as a thread pool work item that
    /// blocks a pool thread, which causes thread churn and measurable CPU load for
    /// devices that stream reports continuously. A dedicated thread with a blocking
    /// read costs no CPU between reports.
    /// </para>
    /// </summary>
    internal class HidReportReceiver
    {
        /// <summary>
        /// Read timeout of the receive loop. Bounded so the loop can notice the stop flag;
        /// a blocking read is otherwise only interruptible by closing the stream under it.
        /// </summary>
        private const int ReadPollTimeoutMilliseconds = 200;

        /// <summary>How long a stop waits for the receive loop to leave its current read.</summary>
        private const int StopWaitMilliseconds = ReadPollTimeoutMilliseconds * 5;

        /// <summary>
        /// One receive generation. The loop only ever checks its own session, so a reader
        /// that outlives a <see cref="Stop"/> (because a callback was still running when
        /// the join timed out) can neither be revived by a following <see cref="Start"/>
        /// nor stop the new generation.
        /// </summary>
        private class Session
        {
            public volatile bool Active = true;

            /// <summary>Only touched by the session's own read thread.</summary>
            public bool CallbackErrorLogged;
        }

        private readonly object StartStopLock = new object();
        private volatile Session ActiveSession;
        private Thread ReadThread;

        public bool IsReceiving
        {
            get
            {
                var session = ActiveSession;
                return session != null && session.Active;
            }
        }

        /// <summary>
        /// Returns the payload of a raw report, i.e. everything after the report ID byte
        /// at index 0. For report parsers whose offsets are payload-relative, following
        /// the vendor protocol tables.
        /// </summary>
        public static byte[] GetPayload(byte[] rawReport)
        {
            byte[] payload = new byte[rawReport.Length - 1];
            Array.Copy(rawReport, 1, payload, 0, payload.Length);
            return payload;
        }

        /// <summary>
        /// Starts the background receive loop. Does nothing if it is already running.
        /// <para>
        /// Both callbacks run on the receive thread — WinForms callers must marshal to the
        /// UI thread. <paramref name="onReport"/> gets each raw input report exactly as read
        /// from the stream, i.e. including the report ID at index 0. After
        /// <paramref name="onError"/> the loop has stopped; call <see cref="Start"/> again
        /// to resume once the cause is dealt with. An orderly <see cref="Stop"/> — or the
        /// stream being closed after <see cref="Stop"/> was requested — does not raise
        /// <paramref name="onError"/>.
        /// </para>
        /// </summary>
        /// <param name="stream">
        /// An open HID stream (typically a HidSharp <c>HidStream</c>). The receiver sets its
        /// <see cref="Stream.ReadTimeout"/> but does not own it: closing the stream remains
        /// the caller's responsibility.
        /// </param>
        /// <param name="bufferSize">
        /// The read buffer size, typically <c>HidDevice.GetMaxInputReportLength()</c>.
        /// Passed in by the caller so hardware that cannot report its input length fails
        /// on the caller's thread instead of via <paramref name="onError"/>.
        /// </param>
        /// <param name="onReport">Called for every received report with a copy of the report bytes.</param>
        /// <param name="onError">Called once when the loop dies on a read error; may be null.</param>
        /// <param name="threadName">Optional thread name to identify the device in debugging tools.</param>
        public void Start(Stream stream, int bufferSize, Action<byte[]> onReport, Action<Exception> onError = null, string threadName = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (onReport == null)
            {
                throw new ArgumentNullException(nameof(onReport));
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            lock (StartStopLock)
            {
                if (IsReceiving) return;

                byte[] buffer = new byte[bufferSize];

                stream.ReadTimeout = ReadPollTimeoutMilliseconds;
                var session = new Session();
                ActiveSession = session;

                // A dedicated thread, not a pool thread: HidSharp has no true async read -
                // its BeginRead queues the *blocking* read onto the thread pool and parks
                // a pool thread for the whole wait.
                ReadThread = new Thread(() => ReadLoop(session, stream, buffer, onReport, onError))
                {
                    IsBackground = true,
                    Name = threadName ?? "HidReportReceiver"
                };
                ReadThread.Start();
            }
        }

        public void Stop()
        {
            Thread readThread;
            lock (StartStopLock)
            {
                var session = ActiveSession;
                if (session == null || !session.Active) return;

                session.Active = false;
                readThread = ReadThread;
                ReadThread = null;
            }

            // A handler stopping from inside the loop must not wait on its own thread.
            if (readThread != null && readThread != Thread.CurrentThread)
            {
                readThread.Join(StopWaitMilliseconds);
            }
        }

        private void ReadLoop(Session session, Stream stream, byte[] buffer, Action<byte[]> onReport, Action<Exception> onError)
        {
            while (session.Active)
            {
                int count;
                try
                {
                    count = stream.Read(buffer, 0, buffer.Length);
                }
                catch (TimeoutException)
                {
                    // No report within the poll window - expected, keeps the loop responsive.
                    continue;
                }
                catch (Exception ex)
                {
                    // A stream closed after Stop() was requested is an orderly shutdown,
                    // not an error worth reporting.
                    if (session.Active)
                    {
                        session.Active = false;

                        // never let error reporting throw out of the receive thread
                        try { onError?.Invoke(ex); } catch { }
                    }
                    return;
                }

                if (count <= 0) continue;

                byte[] report = new byte[count];
                Array.Copy(buffer, 0, report, 0, count);

                try
                {
                    onReport(report);
                }
                catch (Exception ex)
                {
                    // A faulty callback must not take down the receive loop, but silently
                    // dead inputs are undebuggable - log the first failure per session.
                    if (!session.CallbackErrorLogged)
                    {
                        session.CallbackErrorLogged = true;
                        Log.Instance.log($"HID report callback failed on {Thread.CurrentThread.Name}: {ex}", LogSeverity.Error);
                    }
                }
            }
        }
    }
}
