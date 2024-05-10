using System;

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
        
        public void CopyTo(WinwingFcuReport targetReport)
        {
            targetReport.ReportId = this.ReportId;
            targetReport.ButtonState = this.ButtonState;
            targetReport.SpdEncoderValue = this.SpdEncoderValue;
            targetReport.AltEncoderValue = this.AltEncoderValue;
            targetReport.HdgEncoderValue = this.HdgEncoderValue;
            targetReport.VsEncoderValue = this.VsEncoderValue;              
        }

        public void ParseReport(byte[] buffer)
        {
            // get Report ID
            ReportId = buffer[0];
            if (ReportId == BUTTONS_REPORT) 
            {
                // get 32 bit Button report field - First 4 bytes: uint:  [4][3][2][1]
                ButtonState = ((uint)buffer[1] + ((uint)buffer[2] << 8) + ((uint)buffer[3] << 16) + ((uint)buffer[4] << 24));
                SpdEncoderValue = (ushort)(buffer[13] | (buffer[14] << 8)); // create an ushort from bytes 13 and 14
                HdgEncoderValue = (ushort)(buffer[15] | (buffer[16] << 8));
                AltEncoderValue = (ushort)(buffer[17] | (buffer[18] << 8));
                VsEncoderValue  = (ushort)(buffer[19] | (buffer[20] << 8));                          
            }
        }
    }
}
