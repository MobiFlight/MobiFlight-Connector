using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Cdu.Tests
{
    [TestClass]
    public class CduWebsocketHubTests
    {
        // Loopback-only, and each test binds a fresh port, to keep this test project's own
        // sequential run (AssemblyAttributes.cs: DoNotParallelize) from colliding with a
        // real MobiFlight instance's server on the well-known 8320 port.
        private static int NextPort = 18320;

        private WebSocketServer Server = null!;
        private CduWebsocketHub Hub = null!;

        private class RecordingConsumer : ICduDataConsumer
        {
            public List<string> ReceivedData { get; } = new();
            public List<string> ReceivedFont { get; } = new();
            public void OnCduData(string json) => ReceivedData.Add(json);
            public void OnCduFont(string json) => ReceivedFont.Add(json);
        }

        private class ThrowingConsumer : ICduDataConsumer
        {
            public void OnCduData(string json) => throw new InvalidOperationException("boom");
            public void OnCduFont(string json) => throw new InvalidOperationException("boom");
        }

        [TestInitialize]
        public void Setup()
        {
            Server = new WebSocketServer(IPAddress.Loopback, Interlocked.Increment(ref NextPort));
            Hub = new CduWebsocketHub(Server);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Server.IsListening) Server.Stop();
        }

        [TestMethod]
        public void Register_FirstConsumerOnAPath_StartsTheServer()
        {
            // Arrange
            var consumer = new RecordingConsumer();
            // Act
            Hub.Register("/winwing/cdu-captain", consumer);
            // Assert
            Assert.IsTrue(Server.IsListening);
        }

        [TestMethod]
        public void BroadcastData_TwoConsumersOnOnePath_BothReceiveIt()
        {
            // Arrange
            var first = new RecordingConsumer();
            var second = new RecordingConsumer();
            Hub.Register("/winwing/cdu-captain", first);
            Hub.Register("/winwing/cdu-captain", second);
            // Act
            Hub.BroadcastData("/winwing/cdu-captain", "{\"Target\":\"Display\"}");
            // Assert
            Assert.HasCount(1, first.ReceivedData);
            Assert.HasCount(1, second.ReceivedData);
        }

        [TestMethod]
        public void BroadcastFont_ReachesOnlyOnCduFont()
        {
            // Arrange
            var consumer = new RecordingConsumer();
            Hub.Register("/winwing/cdu-captain", consumer);
            // Act
            Hub.BroadcastFont("/winwing/cdu-captain", "{\"Target\":\"Font\",\"Data\":\"Airbus\"}");
            // Assert
            Assert.HasCount(1, consumer.ReceivedFont);
            Assert.IsEmpty(consumer.ReceivedData);
        }

        [TestMethod]
        public void Broadcast_OneConsumerThrows_OtherStillReceivesIt()
        {
            // Arrange
            var throwing = new ThrowingConsumer();
            var recording = new RecordingConsumer();
            Hub.Register("/winwing/cdu-captain", throwing);
            Hub.Register("/winwing/cdu-captain", recording);
            // Act
            Hub.BroadcastData("/winwing/cdu-captain", "{\"Target\":\"Display\"}");
            // Assert
            Assert.HasCount(1, recording.ReceivedData);
        }

        [TestMethod]
        public void Broadcast_PathWithNoRegistrations_DoesNotThrow()
        {
            Hub.BroadcastData("/winwing/cdu-observer", "{}");
        }

        [TestMethod]
        public void Unregister_OneOfTwoConsumers_KeepsTheServiceForTheOther()
        {
            // Arrange
            var first = new RecordingConsumer();
            var second = new RecordingConsumer();
            Hub.Register("/winwing/cdu-captain", first);
            Hub.Register("/winwing/cdu-captain", second);
            // Act
            Hub.Unregister("/winwing/cdu-captain", first);
            Hub.BroadcastData("/winwing/cdu-captain", "{\"Target\":\"Display\"}");
            // Assert
            Assert.IsEmpty(first.ReceivedData);
            Assert.HasCount(1, second.ReceivedData);
            Assert.AreEqual(1, Server.WebSocketServices.Count);
        }

        [TestMethod]
        public void Unregister_LastConsumerOnAPath_RemovesTheWebSocketService()
        {
            // Arrange
            var consumer = new RecordingConsumer();
            Hub.Register("/winwing/cdu-captain", consumer);
            // Act
            Hub.Unregister("/winwing/cdu-captain", consumer);
            // Assert
            Assert.AreEqual(0, Server.WebSocketServices.Count);
        }

        [TestMethod]
        public void Unregister_PathWithNoRegistrations_DoesNotThrow()
        {
            Hub.Unregister("/winwing/cdu-observer", new RecordingConsumer());
        }
    }
}
