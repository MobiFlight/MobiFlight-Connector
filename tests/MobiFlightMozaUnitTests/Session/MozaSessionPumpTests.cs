using System.Collections.Concurrent;
namespace MobiFlightMoza.Session.Tests
{
    /// <summary>
    /// A scriptable stand-in for a CDC stream: each queued step is executed by one
    /// Read() call on the pump thread. When the script is exhausted, Read() blocks until
    /// a new step arrives or throws <see cref="TimeoutException"/> after the configured
    /// read timeout, mirroring SerialPort's timeout behavior.
    /// </summary>
    internal class ScriptedSerialStream : Stream
    {
        private readonly BlockingCollection<Func<byte[], int>> Steps = new BlockingCollection<Func<byte[], int>>();

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override bool CanTimeout => true;
        public override int ReadTimeout { get; set; }
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public void EnqueueData(params byte[] data)
        {
            Steps.Add(buffer =>
            {
                Array.Copy(data, buffer, data.Length);
                return data.Length;
            });
        }

        public void EnqueueError(Exception exception) => Steps.Add(buffer => throw exception);

        public void EnqueueZeroLengthRead() => Steps.Add(buffer => 0);

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!Steps.TryTake(out var step, ReadTimeout)) throw new TimeoutException();
            return step(buffer);
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    /// <summary>
    /// Records every call the pump makes, so its own timing/error-handling behavior can
    /// be checked without a real protocol handshake underneath it.
    /// </summary>
    internal class ScriptedSessionDriver : IMozaSessionDriver
    {
        public int StartCallCount;
        public readonly BlockingCollection<byte[]> ReceivedChunks = new BlockingCollection<byte[]>();
        public readonly BlockingCollection<DateTime> TickCalls = new BlockingCollection<DateTime>();
        public volatile bool ShutdownComplete;
        public Action? OnTickAction;

        public void Start() => StartCallCount++;

        public void OnBytesReceived(byte[] data, int count, DateTime now)
        {
            byte[] copy = new byte[count];
            Array.Copy(data, copy, count);
            ReceivedChunks.Add(copy);
        }

        public void Tick(DateTime now)
        {
            TickCalls.Add(now);
            OnTickAction?.Invoke();
        }

        public bool IsShutdownComplete => ShutdownComplete;
    }

    [TestClass]
    public class MozaSessionPumpTests
    {
        /// <summary>Generous timeout for cross-thread assertions; tests signal much earlier.</summary>
        private const int WaitMilliseconds = 5000;

        private ScriptedSerialStream StreamFake = null!;
        private MozaSessionPump Pump = null!;

        [TestInitialize]
        public void SetUp()
        {
            StreamFake = new ScriptedSerialStream();
            Pump = new MozaSessionPump();
        }

        [TestCleanup]
        public void TearDown() => Pump.Stop();

        private static bool SpinWaitUntil(Func<bool> condition, int timeoutMilliseconds)
        {
            int deadline = Environment.TickCount + timeoutMilliseconds;
            while (Environment.TickCount < deadline)
            {
                if (condition()) return true;
                Thread.Sleep(20);
            }
            return condition();
        }

        #region Startup
        [TestMethod]
        public void Start_NullArguments_Throws()
        {
            var driver = new ScriptedSessionDriver();
            Assert.ThrowsExactly<ArgumentNullException>(() => Pump.Start(null, driver, 64));
            Assert.ThrowsExactly<ArgumentNullException>(() => Pump.Start(StreamFake, null, 64));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Pump.Start(StreamFake, driver, 0));
        }

        [TestMethod]
        public void Start_CallsDriverStartOnceBeforeTheLoop()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            // Act
            Pump.Start(StreamFake, driver, 64);
            // Assert
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            Assert.AreEqual(1, driver.StartCallCount);
        }

        [TestMethod]
        public void Start_WhileRunning_IsIgnored()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            Pump.Start(StreamFake, driver, 64);
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            // Act - a second Start must not spawn a second reader driving a different session
            var secondDriver = new ScriptedSessionDriver();
            Pump.Start(StreamFake, secondDriver, 64);
            // Assert
            Assert.AreEqual(0, secondDriver.StartCallCount);
        }
        #endregion

        #region Byte delivery and idle ticking
        [TestMethod]
        public void Start_DataAvailable_DeliversBytesTrimmedToReadCount()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            StreamFake.EnqueueData(0x01, 0xAA, 0xBB);
            // Act
            Pump.Start(StreamFake, driver, 64);
            // Assert
            Assert.IsTrue(driver.ReceivedChunks.TryTake(out var chunk, WaitMilliseconds));
            CollectionAssert.AreEqual(new byte[] { 0x01, 0xAA, 0xBB }, chunk);
        }

        [TestMethod]
        public void Start_ZeroLengthRead_SkipsBytesButStillTicks()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            StreamFake.EnqueueZeroLengthRead();
            // Act
            Pump.Start(StreamFake, driver, 64);
            // Assert
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            Assert.IsEmpty(driver.ReceivedChunks);
        }

        [TestMethod]
        public void Start_NoDataAvailable_StillTicksOnEachReadTimeout()
        {
            // Arrange - the empty script forces the loop through repeated TimeoutException
            // cycles, since nothing is ever enqueued.
            var driver = new ScriptedSessionDriver();
            // Act
            Pump.Start(StreamFake, driver, 64);
            // Assert - heartbeats/retransmits depend on Tick firing even while idle
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
        }

        [TestMethod]
        public void Start_SurvivesReadTimeoutsBetweenReads()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            Pump.Start(StreamFake, driver, 64);
            // A couple of poll cycles pass with nothing scripted before data arrives.
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            // Act
            StreamFake.EnqueueData(0x02, 0x42);
            // Assert
            Assert.IsTrue(driver.ReceivedChunks.TryTake(out var chunk, WaitMilliseconds), "loop died on read timeout");
            CollectionAssert.AreEqual(new byte[] { 0x02, 0x42 }, chunk);
        }
        #endregion

        #region Error handling
        [TestMethod]
        public void Start_ReadError_RaisesOnErrorOnceAndStopsLoop()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            var errors = new BlockingCollection<Exception>();
            var cause = new IOException("device gone");
            StreamFake.EnqueueError(cause);
            // Act
            Pump.Start(StreamFake, driver, 64, errors.Add);
            // Assert
            Assert.IsTrue(errors.TryTake(out var error, WaitMilliseconds));
            Assert.AreSame(cause, error);
            Assert.IsFalse(Pump.IsRunning);
            Assert.IsEmpty(errors, "onError raised more than once");
        }

        [TestMethod]
        public void Stop_ThenStreamError_DoesNotRaiseOnError()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            var errors = new BlockingCollection<Exception>();
            Pump.Start(StreamFake, driver, 64, errors.Add);
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            // Act
            Pump.Stop();
            Assert.IsFalse(Pump.IsRunning);
            StreamFake.EnqueueError(new ObjectDisposedException("stream"));
            // Assert - an error surfacing after the orderly stop must be swallowed
            Assert.IsFalse(errors.TryTake(out _, 500), "orderly stop reported an error");
        }

        [TestMethod]
        public void Start_DriverThrowsFromTickOnce_DoesNotKillLoop()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            var errors = new BlockingCollection<Exception>();
            int tickCount = 0;
            driver.OnTickAction = () => { if (++tickCount == 1) throw new InvalidOperationException("faulty tick"); };
            // Act
            Pump.Start(StreamFake, driver, 64, errors.Add);
            // Assert
            Assert.IsTrue(errors.TryTake(out var error, WaitMilliseconds));
            Assert.IsInstanceOfType<InvalidOperationException>(error);
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds), "loop died on driver exception");
            Assert.IsTrue(Pump.IsRunning);
        }

        [TestMethod]
        public void Start_DriverKeepsThrowingFromTick_ReportsOnlyFirstFailure()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            var errors = new BlockingCollection<Exception>();
            driver.OnTickAction = () => throw new InvalidOperationException("faulty tick");
            // Act
            Pump.Start(StreamFake, driver, 64, errors.Add);
            // Assert
            Assert.IsTrue(errors.TryTake(out _, WaitMilliseconds));
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            Assert.IsEmpty(errors, "onError raised more than once");
        }
        #endregion

        #region Shutdown
        [TestMethod]
        public void Start_DriverReportsShutdownComplete_StopsLoopAutomatically()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            Pump.Start(StreamFake, driver, 64);
            Assert.IsTrue(driver.TickCalls.TryTake(out _, WaitMilliseconds));
            // Act
            driver.ShutdownComplete = true;
            // Assert
            Assert.IsTrue(SpinWaitUntil(() => !Pump.IsRunning, WaitMilliseconds), "pump did not stop itself once shutdown completed");
        }

        [TestMethod]
        public void Stop_FromInsideDriverCallback_DoesNotDeadlock()
        {
            // Arrange
            var driver = new ScriptedSessionDriver();
            var stopped = new ManualResetEventSlim();
            driver.OnTickAction = () =>
            {
                if (stopped.IsSet) return;
                Pump.Stop();
                stopped.Set();
            };
            // Act
            Pump.Start(StreamFake, driver, 64);
            // Assert
            Assert.IsTrue(stopped.Wait(WaitMilliseconds), "Stop() from the pump thread deadlocked");
            Assert.IsFalse(Pump.IsRunning);
        }

        [TestMethod]
        public void Stop_AllowsRestartWithNewStream()
        {
            // Arrange
            var firstDriver = new ScriptedSessionDriver();
            Pump.Start(StreamFake, firstDriver, 64);
            Assert.IsTrue(firstDriver.TickCalls.TryTake(out _, WaitMilliseconds));
            Pump.Stop();
            // Act
            var secondStream = new ScriptedSerialStream();
            var secondDriver = new ScriptedSessionDriver();
            Pump.Start(secondStream, secondDriver, 64);
            // Assert
            Assert.IsTrue(secondDriver.TickCalls.TryTake(out _, WaitMilliseconds), "pump did not restart");
        }
        #endregion
    }
}
