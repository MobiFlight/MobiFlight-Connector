using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.WinCtrl.Tests
{
    /// <summary>
    /// Pins the payload layout of the WinCtrl report parsers. All offsets are
    /// payload-relative, i.e. without the leading report ID byte, matching the
    /// protocol tables in the report classes.
    /// </summary>
    [TestClass]
    public class WinCtrlFcuReportTests
    {
        private const byte ButtonsReportId = 1;
        private const byte DeviceReportId = 2;

        [TestMethod]
        public void ParseReport_ButtonsReport_ExtractsButtonStatesLittleEndian()
        {
            var report = new WinCtrlFcuReport();
            var data = new byte[40];
            data[0] = 0x01; data[1] = 0x02; data[2] = 0x03; data[3] = 0x04;
            data[4] = 0x05; data[5] = 0x06; data[6] = 0x07; data[7] = 0x08;
            data[8] = 0x09; data[9] = 0x0A; data[10] = 0x0B; data[11] = 0x0C;

            report.ParseReport(ButtonsReportId, data);

            Assert.AreEqual(1u, report.ReportId);
            Assert.AreEqual(0x04030201u, report.ButtonState);
            Assert.AreEqual(0x08070605u, report.ButtonState2);
            Assert.AreEqual(0x0C0B0A09u, report.ButtonState3);
        }

        [TestMethod]
        public void ParseReport_ButtonsReport_NewFirmware_ReadsEncodersFromOffset28()
        {
            var report = new WinCtrlFcuReport();
            var data = new byte[40];
            data[28] = 0x34; data[29] = 0x12; // SPD
            data[30] = 0x78; data[31] = 0x56; // HDG
            data[32] = 0xBC; data[33] = 0x9A; // ALT
            data[34] = 0xF0; data[35] = 0xDE; // VS
            data[36] = 0x22; data[37] = 0x11; // BARO L
            data[38] = 0x44; data[39] = 0x33; // BARO R

            report.ParseReport(ButtonsReportId, data);

            Assert.AreEqual(0x1234, report.SpdEncoderValue);
            Assert.AreEqual(0x5678, report.HdgEncoderValue);
            Assert.AreEqual(0x9ABC, report.AltEncoderValue);
            Assert.AreEqual(0xDEF0, report.VsEncoderValue);
            Assert.AreEqual(0x1122, report.BaroLeftEncoderValue);
            Assert.AreEqual(0x3344, report.BaroRightEncoderValue);
        }

        [TestMethod]
        public void ParseReport_OldFirmwareAnnounced_ReadsEncodersFromOffset12()
        {
            var report = new WinCtrlFcuReport();

            // Firmware answer of the FCU (0x10 0xcb) announcing version 1.15 (< 1.16).
            var firmwareData = new byte[10];
            firmwareData[0] = 0x10; firmwareData[1] = 0xcb;
            firmwareData[4] = 0x05; firmwareData[5] = 0x02;
            firmwareData[8] = 0x15; firmwareData[9] = 0x01;
            report.ParseReport(DeviceReportId, firmwareData);

            var data = new byte[40];
            data[12] = 0x34; data[13] = 0x12; // SPD (old firmware offset)
            data[14] = 0x78; data[15] = 0x56; // HDG
            data[16] = 0xBC; data[17] = 0x9A; // ALT
            data[18] = 0xF0; data[19] = 0xDE; // VS
            report.ParseReport(ButtonsReportId, data);

            Assert.AreEqual(0x1234, report.SpdEncoderValue);
            Assert.AreEqual(0x5678, report.HdgEncoderValue);
            Assert.AreEqual(0x9ABC, report.AltEncoderValue);
            Assert.AreEqual(0xDEF0, report.VsEncoderValue);
        }

        [TestMethod]
        public void ParseReport_UnknownReportId_LeavesStateUntouched()
        {
            var report = new WinCtrlFcuReport();
            var data = new byte[40];
            data[0] = 0xFF;

            report.ParseReport(0xF0, data);

            Assert.AreEqual(0xF0u, report.ReportId);
            Assert.AreEqual(0u, report.ButtonState);
        }

        [TestMethod]
        public void CopyTo_CopiesAllFields()
        {
            var source = new WinCtrlFcuReport();
            var data = new byte[40];
            data[0] = 0xFF;
            data[28] = 0x11; data[30] = 0x22; data[32] = 0x33;
            data[34] = 0x44; data[36] = 0x55; data[38] = 0x66;
            source.ParseReport(ButtonsReportId, data);

            var target = new WinCtrlFcuReport();
            source.CopyTo(target);

            Assert.AreEqual(source.ReportId, target.ReportId);
            Assert.AreEqual(source.ButtonState, target.ButtonState);
            Assert.AreEqual(source.ButtonState2, target.ButtonState2);
            Assert.AreEqual(source.ButtonState3, target.ButtonState3);
            Assert.AreEqual(source.SpdEncoderValue, target.SpdEncoderValue);
            Assert.AreEqual(source.HdgEncoderValue, target.HdgEncoderValue);
            Assert.AreEqual(source.AltEncoderValue, target.AltEncoderValue);
            Assert.AreEqual(source.VsEncoderValue, target.VsEncoderValue);
            Assert.AreEqual(source.BaroLeftEncoderValue, target.BaroLeftEncoderValue);
            Assert.AreEqual(source.BaroRightEncoderValue, target.BaroRightEncoderValue);
        }
    }

    [TestClass]
    public class WinCtrlRmpReportTests
    {
        private const byte ButtonsReportId = 1;

        [TestMethod]
        public void ParseReport_ButtonsReport_ExtractsButtonsAndKnobs()
        {
            var report = new WinCtrlRmpReport();
            var data = new byte[16];
            data[0] = 0x01; data[1] = 0x02; data[2] = 0x03; data[3] = 0x04;
            data[9] = 0x34; data[10] = 0x12;  // outer knob
            data[11] = 0x78; data[12] = 0x56; // inner knob

            report.ParseReport(ButtonsReportId, data);

            Assert.AreEqual(1u, report.ReportId);
            Assert.AreEqual(0x04030201u, report.ButtonState);
            Assert.AreEqual(0x1234, report.OuterKnobValue);
            Assert.AreEqual(0x5678, report.InnerKnobValue);
        }

        [TestMethod]
        public void ParseReport_UnknownReportId_LeavesStateUntouched()
        {
            var report = new WinCtrlRmpReport();
            var data = new byte[16];
            data[0] = 0xFF;

            report.ParseReport(0x02, data);

            Assert.AreEqual(2u, report.ReportId);
            Assert.AreEqual(0u, report.ButtonState);
        }
    }

    [TestClass]
    public class WinCtrlPap3ReportTests
    {
        private const byte ButtonsReportId = 1;

        [TestMethod]
        public void ParseReport_ButtonsReport_ExtractsButtonStatesAndEncoders()
        {
            var report = new WinCtrlPap3Report();
            var data = new byte[32];
            data[0] = 0x01; data[1] = 0x02; data[2] = 0x03; data[3] = 0x04;
            data[4] = 0x05; data[5] = 0x06; data[6] = 0x07; data[7] = 0x08;
            data[8] = 0x09; data[9] = 0x0A; data[10] = 0x0B; data[11] = 0x0C;
            data[20] = 0x11; data[21] = 0x01; // course left
            data[22] = 0x22; data[23] = 0x02; // SPD
            data[24] = 0x33; data[25] = 0x03; // HDG
            data[26] = 0x44; data[27] = 0x04; // ALT
            data[28] = 0x55; data[29] = 0x05; // VS
            data[30] = 0x66; data[31] = 0x06; // course right

            report.ParseReport(ButtonsReportId, data);

            Assert.AreEqual(1u, report.ReportId);
            Assert.AreEqual(0x04030201u, report.ButtonState);
            Assert.AreEqual(0x08070605u, report.ButtonState2);
            Assert.AreEqual(0x0C0B0A09u, report.ButtonState3);
            Assert.AreEqual(0x0111, report.CourseLeftEncoderValue);
            Assert.AreEqual(0x0222, report.SpdEncoderValue);
            Assert.AreEqual(0x0333, report.HdgEncoderValue);
            Assert.AreEqual(0x0444, report.AltEncoderValue);
            Assert.AreEqual(0x0555, report.VsEncoderValue);
            Assert.AreEqual(0x0666, report.CourseRightEncoderValue);
        }

        [TestMethod]
        public void ParseReport_DeviceReport_DoesNotChangeButtonState()
        {
            var report = new WinCtrlPap3Report();
            var data = new byte[10];
            data[0] = 0x0f; data[1] = 0xcf;
            data[4] = 0x05; data[5] = 0x02;
            data[8] = 0x10; data[9] = 0x01;

            report.ParseReport(0x02, data);

            Assert.AreEqual(2u, report.ReportId);
            Assert.AreEqual(0u, report.ButtonState);
        }
    }
}
