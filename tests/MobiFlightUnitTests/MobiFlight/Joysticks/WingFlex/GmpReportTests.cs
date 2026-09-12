namespace MobiFlight.Joysticks.WingFlex.Tests
{
    [TestClass()]
    public class GmpReportTests
    {
        [TestMethod()]
        public void CopyFromInputBuffer_NullBuffer_Throws()
        {
            var report = new GmpReport();
            Assert.ThrowsExactly<ArgumentException>(() => report.CopyFromInputBuffer(null));
        }
        [TestMethod()]
        public void CopyFromInputBuffer_BufferTooShort_Throws()
        {
            var report = new GmpReport();
            Assert.ThrowsExactly<ArgumentException>(() => report.CopyFromInputBuffer(new byte[] { 1, 2, 3, 4 }));
        }
        [TestMethod()]
        public void CopyFromInputBuffer_MinimumLength_DoesNotThrow()
        {
            var report = new GmpReport();
            report.CopyFromInputBuffer(new byte[] { 1, 2, 3, 4, 5 });
        }
        [TestMethod()]
        public void ToJoystickState_ParsesButtonBitsAcrossBytes()
        {
            // Byte 1: bit 0 (button 0) and bit 7 (button 7) set
            // Byte 4: bit 0 (button 24) set
            var buffer = new byte[] { 0, 0b10000001, 0, 0, 0b00000001, 0, 0 };
            var state = new GmpReport().Parse(buffer).ToJoystickState();
            Assert.IsTrue(state.Buttons[0], "Button 0 should be pressed.");
            Assert.IsTrue(state.Buttons[7], "Button 7 should be pressed.");
            Assert.IsTrue(state.Buttons[24], "Button 24 should be pressed.");
            Assert.IsFalse(state.Buttons[1], "Button 1 should not be pressed.");
            Assert.IsFalse(state.Buttons[23], "Button 23 should not be pressed.");
        }
        [TestMethod()]
        public void ToJoystickState_ParsesLandingGearAxis()
        {
            // X = byte[4] << 8 | byte[5]
            var buffer = new byte[] { 0, 0, 0, 0, 0x03, 0xE8, 0 };
            var state = new GmpReport().Parse(buffer).ToJoystickState();
            Assert.AreEqual(0x03E8, state.X);
        }
        [TestMethod()]
        public void ToJoystickState_ParsesLightSensorAxis()
        {
            // Y = byte[6]
            var buffer = new byte[] { 0, 0, 0, 0, 0, 0, 42 };
            var state = new GmpReport().Parse(buffer).ToJoystickState();
            Assert.AreEqual(42, state.Y);
        }
    }
}