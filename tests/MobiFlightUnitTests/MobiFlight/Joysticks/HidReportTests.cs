using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.Tests
{
    [TestClass]
    public class HidReportTests
    {
        [TestMethod]
        public void Buffer_ReturnsRawBytes()
        {
            var raw = new byte[] { 0x01, 0xAA, 0xBB };

            var report = new HidReport(raw);

            Assert.AreSame(raw, report.Buffer);
        }

        [TestMethod]
        public void ReportId_ReturnsFirstByte()
        {
            var report = new HidReport(new byte[] { 0x05, 0xAA, 0xBB });

            Assert.AreEqual(0x05, report.ReportId);
        }

        [TestMethod]
        public void Payload_ReturnsBytesAfterReportId()
        {
            var report = new HidReport(new byte[] { 0x01, 0xAA, 0xBB, 0xCC });

            CollectionAssert.AreEqual(new byte[] { 0xAA, 0xBB, 0xCC }, report.Payload);
        }

        [TestMethod]
        public void Payload_ReportIdOnly_IsEmpty()
        {
            var report = new HidReport(new byte[] { 0x01 });

            Assert.IsEmpty(report.Payload);
        }

        [TestMethod]
        public void Payload_IsCachedAcrossAccesses()
        {
            var report = new HidReport(new byte[] { 0x01, 0xAA });

            Assert.AreSame(report.Payload, report.Payload);
        }
    }
}
