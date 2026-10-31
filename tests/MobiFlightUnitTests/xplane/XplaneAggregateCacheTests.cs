using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MobiFlight.xplane;
using System;

namespace MobiFlight.xplane.Tests
{
    [TestClass]
    public class XplaneAggregateCacheTests
    {
        private Mock<XplaneCacheInterface> _webApiMock;
        private Mock<XplaneCacheInterface> _udpMock;

        [TestInitialize]
        public void Setup()
        {
            _webApiMock = new Mock<XplaneCacheInterface>();
            _udpMock = new Mock<XplaneCacheInterface>();
        }

        private XplaneAggregateCache CreateAggregate(bool webApiEnabled = true)
        {
            return new XplaneAggregateCache(_webApiMock.Object, _udpMock.Object, webApiEnabled);
        }

        [TestMethod]
        public void Connect_WebApiSucceeds_ShortCircuitsUdp()
        {
            _webApiMock.Setup(c => c.Connect()).Returns(true);
            var aggregate = CreateAggregate();

            var result = aggregate.Connect();

            Assert.IsTrue(result);
            _udpMock.Verify(c => c.Connect(), Times.Never());
        }

        [TestMethod]
        public void Connect_WebApiFails_FallsThroughToUdp()
        {
            _webApiMock.Setup(c => c.Connect()).Returns(false);
            _udpMock.Setup(c => c.Connect()).Returns(true);
            var aggregate = CreateAggregate();

            var result = aggregate.Connect();

            Assert.IsTrue(result);
            _webApiMock.Verify(c => c.Connect(), Times.Once());
            _udpMock.Verify(c => c.Connect(), Times.Once());
        }

        [TestMethod]
        public void Connect_BothFail_LeavesDisconnectedWithoutThrowing()
        {
            _webApiMock.Setup(c => c.Connect()).Returns(false);
            _udpMock.Setup(c => c.Connect()).Returns(false);
            var aggregate = CreateAggregate();

            var result = aggregate.Connect();

            Assert.IsFalse(result);
            Assert.IsFalse(aggregate.IsConnected());
        }

        [TestMethod]
        public void Connect_WebApiDisabled_SkipsWebApiCacheEntirely()
        {
            _udpMock.Setup(c => c.Connect()).Returns(true);
            var aggregate = CreateAggregate(webApiEnabled: false);

            var result = aggregate.Connect();

            Assert.IsTrue(result);
            _webApiMock.Verify(c => c.Connect(), Times.Never());
        }

        [TestMethod]
        public void UpdateFrequencyPerSecond_ForwardsToBothInnerCaches_Unconditionally()
        {
            var aggregate = CreateAggregate();

            aggregate.UpdateFrequencyPerSecond = 20;

            _webApiMock.VerifySet(c => c.UpdateFrequencyPerSecond = 20, Times.Once());
            _udpMock.VerifySet(c => c.UpdateFrequencyPerSecond = 20, Times.Once());
        }

        [TestMethod]
        public void StaleEventFromNowInactiveInnerCache_IsNotReRaised()
        {
            _webApiMock.Setup(c => c.Connect()).Returns(false);
            _udpMock.Setup(c => c.Connect()).Returns(true);
            var aggregate = CreateAggregate();
            aggregate.Connect();

            var connectedRaised = false;
            aggregate.Connected += (s, e) => connectedRaised = true;

            _webApiMock.Raise(c => c.Connected += null, _webApiMock.Object, EventArgs.Empty);

            Assert.IsFalse(connectedRaised);
        }

        [TestMethod]
        public void ActiveInnerCacheEvent_IsReRaisedAsAggregateEvent()
        {
            _webApiMock.Setup(c => c.Connect()).Returns(false);
            _udpMock.Setup(c => c.Connect()).Returns(true);
            var aggregate = CreateAggregate();
            aggregate.Connect();

            var connectionLostRaised = false;
            aggregate.ConnectionLost += (s, e) => connectionLostRaised = true;

            _udpMock.Raise(c => c.ConnectionLost += null, _udpMock.Object, EventArgs.Empty);

            Assert.IsTrue(connectionLostRaised);
        }

        [TestMethod]
        public void Disconnect_DelegatesToActiveCache_AndClearsActiveState()
        {
            _webApiMock.Setup(c => c.Connect()).Returns(true);
            _webApiMock.Setup(c => c.Disconnect()).Returns(true);
            var aggregate = CreateAggregate();
            aggregate.Connect();

            var result = aggregate.Disconnect();

            Assert.IsTrue(result);
            Assert.IsFalse(aggregate.IsConnected());
            _webApiMock.Verify(c => c.Disconnect(), Times.Once());
        }

        [TestMethod]
        public void ReadWriteAndSendCommand_AreNoOps_WhenNeverConnected()
        {
            var aggregate = CreateAggregate();

            var value = aggregate.readDataRef("sim/foo/bar");
            aggregate.writeDataRef("sim/foo/bar", 1.0f);
            aggregate.sendCommand("sim/some/command");

            Assert.AreEqual(0f, value);
            _webApiMock.Verify(c => c.readDataRef(It.IsAny<string>()), Times.Never());
            _udpMock.Verify(c => c.readDataRef(It.IsAny<string>()), Times.Never());
        }
    }
}
