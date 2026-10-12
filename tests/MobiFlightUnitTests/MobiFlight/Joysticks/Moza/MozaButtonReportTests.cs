using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MobiFlight.Joysticks.Moza.Tests
{
    // Scaffolded pending MOZA's HID button report documentation (plan risk R-BUTTONREPORT).
    // Once the byte/bit table is known, this becomes the SwitchPanelReportTests shape.
    [TestClass]
    public class MozaButtonReportTests
    {
        [TestMethod]
        public void Parse_ReportOfDeclaredLength_DoesNotThrow()
        {
            var payload = new byte[MozaButtonReport.PayloadLength];

            var report = MozaButtonReport.Parse(payload);

            Assert.IsNotNull(report.ToJoystickState());
        }

        [TestMethod]
        public void Parse_PayloadShorterThanDeclaredLength_Throws()
        {
            var payload = new byte[MozaButtonReport.PayloadLength - 1];

            Assert.ThrowsExactly<ArgumentException>(() => MozaButtonReport.Parse(payload));
        }
    }
}
