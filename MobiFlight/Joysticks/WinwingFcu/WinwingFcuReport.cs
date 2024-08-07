﻿using System;

namespace MobiFlight.Joysticks.WinwingFcu
{
    internal class WinwingFcuReport
    {
        public uint ReportId { get; set; }
        public uint ButtonState { get; set; }
        public ushort SpdEncoderValue { get; set; }
        public ushort HdgEncoderValue { get; set; }
        public ushort AltEncoderValue { get; set; }
        public ushort VsEncoderValue { get; set; }

        private const uint BUTTONS_REPORT = 1;
        private const uint DEVICE_REPORT = 2;
        private bool IsFirmwareGreaterOrEqual_1_16 = true;

        public void CopyTo(WinwingFcuReport targetReport)
        {
            targetReport.ReportId = this.ReportId;
            targetReport.ButtonState = this.ButtonState;
            targetReport.SpdEncoderValue = this.SpdEncoderValue;
            targetReport.AltEncoderValue = this.AltEncoderValue;
            targetReport.HdgEncoderValue = this.HdgEncoderValue;
            targetReport.VsEncoderValue = this.VsEncoderValue;
        }

        public void ParseReport(HidBuffer hidBuffer)
        {
            byte[] data = hidBuffer.HidReport.TransferResult.Data;
            ReportId = hidBuffer.HidReport.ReportId;
            if (ReportId == BUTTONS_REPORT)
            {
                // get 32 bit Button report field - First 4 bytes: uint:  [3][2][1][0]
                ButtonState = ((uint)data[0] + ((uint)data[1] << 8) + ((uint)data[2] << 16) + ((uint)data[3] << 24));

                if (IsFirmwareGreaterOrEqual_1_16) // Since firmware v1.16
                {
                    SpdEncoderValue = (ushort)(data[28] | (data[29] << 8)); // create an ushort from bytes 28 and 29
                    HdgEncoderValue = (ushort)(data[30] | (data[31] << 8));
                    AltEncoderValue = (ushort)(data[32] | (data[33] << 8));
                    VsEncoderValue  = (ushort)(data[34] | (data[35] << 8));
                }
                else // old firmware
                {
                    SpdEncoderValue = (ushort)(data[12] | (data[13] << 8)); // create an ushort from bytes 13 and 14
                    HdgEncoderValue = (ushort)(data[14] | (data[15] << 8));
                    AltEncoderValue = (ushort)(data[16] | (data[17] << 8));
                    VsEncoderValue  = (ushort)(data[18] | (data[19] << 8));
                }
            }
            else if (ReportId == DEVICE_REPORT)
            {
                // Is firmware report
                if (data[5] == 0x02 && data[4] == 0x05 && data[0] == 0x10)
                {
                    Log.Instance.log($"WINWING FCU Firmware: v{data[9].ToString("X2")}.{data[8].ToString("X2")}", LogSeverity.Debug);
                    if (data[9] == 1 && data[8] < 0x16)
                    {
                        IsFirmwareGreaterOrEqual_1_16 = false;                        
                    }
                }
            }
        }
    }
}
