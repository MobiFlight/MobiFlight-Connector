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

            // Use reflection to access private method PackFloats
            var packMethod = typeof(UdpInterface).GetMethod("PackFloats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var bytes = (byte[])packMethod.Invoke(udp, new object[] { testData });
            
            // Parse floats manually since ParseFloats is no longer available
            var floats = ParseFloatsHelper(bytes);

            Assert.AreEqual(testData.Length, floats.Length);
            for (int i = 0; i < testData.Length; i++)
                Assert.AreEqual(testData[i], floats[i], 0.0001f);
        }

        [TestMethod]
        public void DataReceived_Event_ShouldBeInvoked_OnReceive()
        {
            int port = 15000;
            var udpSettingsReceiver = new UdpSettings()
            {
                LocalIp = "127.0.0.1",
                LocalPort = port,
                RemoteIp = "127.0.0.1",
                RemotePort = port + 1
            };

            var udpSettingsSender = new UdpSettings()
            {
                LocalIp = "127.0.0.1",
                LocalPort = port + 1,
                RemoteIp = "127.0.0.1",
                RemotePort = port
            };

            var udpReceiver = new UdpInterface(udpSettingsReceiver);
            var udpSender = new UdpInterface(udpSettingsSender);

            float[] sentData = new float[] { 42.0f, -1.0f, 3.14f };
            byte[] receivedData = null;
            var received = new ManualResetEvent(false);

            udpReceiver.DataReceived += (data) =>
            {
                receivedData = data;
                received.Set();
            };

            udpReceiver.StartListening();
            udpSender.Send(sentData); // Uses the convenience method that converts float[] to byte[]

            Assert.IsTrue(received.WaitOne(1000), "DataReceived event was not triggered.");

            Assert.IsNotNull(receivedData);
            Assert.AreEqual(sentData.Length * 4, receivedData.Length); // Each float is 4 bytes
            
            // Convert received bytes back to floats for comparison
            var receivedFloats = ParseFloatsHelper(receivedData);
            Assert.AreEqual(sentData.Length, receivedFloats.Length);
            for (int i = 0; i < sentData.Length; i++)
                Assert.AreEqual(sentData[i], receivedFloats[i], 0.0001f);

            udpReceiver.StopListening();
            udpReceiver.Dispose();
            udpSender.Dispose();
        }

        /// <summary>
        /// Helper method to parse float array from byte array (since ParseFloats is no longer public)
        /// </summary>
        private float[] ParseFloatsHelper(byte[] buffer)
        {
            int count = buffer.Length / 4;
            float[] result = new float[count];
            for (int i = 0; i < count; i++)
                result[i] = System.BitConverter.ToSingle(buffer, i * 4);
            return result;
        }
    }
}