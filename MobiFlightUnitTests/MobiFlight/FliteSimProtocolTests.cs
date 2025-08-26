using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Joysticks.FliteSim;
using System;
using System.Threading;
using System.Text;
using Moq;
using System.Collections.Generic;

namespace MobiFlight
{
    [TestClass]
    public class FliteSimProtocolTests
    {
        private Mock<IUdpInterface> _mockUdp;
        private FliteSimProtocol _protocol;
        private UdpSettings _testSettings;
        private List<byte[]> _sentPackets;

        [TestInitialize]
        public void Setup()
        {
            _testSettings = new UdpSettings("127.0.0.1", 59110, "127.0.0.1", 59111);
            _mockUdp = new Mock<IUdpInterface>();
            _sentPackets = new List<byte[]>();

            // Setup mock to capture sent packets
            _mockUdp.Setup(x => x.Send(It.IsAny<byte[]>()))
                   .Callback<byte[]>(data => 
                   {
                       var copy = new byte[data.Length];
                       Array.Copy(data, copy, data.Length);
                       _sentPackets.Add(copy);
                   });

            _protocol = new FliteSimProtocol(_testSettings, _mockUdp.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _protocol?.Dispose();
        }

        #region Handshake Tests

        [TestMethod]
        public void InitiateHandshake_ShouldSendCorrectPacket()
        {
            // Act
            _protocol.InitiateHandshake();

            // Assert
            _mockUdp.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Once);
            Assert.AreEqual(1, _sentPackets.Count);
            var packet = _sentPackets[0];
            
            // Verify packet structure: [0xAA55][INIT][1.0f]
            Assert.AreEqual(10, packet.Length);
            Assert.AreEqual(0x55, packet[0]); // Little-endian 0xAA55
            Assert.AreEqual(0xAA, packet[1]);
            Assert.AreEqual("INIT", Encoding.ASCII.GetString(packet, 2, 4));
            Assert.AreEqual(1.0f, BitConverter.ToSingle(packet, 6), 0.001f);
        }

        [TestMethod]
        public void ReceiveHandshakeInit_ShouldSendAckAndCompleteHandshake()
        {
            // Arrange
            bool handshakeCompleted = false;
            _protocol.HandshakeCompleted += () => handshakeCompleted = true;

            var initPacket = CreateProtocolPacket("INIT", 1.0f);

            // Act
            _mockUdp.Raise(x => x.DataReceived += null, initPacket);

            // Assert
            Assert.IsTrue(handshakeCompleted);
            Assert.IsTrue(_protocol.IsConnected);
            _mockUdp.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Once);
            
            var ackPacket = _sentPackets[0];
            Assert.AreEqual("ACKN", Encoding.ASCII.GetString(ackPacket, 2, 4));
        }

        [TestMethod]
        public void ReceiveHandshakeAck_ShouldCompleteHandshake()
        {
            // Arrange
            bool handshakeCompleted = false;
            _protocol.HandshakeCompleted += () => handshakeCompleted = true;

            var ackPacket = CreateProtocolPacket("ACKN", 1.0f);

            // Act
            _mockUdp.Raise(x => x.DataReceived += null, ackPacket);

            // Assert
            Assert.IsTrue(handshakeCompleted);
            Assert.IsTrue(_protocol.IsConnected);
        }

        [TestMethod]
        public void HandshakeTimeout_ShouldRetry()
        {
            // Act - First attempt
            _protocol.InitiateHandshake();
            _sentPackets.Clear();

            // Simulate timeout
            _protocol.CheckHandshakeTimeout();

            // Assert - Should not retry yet (within timeout)
            Assert.AreEqual(0, _sentPackets.Count);

            // Simulate time passing (using reflection to set internal timestamp)
            var protocolType = typeof(FliteSimProtocol);
            var lastAttemptField = protocolType.GetField("_lastHandshakeAttempt", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            lastAttemptField.SetValue(_protocol, DateTime.Now.AddMilliseconds(-4000));

            // Act - Check timeout again
            _protocol.CheckHandshakeTimeout();

            // Assert - Should retry now
            Assert.AreEqual(1, _sentPackets.Count);
        }

        [TestMethod]
        public void MaxHandshakeAttempts_ShouldStopRetrying()
        {
            // Act - Attempt handshake 3 times
            _protocol.InitiateHandshake();
            _protocol.InitiateHandshake();
            _protocol.InitiateHandshake();
            _sentPackets.Clear();

            // Try one more time
            _protocol.InitiateHandshake();

            // Assert - Should not send more packets after max attempts
            Assert.AreEqual(0, _sentPackets.Count);
        }

        #endregion

        #region Protocol Message Tests

        [TestMethod]
        public void ReceiveQuitMessage_ShouldRaiseConnectionLostEvent()
        {
            // Arrange
            bool connectionLost = false;
            _protocol.ConnectionLost += () => connectionLost = true;

            var quitPacket = CreateProtocolPacket("QUIT", 0.0f);

            // Act
            _mockUdp.Raise(x => x.DataReceived += null, quitPacket);

            // Assert
            Assert.IsTrue(connectionLost);
            _mockUdp.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Once);
            
            var ackPacket = _sentPackets[0];
            Assert.AreEqual("ACKQ", Encoding.ASCII.GetString(ackPacket, 2, 4));
        }

        [TestMethod]
        public void ReceiveQuitAck_ShouldRaiseConnectionLostEvent()
        {
            // Arrange
            bool connectionLost = false;
            _protocol.ConnectionLost += () => connectionLost = true;

            var ackqPacket = CreateProtocolPacket("ACKQ", 1.0f);

            // Act
            _mockUdp.Raise(x => x.DataReceived += null, ackqPacket);

            // Assert
            Assert.IsTrue(connectionLost);
        }

        [TestMethod]
        public void ReceiveUnknownProtocolMessage_ShouldLogWarning()
        {
            // Arrange
            var unknownPacket = CreateProtocolPacket("XXXX", 0.0f);

            // Act (should not throw)
            _mockUdp.Raise(x => x.DataReceived += null, unknownPacket);

            // Assert - No exceptions should be thrown
        }

        #endregion

        #region Data Conversion Tests

        [TestMethod]
        public void ReceiveControlData_ShouldParseCorrectly()
        {
            // Arrange
            CompleteHandshake();
            
            float[] receivedData = null;
            _protocol.ControlDataReceived += (data) => receivedData = data;

            var testData = new float[] { 1.0f, 0.5f, -0.5f, 0.8f, -0.2f, 0.1f, 1.0f, 0.0f, 1.0f, 0.5f, -0.3f, 0.7f, 1.0f };
            var bytes = PackFloats(testData);

            // Act
            _mockUdp.Raise(x => x.DataReceived += null, bytes);

            // Assert
            Assert.IsNotNull(receivedData);
            Assert.AreEqual(13, receivedData.Length);
            for (int i = 0; i < testData.Length; i++)
            {
                Assert.AreEqual(testData[i], receivedData[i], 0.001f);
            }
        }

        [TestMethod]
        public void ReceiveControlData_WithoutHandshake_ShouldNotRaiseEvent()
        {
            // Arrange
            bool eventRaised = false;
            _protocol.ControlDataReceived += (data) => eventRaised = true;

            var testData = new float[13];
            var bytes = PackFloats(testData);

            // Act
            _mockUdp.Raise(x => x.DataReceived += null, bytes);

            // Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void SendFlightData_ShouldPackCorrectly()
        {
            // Arrange
            CompleteHandshake();
            _sentPackets.Clear(); // Clear handshake packets
            
            var flightData = new float[30];
            for (int i = 0; i < 30; i++)
            {
                flightData[i] = i * 0.1f;
            }

            // Act
            _protocol.SendFlightData(flightData);

            // Assert
            _mockUdp.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Once);
            Assert.AreEqual(1, _sentPackets.Count);
            var sentBytes = _sentPackets[0];
            Assert.AreEqual(120, sentBytes.Length); // 30 floats * 4 bytes

            var parsedData = ParseFloats(sentBytes);
            Assert.AreEqual(30, parsedData.Length);
            for (int i = 0; i < 30; i++)
            {
                Assert.AreEqual(flightData[i], parsedData[i], 0.001f);
            }
        }

        [TestMethod]
        public void SendFlightData_WithoutHandshake_ShouldNotSend()
        {
            // Arrange
            var flightData = new float[30];

            // Act
            _protocol.SendFlightData(flightData);

            // Assert
            _mockUdp.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void SendFlightData_InvalidSize_ShouldNotSend()
        {
            // Arrange
            CompleteHandshake();
            _sentPackets.Clear();
            var invalidData = new float[25]; // Wrong size

            // Act
            _protocol.SendFlightData(invalidData);

            // Assert
            _mockUdp.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void ReceiveInvalidControlDataSize_ShouldNotRaiseEvent()
        {
            // Arrange
            CompleteHandshake();
            
            bool eventRaised = false;
            _protocol.ControlDataReceived += (data) => eventRaised = true;

            var invalidData = new float[10]; // Wrong size
            var bytes = PackFloats(invalidData);

            // Act
            _mockUdp.Raise(x => x.DataReceived += null, bytes);

            // Assert
            Assert.IsFalse(eventRaised);
        }

        #endregion

        #region State Management Tests

        [TestMethod]
        public void Dispose_ShouldSendQuitMessage()
        {
            // Arrange
            CompleteHandshake();
            _sentPackets.Clear();

            // Act
            _protocol.Dispose();

            // Assert
            _mockUdp.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Once);
            Assert.AreEqual(1, _sentPackets.Count);
            var quitPacket = _sentPackets[0];
            Assert.AreEqual("QUIT", Encoding.ASCII.GetString(quitPacket, 2, 4));
            Assert.AreEqual(0.0f, BitConverter.ToSingle(quitPacket, 6), 0.001f);
        }

        [TestMethod]
        public void Dispose_WithoutHandshake_ShouldNotSendQuit()
        {
            // Act
            _protocol.Dispose();

            // Assert
            _mockUdp.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Never);
        }

        #endregion

        #region Helper Methods

        private void CompleteHandshake()
        {
            var ackPacket = CreateProtocolPacket("ACKN", 1.0f);
            _mockUdp.Raise(x => x.DataReceived += null, ackPacket);
        }

        private byte[] CreateProtocolPacket(string flag, float value)
        {
            var packet = new byte[10];
            var headerBytes = BitConverter.GetBytes((ushort)0xAA55);
            var flagBytes = Encoding.ASCII.GetBytes(flag);
            var valueBytes = BitConverter.GetBytes(value);

            Array.Copy(headerBytes, 0, packet, 0, 2);
            Array.Copy(flagBytes, 0, packet, 2, 4);
            Array.Copy(valueBytes, 0, packet, 6, 4);
            return packet;
        }

        private byte[] PackFloats(float[] data)
        {
            var bytes = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
                Array.Copy(BitConverter.GetBytes(data[i]), 0, bytes, i * 4, 4);
            return bytes;
        }

        private float[] ParseFloats(byte[] buffer)
        {
            int count = buffer.Length / 4;
            float[] result = new float[count];
            for (int i = 0; i < count; i++)
                result[i] = BitConverter.ToSingle(buffer, i * 4);
            return result;
        }

        #endregion
    }
}