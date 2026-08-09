using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MobiFlight.Joysticks.Tests
{
    /// <summary>
    /// A scriptable stand-in for a HID stream: each queued step is executed by one
    /// Read() call on the receive thread. When the script is exhausted, Read() blocks
    /// until new steps arrive or throws <see cref="TimeoutException"/> after the
    /// configured read timeout, mirroring HidSharp's timeout behavior.
    /// </summary>
    internal class ScriptedHidStream : Stream
    {
        private readonly BlockingCollection<Func<byte[], int>> Steps = new BlockingCollection<Func<byte[], int>>();

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override bool CanTimeout { get { return true; } }
        public override int ReadTimeout { get; set; }
        public override long Length { get { throw new NotSupportedException(); } }
        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }

        public void EnqueueReport(params byte[] report)
        {
            Steps.Add(buffer =>
            {
                Array.Copy(report, buffer, report.Length);
                return report.Length;
            });
        }

        public void EnqueueError(Exception exception)
        {
            Steps.Add(buffer => { throw exception; });
        }

        public void EnqueueZeroLengthRead()
        {
            Steps.Add(buffer => 0);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Func<byte[], int> step;
            if (!Steps.TryTake(out step, ReadTimeout))
            {
                throw new TimeoutException();
            }
            return step(buffer);
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
    }

    [TestClass]
    public class HidReportReceiverTests
    {
        /// <summary>Generous timeout for cross-thread assertions; tests signal much earlier.</summary>
        private const int WaitMilliseconds = 5000;

        private ScriptedHidStream StreamFake;
        private HidReportReceiver Receiver;

        [TestInitialize]
        public void SetUp()
        {
            StreamFake = new ScriptedHidStream();
            Receiver = new HidReportReceiver();
        }

        [TestCleanup]
        public void TearDown()
        {
            Receiver.Stop();
        }

        [TestMethod]
        public void Start_NullArguments_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => Receiver.Start(null, 64, report => { }));
            Assert.ThrowsExactly<ArgumentNullException>(() => Receiver.Start(StreamFake, 64, null));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Receiver.Start(StreamFake, 0, report => { }));
        }

        [TestMethod]
        public void Start_DeliversReportTrimmedToReadCount()
        {
            var received = new BlockingCollection<byte[]>();
            StreamFake.EnqueueReport(0x01, 0xAA, 0xBB);

            Receiver.Start(StreamFake, 64, received.Add);

            byte[] report;
            Assert.IsTrue(received.TryTake(out report, WaitMilliseconds), "no report delivered");
            CollectionAssert.AreEqual(new byte[] { 0x01, 0xAA, 0xBB }, report);
            Assert.IsTrue(Receiver.IsReceiving);
        }

        [TestMethod]
        public void Start_SurvivesReadTimeoutsBetweenReports()
        {
            var received = new BlockingCollection<byte[]>();
            Receiver.Start(StreamFake, 64, received.Add);

            // The empty script forces the loop through several TimeoutException cycles
            // (poll timeout is a fraction of this sleep) before the report arrives.
            Thread.Sleep(500);
            StreamFake.EnqueueReport(0x02, 0x42);

            byte[] report;
            Assert.IsTrue(received.TryTake(out report, WaitMilliseconds), "loop died on read timeout");
            CollectionAssert.AreEqual(new byte[] { 0x02, 0x42 }, report);
        }

        [TestMethod]
        public void Start_ReadError_RaisesOnErrorOnceAndStopsLoop()
        {
            var errors = new BlockingCollection<Exception>();
            var cause = new IOException("device gone");
            StreamFake.EnqueueError(cause);

            Receiver.Start(StreamFake, 64, report => { }, errors.Add);

            Exception error;
            Assert.IsTrue(errors.TryTake(out error, WaitMilliseconds), "onError not raised");
            Assert.AreSame(cause, error);
            Assert.IsFalse(Receiver.IsReceiving);
            Assert.IsEmpty(errors, "onError raised more than once");
        }

        [TestMethod]
        public void Stop_ThenStreamError_DoesNotRaiseOnError()
        {
            var errors = new BlockingCollection<Exception>();
            Receiver.Start(StreamFake, 64, report => { }, errors.Add);

            Receiver.Stop();
            Assert.IsFalse(Receiver.IsReceiving);

            // An error surfacing after the orderly stop must be swallowed.
            StreamFake.EnqueueError(new ObjectDisposedException("stream"));
            Assert.IsFalse(errors.TryTake(out _, 500), "orderly stop reported an error");
        }

        [TestMethod]
        public void Start_CallbackException_DoesNotKillLoop()
        {
            var received = new BlockingCollection<byte[]>();
            StreamFake.EnqueueReport(0x01, 0x01);
            StreamFake.EnqueueReport(0x01, 0x02);

            Receiver.Start(StreamFake, 64, incoming =>
            {
                received.Add(incoming);
                throw new InvalidOperationException("faulty handler");
            });

            byte[] report;
            Assert.IsTrue(received.TryTake(out report, WaitMilliseconds));
            Assert.IsTrue(received.TryTake(out report, WaitMilliseconds), "loop died on callback exception");
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02 }, report);
            Assert.IsTrue(Receiver.IsReceiving);
        }

        [TestMethod]
        public void Start_ZeroLengthRead_IsSkipped()
        {
            var received = new BlockingCollection<byte[]>();
            StreamFake.EnqueueZeroLengthRead();
            StreamFake.EnqueueReport(0x03);

            Receiver.Start(StreamFake, 64, received.Add);

            byte[] report;
            Assert.IsTrue(received.TryTake(out report, WaitMilliseconds));
            CollectionAssert.AreEqual(new byte[] { 0x03 }, report);
            Assert.IsEmpty(received, "zero-length read must not produce a report");
        }

        [TestMethod]
        public void Start_WhileReceiving_IsIgnored()
        {
            var received = new BlockingCollection<byte[]>();
            Receiver.Start(StreamFake, 64, received.Add);

            // A second Start must not spawn a second reader feeding a different callback.
            Receiver.Start(StreamFake, 64, ignored => Assert.Fail("second Start must be ignored"));

            StreamFake.EnqueueReport(0x04);
            byte[] report;
            Assert.IsTrue(received.TryTake(out report, WaitMilliseconds));
            CollectionAssert.AreEqual(new byte[] { 0x04 }, report);
        }

        [TestMethod]
        public void Stop_FromInsideCallback_DoesNotDeadlock()
        {
            var stopped = new ManualResetEventSlim();
            StreamFake.EnqueueReport(0x05);

            Receiver.Start(StreamFake, 64, ignored =>
            {
                Receiver.Stop();
                stopped.Set();
            });

            Assert.IsTrue(stopped.Wait(WaitMilliseconds), "Stop() from the receive thread deadlocked");
            Assert.IsFalse(Receiver.IsReceiving);
        }

        [TestMethod]
        public void Stop_AllowsRestartWithNewStream()
        {
            var firstReports = new BlockingCollection<byte[]>();
            StreamFake.EnqueueReport(0x06);
            Receiver.Start(StreamFake, 64, firstReports.Add);
            byte[] report;
            Assert.IsTrue(firstReports.TryTake(out report, WaitMilliseconds));

            Receiver.Stop();

            var secondStream = new ScriptedHidStream();
            var secondReports = new BlockingCollection<byte[]>();
            secondStream.EnqueueReport(0x07);
            Receiver.Start(secondStream, 64, secondReports.Add);

            Assert.IsTrue(secondReports.TryTake(out report, WaitMilliseconds), "receiver did not restart");
            CollectionAssert.AreEqual(new byte[] { 0x07 }, report);
        }
    }
}
