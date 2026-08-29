using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.Logitech.Tests
{
    [TestClass]
    public class SwitchPanelLedStateTests
    {
        [TestMethod]
        [DataRow(0x00)]
        [DataRow(0x01)]
        [DataRow(0x02)]
        [DataRow(0x04)]
        [DataRow(0x08)]
        [DataRow(0x10)]
        [DataRow(0x20)]
        [DataRow(0x09)]
        [DataRow(0x12)]
        [DataRow(0x24)]
        [DataRow(0x07)]
        [DataRow(0x38)]
        [DataRow(0x3F)]
        public void ToFeatureReport_SerializesIndependentLedChannels(int expectedValue)
        {
            var state = new SwitchPanelLedState();
            for (var channel = 0; channel < SwitchPanelLedState.ChannelCount; channel++)
            {
                state.SetChannel(channel, (expectedValue & (1 << channel)) != 0);
            }

            CollectionAssert.AreEqual(new byte[] { 0x00, (byte)expectedValue }, state.ToFeatureReport());
        }

        [TestMethod]
        public void SetChannel_ClearsOnlySelectedChannel()
        {
            var state = new SwitchPanelLedState();
            state.SetChannel(0, true);
            state.SetChannel(3, true);
            state.SetChannel(0, false);

            Assert.AreEqual(0x08, state.Value);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(SwitchPanelLedState.ChannelCount)]
        public void SetChannel_InvalidChannel_Throws(int channel)
        {
            var state = new SwitchPanelLedState();

            Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => state.SetChannel(channel, true));
        }
    }
}
