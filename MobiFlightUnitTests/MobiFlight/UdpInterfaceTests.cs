using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MobiFlight
{

    [TestClass]
    public class UdpInterfaceTests
    {
        [TestMethod]
        public void PackFloats_And_ParseFloats_RoundTrip_ShouldMatchOriginal()
        {
            var testData = new float[] { 1.23f, 4.56f, -7.89f, 0f };
            var udp = new UdpInterface(new UdpSettings() { LocalIp = "127.0.0.1", LocalPort = 0, RemoteIp = "127.0.0.1", RemotePort = 0 });

            // Use reflection to access private methods
            var packMethod = typeof(UdpInterface).GetMethod("PackFloats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var parseMethod = typeof(UdpInterface).GetMethod("ParseFloats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var bytes = (byte[])packMethod.Invoke(udp, new object[] { testData });
            var floats = (float[])parseMethod.Invoke(udp, new object[] { bytes });

            Assert.AreEqual(testData.Length, floats.Length);
            for (int i = 0; i < testData.Length; i++)
                Assert.AreEqual(testData[i], floats[i], 0.0001f);
        }

        [TestMethod]
        public void DataReceived_Event_ShouldBeInvoked_OnReceive()
        {
            int port = 15000;
            var udpSettings = new UdpSettings()
            {
                LocalIp = "127.0.0.1",
                LocalPort = port,
                RemoteIp = "127.0.0.1",
                RemotePort = port
            };

            var udpReceiver = new UdpInterface(udpSettings);
            var udpSender = new UdpInterface(udpSettings);

            float[] sentData = new float[] { 42.0f, -1.0f, 3.14f };
            float[] receivedData = null;
            var received = new ManualResetEvent(false);

            udpReceiver.DataReceived += (data) =>
            {
                receivedData = data;
                received.Set();
            };

            udpReceiver.StartListening();
            udpSender.Send(sentData);

            Assert.IsTrue(received.WaitOne(1000), "DataReceived event was not triggered.");

            Assert.IsNotNull(receivedData);
            Assert.AreEqual(sentData.Length, receivedData.Length);
            for (int i = 0; i < sentData.Length; i++)
                Assert.AreEqual(sentData[i], receivedData[i], 0.0001f);

            udpReceiver.StopListening();
            udpReceiver.Dispose();
            udpSender.Dispose();
        }
    }
}