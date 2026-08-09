using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.Octavi.Tests
{
    /// <summary>
    /// Pins the raw report layout: report ID at index 0, 32-bit button field at 1..4
    /// (little endian), signed encoder deltas at 5 and 6, context state at 7.
    /// </summary>
    [TestClass]
    public class OctaviReportTests
    {
        [TestMethod]
        public void ParseReport_ExtractsAllFieldsFromRawBuffer()
        {
            var report = new OctaviReport();

            report.parseReport(new byte[] { 0x01, 0x10, 0x00, 0x01, 0x00, 0xFF, 0x02, 0x03 });

            Assert.AreEqual(1u, report.reportId);
            Assert.AreEqual(
                OctaviReport.OctaviButtons.HID_BTN_DCT | OctaviReport.OctaviButtons.HID_BTN_AP_NAV,
                report.buttonState);
            Assert.AreEqual(-1, report.outerEncoderDelta);
            Assert.AreEqual(2, report.innerEncoderDelta);
            Assert.AreEqual(OctaviReport.OctaviState.STATE_NAV2, report.contextState);
        }

        [TestMethod]
        public void ParseReport_ZeroBuffer_YieldsDefaultState()
        {
            var report = new OctaviReport();

            report.parseReport(new byte[8]);

            Assert.AreEqual(0u, report.reportId);
            Assert.AreEqual((OctaviReport.OctaviButtons)0, report.buttonState);
            Assert.AreEqual(0, report.outerEncoderDelta);
            Assert.AreEqual(0, report.innerEncoderDelta);
            Assert.AreEqual(OctaviReport.OctaviState.STATE_COM1, report.contextState);
        }

        [TestMethod]
        public void ParseReport_ButtonFieldIsLittleEndian()
        {
            var report = new OctaviReport();

            report.parseReport(new byte[] { 0x01, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00 });

            Assert.AreEqual(
                OctaviReport.OctaviButtons.HID_BTN_AP_ALT | OctaviReport.OctaviButtons.HID_BTN_AP_VS,
                report.buttonState);
        }
    }
}
